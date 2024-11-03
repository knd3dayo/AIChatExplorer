
import json, sys
from langchain.prompts import PromptTemplate
from langchain.agents import create_react_agent, AgentExecutor
from langchain.docstore.document import Document
from langchain_core.messages import AIMessage, HumanMessage, SystemMessage
from langchain.retrievers.multi_vector import MultiVectorRetriever
from langchain_community.callbacks.manager import get_openai_callback
import langchain
from langchain_core.runnables import chain
from langchain_core.tools.structured import StructuredTool

sys.path.append("python")
from langchain_client import LangChainOpenAIClient
from openai_props import OpenAIProps, VectorDBProps
from langchain_vector_db import get_vector_db, VectorSearchParameter

from typing import Any
from langchain_core.callbacks import (
    CallbackManagerForRetrieverRun,
)
from collections import defaultdict

from pydantic import BaseModel, Field


class LangChainChatParameter:
    def __init__(self, props_json: str ="{}", request_json: str ="{}"):
        # OpenAIPorpsを生成
        props = json.loads(props_json)
        # OpenAIPorpsを生成
        self.openai_props = OpenAIProps(props)
        self.vector_db_items = self.openai_props.VectorDBItems


        #  openai_props, vector_db_items, prompt, chat_historyを設定する
        request = json.loads(request_json)
        # messagesを取得
        messages = request.get("messages", [])
        # messagesのlengthが0の場合はエラーを返す
        if len(messages) == 0:
            self.prompt = ""
        else:
            # messagesの最後のメッセージを取得
            last_message = messages[-1]
            # contentを取得
            content = last_message.get("content", {})
            # contentのうちtype: textのもののtextを取得
            prompt_array = [ c["text"] for c in content if c["type"] == "text"]
            # prpmpt_arrayのlengthが0の場合はエラーを返す
            if len(prompt_array) > 0:
                # promptを取得
                self.prompt = prompt_array[0]
                # messagesから最後のメッセージを削除
                messages.pop()
            else:
                raise ValueError("prompt is empty")

        # messagesをjson文字列に変換
        chat_history_json = json.dumps(messages, ensure_ascii=False, indent=4)
        self.chat_history = RetrievalQAUtil.convert_to_langchain_chat_history(chat_history_json)
        # デバッグ出力
        print(f'prompt: {self.prompt}')
        print(f'chat_history: {self.chat_history}')
        print('vector db')
        for item in self.vector_db_items:
            print(f'Name:{item.Name} VectorDBDescription:{item.VectorDBDescription} VectorDBTypeString:{item.VectorDBTypeString} VectorDBURL:{item.VectorDBURL} CollectionName:{item.CollectionName}')

class CustomToolInput(BaseModel):
    question: str = Field(description="question")

class CustomMultiVectorRetriever(MultiVectorRetriever):
    def _get_relevant_documents(self, query: str, *, run_manager: CallbackManagerForRetrieverRun) -> list[Document]:
        """Get documents relevant to a query.
        Args:
            query: String to find relevant documents for
            run_manager: The callbacks handler to use
        Returns:
            List of relevant documents
        """
        results = self.vectorstore.similarity_search_with_score(query, **self.search_kwargs)

        # Map doc_ids to list of sub-documents, adding scores to metadata
        id_to_doc = defaultdict(list)
        for doc, score in results:
            doc_id = doc.metadata.get("doc_id")
            if doc_id:
                doc.metadata["score"] = score
                id_to_doc[doc_id].append(doc)

        # Fetch documents corresponding to doc_ids, retaining sub_docs in metadata
        docs = []
        for _id, sub_docs in id_to_doc.items():
            docstore_docs = self.docstore.mget([_id])
            if docstore_docs:
                docstore_doc: Optional[Document]= docstore_docs[0]
                if docstore_doc is not None:
                    docstore_doc.metadata["sub_docs"] = sub_docs
                    docs.append(docstore_doc)

        return docs
            
