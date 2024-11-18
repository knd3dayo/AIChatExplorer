import os, json
from typing import Any
from collections.abc import Generator
from io import StringIO
import sys
sys.path.append("python")

from ai_app_openai.ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps
import ai_app

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"
# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

# stdout,stderrを文字列として取得するためラッパー関数を定義
def capture_stdout_stderr(func):
    def wrapper(*args, **kwargs) -> str:
        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer
        try:
            # debug用
            # HTTPS_PROXY環境変数
            print(f"HTTPS_PROXY:{os.environ.get('HTTPS_PROXY')}")
            # NO_PROXY環境変数
            print(f"NO_PROXY:{os.environ.get('NO_PROXY')}")

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

# stdout,stderrを文字列として取得するためラッパー関数を定義
def capture_generator_stdout_stderr(func):
    def wrapper(*args, **kwargs) -> Generator[str, None, None]:

        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer
        for result in func(*args, **kwargs):
            try:
                # resultがdictでない場合は例外をスロー
                if not isinstance(result, dict):
                    raise ValueError("result must be dict")
                json_string = json.dumps(result, ensure_ascii=False, indent=4)
                print(json_string)
                yield json_string

            except Exception as e:
                # エラーが発生した場合はエラーメッセージを出力
                e.printStackTrace()
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
# Autogen関連
########################
def run_autogen_group_chat( props_json: str, vector_db_items_json:str, work_dir: str, input_text: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> Generator[dict, None, None]:
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)
        vector_db_items = json.loads(vector_db_items_json)

        # process_langchain_chat_parameterを実行
        # langchan_chatを実行
        result = ai_app.run_autogen_group_chat(openai_props, vector_db_items, work_dir, input_text)
        for message, is_last_message in result:
            # dictを作成
            result_dict = {"message": message, "is_last_message": is_last_message}
            yield result_dict
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_generator_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()


########################
# langchain関連
########################

def run_langchain_chat( props_json: str, vector_db_items_json:str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:

        # process_langchain_chat_parameterを実行
        from ai_app_langchain.ai_app_langchain_util import LangChainChatParameter
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

def vector_search(openai_props_json: str, vector_db_items_json: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        from ai_app_vector_db.ai_app_vector_db_util import VectorSearchParameter
        params:VectorSearchParameter = VectorSearchParameter.from_json(openai_props_json, vector_db_items_json, request_json)
        result = ai_app.vector_search(params)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# vector db関連
# ベクトルDBのインデックスを削除する
def delete_index(props_json: str, vector_db_items_json: str, request_json: str):
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, sourceを取得
        from ai_app_langchain.langchain_vector_db import ContentUpdateOrDeleteRequestParams
        params:ContentUpdateOrDeleteRequestParams = ContentUpdateOrDeleteRequestParams.from_content_or_image_json(
            props_json, vector_db_items_json, request_json)
        ai_app.update_or_delete_content_index(params)
        return {}

    # strout, stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# ベクトルDBのコンテンツインデックスを更新する
def update_content_index(props_json: str, vector_db_items_json: str, request_json: str):
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, sourceを取得
        from ai_app_langchain.langchain_vector_db import ContentUpdateOrDeleteRequestParams
        params:ContentUpdateOrDeleteRequestParams = ContentUpdateOrDeleteRequestParams.from_content_or_image_json(
            props_json, vector_db_items_json, request_json
            )
        
        ai_app.update_or_delete_content_index(params)
        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_file_index(props_json: str, vector_db_items_json: str, request_json: str):
    def func () -> dict:
        from ai_app_langchain.langchain_vector_db import FileUpdateOrDeleteRequestParams
        params:FileUpdateOrDeleteRequestParams = FileUpdateOrDeleteRequestParams.from_json(props_json, vector_db_items_json, request_json)
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
    return ai_app.get_mime_type(filename)

# Excelのシート名一覧を取得する
def get_sheet_names(filename):
    return ai_app.get_sheet_names(filename)

# Excelのシートのデータを取得する
def extract_excel_sheet(filename, sheet_name):
    return ai_app.extract_text_from_sheet(filename, sheet_name)

# ファイルからテキストを抽出する
def extract_text_from_file(filename):
    return ai_app.extract_text_from_file(filename)

# base64形式のデータからテキストを抽出する
def extract_base64_to_text(base64_data: str, extension: str):
    return ai_app.extract_base64_to_text(base64_data, extension)

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
    # iteratableなオブジェクトを返す
    words = ['Hello', 'World']

    iter_words = iter(words)    
    return iter_words
