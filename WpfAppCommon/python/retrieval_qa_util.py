
from io import BytesIO
from math import fabs
import os, json, sys
from langchain.prompts import PromptTemplate
from langchain.agents import create_openai_tools_agent, AgentExecutor
from langchain.prompts import PromptTemplate
from langchain.tools import Tool
from langchain.docstore.document import Document
from langchain.chains.qa_with_sources.retrieval import RetrievalQAWithSourcesChain
from langchain.tools import Tool
from langchain.docstore.document import Document
from langchain_core.messages import AIMessage, HumanMessage, SystemMessage

sys.path.append("python")
from langchain_openai_client import LangChainOpenAIClient
from langchain_vector_db import LangChainVectorDB

class RetrievalQAUtil:

    def __init__(self, client: LangChainOpenAIClient, vector_db_items:dict):
        self.client = client

        # ツールのリストを作成
        self.load_tools = self.__create_tool_list(vector_db_items)
        self.tools = self.load_tools

    def __create_tool_list(self, vector_db_items=None):

        tools = []

        for item in vector_db_items:
            name = item.get("name", "")
            collection_name = item.get("collection_name", "")
            description = item.get("description", "")
            prompt = item.get("prompt", "")
            vector_db_url = item.get("vector_db_url", "")
            vector_db_type_string = item.get("vector_db_type_string", "")

            tools.append(self.__create_tool(name, vector_db_type_string, vector_db_url, collection_name, prompt, description))
  
        return tools

    def __create_tool(self, name, vector_db_type_string, vector_db_url, collection_name, prompt_str, description):
        '''
        # 関数の説明
        # ツールを追加する。
        #
        # 引数
        # name: str
        #   ツール名
        # vector_db_type_string: str
        #   ベクトルDBのタイプ
        # vector_db_url: str
        #   ベクトルDBのURL
        # collection_name: str
        #   コレクション名
        # prompt_str: str
        #   プロンプト
        # description: str
        #   ツールの説明
        # 戻り値
        # tool: Tool
        #   ツール
        '''
        # RetrievalQAオブジェクトを作成して、Toolオブジェクトを作成
        qa = self.__create_retrieval_qa(vector_db_type_string, vector_db_url, collection_name, prompt_str)
        print(name)
        print(qa)
        print(description)
        tool = Tool(
                name=name,
                func=qa,
                description = description
            )
        return tool

    def __create_default_prompt_template(self):
        '''
        # 関数の説明
        # デフォルトのプロンプトのテンプレートを作成する。
        # 
        # 引数
        # なし
        # 戻り値
        # prompt_template_str: str
        #   プロンプトのテンプレート
        # 例
        # prompt_template_str = create_default_prompt_template()
        '''
        # デフォルトのプロンプトのテンプレート
        # contextはベクトルDB検索用のRetrieverオブジェクトの検索結果のcontext
        # questionはユーザーからの質問
        prompt_template_str = """あなたは親切で優しいアシスタントです。丁寧に、日本語でお答えください！
        もし以下の情報が探している情報に関連していない場合は、不明とのみ答えてください。

        {summaries}

        質問: {question}
        回答（日本語）:"""
        return prompt_template_str

    
    def __create_retrieval_qa(self, vector_db_type_string: str , vector_db_url: str, collection_name: str, prompt_template_str: str = None) -> RetrievalQAWithSourcesChain: 
        '''
        # 関数の説明
        # prompt_template_strからPromptTemplateを作成する。
        # PromptTemplateからchain_type_kwargsを作成する。
        # vectorstore, llm, chain_type_kwargsからRetrievalQAを作成する。
        # 引数:
        # prompt_template_str: str
        #   プロンプトのテンプレート
        # 戻り値:
        # RetrievalQA
        #   RetrievalQAオブジェクト
        # 例:
        # retrieval_qa = create_retrieval_qa(vectorstore, llm, prompt_template_str)
        '''
        if not prompt_template_str:
            # デフォルトのプロンプトのテンプレート文字列を作成
            prompt_template_str = self.__create_default_prompt_template()

        prompt = PromptTemplate(
                template=prompt_template_str, 
                input_variables=["summaries", "question"],
                output_variables=["answer", "source_documents"]
        )
        chain_type_kwargs = {"prompt": prompt}

        # ベクトルDB検索用のRetrieverオブジェクトの作成と設定
        # vector_db_type_stringが"Faiss"の場合、FaissVectorDBオブジェクトを作成
        if vector_db_type_string == "Faiss":
            from langchain_vector_db_faiss import LangChainVectorDBFaiss
            langChainVectorDB = LangChainVectorDB(self.client, vector_db_url)
            retriever = langChainVectorDB.db.as_retriever(
                search_kwargs={"score_threshold": 0.5}
            )
        # それ以外の場合は現在未対応のためExceptionを発生させる
        else:
            raise Exception("Unsupported vector_db_type_string: " + vector_db_type_string)
            
        # RetrievalQAオブジェクトを作成して、Toolオブジェクトを作成
        # langchainのエージェントはユーザーからの質問が来た場合、それがどのツールに対する質問なのかを判断する。
        # 料理に関する質問が来た場合、料理に関する質問に答えるツールを呼び出す。
        qa = RetrievalQAWithSourcesChain.from_chain_type(
            llm=self.client.llm,
            chain_type='stuff',
            retriever=retriever,
            return_source_documents=True,
            chain_type_kwargs=chain_type_kwargs
        )

        return qa
    
    def create_agent_executor(self):
        '''
        # 関数の説明
        # chat_agentを作成する。
        # chat_agentは、ユーザーからの質問に答える。
        # 
        # 引数
        # なし
        # 戻り値
        # chat_agent: ChatAgent
        #   チャットエージェント
        # 例
        # chat_agent = create_agent()
        '''
        from langchain_core.prompts import ChatPromptTemplate, MessagesPlaceholder

        prompt = ChatPromptTemplate.from_messages(
            [
                ("system", "You are a helpful assistant"),
                MessagesPlaceholder("chat_history", optional=True),
                ("human", "{input}"),
                MessagesPlaceholder("agent_scratchpad"),
            ]
        )
        # ChatAgentオブジェクトを作成
        chat_agent = create_openai_tools_agent(
                self.client.llm,
                self.tools,
                prompt
        )
        agent_executor = AgentExecutor(
            agent=chat_agent, tools=self.tools, 
            return_source_documents=True,
            return_intermediate_steps=True,
            stream_runnable=False
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
        
        for step in intermediate_steps:
            observation = step[1]
            source_documents = observation.get("source_documents",[])
            for source_document in source_documents:
                source_document: Document = source_document
                source = source_document.metadata.get("source","")
                source_url = source_document.metadata.get("source_url","")
                
                page_content_list.append(source_document.page_content)
                page_source_list.append({"source": source, "source_url": source_url})
        
            # verbose情報を取得
            verbose = self.serialize_intermediate_step(step)
            verbose_list.append(verbose)
        
        # verbose情報をjson文字列に変換
        verbose_json = json.dumps(verbose_list, ensure_ascii=False, indent=4)    
        
        return page_content_list, page_source_list, verbose_json
    
    # intermediate_stepsをシリアライズする
    def serialize_intermediate_step(self, step):
        return {
            "tool": step[0].tool,
            "tool_input": step[0].tool_input,
            "log": step[0].log,
            "message_log": str(step[0].message_log),
            "tool_call_id": step[0].tool_call_id,
            "output": str(step[1]),
        }
    def convert_to_langchain_chat_history(self, chat_history_json: str):
        # openaiのchat_historyをlangchainのchat_historyに変換
        langchain_chat_history = []
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

# グローバル変数
RetrievalQAUtilInstance: RetrievalQAUtil = None
ChatAgentExecutorInstance = None

def langchain_chat( props: dict, vector_db_items_json: str, prompt: str, chat_history_json: str = None):
    global RetrievalQAUtilInstance, ChatAgentExecutorInstance
    if RetrievalQAUtilInstance is None:
        import langchain
        # langchainのログを出力する
        langchain.verbose = True
        
        # vector_db_items_jsonをjsonオブジェクトに変換
        vector_db_items = json.loads(vector_db_items_json)

        client = LangChainOpenAIClient(props)
        RetrievalQAUtilInstance = RetrievalQAUtil(client, vector_db_items)
        ChatAgentExecutorInstance = RetrievalQAUtilInstance.create_agent_executor()

    # openaiのchat_historyのjson文字列をlangchainのchat_historyに変換
    if chat_history_json is not None:
        chat_history = RetrievalQAUtilInstance.convert_to_langchain_chat_history(chat_history_json)
    else:
        chat_history = []

    
    result = ChatAgentExecutorInstance.invoke(
            {
                "input": prompt,
                "chat_history": chat_history,
            }
        )
        
    result_dict = {}
    result_dict["output"] = result.get("output", "")
    page_conetnt_list, page_source_list, verbose_json = RetrievalQAUtilInstance.process_intermadiate_steps(result.get("intermediate_steps",[]))
    result_dict["page_content_list"] = page_conetnt_list
    result_dict["page_source_list"] = page_source_list
    result_dict["verbose"] = verbose_json
    return result_dict

if __name__ == '__main__':

    import env_to_props
    props = env_to_props.get_props()
    vector_db_items_json = env_to_props.get_vector_db_settings()

    question1 = input("質問をどうぞ:")
    result1 = langchain_chat(props, vector_db_items_json, question1)

    print(result1.get("output",""))
    page_conetnt_list = result1.get("page_content_list", [])
    page_source_list = result1.get("page_source_list", [])
    for page_content, page_source in zip(page_conetnt_list, page_source_list):
        print(page_source)
        print(page_content)
        print('---------------------')
