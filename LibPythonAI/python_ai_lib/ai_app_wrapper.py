import os, json
from typing import Any
from collections.abc import Generator
from io import StringIO
import sys

from ai_app_openai import OpenAIProps, OpenAIClient, RequestContext
from ai_app_vector_db import VectorDBProps, VectorDBCatalog
from ai_app_autogen import AutoGenProps

import ai_app

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"
# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

request_context_name = "context"
openai_props_name = "openai_props"
vector_db_items_name = "vector_db_items"
autogen_props_name = "autogen_props"
chat_request_context_name = "chat_request_context"
chat_request_name = "chat_request"
token_count_request_name = "token_count_request"
autogen_request_name = "autogen_request"
catalog_request_name = "catalog_request"
query_request_name = "query_request"
excel_request_name = "excel_request"

# stdout,stderrを文字列として取得するためラッパー関数を定義
def capture_stdout_stderr(func):
    def wrapper(*args, **kwargs) -> str:
        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer
        result = {}
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
            import traceback
            traceback.print_exc()            
            result["error"] = "\n".join(traceback.format_exception(type(e), e, e.__traceback__))

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
        result = None # 初期化
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
                result["error"] = str(e)

        # strout,stderrorを元に戻す
        sys.stdout = sys.__stdout__
        sys.stderr = sys.__stderr__
        
        # resultにlogを追加して返す
        result["log"] = buffer.getvalue()
        # jsonを返す
        return json.dumps(result, ensure_ascii=False, indent=4)

    return wrapper

########################
# parametar関連
########################
def get_chat_request_context_objects(request_dict: dict) -> dict:
    # contextを取得
    request_context:dict = request_dict.get(request_context_name, None)
    if not request_context:
        raise ValueError("context is not set.")

    request_context_dict = request_context.get(chat_request_context_name, None)
    if not request_context_dict:
        raise ValueError("request_context is not set.")

    request_context = RequestContext(request_context_dict)
    return request_context

def get_openai_objects(request_dict: dict) -> tuple[OpenAIProps, OpenAIClient]:
    # contextを取得
    request_context:dict = request_dict.get(request_context_name, None)
    if not request_context:
        raise ValueError("context is not set.")
    
    # OpenAIPorps, OpenAIClientを生成
    openai_props_dict = request_context.get(openai_props_name, None)
    if not openai_props_dict:
        raise ValueError("openai_props is not set.")

    openai_props = OpenAIProps(openai_props_dict)
    client = OpenAIClient(openai_props)
    return openai_props, client

def get_vector_db_objects(request_dict: dict) -> list[VectorDBProps]:
    # contextを取得
    request_context:dict = request_dict.get(request_context_name, None)
    if not request_context:
        raise ValueError("context is not set.")
    # VectorDBPropsを生成
    vector_db_items = request_context.get(vector_db_items_name, None)
    if not vector_db_items:
        print("vector_db_items is not set")
        return []
    
    vector_db_props = [VectorDBProps(item) for item in vector_db_items]
    return vector_db_props

def get_autogen_objects(request_dict: dict) -> AutoGenProps:
    # contextを取得
    request_context:dict = request_dict.get(request_context_name, None)
    if not request_context:
        raise ValueError("context is not set.")
    # AutoGenPropsを生成
    props_dict = request_context.get(autogen_props_name, None)
    if not props_dict:
        raise ValueError("autogen_props is not set")

    autogen_props = AutoGenProps(props_dict["autogen_props"])
    return autogen_props

def get_token_count_objects(request_dict: dict) -> dict:

    # token_count_request_nameを取得
    token_count_request = request_dict.get(token_count_request_name, None)
    if not token_count_request:
        raise ValueError("token_count_request is not set")
    return token_count_request

