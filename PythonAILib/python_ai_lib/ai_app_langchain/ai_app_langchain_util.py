
import json, sys
from langchain.prompts import PromptTemplate
from langchain.agents import create_react_agent, AgentExecutor
from langchain.docstore.document import Document
from langchain_community.callbacks.manager import get_openai_callback
import langchain
from langchain_core.tools.structured import StructuredTool

from typing import Any
from pydantic import BaseModel, Field


from ai_app_langchain.ai_app_langchain_client import LangChainOpenAIClient, LangChainChatParameter
from ai_app_langchain.langchain_vector_db import LangChainVectorDB

from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps

class CustomToolInput(BaseModel):
    question: str = Field(description="question")

class RetrievalQAUtil:

    def __init__(self, client: LangChainOpenAIClient, vector_db_items:list[VectorDBProps]):
        self.client = client
        self.vector_db_items = vector_db_items

        # ツールのリストを作成
        self.tools = self.create_vector_search_tools(self.client, self.vector_db_items)

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


    # ベクトル検索結果を返すToolを作成する関数
    def create_vector_search_tools(self, client: LangChainOpenAIClient, vector_db_props: list[VectorDBProps]) -> list[Any]:
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
                search_kwargs = {"k": 1, "filter":{"content_type": "text"}}

                retriever = LangChainVectorDB(client, item).create_retriever(search_kwargs)
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


    @staticmethod
    def langchain_chat(openai_props: OpenAIProps, vector_db_items: list[VectorDBProps], params: LangChainChatParameter):

        # langchainのログを出力する
        langchain.verbose = True
        print("langchain_chat:start")
        client = LangChainOpenAIClient(openai_props)
        RetrievalQAUtilInstance = RetrievalQAUtil(client, vector_db_items)
        ChatAgentExecutorInstance = RetrievalQAUtilInstance.create_agent_executor()
        print("langchain_chat:init done")
        
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

        print("langchain_chat:end")

        return result_dict

if __name__ == '__main__':

    question1 = input("Please enter your question:")

    from ai_app_openai.ai_app_openai_util import OpenAIProps
    from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps
    openai_props:OpenAIProps  = OpenAIProps.env_to_props()
    vector_db_item: VectorDBProps = VectorDBProps.get_vector_db_settings()

    params: LangChainChatParameter = LangChainChatParameter()

    params.prompt = question1
    result1 = LangChainUtil.langchain_chat(openai_props, [vector_db_item], params)

    print(result1.get("output",""))
    page_conetnt_list = result1.get("page_content_list", [])
    page_source_list = result1.get("page_source_list", [])
    for page_content, page_source in zip(page_conetnt_list, page_source_list):
        print(page_source)
        print(page_content)
        print('---------------------')
    
    verbose = result1.get("verbose", "")
    print(verbose)

