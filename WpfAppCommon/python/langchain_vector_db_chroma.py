
import os, json, sys
from langchain_community.vectorstores import Chroma
from langchain_core.vectorstores import VectorStore
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback

sys.path.append("python")
from langchain_openai_client import LangChainOpenAIClient
from langchain_vector_db import LangChainVectorDB

class LangChainVectorDBChroma(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient,
                 vector_db_url, collection : str = None):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_url = vector_db_url
        self.collection = collection

        self.__load(vector_db_url)

    def __load(self, _vector_db_url:str=None):
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not _vector_db_url or not os.path.exists(_vector_db_url):
            # ディレクトリを作成
            os.makedirs(_vector_db_url)
        self.db = Chroma(
            embedding_function = self.langchain_openai_client.embeddings, 
            persist_directory=_vector_db_url
            )

    def __save(self, _vector_db_url, documents:list=None):
        if not _vector_db_url:
            return
        self.db.add_documents(documents=documents, embedding=self.langchain_openai_client.embeddings)
        self.db.persist()

        
    def __delete(self, sources:list=None):
        # 既存のDBから指定されたsourceを持つドキュメントを削除
        docs = self.db.get(where = lambda doc: doc.metadata.get("source", None) in [source.metadata.get("source", None) for source in sources])
        self.db.delete(docs)
        self.__save(self.vector_db_url)
            