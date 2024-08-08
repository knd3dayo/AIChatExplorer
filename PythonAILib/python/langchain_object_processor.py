import sys, json, os
sys.path.append('python')
from langchain.docstore.document import Document
from langchain_client import LangChainOpenAIClient
import langchain_util

from langchain_vector_db import LangChainVectorDB
from openai_props import OpenAIProps, VectorDBProps

class LangChainObjectProcessor:
    def __init__(self, openai_props: OpenAIProps, vector_db_props: VectorDBProps):
        
        langchain_client = LangChainOpenAIClient(openai_props)
        self.vector_db = langchain_util.get_vector_db(langchain_client, vector_db_props)

    def delete_content_index(self, source: str):
        # DBからsourceを指定して既存ドキュメントを削除
        delete_count = self.vector_db.delete_doucments([source])
        # 削除したドキュメント数を返す
        return delete_count
    
    def update_content_index(self, text: str, source: str, source_url: str):
        # 既に存在するドキュメントを削除
        self.delete_content_index(source)
        # ドキュメントを取得
        documents = self._get_document_list(text, source, source_url)
        # DBにdockumentsを更新
        self.vector_db.add_documents(documents)
        # 更新したドキュメント数を返す
        return len(documents)
    
    def update_image_index(self, image_url: str, source: str):
        # ★TODO *_content_indexと同じ処理になっているので、共通化する
        
        # 既に存在するドキュメントを削除
        delete_count = self.delete_content_index(source)
        # ドキュメントを取得
        documents = self.get_image_document_list(image_url, source)
        # DBにdockumentsを更新
        self.vector_db.add_documents(documents)
        # 更新したドキュメント数を返す
        return len(documents)
    
    def _get_document_list(self, text: str, source: str, source_url: str):
        text_list = self._split_text(text)
        return [ Document(page_content=text, metadata={"source_url": source_url, "source": source}) for text in text_list]    

    def _split_text(self, text: str, chunk_size: int=500):
        text_list = []
        # テキストをchunk_sizeで分割
        for i in range(0, len(text), chunk_size):
            text_list.append(text[i:i + chunk_size])
        return text_list

def process_content_update_or_datele_request_params(props_json: str, request_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    vector_db_props = openai_props.VectorDBItems[0]
    
    # request_jsonをdictに変換
    request = json.loads(request_json)
    text = request["Content"]
    source = request["Id"]

    return openai_props, vector_db_props, text, source        

def process_image_update_or_datele_request_params(props_json: str, request_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    vector_db_props = openai_props.VectorDBItems[0]
    
    # request_jsonをdictに変換
    request = json.loads(request_json)
    image_url = request["image_url"]
    source = request["Id"]

    return openai_props, vector_db_props, image_url, source
