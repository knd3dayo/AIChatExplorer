import os, sys

from typing import Tuple, List, Any
import chromadb.config
from langchain_chroma.vectorstores import Chroma # type: ignore
import chromadb
from langchain_core.vectorstores import VectorStore # type: ignore
from ai_chat_lib.langchain_modules.langchain_util import LangChainOpenAIClient
from ai_chat_lib.langchain_modules.langchain_vector_db import LangChainVectorDB

from ai_chat_lib.db_modules import VectorDBItem
from ai_chat_lib.langchain_modules.langchain_doc_store import SQLDocStore

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

class LangChainVectorDBChroma(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_url :str, collection_name: str = "", multi_vector_doc_store_url: str = "", chunk_size: int = 1024):
        super().__init__(langchain_openai_client, vector_db_url, collection_name,  multi_vector_doc_store_url, chunk_size)
        self.db = self._load()
        if multi_vector_doc_store_url:
            logger.info(f"doc_store_url: {multi_vector_doc_store_url}")
            self.doc_store = SQLDocStore(multi_vector_doc_store_url)
        else:
            logger.info("doc_store_url is None")

    def _load(self) -> VectorStore:

        # ベクトルDB用のディレクトリが存在しない場合
        if not os.path.exists(self.vector_db_url):
            # ディレクトリを作成
            os.makedirs(self.vector_db_url)
            # ディレクトリが作成されたことをログに出力
            logger.info(f"create directory:{self.vector_db_url}")
        # params
        settings = chromadb.config.Settings(anonymized_telemetry=False)

        params: dict[str, Any]= {}
        params["client"] = chromadb.PersistentClient(path=self.vector_db_url, settings=settings)
        params["embedding_function"] = self.langchain_openai_client.get_embedding_client()
        params["collection_metadata"] = {"hnsw:space":"cosine"}
        # collectionが指定されている場合
        logger.info(f"collection_name:{self.collection_name}")
        if self.collection_name:
            params["collection_name"] = self.collection_name
                    
        db: VectorStore = Chroma(
            **params
            )
        return db

    def _get_document_ids_by_tag(self, name:str="", value:str="") -> Tuple[List, List]:
        ids=[]
        metadata_list = []
        doc_dict = self.db.get(where={name: value}) # type: ignore

        # デバッグ用
        logger.debug("_get_document_ids_by_tag doc_dict:", doc_dict)

        # vector idを取得してidsに追加
        ids.extend(doc_dict.get("ids", []))
        metadata_list.extend(doc_dict.get("metadata", []))

        return ids, metadata_list

        

