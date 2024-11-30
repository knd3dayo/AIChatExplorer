import os, json
from typing import Any
from collections.abc import Generator
from io import StringIO
import sys
sys.path.append("python")

from ai_app_openai.ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps
from ai_app_autogen.ai_app_autogen_props import AutoGenProps

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
def get_openai_objects(context_json: str) -> tuple[OpenAIProps, OpenAIClient]:
    # ChatRequestContextからOpenAIPorpsを生成
    props_dict = json.loads(context_json)
    openai_props_dict = props_dict["openai_props"]
    openai_props = OpenAIProps(openai_props_dict)
    client = OpenAIClient(openai_props)
    return openai_props, client

def get_vector_db_objects(context_json: str) -> list[VectorDBProps]:
    # ChatRequestContextからVectorDBPropsを生成
    props_dict = json.loads(context_json)
    vector_db_items = props_dict["vector_db_items"]
    vector_db_props = [VectorDBProps(item) for item in vector_db_items]
    return vector_db_props

def get_autogen_objects(openai_props: OpenAIProps, vector_db_items: list[VectorDBProps], context_json: str) -> AutoGenProps:
    # ChatRequestContextからAutoGenPropsを生成
    props_dict = json.loads(context_json)
    autogen_props = AutoGenProps(openai_props, vector_db_items, props_dict["autogen_props"])
    return autogen_props

########################
# openai関連
########################
def run_openai_chat(context_json: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict[str, Any]:
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(context_json)
        # request_jsonをdictに変換
        request = json.loads(request_json)
        result:dict = ai_app.run_openai_chat(openai_props, request)
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()


def openai_embedding(context_json: str, input_text: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict[str, Any]:
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        _, openai_client = get_openai_objects(context_json)
        result: dict = {}
        vector =  openai_client.openai_embedding(input_text)
        result["vector"] = vector
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()


def list_openai_models(context_json: str):
    # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
    openai_props, openai_client = get_openai_objects(context_json)
    return openai_client.list_openai_models()

########################
# Autogen関連
########################
def run_autogen_group_chat( context_json:str, input_text: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> Generator[dict, None, None]:
        openai_props, _ = get_openai_objects(context_json)
        vector_db_items = get_vector_db_objects(context_json)
        autogen_props = get_autogen_objects(openai_props, vector_db_items, context_json)

        result = ai_app.run_autogen_group_chat(autogen_props, input_text)
        for message, is_last_message in result:
            # dictを作成
            result_dict = {"message": message, "is_last_message": is_last_message}
            yield result_dict
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_generator_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# Autogenのtoolをjson形式で返す
def get_autogen_default_definition(context_json):
    def func() -> dict:
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(context_json)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(context_json)
        autogen_props = get_autogen_objects(openai_props, vector_db_items, context_json)
        from ai_app_autogen.ai_app_autogen_tools import AutoGenTools, AutoGenToolGenerator
        autogen_tools = AutoGenTools(autogen_props,[])
        tools_definiton = AutoGenToolGenerator.create_definition_from_tools(autogen_tools.tools)
        result: dict = {}
        result["tools"] = tools_definiton

        from ai_app_autogen.ai_app_autogen_agent import AutoGenAgentGenerator
        agent_dfinition_list = AutoGenAgentGenerator.create_default_agents(autogen_props, autogen_tools)
        result["agents"] = [ value[2] for key, value in agent_dfinition_list.items()]

        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()


########################
# langchain関連
########################

def run_langchain_chat( context_json: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:

        # process_langchain_chat_parameterを実行
        from ai_app_langchain.ai_app_langchain_util import LangChainChatParameter
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(context_json)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(context_json)
        params:LangChainChatParameter = LangChainChatParameter(request_json)
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
def delete_collection(context_json: str):
    def func() -> dict:
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(context_json)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(context_json)
        # vector_db_itemsからVectorDBPropsを取得
        vector_db_props = [VectorDBProps(item) for item in vector_db_items]
        
        ai_app.delete_collection(openai_props, vector_db_props)
        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def vector_search(context_json: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(context_json)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(context_json)

        from ai_app_vector_db.ai_app_vector_db_props import VectorSearchParameter
        params:VectorSearchParameter = VectorSearchParameter(openai_props, vector_db_items, request_json)
        result = ai_app.vector_search(params)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# vector db関連
# ベクトルDBのインデックスを削除する
def delete_index(context_json: str, request_json: str):
    def func () -> dict:
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(context_json)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(context_json)
        from ai_app_langchain.langchain_vector_db import ContentUpdateOrDeleteRequestParams
        params:ContentUpdateOrDeleteRequestParams = ContentUpdateOrDeleteRequestParams(
            openai_props, vector_db_items, request_json)
        ai_app.update_or_delete_content_index(params)
        return {}

    # strout, stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# ベクトルDBのコンテンツインデックスを更新する
def update_content_index(context_json: str, request_json: str):
    def func () -> dict:
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(context_json)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(context_json)

        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, sourceを取得
        from ai_app_langchain.langchain_vector_db import ContentUpdateOrDeleteRequestParams
        params:ContentUpdateOrDeleteRequestParams = ContentUpdateOrDeleteRequestParams(
            openai_props, vector_db_items, request_json
            )
        
        ai_app.update_or_delete_content_index(params)
        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_file_index(context_json: str, request_json: str):
    def func () -> dict:
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(context_json)
        # ChatRequestContextからVectorDBPropsを生成
        vector_db_items = get_vector_db_objects(context_json)

        from ai_app_langchain.langchain_vector_db import FileUpdateOrDeleteRequestParams
        params:FileUpdateOrDeleteRequestParams = FileUpdateOrDeleteRequestParams(openai_props, vector_db_items, request_json)
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
