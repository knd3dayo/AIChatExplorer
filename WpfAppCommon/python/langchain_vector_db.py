
import os, json, sys
import uuid
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback

sys.path.append("python")
from langchain_client import LangChainOpenAIClient

class LangChainVectorDB:

    def __init__(self, langchain_openai_client: LangChainOpenAIClient,
                 vector_db_url, collection : str = None, doc_store_url=None):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_url = vector_db_url
        self.collection = collection
        self.doc_store_url = doc_store_url

        self.load()

    def load(self):
        pass

    def _save(self, documents:list=[]):
        pass

    def _delete(self, sources:list=[]):
        pass

    def vector_search(self, query, k=10 , score_threshold=0.0):
        answers = self.db.similarity_search_with_relevance_scores(
            query, k=k, score_threshold=score_threshold)

        return answers

    def add_documents(self, documents: list):
        # doc_store_urlが指定されている場合はdoc_idsを取得して、documentsに追加
        if self.doc_store_url:
            id_key = "doc_id"
            doc_ids = []
            for doc in documents:
                doc_id = str(uuid.uuid4())
                doc_ids.append(doc.doc_id)
                doc.metadata[id_key] = doc_id
            # docstoreを取得
                
        self._save(documents)

        return len(documents)
        
    def delete_doucments(self, sources :list ):
        self._delete(sources)
            
        return len(sources)
    


        
    
