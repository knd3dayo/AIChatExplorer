
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

    def _load(self):
        # VectorDBTypeStringが"Chroma"でない場合は例外をスロー
        if self.vector_db_props.VectorDBTypeString != "Chroma":
            raise ValueError("VectorDBTypeString must be 'Chroma'")
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not self.vector_db_props.VectorDBURL or not os.path.exists(self.vector_db_props.VectorDBURL):
            # ディレクトリを作成
            os.makedirs(self.vector_db_props.VectorDBURL)
        # params
        params = {}
        params["client"] = chromadb.PersistentClient(path=self.vector_db_props.VectorDBURL)
        params["embedding_function"] = self.langchain_openai_client.get_embedding_client()
        # collectionが指定されている場合
        print("CollectionName:", self.vector_db_props.CollectionName)
        if self.vector_db_props.CollectionName:
            params["collection_name"] = self.vector_db_props.CollectionName
        
        self.db = Chroma(
            **params
            )

    def _get_metadata_by_source(self, sources:str=None) -> tuple[list, list]:
        ids=[]
        metadata = []
        for source in sources:
            doc_dict = self.db.get(where={"source": source})

            # デバッグ用
            print("_get_documents_by_source doc_dict:", doc_dict)

            # vector idを取得してidsに追加
            ids.extend(doc_dict.get("ids", []))
            # documentsを取得してdocumentsに追加
            metadata.extend(doc_dict.get("metadata", []))


        return ids, metadata
    
    def _save(self, documents:list=[]):
        self.db.add_documents(documents=documents, embedding=self.langchain_openai_client.get_embedding_client())
        
    def _delete(self, doc_ids:list=[]):
        if len(doc_ids) == 0:
            return

        self.db._collection.delete(ids=doc_ids)

        return len(doc_ids)    
 
