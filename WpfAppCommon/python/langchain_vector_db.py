
import os, json, sys
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback

sys.path.append("python")
from langchain_client import LangChainOpenAIClient

class LangChainVectorDB:

    def __init__(self, langchain_openai_client: LangChainOpenAIClient,
                 vector_db_url, collection : str = None):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_url = vector_db_url
        self.collection = collection

        self.load(vector_db_url)

    def load(self, _vector_db_url=None):
        pass

    def save(self, _vector_db_url, documents:list=None):
        print("ok")

    def delete(self, _vector_db_url, sources:list=None):
        pass

    def vector_search(self, query, k=10 , score_threshold=0.0):
        answers = self.db.similarity_search_with_relevance_scores(
            query, k=k, score_threshold=score_threshold)

        return answers

    def add_documents(self, documents: list):
        total_tokens = 0
        with get_openai_callback() as cb:
            self.save(self.vector_db_url, documents)
            total_tokens = cb.total_tokens
        
        return total_tokens

    def delete_doucments_by_sources(self, sources :list ):
        self.delete(sources)
            
        return 0
    


        
    
