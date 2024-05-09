
import os, json, sys
from langchain_community.vectorstores import FAISS
from langchain_core.vectorstores import VectorStore
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback

sys.path.append("python")
from langchain_openai_client import LangChainOpenAIClient
from langchain_vector_db import LangChainVectorDB

class LangChainVectorDBFaiss(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient,
                 vector_db_url, collection : str = None):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_url = vector_db_url
        self.collection = collection

        self.__load(vector_db_url)

    def __load(self, _vector_db_url:str=None, docs:list=None):
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not _vector_db_url or not os.path.exists(self.vector_db_url):
            # ディレクトリを作成
            os.makedirs(self.vector_db_url)
        if len(os.listdir(self.vector_db_url)) == 0:    
            # faissのインデックスを読み込む
            if not docs:
                docs = [
                    Document(
                        page_content="",
                        metadata={"source": ""}
                    )
                ]
            self.db = FAISS.from_documents(docs, self.langchain_openai_client.embeddings)
            
        else:
            self.db :VectorStore = FAISS.load_local(
                self.vector_db_url, self.langchain_openai_client.embeddings,
                allow_dangerous_deserialization=True
                )  

    def __save(self, _vector_db_url, documents:list=None):
        if not _vector_db_url:
            return
        # 新しいDBを作成
        new_db = self.__load(docs=documents)
        # 既存のDBにマージ
        self.db.merge_from(new_db)
            
        self.db.save_local(_vector_db_url)
