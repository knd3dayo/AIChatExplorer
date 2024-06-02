
import os, json, sys
from langchain_community.vectorstores import FAISS
from langchain_core.vectorstores import VectorStore
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback

sys.path.append("python")
from langchain_client import LangChainOpenAIClient
from langchain_vector_db import LangChainVectorDB

class LangChainVectorDBFaiss(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient,
                 vector_db_url, collection : str = None):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_url = vector_db_url
        self.collection = collection

        self.load(vector_db_url)

    def load(self, _vector_db_url:str=None, docs:list=None):
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

    def save(self, _vector_db_url, documents:list=None):
        if not _vector_db_url:
            return
        # 新しいDBを作成
        new_db = self.load(docs=documents)
        # 既存のDBにマージ
        self.db.merge_from(new_db)
            
        self.db.save_local(_vector_db_url)

    def delete(self, _vector_db_url, sources:list=None):
        if not _vector_db_url:
            return
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
            self.save(self.vector_db_url)
        return len(doc_ids)
    
