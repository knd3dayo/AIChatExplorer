import os, sys

from langchain_openai import AzureOpenAIEmbeddings
from langchain_openai import AzureChatOpenAI
from langchain_openai import OpenAIEmbeddings
from langchain_openai import ChatOpenAI

class LangChainOpenAIClient:
    def __init__(self, props: dict):
        if props == {}: 
            raise ValueError("props is empty")
        
        self.embeddings, self.llm = self.__get_openai_object(props)

    def __get_openai_object(self, props: dict):
        # OpenAIオブジェクトを作成
        openai_api_key = props.get("OpenAIKey", None)
        azure_openai_endpoint = props.get("AzureOpenAIEndpoint", None)
        chat_model_name = props.get("OpenAICompletionModel", None)
        embedding_model_name = props.get("OpenAIEmbeddingModel", None)
        azure_openai_string = props.get("AzureOpenAI", False)
        openai_base_url = props.get("OpenAIBaseURL", None)
        # azure_openaiがTrueの場合、AzureOpenAIを使用する.azure_openai_stringを大文字にしてTRUEの場合はTrueに変換する
        
        azure_openai = azure_openai_string.upper() == "TRUE"

        if (azure_openai):
            embeddings = AzureOpenAIEmbeddings(
                azure_deployment=embedding_model_name,
                openai_api_version="2023-12-01-preview",
                base_url=azure_openai_endpoint,
                openai_api_key=openai_api_key,
                azure_endpoint=azure_openai_endpoint
            )
            llm = AzureChatOpenAI(
                azure_deployment=azure_openai_endpoint,
                openai_api_version="2023-12-01-preview",
                temperature=0.5,
                base_url=openai_base_url,
                openai_api_key=openai_api_key,
                azure_endpoint=azure_openai_endpoint
            )
            return embeddings, llm
        else:
            embeddings = OpenAIEmbeddings(
                openai_api_key=openai_api_key,
                base_url=openai_base_url
            )
            
            llm = ChatOpenAI(
                openai_api_key=openai_api_key,
                base_url=openai_base_url
            )
            return embeddings, llm
        