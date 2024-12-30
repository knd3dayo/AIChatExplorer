
import os, json, sys
from langchain.docstore.document import Document
from langchain_core.stores import BaseStore
from typing import Sequence, Optional, Tuple, Iterator, Union, TypeVar
from sqlalchemy import create_engine
from sqlalchemy import text
import abc
from typing import Any

sys.path.append("python")
K = TypeVar("K")
V = TypeVar("V")

class SQLDocStore(BaseStore):
    
    def __init__(self, url:str):
        self.url = url
        self.engine  = create_engine(url)
        connection = self.engine.connect()
        # documentsテーブルがなければ作成
        sql = text("CREATE TABLE IF NOT EXISTS documents (id TEXT PRIMARY KEY, data TEXT)")
        connection.execute(sql)
        connection.commit()
        connection.close()
        
        
    def mdelete(self, keys: Sequence[K]) -> None:
        # documentsテーブルから指定されたkeyのレコードを削除
        connection = self.engine.connect()
        for key in keys:
            sql = text("DELETE FROM documents WHERE id = :key")
            connection.execute(sql, parameters=dict(key = key) )
        connection.commit()
        connection.close()
            
    
    def mget(self, keys: Sequence[K]) -> list[Optional[V]]:
        # documentsテーブルから指定されたkeyのレコードを取得
        connection = self.engine.connect()
        result:list[Any] = []
        for key in keys:
            sql = text("SELECT data FROM documents WHERE id = :key")
            row = connection.execute(sql, parameters=dict(key = key)).fetchall()
            for r in row:
                # 結果をjson文字列からdictに変換
                dict_item = json.loads(r[0])
                # dict_itemからDocumentを作成
                result.append(Document(page_content=dict_item["page_content"], metadata=dict_item["metadata"]))
                
        connection.close()
        return result
    
    
    def mset(self, key_value_pairs: Sequence[Tuple[K, Document]]) -> None:
        # documentsテーブルにkey-valueのペアを保存. keyが既に存在する場合は上書き
        connection = self.engine.connect()
        for key, value in key_value_pairs:
            # valueのpage_contentとmetadataをjson文字列に変換
            dict_item = {"page_content": value.page_content, "metadata": value.metadata}
            json_value = json.dumps(dict_item, ensure_ascii=False)
            sql = text("INSERT OR REPLACE INTO documents (id, data) VALUES (:v1, :v2)")
            connection.execute(sql, parameters=dict(v1 = key, v2 = json_value))

        connection.commit()
        connection.close()
    
    def yield_keys(self, *, prefix: Optional[str] = None) -> Union[Iterator[K], Iterator[str]]:
        return iter([])
    