def get_autogen_request_objects(request_dict: dict) -> dict:
    # contextを取得
    request:dict = request_dict.get(autogen_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    return request

def get_catalog_request_objects(request_dict: dict) -> dict:
    # contextを取得
    request:dict = request_dict.get(catalog_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    # catalog_db_url, vector_db_url, collection, folder_id, descriptionを取得

    return request

def get_query_request_objects(request_dict: dict) -> dict:
    # contextを取得
    request:dict = request_dict.get(query_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    return request

def get_excel_request_objects(request_dict: dict) -> dict:
    # contextを取得
    request:dict = request_dict.get(excel_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    # file_pathとdata_jsonを取得
    file_path = request.get("file_path", None)
    data_json = request.get("data_json", None)

    return file_path, data_json

########################
# openai関連
########################
def openai_chat(request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict[str, Any]:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # OpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # context_jsonからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(request_dict)
        # context_jsonからChatRequestContextを生成
        chat_request_context = get_chat_request_context_objects(request_dict)
        # chat_requestを取得
        chat_request_dict = request_dict.get(chat_request_name, None)
        if not chat_request_dict:
            raise ValueError("chat_request is not set")

        result:dict = ai_app.run_openai_chat(openai_props, vector_db_items, chat_request_context, chat_request_dict)
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_token_count(request_json: str):
    # get_token_countを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        
        openai_props, _ = get_openai_objects(request_dict)

        # input_textを取得
        token_count_request = request_dict.get(token_count_request_name, None)
        if not token_count_request:
            raise ValueError("token_count_request is not set")
        input_text = token_count_request.get("input_text", "")
        if not input_text:
            raise ValueError("input_text is not set")
        # OpenAIClientを生成
        openai_client = OpenAIClient(openai_props)
        result: dict = {}
        result["total_tokens"] = openai_client.get_token_count(input_text)

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

########################
# Autogen関連
########################
def autogen_group_chat( request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> Generator[dict, None, None]:
        openai_props, _ = get_openai_objects(request_json)
        vector_db_items = get_vector_db_objects(request_json)
        autogen_props = get_autogen_objects( request_json)
        autogen_request = get_autogen_request_objects(request_json)
        input_text = autogen_request.get("input_text", "")
        if not input_text:
            raise ValueError("input_text is not set")

        for message in ai_app.run_autogen_group_chat(autogen_props, openai_props, vector_db_items,  input_text):
            if not message:
                break
            # dictを作成
            result_dict = {"message": message }
            yield result_dict
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_generator_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

########################
# langchain関連
########################

def langchain_chat( request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # process_langchain_chat_parameterを実行
        from ai_app_langchain.ai_app_langchain_util import LangChainChatParameter
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(request_dict)

        # chat_requestを取得
        chat_request_dict = request_dict.get(chat_request_name, None)
        params:LangChainChatParameter = LangChainChatParameter(chat_request_dict)
        # langchan_chatを実行
        result = ai_app.run_langchain_chat(openai_props, vector_db_items, params)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

########################
# ベクトルDB関連
########################
def get_catalog_list(request_json: str) -> dict:
    # 未使用

    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        vector_db_items = get_vector_db_objects(request_dict)
        # 先頭のVectorDBPropsを取得
        vector_db_props = vector_db_items[0]
        catalog_db_url = vector_db_props.CatalogDBURL
        vectordb__url = vector_db_props.VectorDBURL
        vector_db_catalog = ai_app.get_catalogs(catalog_db_url, vectordb__url)
        result = {}
        result["catalog_list"] = vector_db_catalog
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_catalog(request_json: str) -> dict:

    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        catalog_request = get_catalog_request_objects(request_dict)
        catalog_db_url = catalog_request.get("catalog_db_url", None)
        vector_db_url = catalog_request.get("vector_db_url", None)
        collection = catalog_request.get("collection", None)
        folder_id = catalog_request.get("folder_id", None)
        description = catalog_request.get("description", None)

        catalog_list = []

        vector_db_catalog = ai_app.get_catalog(catalog_db_url, vector_db_url, collection, folder_id)
        catalog_list.append(vector_db_catalog)
        result = {}
        result["catalog_list"] = catalog_list
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_catalog_description(request_json: str):
    def func() -> dict:

        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        catalog_request = get_catalog_request_objects(request_dict)
        catalog_db_url = catalog_request.get("catalog_db_url", None)
        vector_db_url = catalog_request.get("vector_db_url", None)
        collection = catalog_request.get("collection", None)
        folder_id = catalog_request.get("folder_id", None)

        description = ai_app.get_catalog_entry(catalog_db_url, vector_db_url, collection, folder_id)

        result = {}
        result["output"] = description
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_catalog_description(request_json: str):
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        catalog_request = get_catalog_request_objects(request_dict)
        catalog_db_url = catalog_request.get("catalog_db_url", None)
        vector_db_url = catalog_request.get("vector_db_url", None)
        collection = catalog_request.get("collection", None)
        folder_id = catalog_request.get("folder_id", None)
        description = catalog_request.get("description", None)

        ai_app.update_catalog(catalog_db_url, vector_db_url, collection, folder_id, description)
        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

########################

def vector_search(request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(request_dict)
        # queryを取得
        query_request = get_query_request_objects(request_dict)
        query = query_request.get("input_text", "")

        from ai_app_vector_db.ai_app_vector_db_props import VectorSearchParameter
        params:VectorSearchParameter = VectorSearchParameter(openai_props, vector_db_items, query)
        result = ai_app.vector_search(params)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_collection(request_json: str):
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(request_dict)

        ai_app.update_collection(openai_props, vector_db_items)

        # Catalogを更新
        for vector_db_props in vector_db_items:
            catalog_db_url = vector_db_props.CatalogDBURL
            db_url = vector_db_props.VectorDBURL
            collection = vector_db_props.CollectionName
            description = vector_db_props.VectorDBDescription
            folder_id = vector_db_props.FolderID
            ai_app.update_catalog(catalog_db_url, db_url, collection, folder_id, description)

        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def delete_collection(request_json: str):
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(request_dict)
        
        ai_app.delete_collection(openai_props, vector_db_items)

        # Catalogを削除
        for vector_db_props in vector_db_items:
            catalog_db_url = vector_db_props.CatalogDBURL
            db_url = vector_db_props.VectorDBURL
            collection = vector_db_props.CollectionName
            folder_id = vector_db_props.FolderID
            ai_app.delete_catalog(catalog_db_url, db_url, collection, folder_id)

        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# ベクトルDBのインデックスを削除する
def delete_embeddings(request_json: str):
    def func () -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(request_dict)
        for vector_db_item in vector_db_items:
            ai_app.delete_embeddings(openai_props, vector_db_item)

        return {}

    # strout, stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# ベクトルDBのコンテンツインデックスを更新する
def update_embeddings(request_json: str):
    def func () -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(request_dict)
        for vector_db_item in vector_db_items:
            ai_app.update_embeddings(openai_props, vector_db_item)
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
    def func () -> dict:
        text = ai_app.extract_text_from_file(filename)
        return {"output": text}
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# base64形式のデータからテキストを抽出する
def extract_base64_to_text(base64_data: str, extension: str):
    def func () -> dict:
        text = ai_app.extract_base64_to_text(base64_data, extension)
        return {"output": text}
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def extract_webpage(url):
    def func () -> dict:
        text, urls = ai_app.extract_webpage(url)
        result = {}
        result["output"] = text
        result["urls"] = urls
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# export_to_excelを実行する
def export_to_excel(request_json: str):
    # export_to_excelを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # file_pathとdata_jsonを取得
        file_path, dataJson = get_excel_request_objects(request_dict)
        ai_app.export_to_excel(file_path, dataJson)
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
