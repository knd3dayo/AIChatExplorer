
import json
from langchain_core.messages import AIMessage, HumanMessage, SystemMessage
from langchain_openai import AzureOpenAIEmbeddings
from langchain_openai import AzureChatOpenAI
from langchain_openai import OpenAIEmbeddings
from langchain_openai import ChatOpenAI
from typing import Any

from ai_chat_lib.openai_modules.openai_util import OpenAIProps

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

class LangChainOpenAIClient:
    def __init__(self, props: OpenAIProps, embedding_model: str):
        
        self.props: OpenAIProps = props
        self.embedding_model: str = embedding_model

    def get_embedding_client(self):
        if not self.embedding_model:
            raise ValueError("embedding_model is not set.")

        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_dict()
            # modelを設定する。
            params["model"] = self.embedding_model
            embeddings = AzureOpenAIEmbeddings(
                **params
            )
        else:
            params =self.props.create_openai_dict()
            # modelを設定する。
            params["model"] = self.embedding_model
            embeddings = OpenAIEmbeddings(
                **params
            )
        return embeddings
        

class LangChainChatParameter:
    def __init__(self, chat_request_dict: dict):

        # messagesを取得
        messages = chat_request_dict.get("messages", [])
        # messagesのlengthが0の場合はエラーを返す
        if len(messages) == 0:
            self.prompt = ""
        else:
            # messagesの最後のメッセージを取得
            last_message: dict = messages[-1]
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
        logger.debug ("LangChainChatParameter, __init__")
        logger.debug(f'prompt: {self.prompt}')
        logger.debug(f'chat_history: {self.chat_history}')

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