class RetrieverUtil:
    
    def __init__(self,  client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
        self.client = client
        self.vector_db_props = vector_db_props

    def create_retriever(self, search_kwargs: dict[str, Any] = {}) -> Any:
        # ベクトルDB検索用のRetrieverオブジェクトの作成と設定

        vector_db_props = self.vector_db_props
        if not search_kwargs:
            search_kwargs = {"k": 10}

        # IsUseMultiVectorRetriever=Trueの場合はMultiVectorRetrieverを生成
        if vector_db_props.IsUseMultiVectorRetriever:
            print("Creating MultiVectorRetriever")
            
            langChainVectorDB = get_vector_db(self.client.props, vector_db_props)
            retriever = CustomMultiVectorRetriever(
                vectorstore=langChainVectorDB.db,
                docstore=langChainVectorDB.doc_store,
                id_key="doc_id",
                search_kwargs=search_kwargs
            )

        else:
            print("Creating a regular Retriever")
            langChainVectorDB = get_vector_db(self.client.props, vector_db_props)
            retriever = self.__create_decorated_retriever(langChainVectorDB.db, search_kwargs=search_kwargs)
         
        return retriever

    def __create_decorated_retriever(self, vectorstore, **kwargs: Any):
        # ベクトル検索の結果にスコアを追加する
        @chain
        def retriever(query: str) -> list[Document]:
            result = []
            docs, scores = zip(*vectorstore.similarity_search_with_score(query, kwargs))
            for doc, score in zip(docs, scores):
                doc.metadata["score"] = score
                result.append(doc)
            return result

        return retriever

class RetrievalQAUtil:

    def __init__(self, client: LangChainOpenAIClient, vector_db_items:list[VectorDBProps]):
        self.client = client
        self.vector_db_items = vector_db_items

        # ツールのリストを作成
        self.tools = create_vector_search_tools(self.client, self.vector_db_items)

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
        print(f"intermediate_steps:{intermediate_steps}")

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

    @classmethod
    def convert_to_langchain_chat_history(cls, chat_history_json: str):
        # openaiのchat_historyをlangchainのchat_historyに変換
        langchain_chat_history : list[Any]= []
        chat_history = json.loads(chat_history_json)
        for chat in chat_history:
            role = chat["role"]
            content = chat["content"]
            if role == "system":
                langchain_chat_history.append(SystemMessage(content))
            elif role == "user":
                langchain_chat_history.append(HumanMessage(content))
            elif role == "assistant":
                langchain_chat_history.append(AIMessage(content))
        return langchain_chat_history

# ベクトル検索を行う
def run_vector_search( params: VectorSearchParameter):    

    client = LangChainOpenAIClient(params.openai_props)

    # documentsの要素からcontent, source, source_urlを取得
    result = []
    # vector_db_propsの要素毎にRetrieverを作成して、検索を行う
    for vector_db_item in params.vector_db_props:

        # デバッグ出力
        print(f'検索文字列: {params.query}')
        print(f'検索条件: {params.search_kwarg}')
        print('ベクトルDBの設定')
        print(f'Name:{vector_db_item.Name} VectorDBDescription:{vector_db_item.VectorDBDescription} VectorDBTypeString:{vector_db_item.VectorDBTypeString} VectorDBURL:{vector_db_item.VectorDBURL} CollectionName:{vector_db_item.CollectionName}')
        retriever = RetrieverUtil(client, vector_db_item).create_retriever(params.search_kwarg)
        documents: list[Document] = retriever.invoke(params.query)

        print(f"documents:\n{documents}")
        for doc in documents:
            content = doc.page_content
            source = doc.metadata.get("source", "")
            source_url = doc.metadata.get("source_url", "")
            score = doc.metadata.get("score", 0.0)
            # description, reliabilityを取得
            description = doc.metadata.get("description", "")
            reliability = doc.metadata.get("reliability", 0)

            sub_docs = doc.metadata.get("sub_docs", [])
            # sub_docsの要素からcontent, source, source_url,scoreを取得してdictのリストに追加
            sub_docs_result = []
            for sub_doc in sub_docs:
                sub_content = sub_doc.page_content
                sub_source = sub_doc.metadata.get("source", "")
                sub_source_url = sub_doc.metadata.get("source_url", "")
                sub_score = sub_doc.metadata.get("score", 0.0)
                
                sub_docs_result.append({"content": sub_content, "source": sub_source, "source_url": sub_source_url, "score": sub_score})

            result.append(
                {"content": content, "source": source, "source_url": source_url, "score": score, 
                 "description": description, "reliability": reliability, "sub_docs": sub_docs_result})
        
    return {"documents": result}

# ベクトル検索結果を返すToolを作成する関数
def create_vector_search_tools(client: LangChainOpenAIClient, vector_db_props: list[VectorDBProps]) -> list[Any]:
    tools = []
    for i in range(len(vector_db_props)):
        item = vector_db_props[i]
        # description item.VectorDBDescriptionが空の場合はデフォルトの説明を設定
        description = item.VectorDBDescription
        if not description:
            description = "ユーザーの質問に関連する情報をベクトルDBから検索するための汎用的なツールです。"

        # ツールを作成
        def vector_search_function(question: str) -> list[Document]:
            # Retrieverを作成
            # ★TODO search_kwargsの処理は現在はvector_db_propsのMaxSearchResults + content_type=textを使っている.
            # content_typeもvector_db_propsで指定できるようにする
            search_kwargs = {"k": item.MaxSearchResults, "filter":{"content_type": "text"}}

            retriever = RetrieverUtil(client, item).create_retriever(search_kwargs)
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

from typing import Optional

def langchain_chat(params: LangChainChatParameter):

    # langchainのログを出力する
    langchain.verbose = True
        
    client = LangChainOpenAIClient(props)
    RetrievalQAUtilInstance = RetrievalQAUtil(client, params.vector_db_items)
    ChatAgentExecutorInstance = RetrievalQAUtilInstance.create_agent_executor()
    
    result_dict = {}
    with get_openai_callback() as cb:
        result = ChatAgentExecutorInstance.invoke(
                {
                    "input": params.prompt,
                    "chat_history": params.chat_history,
                }
            )
        result_dict["output"] = result.get("output", "")
        page_conetnt_list, page_source_list, verbose_json = RetrievalQAUtilInstance.process_intermadiate_steps(result.get("intermediate_steps",[]))
        result_dict["page_content_list"] = page_conetnt_list
        result_dict["page_source_list"] = page_source_list
        result_dict["verbose"] = verbose_json
        result_dict["total_tokens"] = cb.total_tokens
    return result_dict

if __name__ == '__main__':

    question1 = input("Please enter your question:")

    from openai_props import OpenAIProps, VectorDBProps, env_to_props, get_vector_db_settings
    props:OpenAIProps  = env_to_props()
    vector_db_item: VectorDBProps = get_vector_db_settings()

    params: LangChainChatParameter = LangChainChatParameter()
    params.chat_history = []
    params.openai_props = props
    params.vector_db_items = [vector_db_item]

    params.prompt = question1
    result1 = langchain_chat(params)

    print(result1.get("output",""))
    page_conetnt_list = result1.get("page_content_list", [])
    page_source_list = result1.get("page_source_list", [])
    for page_content, page_source in zip(page_conetnt_list, page_source_list):
        print(page_source)
        print(page_content)
        print('---------------------')
    
    verbose = result1.get("verbose", "")
    print(verbose)


