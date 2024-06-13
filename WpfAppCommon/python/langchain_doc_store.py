
import os, json, sys
import uuid
from langchain.docstore.document import Document
from langchain_core.stores import BaseStore
from typing import Sequence, Optional, Tuple, Iterator, Union, TypeVar
from sqlalchemy import create_engine
from sqlalchemy import select

sys.path.append("python")
K = TypeVar("K")
V = TypeVar("V")

class SQLDocStore(BaseStore):
    
    def __init__(self, url:str):
        self.url = url
        self.engine  = create_engine(url)
        connection = self.engine.connect()
        # documentsテーブルがなければ作成
        connection.execute("CREATE TABLE IF NOT EXISTS documents (id TEXT PRIMARY KEY, data TEXT)")
        connection.commit()
        connection.close()
        
        
    def mdelete(self, keys: Sequence[K]) -> None:
        # documentsテーブルから指定されたkeyのレコードを削除
        connection = self.engine.connect()
        for key in keys:
            connection.execute("DELETE FROM documents WHERE id = ?", (key,))
        connection.commit()
        connection.close()
            
    
    def mget(self, keys: Sequence[K]) -> list[Optional[V]]:
        # documentsテーブルから指定されたkeyのレコードを取得
        connection = self.engine.connect()
        result = []
        for key in keys:
            row = connection.execute("SELECT data FROM documents WHERE id = ?", (key,)).fetchone()
            if row is None:
                result.append(None)
            else:
                result.append(row[0])
        connection.close()
        return result
    
    
    def mset(self, key_value_pairs: Sequence[Tuple[K, V]]) -> None:
        # documentsテーブルにkey-valueのペアを保存. keyが既に存在する場合は上書き
        connection = self.engine.connect()
        for key, value in key_value_pairs:
            connection.execute("INSERT OR REPLACE INTO documents (id, data) VALUES (?, ?)", (key, value))
        connection.commit()
        connection.close()
    
    def yield_keys(*, prefix: Optional[str] = None) -> Union[Iterator[K], Iterator[str]]:
        pass
    
