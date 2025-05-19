
from typing import Any, Sequence, Tuple, List
from calendar import c
import sys

from langchain_postgres import PGVector
from langchain_postgres.vectorstores import PGVector
from sqlalchemy.orm import Session
import sqlalchemy
from sqlalchemy.sql import text
from langchain_core.vectorstores import VectorStore
from ai_chat_lib.langchain_modules.langchain_util import LangChainOpenAIClient
from ai_chat_lib.langchain_modules.langchain_vector_db import LangChainVectorDB
from ai_chat_lib.langchain_modules.langchain_doc_store import SQLDocStore

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)
    
class LangChainVectorDBPGVector(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_url :str, collection_name: str = "", multi_vector_doc_store_url: str = "", chunk_size: int = 1024):
        super().__init__(langchain_openai_client, vector_db_url, collection_name, multi_vector_doc_store_url, chunk_size)
        self.db = self._load()
        if multi_vector_doc_store_url:
            logger.info("doc_store_url:", multi_vector_doc_store_url)
            self.doc_store = SQLDocStore(multi_vector_doc_store_url)
        else:
            logger.info("doc_store_url is None")

    def _load(self) -> VectorStore:

        # params
        params: dict[str, Any] = {}
        params["connection"] = self.vector_db_url
        params["embeddings"] = self.langchain_openai_client.get_embedding_client()
        params["use_jsonb"] = True
        
        # collectionが指定されている場合
        logger.info("collection_name:", self.collection_name)
        if self.collection_name:
            params["collection_name"] = self.collection_name

        db: VectorStore = PGVector(
            **params
            )
        return db

    def _get_document_ids_by_tag(self, name:str="", value:str="") -> Tuple[List, List]:
        engine = sqlalchemy.create_engine(self.vector_db_url)
        with Session(engine) as session:
            stmt = text("select uuid from langchain_pg_collection where name=:name")
            stmt = stmt.bindparams(name=self.collection_name)
            rows  = session.execute(stmt).fetchone()
            if rows is None or len(rows) == 0:
                return [], []
            collection_id = rows[0]
            logger.info(collection_id)
            stmt = text("select id, cmetadata from langchain_pg_embedding where collection_id=:collection_id and cmetadata->>:name=:value")
            stmt = stmt.bindparams(collection_id=collection_id, name=name, value=value)
            rows2: Sequence[Any] = session.execute(stmt).all() 
            document_ids = [row[0] for row in rows2]
            metadata_list = [row[1] for row in rows2]
            
            return document_ids, metadata_list


