
import os, json, sys
from langchain_community.vectorstores import FAISS
from langchain_core.vectorstores import VectorStore
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback

sys.path.append("python")
from langchain_client import LangChainOpenAIClient
from langchain_vector_db import LangChainVectorDB
from openai_props import VectorDBProps

class LangChainVectorDBFaiss(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
        super().__init__(langchain_openai_client, vector_db_props)

    def load(self, docs:list=[]):
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not self.vector_db_props.VectorDBURL or not os.path.exists(self.vector_db_props.VectorDBURL):
            # ディレクトリを作成
            os.makedirs(self.vector_db_props.VectorDBURL)
        if len(os.listdir(self.vector_db_props.VectorDBURL)) == 0:    
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
                self.vector_db_props.VectorDBURL, self.langchain_openai_client.embeddings,
                allow_dangerous_deserialization=True
                )  

    def _save(self, documents:list=[]):
        if not self.vector_db_props.VectorDBURL:
            return
        # 新しいDBを作成
        new_db = self.load(docs=documents)
        # 既存のDBにマージ
        self.db.merge_from(new_db)
            
        self.db.save_local(self.vector_db_props.VectorDBURL)

    def _delete(self, sources:list=None):
        if not self.vector_db_props.VectorDBURL:
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
            self._save(self.vector_db_props.VectorDBURL)
        return len(doc_ids)
    
