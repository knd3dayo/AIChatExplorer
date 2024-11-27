from dotenv import load_dotenv
import os, json
from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps
from sqlalchemy import create_engine
from sqlalchemy import text
from uuid import uuid4
# VectorDBの場所、コレクション、Descriptionなどの設定を保持するクラス
class VectorDBCatalog:
    def __init__(self, vector_db_props: VectorDBProps):
        self.vector_db_props = vector_db_props
        catalb_db_url = vector_db_props.CatalogDBURL
        self.engine  = create_engine(catalb_db_url)
        connection = self.engine.connect()
        # documentsテーブルがなければ作成
        sql = text("CREATE TABLE IF NOT EXISTS vector_db_catalog (id TEXT PRIMARY KEY, db_path TEXT, collection TEXT, description TEXT)")
        connection.execute(sql)
        connection.commit()
        connection.close()

    # catalogに登録されている情報を取得
    def get_catalog_by_id(self, id: str):
        connection = self.engine.connect()
        sql = text("SELECT db_path, collection, description FROM vector_db_catalog WHERE id = :id")
        row = connection.execute(sql, parameters=dict(id = id)).fetchall()
        result = None
        for r in row:
            result = {"db_path": r[0], "collection": r[1], "description": r[2]}
        connection.close()
        return result
    # catalogに登録されている情報を取得
    def get_catalog_by_db_path_and_collection(self, db_path: str, collection: str):
        connection = self.engine.connect()
        sql = text("SELECT id, description FROM vector_db_catalog WHERE db_path = :db_path AND collection = :collection")
        row = connection.execute(sql, parameters=dict(db_path = db_path, collection = collection)).fetchall()
        result = []
        for r in row:
            result.append({"id": r[0], "description": r[1]})
        connection.close()
        return result

    # catalogに登録 idは自動採番
    def set_catalog(self, db_path: str, collection: str, description: str):
        connection = self.engine.connect()
        id = str(uuid4())
        sql = text("INSERT INTO vector_db_catalog (id, db_path, collection, description) VALUES (:id, :db_path, :collection, :description)")
        connection.execute(sql, parameters=dict(id = id, db_path = db_path, collection = collection, description = description))
        connection.commit()
        connection.close()
        return id
    
    # update
    def update_catalog(self, id: str, db_path: str, collection: str, description: str):
        connection = self.engine.connect()
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
