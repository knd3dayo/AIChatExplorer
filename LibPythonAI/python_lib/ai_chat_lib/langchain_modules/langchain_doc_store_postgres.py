
import os, json, sys
from langchain.docstore.document import Document
from langchain_core.stores import BaseStore
from typing import Sequence, Optional, Tuple, Iterator, Union, TypeVar
from sqlalchemy import create_engine
from sqlalchemy import text
from ai_chat_lib.langchain_modules.langchain_doc_store import SQLDocStore

sys.path.append("python")
K = TypeVar("K")
V = TypeVar("V")

class SQLDocStorePostgres(SQLDocStore):
    
    def __init__(self, url:str):
        self.url = url
        self.engine  = create_engine(url)
        connection = self.engine.connect()
        # documentsテーブルがなければ作成
        sql = text("CREATE TABLE IF NOT EXISTS documents (id TEXT PRIMARY KEY, data TEXT)")
        connection.execute(sql)
        connection.commit()
        connection.close()
