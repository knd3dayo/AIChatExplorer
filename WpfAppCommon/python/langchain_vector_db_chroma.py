
import os, json, sys, uuid
from langchain_community.vectorstores import Chroma
from langchain_core.vectorstores import VectorStore
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback
import chromadb

sys.path.append("python")
from langchain_client import LangChainOpenAIClient
from langchain_vector_db import LangChainVectorDB
from openai_props import VectorDBProps

class LangChainVectorDBChroma(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
        super().__init__(langchain_openai_client, vector_db_props)


    def load(self):
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not self.vector_db_props.VectorDBURL or not os.path.exists(self.vector_db_props.VectorDBURL):
            # ディレクトリを作成
            os.makedirs(self.vector_db_props.VectorDBURL)
        # params
        params = {}
        params["client"] = chromadb.PersistentClient(path=self.vector_db_props.VectorDBURL)
        params["embedding_function"] = self.langchain_openai_client.get_embedding_client()
        # collectionが指定されている場合
        if self.vector_db_props.CollectionName:
            params["collection_name"] = self.vector_db_props.CollectionName
        
        self.db = Chroma(
            **params
            )

    def _save(self, documents:list=None):
        if not self.vector_db_props.VectorDBURL:
            return
        
        self.db.add_documents(documents=documents, embedding=self.langchain_openai_client.get_embedding_client())

        
    def _delete(self, sources:list=None):
        # 既存のDBから指定されたsourceを持つドキュメントを削除
        for source in sources:
            docs = self.db.get(where={"source": source})

            print("docs:", docs)
            
            # docsのmetadataのdoc_idを取得
            metadatas = docs.get("metadatas", [])
            ids = [metadata.get("doc_id", None) for metadata in metadatas]
            self._delete_docstore_data(doc_ids=ids)

            ids = docs.get("ids", [])
            if len(ids) > 0:
                self.db._collection.delete(ids=ids)

        return len(ids)    
 
