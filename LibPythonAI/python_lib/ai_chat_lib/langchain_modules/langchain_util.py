
import json, sys
from langchain.prompts import PromptTemplate
from langchain.agents import create_react_agent, AgentExecutor
from langchain.docstore.document import Document
from langchain_community.callbacks.manager import get_openai_callback
import langchain
from langchain_core.tools.structured import StructuredTool

from typing import Any
from pydantic import BaseModel, Field


from ai_chat_lib.langchain_modules.langchain_client import LangChainOpenAIClient, LangChainChatParameter
from ai_chat_lib.langchain_modules.langchain_vector_db import LangChainVectorDB

from ai_chat_lib.openai_modules.openai_util import OpenAIProps
from ai_chat_lib.db_modules import *

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)



class CustomToolInput(BaseModel):
    question: str = Field(description="question")

class RetrievalQAUtil:

    def __init__(self, client: LangChainOpenAIClient, vector_search_requests:list[VectorSearchRequest]):
        self.client = client
        self.vector_search_requests = vector_search_requests

        # ツールのリストを作成
        self.tools = self.create_vector_search_tools(self.client, self.vector_search_requests)

    def create_agent_executor(self):
        '''
        # see https://python.langchain.com/api_reference/langchain/agents/langchain.agents.react.agent.create_react_agent.html
        '''
        template = '''Answer the following questions as best you can. You have access to the following tools:

            {tools}

            Use the following format:

            Question: the input question you must answer
            Thought: Do I need to use a tool? (Yes or No)
            Action: the action to take, should be one of [{tool_names}], if using a tool, otherwise answer on your own
            Action Input: the input to the action
            Observation: the result of the action
            ... (this Thought/Action/Action Input/Observation can repeat N times)
            Thought: I now know the final answer
            Final Answer: the final answer to the original input question

            Begin!

            Question: {input}
            Thought:{agent_scratchpad}'''

        prompt = PromptTemplate.from_template(template)
        # ChatAgentオブジェクトを作成
        chat_agent = create_react_agent(
                self.client.get_completion_client(),
                self.tools,
                prompt
        )
        agent_executor = AgentExecutor(
            agent=chat_agent, tools=self.tools, 
            return_source_documents=True,
            return_intermediate_steps=True,
            stream_runnable=False,
            verbose=True,
            handle_parsing_errors = True
            )
        return agent_executor

    def process_intermadiate_steps(self, intermediate_steps):
        '''
        # 関数の説明
        # intermediate_stepsを処理する。
        # 
        # 引数
        # intermediate_steps: list
        #   intermediate_steps
        # 戻り値
        # なし
        # 例
        # process_intermadiate_steps(intermediate_steps)
        '''
        page_content_list = []
        page_source_list = []
        #  verbose情報
        verbose_list = []
        logger.debug(f"intermediate_steps:{intermediate_steps}")

        for step in intermediate_steps:
            # 0: AgentAction, 1: Observation ( create_vector_search_toolsで作成したツールを使っている場合はlist[Document]が返る)
            observation = step[1]
            if not isinstance(observation, list):
                continue
            # source, source_urlを取得
            for source_document in observation:
                if not isinstance(source_document, Document):
                    continue
                source = source_document.metadata.get("source","")
                source_url = source_document.metadata.get("source_url","")
                
                page_content_list.append(source_document.page_content)
                page_source_list.append({"source": source, "source_url": source_url})
        
            # verbose情報を取得
            verbose = self.__serialize_intermediate_step(step)
            verbose_list.append(verbose)
        
        # verbose情報をjson文字列に変換
        verbose_json = json.dumps(verbose_list, ensure_ascii=False, indent=4)    
        
        return page_content_list, page_source_list, verbose_json
    
    # intermediate_stepsをシリアライズする
    def __serialize_intermediate_step(self, step):
        return {
            "tool": step[0].tool,
            "tool_input": step[0].tool_input,
            "log": step[0].log,
            "output": str(step[1]),
        }


    # ベクトル検索結果を返すToolを作成する関数
    def create_vector_search_tools(self, client: LangChainOpenAIClient, vector_search_requests: list[VectorSearchRequest]) -> list[Any]:
        tools = []
        for i in range(len(vector_search_requests)):
            item = vector_search_requests[i]
            vector_db_item: VectorDBItem = item.get_vector_db_item()
            # ベクトルDBのURLを取得
            # description item.VectorDBDescriptionが空の場合はデフォルトの説明を設定
            description = vector_db_item.Description
            vector_db_url = vector_db_item.VectorDBURL
            doc_store_url = ""
            if vector_db_item.IsUseMultiVectorRetriever:
                doc_store_url = vector_db_item.DocStoreURL
            collection_name = vector_db_item.CollectionName
            chunk_size = vector_db_item.ChunkSize

            # ツールを作成
            def vector_search_function(question: str) -> list[Document]:
                # Retrieverを作成
                search_kwargs = {"k": 4}

                retriever = LangChainVectorDB(client, vector_db_url, collection_name, doc_store_url, chunk_size).create_retriever(search_kwargs)
                docs: list[Document] = retriever.invoke(question)
                # page_contentを取得
                result_docs = []
                for doc in docs:
                    result_docs.append(doc)
                return result_docs

            # StructuredTool.from_functionを使ってToolオブジェクトを作成
            vector_search_tool = StructuredTool.from_function(
                func=vector_search_function, name="vector_search_tool-" + str(i), description=description, args_schema=CustomToolInput  
            )

            tools.append(vector_search_tool)

        return tools


