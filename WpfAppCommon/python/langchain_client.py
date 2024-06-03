import os, sys

from langchain_openai import AzureOpenAIEmbeddings
from langchain_openai import AzureChatOpenAI
from langchain_openai import OpenAIEmbeddings
from langchain_openai import ChatOpenAI
from openai_props import OpenAIProps

class LangChainOpenAIClient:
    def __init__(self, props: OpenAIProps):
        
        self.props = props

    def get_completion_client(self):
        
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_completion_dict()
            llm = AzureChatOpenAI(
                **params
            )

        else:
            params =self.props.create_openai_completion_dict()
            llm = ChatOpenAI(
                **params
            )
        return llm
        
    def get_embedding_client(self):
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_embedding_dict()
            embeddings = AzureOpenAIEmbeddings(
                **params
            )
        else:
            params =self.props.create_openai_embedding_dict()
            embeddings = OpenAIEmbeddings(
                **params
            )
        return embeddings
        