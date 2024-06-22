
import json, sys
from langchain.prompts import PromptTemplate
from langchain.agents import create_openai_tools_agent, AgentExecutor
from langchain.prompts import PromptTemplate
from langchain.tools import Tool
from langchain.docstore.document import Document
from langchain.chains.qa_with_sources.retrieval import RetrievalQAWithSourcesChain
from langchain.tools import Tool
from langchain.docstore.document import Document
from langchain_core.messages import AIMessage, HumanMessage, SystemMessage
from langchain.retrievers.multi_vector import MultiVectorRetriever

import langchain

sys.path.append("python")
from langchain_client import LangChainOpenAIClient
from openai_props import OpenAIProps, VectorDBProps

# DB取得用の関数
def get_vector_db(client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
    db_type = vector_db_props.VectorDBTypeString
    if db_type == "Faiss":
        from langchain_vector_db_faiss import LangChainVectorDBFaiss
        return LangChainVectorDBFaiss(client, vector_db_props)
    elif db_type == "Chroma":
        from langchain_vector_db_chroma import LangChainVectorDBChroma
        return LangChainVectorDBChroma(client, vector_db_props)
    else:
        raise Exception("Unsupported vector_db_type_string: " + db_type)

class RetrieverUtil:
    
    def __init__(self,  client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
        self.client = client
        self.vector_db_props = vector_db_props

    def create_retriever(self):
        vector_db_props = self.vector_db_props
        # for refine 2024/06/08 Error Raised: 2 validation errors for RefineDocumentsChain
        # chain_type_kwargs = {"prompt": prompt,  "existing_answer": ""}

        # ベクトルDB検索用のRetrieverオブジェクトの作成と設定
        # vector_db_type_stringが"Faiss"の場合、FaissVectorDBオブジェクトを作成
        print("CollectionName:", vector_db_props.CollectionName)

        # IsUseMultiVectorRetriever=Trueの場合はMultiVectorRetrieverを生成
        if vector_db_props.IsUseMultiVectorRetriever:
            langChainVectorDB = get_vector_db(self.client, vector_db_props)
            retriever = MultiVectorRetriever(
                vectorstore=langChainVectorDB.db,
                docstore=langChainVectorDB.doc_store,
                id_key="doc_id",
                search_kwargs={"k": 10}
            )

        else:
            langChainVectorDB = get_vector_db(self.client, vector_db_props)
            retriever = langChainVectorDB.db.as_retriever(
                # search_type="similarity_score_threshold", search_kwargs={"score_threshold": 0.5}
                search_kwargs={"k": 10}
            )
         
        return retriever



class RetrievalQAUtil:

    def __init__(self, client: LangChainOpenAIClient, vector_db_items:list[VectorDBProps]):
        self.client = client
        self.vector_db_items = vector_db_items

        # ツールのリストを作成
        self.load_tools = self.__create_tool_list()
        self.tools = self.load_tools

    def __create_tool_list(self):

        tools = []

        for item in self.vector_db_items:
            tools.append(self.__create_tool(item))
  
        return tools

    def __create_tool(self, vector_db_item:VectorDBProps):
        '''
        # 関数の説明
        # ツールを追加する。
        #
        # 引数
        # vector_db_item: VectorDBProps
        # 戻り値
        # tool: Tool
        #   ツール
        '''
        # RetrievalQAオブジェクトを作成して、Toolオブジェクトを作成
        qa = self.__create_retrieval_qa(vector_db_item)
        tool = Tool(
                name=vector_db_item.Name,
                func=qa,
                description = vector_db_item.VectorDBDescription
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

    
    def __create_retrieval_qa(self, vector_db_props:VectorDBProps, prompt_template_str: str = None) -> RetrievalQAWithSourcesChain: 
        '''
        # 関数の説明
        # prompt_template_strからPromptTemplateを作成する。
        # PromptTemplateからchain_type_kwargsを作成する。
        # vectorstore, llm, chain_type_kwargsからRetrievalQAを作成する。
        # 引数:
        # vectorstore: VectorStore
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
            # for stuff
            prompt_template_str = self.__create_default_prompt_template()

        # for refine
        # prompt_template_str = "Answer the following question based on the retrieved document: {question}\n\n{context_str}"
        prompt = PromptTemplate(
                template=prompt_template_str, 
                # for stuff 
                input_variables=["question", "summaries"],
                output_variables=["answer", "source_documents"]
                # for refine 2024/06/08 Error Raised: 2 validation errors for RefineDocumentsChain
                # input_variables=["question", "context_str"],
                # output_variables=["source_documents"]   
        )
        
        # for stuff
        chain_type_kwargs = {"prompt": prompt}

        # Retrieverを作成
        retriever = RetrieverUtil(self.client, vector_db_props).create_retriever()
            
        # RetrievalQAオブジェクトを作成して、Toolオブジェクトを作成
        # langchainのエージェントはユーザーからの質問が来た場合、それがどのツールに対する質問なのかを判断する。
        # 料理に関する質問が来た場合、料理に関する質問に答えるツールを呼び出す。
        qa = RetrievalQAWithSourcesChain.from_chain_type(
            llm=self.client.get_completion_client(),
            # stuff , map_reduce , refine , map_relankなどを指定
            # chain_type='refine',
            chain_type='stuff',
            retriever=retriever,
            return_source_documents=True,
            chain_type_kwargs=chain_type_kwargs,
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
                self.client.get_completion_client(),
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

def run_langchain_chat( props_json: str, prompt, request_json: str):
    
    # request_jsonをdictに変換
    request = json.loads(request_json)
    # messagesを取得
    messages = request.get("messages", [])
    # messagesのlengthが0の場合はエラーを返す
    if len(messages) == 0:
        raise Exception("messages is empty")
    
    # messagesの最後のメッセージを取得
    last_message = messages[-1]

    # contentを取得
    content = last_message.get("content", {})
    # contentのうちtype: textのもののtextを取得
    prompt_array = [ c["text"] for c in content if c["type"] == "text"]
    # prpmpt_arrayのlengthが0の場合はエラーを返す
    if len(prompt_array) == 0:
        raise Exception("prompt is empty")
    
    # promptを取得
    prompt = prompt_array[0]

    # promptが空の場合はエラーを返す
    if prompt == "":
        raise Exception("prompt is empty")
    
    # messagesから最後のメッセージを削除
    messages.pop()
    # messagesをjson文字列に変換
    chat_history_json = json.dumps(messages, ensure_ascii=False, indent=4)

    # OpenAIPorpsを生成
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    vector_db_props = openai_props.VectorDBItems

    # デバッグ出力
    print(f'プロンプト: {prompt}')
    print(f'chat_history_json: {chat_history_json}')
    print('ベクトルDBの設定')
    for item in vector_db_props:
        print(f'Name:{item.Name} VectorDBDescription:{item.VectorDBDescription} VectorDBTypeString:{item.VectorDBTypeString} VectorDBURL:{item.VectorDBURL} CollectionName:{item.CollectionName}')
        
    # langchan_chatを実行
    result = langchain_chat(openai_props, vector_db_props, prompt, chat_history_json)
    return result

def langchain_chat( props: OpenAIProps, vector_db_items: list[VectorDBProps], prompt: str, chat_history_json: str = None):

    # langchainのログを出力する
    langchain.verbose = True
        
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

    from openai_props import OpenAIProps, VectorDBProps, env_to_props, get_vector_db_settings
    props:OpenAIProps  = env_to_props()
    vector_db_item: VectorDBProps = get_vector_db_settings()

    question1 = input("質問をどうぞ:")
    result1 = langchain_chat(props, [vector_db_item], question1)

    print(result1.get("output",""))
    page_conetnt_list = result1.get("page_content_list", [])
    page_source_list = result1.get("page_source_list", [])
    for page_content, page_source in zip(page_conetnt_list, page_source_list):
        print(page_source)
        print(page_content)
        print('---------------------')
    
    verbose = result1.get("verbose", "")
    print(verbose)


