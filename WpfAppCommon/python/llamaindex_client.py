import os, sys

from llama_index.llms.openai import OpenAI
from llama_index.embeddings.openai import OpenAIEmbedding

from llama_index.llms.azure_openai import AzureOpenAI
from llama_index.embeddings.azure_openai import AzureOpenAIEmbedding

from openai_props import OpenAIProps

class LlamaIndexClient:
    def __init__(self, props: OpenAIProps):
        
        self.props = props

    def get_completion_client(self):
        
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_completion_dict()
            llm = AzureOpenAI(
                **params
            )

        else:
            params =self.props.create_openai_completion_dict()
            llm = OpenAI(
                **params
            )
        return llm
        
    def get_embedding_client(self):
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_embedding_dict()
            embeddings = AzureOpenAIEmbedding(
                **params
            )
        else:
            params =self.props.create_openai_embedding_dict()
            embeddings = OpenAIEmbedding(
                **params
            )
        return embeddings
        