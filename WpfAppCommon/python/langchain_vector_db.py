
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

        self.load()

    def load(self):
        pass

    def save(self, documents:list=[]):
        pass

    def delete(self, sources:list=[]):
        pass

    def vector_search(self, query, k=10 , score_threshold=0.0):
        answers = self.db.similarity_search_with_relevance_scores(
            query, k=k, score_threshold=score_threshold)

        return answers

    def add_documents(self, documents: list):
        with get_openai_callback() as cb:
            self.save(documents)

            return len(documents)
        
    def delete_doucments_by_sources(self, sources :list ):
        self.delete(sources)
            
        return 0
    


        
    
