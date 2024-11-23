
import json, sys
from langchain_core.messages import AIMessage, HumanMessage, SystemMessage
from langchain_openai import AzureOpenAIEmbeddings
from langchain_openai import AzureChatOpenAI
from langchain_openai import OpenAIEmbeddings
from langchain_openai import ChatOpenAI


from typing import Any



from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps

class LangChainOpenAIClient:
    def __init__(self, props: OpenAIProps):
        
        self.props: OpenAIProps = props

    def get_completion_client(self):
        
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_completion_dict()
            # modelを設定する。
            params["model"] = self.props.OpenAICompletionModel
            llm = AzureChatOpenAI(
                **params
            )

        else:
            params =self.props.create_openai_completion_dict()
            # modelを設定する。
            params["model"] = self.props.OpenAICompletionModel
            llm = ChatOpenAI(
                **params
            )
        return llm
        
    def get_embedding_client(self):
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_embedding_dict()
            # modelを設定する。
            params["model"] = self.props.OpenAIEmbeddingModel
            embeddings = AzureOpenAIEmbeddings(
                **params
            )
        else:
            params =self.props.create_openai_embedding_dict()
            # modelを設定する。
            params["model"] = self.props.OpenAIEmbeddingModel
            embeddings = OpenAIEmbeddings(
                **params
            )
        return embeddings
        

class LangChainChatParameter:
    def __init__(self, request_json: str):

        # request_jsonをdictに変換
        request_dict = json.loads(request_json)
        # messagesを取得
        messages = request_dict.get("messages", [])
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
        self.chat_history = LangChainChatParameter.convert_to_langchain_chat_history(chat_history_json)
        # デバッグ出力
        print ("LangChainChatParameter, __init__")
        print(f'prompt: {self.prompt}')
        print(f'chat_history: {self.chat_history}')

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
