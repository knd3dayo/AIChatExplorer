
from typing import Any, Sequence, Tuple, List
from calendar import c
import sys

from langchain_postgres import PGVector
from langchain_postgres.vectorstores import PGVector
from sqlalchemy.orm import Session
import sqlalchemy
from sqlalchemy.sql import text
from langchain_core.vectorstores import VectorStore
from ai_app_langchain.ai_app_langchain_util import LangChainOpenAIClient
from ai_app_langchain.langchain_vector_db import LangChainVectorDB
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps
    
class LangChainVectorDBPGVector(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
        super().__init__(langchain_openai_client, vector_db_props)


    def _load(self):
        # VectorDBTypeStringが"PGVector"でない場合は例外をスロー
        if self.vector_db_props.VectorDBTypeString != "PGVector":
            raise ValueError("VectorDBTypeString must be 'PGVector'")

        # params
        params = {}
        params["connection"] = self.vector_db_props.VectorDBURL
        params["embeddings"] = self.langchain_openai_client.get_embedding_client()
        params["use_jsonb"] = True
        
        # collectionが指定されている場合
        print("CollectionName:", self.vector_db_props.CollectionName)
        if self.vector_db_props.CollectionName:
            params["collection_name"] = self.vector_db_props.CollectionName
        
        self.db: VectorStore = PGVector(
            **params
            )

    def _get_document_ids_by_tag(self, name:str="", value:str="") -> Tuple[List, List]:
        engine = sqlalchemy.create_engine(self.vector_db_props.VectorDBURL)
        with Session(engine) as session:
            stmt = text("select uuid from langchain_pg_collection where name=:name")
            stmt = stmt.bindparams(name=self.vector_db_props.CollectionName)
            rows  = session.execute(stmt).fetchone()
            if rows is None or len(rows) == 0:
                return [], []
            collection_id = rows[0]
            print(collection_id)
            stmt = text("select id, cmetadata from langchain_pg_embedding where collection_id=:collection_id and cmetadata->>:name=:value")
            stmt = stmt.bindparams(collection_id=collection_id, name=name, value=value)
            rows2: Sequence[Any] = session.execute(stmt).all() 
            document_ids = [row[0] for row in rows2]
            metadata_list = [row[1] for row in rows2]
            
            return document_ids, metadata_list


