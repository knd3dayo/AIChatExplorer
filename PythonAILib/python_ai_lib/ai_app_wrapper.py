import os, json
from typing import Any
from io import StringIO
import sys
sys.path.append("python")

from openai_props import OpenAIProps, VectorDBProps
from openai_client import OpenAIClient
import langchain_util
import ai_app
import langchain_vector_db

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"

# stdout,stderrを文字列として取得するためラッパー関数を定義
def capture_stdout_stderr(func):
    def wrapper(*args, **kwargs) -> str:
        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer
        try:
            result = func(*args, **kwargs)
            # resultがdictでない場合は例外をスロー
            if not isinstance(result, dict):
                raise ValueError("result must be dict")
        except Exception as e:
            # エラーが発生した場合はエラーメッセージを出力
            print(e)
            result = {}
        # strout,stderrorを元に戻す
        sys.stdout = sys.__stdout__
        sys.stderr = sys.__stderr__
        
        # resultにlogを追加して返す
        result["log"] = buffer.getvalue()
        # jsonを返す
        return json.dumps(result, ensure_ascii=False, indent=4)

    return wrapper

########################
# openai関連
########################
def run_openai_chat(props_json: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict[str, Any]:
        # OpenAIPorpsを生成
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)
        # request_jsonをdictに変換
        request = json.loads(request_json)
        result:dict = ai_app.run_openai_chat(openai_props, request)
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()


def openai_embedding(props_json: str, input_text: str):
    # OpenAIPorpsを生成
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    # OpenAIClientを生成
    openai_client = OpenAIClient(openai_props)
    
    return openai_client.openai_embedding(input_text)

def list_openai_models(props_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    client = OpenAIClient(openai_props)
    return client.list_openai_models()

########################
# langchain関連
########################

def run_langchain_chat( props_json: str, vector_db_items_json:str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:

        # process_langchain_chat_parameterを実行
        from langchain_util import LangChainChatParameter
        params:LangChainChatParameter = LangChainChatParameter(props_json, vector_db_items_json, request_json)
        # langchan_chatを実行
        result = ai_app.run_langchain_chat(params)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

########################
# ベクトルDB関連
########################
def delete_collection(props_json: str, vector_db_items_json: str):
    def func() -> dict:
    # props_jsonからOpenAIProps, VectorDBPropsを取得
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)
        vector_db_items = json.loads(vector_db_items_json)

        # vector_db_itemsからVectorDBPropsを取得
        vector_db_props = [VectorDBProps(item) for item in vector_db_items]
        
        ai_app.delete_collection(openai_props, vector_db_props)
        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def run_vector_search(openai_props_json: str, vector_db_items_json: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        from langchain_vector_db import VectorSearchParameter
        params:VectorSearchParameter = VectorSearchParameter(openai_props_json, vector_db_items_json, request_json)
        result = ai_app.run_vector_search(params)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# vector db関連
# ベクトルDBのコンテンツインデックスを削除する
def delete_content_index(props_json: str, vector_db_items_json: str, request_json: str):
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, sourceを取得
        from langchain_vector_db import ContentUpdateOrDeleteRequestParams
        params:ContentUpdateOrDeleteRequestParams = ContentUpdateOrDeleteRequestParams(props_json, vector_db_items_json, request_json)
        ai_app.update_or_delete_content_index(params)
        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# ベクトルDBのコンテンツインデックスを更新する
def update_content_index(props_json: str, vector_db_items_json: str, request_json: str):
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, sourceを取得
        from langchain_vector_db import ContentUpdateOrDeleteRequestParams
        params:ContentUpdateOrDeleteRequestParams = ContentUpdateOrDeleteRequestParams(props_json, vector_db_items_json, request_json)
        ai_app.update_or_delete_content_index(params)
        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# ベクトルDBの画像インデックスを削除する
def delete_image_index(props_json: str, vector_db_items_json: str, request_json: str):
    # delete_indexを実行する関数を定義
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, image_url, sourceを取得
        from langchain_vector_db import ImageUpdateOrDeleteRequestParams
        params:ImageUpdateOrDeleteRequestParams = ImageUpdateOrDeleteRequestParams(props_json, vector_db_items_json, request_json)
        ai_app.update_or_delete_image_index(params)
        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# ベクトルDBの画像インデックスを更新する
def update_image_index(props_json: str, vector_db_items_json: str, request_json: str):
    # update_indexを実行する関数を定義
    def func () -> dict:
        from langchain_vector_db import ImageUpdateOrDeleteRequestParams
        params:ImageUpdateOrDeleteRequestParams = ImageUpdateOrDeleteRequestParams(props_json, vector_db_items_json, request_json)
        ai_app.update_or_delete_image_index(params)
        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_file_index(props_json: str, vector_db_items_json: str, request_json: str):
    def func () -> dict:
        from langchain_vector_db import FileUpdateOrDeleteRequestParams
        params:FileUpdateOrDeleteRequestParams = FileUpdateOrDeleteRequestParams(props_json, vector_db_items_json, request_json)
        ai_app.update_or_delete_file_index(params)
        return {}
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def delete_file_index(props_json: str, vector_db_items_json: str, request_json: str):
    def func () -> dict:
        from langchain_vector_db import FileUpdateOrDeleteRequestParams
        params:FileUpdateOrDeleteRequestParams = FileUpdateOrDeleteRequestParams(props_json, vector_db_items_json, request_json)
        ai_app.update_or_delete_file_index(params)
        return {}
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()



########################
# ファイル関連
########################
# ファイルのMimeTypeを取得する
def get_mime_type(filename):
    import file_extractor
    return file_extractor.get_mime_type(filename)

# Excelのシート名一覧を取得する
def get_excel_sheet_names(filename):
    import excel_util
    return excel_util.get_excel_sheet_names(filename)

# Excelのシートのデータを取得する
def extract_excel_sheet(filename, sheet_name):
    import excel_util
    return excel_util.extract_text_from_sheet(filename, sheet_name)

# ファイルからテキストを抽出する
def extract_text_from_file(filename):
    return ai_app.extract_text_from_file(filename)

# base64形式のデータからテキストを抽出する
def extract_base64_to_text(base64_data):
    return ai_app.extract_base64_to_text(base64_data)

# export_to_excelを実行する
def export_to_excel(filePath, dataJson):
    # export_to_excelを実行する関数を定義
    def func() -> dict:
        ai_app.export_to_excel(filePath, dataJson)
        # 結果用のdictを生成
        return {}
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# import_from_excelを実行する
def import_from_excel(filePath):
    # import_to_excelを実行する関数を定義
    def func() -> dict:
        return ai_app.import_from_excel(filePath)
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# テスト用
def hello_world():
    return "Hello World"
