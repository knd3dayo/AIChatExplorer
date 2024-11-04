
import os, sys
sys.path.append("python")

from typing import Tuple, List
from langchain_community.vectorstores.chroma import Chroma
import chromadb

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

    def _get_document_ids_by_tag(self, name:str="", value:str="") -> Tuple[List, List]:
        ids=[]
        metadata_list = []
        doc_dict = self.db.get(where={name: value})

        # デバッグ用
        print("_get_document_ids_by_tag doc_dict:", doc_dict)

        # vector idを取得してidsに追加
        ids.extend(doc_dict.get("ids", []))
        metadata_list.extend(doc_dict.get("metadata", []))

        return ids, metadata_list

    def _save(self, documents:list=[]):
        self.db.add_documents(documents=documents, embedding=self.langchain_openai_client.get_embedding_client())
        
    def _delete(self, doc_ids:list=[]):
        if len(doc_ids) == 0:
            return

        self.db._collection.delete(ids=doc_ids)

        return len(doc_ids)    
    
    def _delete_collection(self):
        self.db.delete_collection()
        self.db.persist()

