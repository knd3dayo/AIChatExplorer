
import os, json, sys

sys.path.append("python")
from llamaindex_client import LlamaIndexClient

class LlamaIndexVectorDB:

    def __init__(self, llamaindex_openai_client: LlamaIndexClient,
                 vector_db_url, collection : str = None):
        self.llamaindex_openai_client = llamaindex_openai_client
        self.vector_db_url = vector_db_url
        self.collection = collection

        self.load()

    def load(self):
        pass

    def save(self, documents:list=None):
        pass

    def delete(self, sources:list=None):
        pass

    def vector_search(self, query, k=10 , score_threshold=0.0):
        answers = self.db.similarity_search_with_relevance_scores(
            query, k=k, score_threshold=score_threshold)

        return answers

    def add_documents(self, documents: list):
        self.save(documents)

        return len(documents)
        
    def delete_doucments_by_sources(self, sources :list ):
        self.delete(sources)
            
        return 0
    


        
    