class LangChainUtil:

    @classmethod
    def vector_search_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = OpenAIProps.get_openai_objects(request_dict)
        # queryを取得
        vector_search_requests: list[VectorSearchRequest] = VectorSearchRequest.get_vector_search_requests_objects(request_dict)

        result = cls.vector_search(openai_props, vector_search_requests)
        return result

    @classmethod
    def update_collection_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからVectorDBItemを生成
        vector_search_requests = EmbeddingData.get_embedding_request_objects(request_dict)

        # 現時点では処理なし
        return {}

    @classmethod
    def delete_collection_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = OpenAIProps.get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBItemを生成
        embedding_data = EmbeddingData.get_embedding_request_objects(request_dict)
        
        main_db = MainDB()
        vector_db_item = main_db.get_vector_db_by_name(embedding_data.name)
        if vector_db_item is None:
            raise ValueError(f"VectorDBItem with name {embedding_data.name} not found.")
        vector_db: LangChainVectorDB = LangChainUtil.get_vector_db(openai_props, vector_db_item, embedding_data.model)
        # delete_collectionを実行
        vector_db.delete_collection()

        return {}

    @classmethod
    def delete_embeddings_by_folder_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = OpenAIProps.get_openai_objects(request_dict)

        # embedding_requestを取得
        embedding_data = EmbeddingData.get_embedding_request_objects(request_dict)
        # MainDBを取得
        main_db = MainDB()

        vector_db_item = main_db.get_vector_db_by_name(embedding_data.name)
        if vector_db_item is None:
            raise ValueError(f"VectorDBItem with name {embedding_data.name} not found.")
        
        # LangChainVectorDBを生成
        vector_db: LangChainVectorDB = LangChainUtil.get_vector_db(openai_props, vector_db_item, embedding_data.model)

        folder_id = embedding_data.FolderId
        # delete_folder_embeddingsを実行
        vector_db.delete_folder(folder_id)

        return {}

    @classmethod
    def delete_embeddings_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = OpenAIProps.get_openai_objects(request_dict)

        # embedding_requestを取得
        embedding_data = EmbeddingData.get_embedding_request_objects(request_dict)
        # MainDBを取得
        main_db = MainDB()
        vector_db_item = main_db.get_vector_db_by_name(embedding_data.name)
        if vector_db_item is None:
            raise ValueError(f"VectorDBItem with name {embedding_data.name} not found.")
        vector_db: LangChainVectorDB = LangChainUtil.get_vector_db(openai_props, vector_db_item, embedding_data.model)
        vector_db.delete_document(embedding_data.source_id)

        return {}

    @classmethod
    async def update_embeddings_api(cls, request_json: str) -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = OpenAIProps.get_openai_objects(request_dict)
        # embedding_requestを取得
        embedding_data: EmbeddingData = EmbeddingData.get_embedding_request_objects(request_dict)

        # MainDBを取得
        main_db = MainDB()
        vector_db_item = main_db.get_vector_db_by_name(embedding_data.name)
        if vector_db_item is None:
            raise ValueError(f"VectorDBItem with name {embedding_data.name} not found.")
        # LangChainVectorDBを生成
        vector_db: LangChainVectorDB = LangChainUtil.get_vector_db(openai_props, vector_db_item, embedding_data.model)
        await vector_db.update_document(embedding_data)

        return {}


    chat_request_name = "chat_request"

    @classmethod
    def get_vector_db(cls, openai_props: OpenAIProps, vector_db_props: VectorDBItem, embedding_model: str) -> LangChainVectorDB:

        langchain_openai_client = LangChainOpenAIClient(openai_props, embedding_model)

        vector_db_url = vector_db_props.VectorDBURL
        if vector_db_props.IsUseMultiVectorRetriever:
            doc_store_url = vector_db_props.DocStoreURL
        else:
            doc_store_url = ""
        collection_name = vector_db_props.CollectionName
        chunk_size = vector_db_props.ChunkSize

        # ベクトルDBのタイプがChromaの場合
        if vector_db_props.VectorDBTypeString == "Chroma":
            from ai_chat_lib.langchain_modules.langchain_vector_db_chroma import LangChainVectorDBChroma
            return LangChainVectorDBChroma(langchain_openai_client, vector_db_url, collection_name, doc_store_url, chunk_size)
        # ベクトルDBのタイプがPostgresの場合
        elif vector_db_props.VectorDBTypeString == "PGVector":
            from ai_chat_lib.langchain_modules.langchain_vector_db_pgvector import LangChainVectorDBPGVector
            return LangChainVectorDBPGVector(langchain_openai_client, vector_db_url, collection_name, doc_store_url, chunk_size)
        else:
            # それ以外の場合は例外
            raise ValueError("VectorDBType is invalid")

    # ベクトル検索を行う
    @classmethod
    def vector_search(cls, openai_props: OpenAIProps, vector_search_requests: list[VectorSearchRequest]) -> dict[str, Any]:    

        if not openai_props:
            raise ValueError("openai_props is None")

        main_db = MainDB()

        # documentsの要素からcontent, source, source_urlを取得
        result = []
        # vector_db_propsの要素毎にRetrieverを作成して、検索を行う
        for request in vector_search_requests:
            # vector_db_itemを取得
            vector_db_item = main_db.get_vector_db_by_name(request.name)
            if vector_db_item is None:
                raise ValueError(f"vector_db_item is None. name:{request.name}")

            langchain_db = cls.get_vector_db(openai_props, vector_db_item, request.model)
            
            # デバッグ出力
            logger.info('ベクトルDBの設定')
            logger.info(f'''
                        name:{vector_db_item.Name} vector_db_description:{vector_db_item.Description} 
                        VectorDBTypeString:{vector_db_item.VectorDBTypeString} VectorDBURL:{vector_db_item.VectorDBURL} 
                        CollectionName:{vector_db_item.CollectionName}'
                        ChunkSize:{vector_db_item.ChunkSize} IsUseMultiVectorRetriever:{vector_db_item.IsUseMultiVectorRetriever}
                        ''')

            logger.info(f'Query: {request.query}')
            logger.info(f'SearchKwargs:{request.search_kwargs}')
            retriever = langchain_db.create_retriever(request.search_kwargs)
            documents: list[Document] = retriever.invoke(request.query)

            # 重複排除用のリストを作成
            doc_ids = []
            # folder_idからfolder_pathを生成するためのMainDBを取得
            main_db = MainDB()

            for doc in documents:
                # doc_idを取得
                doc_id = doc.metadata.get("doc_id", "")
                # 既にdoc_idが存在する場合はスキップ
                if doc_id in doc_ids:
                    continue
                # doc_idを追加
                doc_ids.append(doc_id)
                # folder_idを取得
                folder_id = doc.metadata.get("folder_id", "")
                if folder_id:
                    # folder_idからfolder_pathを取得
                    folder_path = main_db.get_content_folder_path_by_id(folder_id)
                    if folder_path:
                        doc.metadata["folder_path"] = folder_path
                    else:
                        # folder_pathが存在しない場合は空文字を設定
                        doc.metadata["folder_path"] = ""

                content = doc.page_content
                doc_dict = LangChainVectorDB.create_metadata_from_document(doc)
                doc_dict["content"] = content

                sub_docs: list[Document]= doc.metadata.get("sub_docs", [])
                # sub_docsの要素からcontent, source, source_url,scoreを取得してdictのリストに追加
                sub_docs_result = []
                for sub_doc in sub_docs:
                    content = sub_doc.page_content
                    # folder_idを取得
                    folder_id = sub_doc.metadata.get("folder_id", "")
                    if folder_id:
                        # folder_idからfolder_pathを取得
                        folder_path = main_db.get_content_folder_path_by_id(folder_id)
                        if folder_path:
                            sub_doc.metadata["folder_path"] = folder_path
                        else:
                            # folder_pathが存在しない場合は空文字を設定
                            sub_doc.metadata["folder_path"] = ""

                    sub_doc_dict = LangChainVectorDB.create_metadata_from_document(sub_doc)
                    sub_doc_dict["content"] = content
                    sub_docs_result.append(sub_doc_dict)

                doc_dict["sub_docs"] = sub_docs_result
                result.append(doc_dict)

            # logger.debug(f"documents:\n{documents}")
            
        return {"documents": result}
    
