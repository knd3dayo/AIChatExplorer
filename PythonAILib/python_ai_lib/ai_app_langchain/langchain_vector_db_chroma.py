
import os, sys
sys.path.append("python")

from typing import Tuple, List
from langchain_chroma.vectorstores import Chroma
import chromadb
from langchain_core.vectorstores import VectorStore
from ai_app_langchain.ai_app_langchain_util import LangChainOpenAIClient
from ai_app_langchain.langchain_vector_db import LangChainVectorDB

from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps

class LangChainVectorDBChroma(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
        super().__init__(langchain_openai_client, vector_db_props)

    def _load(self):
        # VectorDBTypeStringが"Chroma"でない場合は例外をスロー
        if self.vector_db_props.VectorDBTypeString != "Chroma":
            raise ValueError("vector_db_type_string must be 'Chroma'")
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not self.vector_db_props.VectorDBURL or not os.path.exists(self.vector_db_props.VectorDBURL):
            # ディレクトリを作成
            os.makedirs(self.vector_db_props.VectorDBURL)
        # params
        params = {}
        params["client"] = chromadb.PersistentClient(path=self.vector_db_props.VectorDBURL)
        params["embedding_function"] = self.langchain_openai_client.get_embedding_client()
        params["collection_metadata"] = {"hnsw:space":"cosine"}
        # collectionが指定されている場合
        print("collection_name:", self.vector_db_props.CollectionName)
        if self.vector_db_props.CollectionName:
            params["collection_name"] = self.vector_db_props.CollectionName
                    
        self.db: VectorStore = Chroma(
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

        

