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
        azure_openai_endpoint = props.get("AzureOpenAIEndpoint", None)
        openai_completion_base_url = props.get("OpenAICompletionBaseURL", None)
        openai_embedding_base_url = props.get("OpenAIEmbeddingBaseURL", None)
        
        # azure_openaiがTrueの場合、AzureOpenAIを使用する.azure_openai_stringを大文字にしてTRUEの場合はTrueに変換する
        
        azure_openai = azure_openai_string.upper() == "TRUE"

        if (azure_openai):
            params = {}
            params["openai_api_key"] = openai_api_key
            params["openai_api_version"] = "2023-12-01-preview"

            if openai_embedding_base_url:
                params["base_url"] = openai_embedding_base_url
            else:
                params["base_url"] = azure_openai_endpoint
                
            embeddings = AzureOpenAIEmbeddings(
                **params
            )

            params["temperature"] = 0.5
            if openai_completion_base_url:
                params["base_url"] = openai_completion_base_url
            else:
                params["base_url"] = azure_openai_endpoint

            llm = AzureChatOpenAI(
                **params
            )

            return embeddings, llm
        else:
            embeddings = OpenAIEmbeddings(
                openai_api_key=openai_api_key,
                base_url=openai_completion_base_url
            )
            
            llm = ChatOpenAI(
                temperature=0.5,
                openai_api_key=openai_api_key,
                base_url=openai_embedding_base_url,
            )
            return embeddings, llm
        