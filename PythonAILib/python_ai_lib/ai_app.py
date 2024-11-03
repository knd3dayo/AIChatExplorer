import os, json
from typing import Any, Tuple
import tempfile
import sys
sys.path.append("python")

from openai_props import OpenAIProps, VectorDBProps
from openai_client import OpenAIClient
import langchain_util
import langchain_vector_db
import base64
from langchain_vector_db import ContentUpdateOrDeleteRequestParams, ImageUpdateOrDeleteRequestParams, FileUpdateOrDeleteRequestParams

import excel_util

# ファイルからテキストを抽出する
def extract_text_from_file(filename:str) -> str:
    import file_extractor
    return file_extractor.extract_text_from_file(filename)

# base64形式のデータからテキストを抽出する
def extract_base64_to_text(base64_data:str) -> str:
    # base64データから一時ファイルを生成
    with tempfile.NamedTemporaryFile(mode="wb", delete=False) as temp:
        # base64からバイナリデータに変換
        base64_data_bytes = base64.b64decode(base64_data)
        temp.write(base64_data_bytes)
        temp_path = temp.name
        temp.close()
        import file_extractor
        # 一時ファイルからテキストを抽出
        text = file_extractor.extract_text_from_file(temp_path)
        # 一時ファイルを削除
        os.remove(temp_path)
        return text

########################
# openai関連
########################
def run_openai_chat(openai_props: OpenAIProps, request: dict) -> Tuple[str, str]:
    openai_client = OpenAIClient(openai_props)
    return openai_client.run_openai_chat(request)

def openai_embedding(openai_props: OpenAIProps, input_text: str):
    openai_client = OpenAIClient(openai_props)
    return openai_client.openai_embedding(input_text)

def list_openai_models(openai_props: OpenAIProps):
    client = OpenAIClient(openai_props)
    return client.list_openai_models()

########################
# langchain関連
########################

def run_vector_search(openai_props:OpenAIProps, vector_db_items:list[VectorDBProps], query:str, search_kwargs: dict):
    result = langchain_util.run_vector_search(openai_props, vector_db_items, query, search_kwargs)
    return result

def run_langchain_chat( openai_props:OpenAIProps, vector_db_props:list[VectorDBProps], prompt:str, chat_history: list[Any]) -> dict:
    # langchan_chatを実行
    result = langchain_util.langchain_chat(openai_props, vector_db_props, prompt, chat_history)
    return result

# vector db関連
def update_or_delete_file_index(params: FileUpdateOrDeleteRequestParams):

    # LangChainVectorDBを生成
    vector_db = langchain_vector_db.get_vector_db(params.openai_props, params.vector_db_props)

    # modeに応じて処理を分岐
    if params.mode == "delete":
        # delete_file_indexを実行
        vector_db.delete_file_index(params.document_root, params.relative_path, params.source_url)
    elif params.mode == "update":
        # update_file_indexを実行
        vector_db.update_file_index(params.document_root, params.relative_path, params.source_url, description=params.description, reliability=params.reliability)
    else:
        raise Exception("mode is invalid")
    
    # 結果用のdictを生成
    result: dict = {}
    return result

def update_or_delete_content_index(params: ContentUpdateOrDeleteRequestParams):
    # props_json, request_jsonからOpenAIProps, VectorDBProps, text, sourceを取得
    # openai_props, vector_db_props, text, source, source_url, description  = langchain_vector_db.process_content_update_or_datele_request_params(props_json, request_json)

    # LangChainVectorDBを生成
    vector_db = langchain_vector_db.get_vector_db(params.openai_props, params.vector_db_props)
    
    if params.mode == "delete":
        # delete_content_indexを実行
        vector_db.delete_content_index(params.source)
    elif params.mode == "update":
        # update_content_indexを実行
        vector_db.update_content_index(params.text, params.source, params.source_url, description=params.description, reliability=params.reliability)
    else:
        raise Exception("mode is invalid")

def update_or_delete_image_index(params: ImageUpdateOrDeleteRequestParams):
    # props_json, request_jsonからOpenAIProps, VectorDBProps, text, image_url, sourceを取得
    # openai_props, vector_db_props, text, source, source_url, description, image_url = langchain_vector_db.process_image_update_or_datele_request_params(props_json, request_json)
    # LangChainVectorDBを生成
    vector_db = langchain_vector_db.get_vector_db(params.openai_props, params.vector_db_props)
    
    # 初期化
    update_count = 0
    delete_count = 0
    
    if params.mode == "delete":
        # delete_image_indexを実行
            vector_db.delete_image_index(params.source)
    elif params.mode == "update":
        # update_image_indexを実行
        vector_db.update_image_index(params.text, params.source, params.source_url, image_url=params.image_url, description=params.description, reliability=params.reliability)
    else:
        raise Exception("mode is invalid")
    
# export_to_excelを実行する
def export_to_excel(filePath, dataJson):
    # dataJsonをdictに変換
    data = json.loads(dataJson)
    # export_to_excelを実行
    print(data)
    excel_util.export_to_excel(filePath, data.get("rows",[]))

# import_from_excelを実行する
def import_from_excel(filePath) -> dict:
    # import_to_excelを実行
    data = excel_util.import_from_excel(filePath)
    # 結果用のdictを生成
    result = {}
    result["rows"] = data
    return result

# テスト用
def hello_world():
    return "Hello World"
