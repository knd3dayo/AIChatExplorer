import os, json
from typing import Any
from io import StringIO
import sys

from ai_chat_explorer.openai_modules import OpenAIClient
from ai_chat_explorer.db_modules import VectorSearchParameter

import ai_chat_explorer.api_modules.ai_app as ai_app

from ai_chat_explorer.api_modules.ai_app_util import *

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"
# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

########################
# openai関連
########################
def openai_chat(request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict[str, str]:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # OpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # context_jsonからVectorDBItemを生成
        vector_db_items = get_vector_db_objects(request_dict)
        # context_jsonからChatRequestContextを生成
        chat_request_context = get_chat_request_context_objects(request_dict)
        # chat_requestを取得
        chat_request_dict = request_dict.get(chat_request_name, None)
        if not chat_request_dict:
            raise ValueError("chat_request is not set")

        result = ai_app.run_openai_chat(openai_props, vector_db_items, chat_request_context, chat_request_dict)
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
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

########################
# Autogen関連
########################
async def autogen_chat( request_json: str):

    result = None # 初期化
    # request_jsonからrequestを作成
    request_dict: dict = json.loads(request_json)
    autogen_props = get_autogen_objects( request_dict)
    autogen_request = get_autogen_request_objects(request_dict)
    input_text = autogen_request.get("input_text", "")
    if not input_text:
        raise ValueError("input_text is not set")

    # strout,stderrorをStringIOでキャプチャする
    buffer = StringIO()
    sys.stdout = buffer
    sys.stderr = buffer

    async for message in ai_app.run_autogen_chat(autogen_props, input_text):
        if not message:
            break
        # dictを作成
        result = {"message": message }
        # resultにlogを追加して返す
        result["log"] = buffer.getvalue()
        json_string = json.dumps(result, ensure_ascii=False, indent=4)
        # bufferをクリア
        buffer.truncate(0)
    
        yield json_string

    # strout,stderrorを元に戻す
    sys.stdout = sys.__stdout__
    sys.stderr = sys.__stderr__

 
########################
# langchain関連
########################

def langchain_chat( request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # process_langchain_chat_parameterを実行
        from ai_chat_explorer.langchain_modules.langchain_util import LangChainChatParameter
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBItemを生成
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
# get_vector_db_by_nameを実行する
def get_vector_db_by_name(request_json: str):
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # vector_db_nameを取得
        vector_db_name = request_dict.get("vector_db_name", None)
        if not vector_db_name:
            raise ValueError("vector_db_name is not set")
        # vector_dbを取得
        vector_db = ai_app.get_vector_db_by_name(vector_db_name)
        
        result: dict = {}
        result["vector_db"] = vector_db
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def vector_search(request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBItemを生成
        vector_db_items = get_vector_db_objects(request_dict)
        # queryを取得
        vector_search_request = get_vector_search_request_objects(request_dict)
        query = vector_search_request.get("input_text", "")

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
        # ChatRequestContextからVectorDBItemを生成
        vector_db_items = get_vector_db_objects(request_dict)

        ai_app.update_collection(openai_props, vector_db_items)

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
        # ChatRequestContextからVectorDBItemを生成
        vector_db_items = get_vector_db_objects(request_dict)
        
        ai_app.delete_collection(openai_props, vector_db_items)

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
        # ChatRequestContextからVectorDBItemを生成
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
        # ChatRequestContextからVectorDBItemを生成
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
def get_mime_type(request_json: str):
    def func () -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # file_requestを取得
        file_request = get_file_request_objects(request_dict)
        # file_pathを取得
        file_path = file_request.get("file_path", None)
        text = ai_app.get_mime_type(file_path)
        return {"output": text}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# Excelのシート名一覧を取得する
def get_sheet_names(request_json: str):
    # 未使用
    def func () -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # excel_requestを取得
        filename, _ = get_excel_request_objects(request_dict)
        text = ai_app.get_sheet_names(filename)
        return {"output": text}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# Excelのシートのデータを取得する
def extract_excel_sheet(request_json: str):
    # 未使用
    def func () -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # excel_requestを取得
        filename, excel_request = get_excel_request_objects(request_dict)
        # excel_sheet_nameを取得
        sheet_name = excel_request.get("excel_sheet_name", None)

        text = ai_app.extract_text_from_sheet(filename, sheet_name)
        return {"output": text}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# ファイルからテキストを抽出する
def extract_text_from_file(request_json: str):
    def func () -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # file_requestを取得
        file_request = get_file_request_objects(request_dict)
        # file_pathを取得
        filename = file_request.get("file_path", None)
        text = ai_app.extract_text_from_file(filename)
        return {"output": text}
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# base64形式のデータからテキストを抽出する
def extract_base64_to_text(request_json: str):
    def func () -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # file_requestを取得
        file_request = get_file_request_objects(request_dict)
        # extensionを取得
        extension = file_request.get("extension", None)
        # base64_dataを取得
        base64_data = file_request.get("base64_data", None)
        text = ai_app.extract_base64_to_text(base64_data, extension)
        return {"output": text}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def extract_webpage(request_json: str):
    def func () -> dict[str, Any]:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # web_requestを取得
        request = get_web_request_objects(request_dict)

        url = request.get("url", None)
        text, urls = ai_app.extract_webpage(url)
        result: dict[str, Any] = {}
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
def import_from_excel(request_json: str):
    # import_to_excelを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # file_requestを取得
        file_request = get_file_request_objects(request_dict)
        # file_pathを取得
        file_path = file_request.get("file_path", None)
        result = ai_app.import_from_excel(file_path)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# テスト用
def hello_world():
    return {"output": "Hello, World!"}
