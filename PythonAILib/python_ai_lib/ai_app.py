import os, json
from typing import Any, Tuple
import tempfile
import sys
sys.path.append("python")

from ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db_util import VectorDBProps
import base64
from ai_app_vector_db_util import VectorSearchParameter, ContentUpdateOrDeleteRequestParams, ImageUpdateOrDeleteRequestParams, FileUpdateOrDeleteRequestParams
from ai_app_langchain_util import LangChainChatParameter, LangChainUtil
from ai_app_file_util import ExcelUtil, FileUtil


########################
# ファイル関連
########################
# ファイルのMimeTypeを取得する
def get_mime_type(filename):
    return FileUtil.get_mime_type(filename)

# Excelのシート名一覧を取得する
def get_sheet_names(filename):
    return ExcelUtil.get_sheet_names(filename)

# Excelのシートのデータを取得する
def extract_text_from_sheet(filename, sheet_name):
    return ExcelUtil.extract_text_from_sheet(filename, sheet_name)


# ファイルからテキストを抽出する
def extract_text_from_file(filename:str) -> str:
    from ai_app_file_util import FileUtil
    return FileUtil.extract_text_from_file(filename)

# base64形式のデータからテキストを抽出する
def extract_base64_to_text(base64_data:str) -> str:
    # base64データから一時ファイルを生成
    with tempfile.NamedTemporaryFile(mode="wb", delete=False) as temp:
        # base64からバイナリデータに変換
        base64_data_bytes = base64.b64decode(base64_data)
        temp.write(base64_data_bytes)
        temp_path = temp.name
        temp.close()
        from ai_app_file_util import FileUtil
        # 一時ファイルからテキストを抽出
        text = FileUtil.extract_text_from_file(temp_path)
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

def run_vector_search(params:VectorSearchParameter) -> dict:
    result = LangChainUtil.run_vector_search(params)
    return result

def run_langchain_chat(params:LangChainChatParameter) -> dict:
    # langchan_chatを実行
    result = LangChainUtil.langchain_chat(params)
    return result

# vector db関連
def delete_collection(openai_props: OpenAIProps, vector_db_items: list[VectorDBProps]):
    # vector_db_itemsからVectorDBPropsを取得
    # LangChainVectorDBを生成
    for vector_db_props in vector_db_items:
        vector_db = LangChainUtil.get_vector_db(openai_props, vector_db_props)
        # delete_collectionを実行
        vector_db.delete_collection()

def update_or_delete_content_index(params: ContentUpdateOrDeleteRequestParams):
    # props_json, request_jsonからOpenAIProps, VectorDBProps, text, sourceを取得
    # openai_props, vector_db_props, text, source, source_url, description  = langchain_vector_db.process_content_update_or_datele_request_params(props_json, request_json)

    # LangChainVectorDBを生成
    vector_db_props = params.vector_db_props_list[0]
    vector_db = LangChainUtil.get_vector_db(params.openai_props, vector_db_props)
    
    if params.mode == "delete":
        # delete_content_indexを実行
        vector_db.delete_content_index(params.source)
    elif params.mode == "update":
        # update_content_indexを実行
        vector_db.update_content_index(params)
    else:
        raise Exception("mode is invalid")

def update_or_delete_image_index(params: ImageUpdateOrDeleteRequestParams):
    # props_json, request_jsonからOpenAIProps, VectorDBProps, text, image_url, sourceを取得
    # openai_props, vector_db_props, text, source, source_url, description, image_url = langchain_vector_db.process_image_update_or_datele_request_params(props_json, request_json)
    # LangChainVectorDBを生成
    vector_db_props = params.vector_db_props_list[0]
    vector_db = LangChainUtil.get_vector_db(params.openai_props, vector_db_props)

    if params.mode == "delete":
        # delete_image_indexを実行
            vector_db.delete_content_index(params.source)
    elif params.mode == "update":
        # update_image_indexを実行
        vector_db.update_image_index(params)
    else:
        raise Exception("mode is invalid")
    
def update_or_delete_file_index(params: FileUpdateOrDeleteRequestParams):

    # LangChainVectorDBを生成
    vector_db_props = params.vector_db_props_list[0]
    vector_db = LangChainUtil.get_vector_db(params.openai_props, vector_db_props)

    # modeに応じて処理を分岐
    if params.mode == "delete":
        # delete_file_indexを実行
        vector_db.delete_content_index(params.relative_path)
    elif params.mode == "update":
        # update_file_indexを実行
        vector_db.update_file_index(params)
    else:
        raise Exception("mode is invalid")
    
    # 結果用のdictを生成
    result: dict = {}
    return result

# export_to_excelを実行する
def export_to_excel(filePath, dataJson):
    # dataJsonをdictに変換
    data = json.loads(dataJson)
    # export_to_excelを実行
    print(data)
    ExcelUtil.export_to_excel(filePath, data.get("rows",[]))

# import_from_excelを実行する
def import_from_excel(filePath) -> dict:
    # import_to_excelを実行
    data = ExcelUtil.import_from_excel(filePath)
    # 結果用のdictを生成
    result = {}
    result["rows"] = data
    return result

# テスト用
def hello_world():
    return "Hello World"
