from dotenv import load_dotenv
import os, json
from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps
from sqlalchemy import Row, create_engine
from sqlalchemy import text
from uuid import uuid4
# VectorDBの場所、コレクション、Descriptionなどの設定を保持するクラス
class VectorDBCatalog:
    def __init__(self, catalb_db_url: str):
        self.engine  = create_engine(catalb_db_url)
        connection = self.engine.connect()
        # documentsテーブルがなければ作成
        sql = text("CREATE TABLE IF NOT EXISTS vector_db_catalog (id TEXT PRIMARY KEY, db_path TEXT, collection TEXT, description TEXT)")
        connection.execute(sql)
        connection.commit()
        connection.close()

    # 指定されたdb_pathの情報を取得
    def get_catalogs(self, db_path: str) -> list[dict]:
        connection = self.engine.connect()
        sql = text("SELECT id, db_path, collection, description FROM vector_db_catalog WHERE db_path = :db_path")
        rows = connection.execute(sql, parameters=dict(db_path = db_path)).fetchall()
        result = []
        for row in rows:
            result.append({"id": row[0], "db_path": row[1], "collection": row[2], "description": row[3]})
        connection.close()
        return result
    
    # catalogに登録されている情報を取得
    def get_catalog(self, db_path, collection :str) -> dict:
        connection = self.engine.connect()
        if collection is None:
            sql = text("SELECT id, db_path, collection, description FROM vector_db_catalog WHERE db_path = :db_path")
            row : Row = connection.execute(sql, parameters=dict(db_path = db_path)).fetchone()
        else:
            sql = text("SELECT id, db_path, collection, description FROM vector_db_catalog WHERE db_path = :db_path AND collection = :collection")
            row : Row = connection.execute(sql, parameters=dict(id = id, db_path = db_path, collection = collection)).fetchone()

        result = {}
        if row is not None:
            result = {"id": row[0], "db_path": row[1], "collection": row[2], "description": row[3]}

        connection.close()
        return result
    
    # update
    def update_catalog(self, db_path: str, collection: str, description: str):
        # idを取得
        id = self.get_catalog(db_path, collection).get("id", None)
        connection = self.engine.connect()
        if id is None:
            id = str(uuid4())
            sql = text("INSERT INTO vector_db_catalog (id, db_path, collection, description) VALUES (:id, :db_path, :collection, :description)")
            connection.execute(sql, parameters=dict(id = id, db_path = db_path, collection = collection, description = description))
        else:
            sql = text("UPDATE vector_db_catalog SET db_path = :db_path, collection = :collection, description = :description WHERE id = :id")
            connection.execute(sql, parameters=dict(id = id, db_path = db_path, collection = collection, description = description))
    
        connection.commit()
        connection.close()

    # delete
    def delete_catalog(self, id: str):
        connection = self.engine.connect()
        sql = text("DELETE FROM vector_db_catalog WHERE id = :id")
        connection.execute(sql, parameters=dict(id = id))
        connection.commit()
        connection.close()
