
import os, json, sys
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback

sys.path.append("python")
from langchain_openai_client import LangChainOpenAIClient

class LangChainVectorDB:

    def __init__(self, langchain_openai_client: LangChainOpenAIClient,
                 vector_db_url, collection : str = None):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_url = vector_db_url
        self.collection = collection

        self.__load(vector_db_url)

    def __load(self, _vector_db_url=None):
        pass

    def __save(self, _vector_db_url, documents:list=None):
        pass

    def vector_search(self, query, k=10 , score_threshold=0.0):
        answers = self.db.similarity_search_with_relevance_scores(
            query, k=k, score_threshold=score_threshold)

        return answers

    def add_documents(self, documents: list):
        total_tokens = 0
        with get_openai_callback() as cb:
            self.__save(self.vector_db_url, documents)
            total_tokens = cb.total_tokens
        
        return total_tokens

    def delete_doucments_by_sources(self, sources :list ):
        doc_ids = []
        # 既存のDBから指定されたsourceを持つドキュメントを削除
        
        for _id, doc in self.db.docstore._dict.items():
            if not doc.metadata.get("source_url", None):
                if doc.metadata.get("source", None) in [source.metadata.get("source",None) for source in sources]:
                    doc_ids.append(_id)
            else:
                source_url_check =  doc.metadata.get("source_url", None)in [source.metadata.get("source_url", None) for source in sources]
                source_path_check = doc.metadata.get("source", None) in [source.metadata.get("source", None) for source in sources]
                if source_url_check and source_path_check:
                    doc_ids.append(_id)

        if len(doc_ids) > 0:
            self.db.delete(doc_ids)
            self.__save(self.vector_db_url)
            
        return 0
    
    def update_documents(self, documents: list, props: dict):
        token_count = 0
        if len(documents) == 0:
            print("No documents to update.")
            return token_count
        
        # 既存のDBからソースファイルが一致するドキュメントを削除
        self.delete_doucments_by_sources(documents)
        return self.add_documents(documents)
        
    
