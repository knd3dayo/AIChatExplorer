import os, json
from typing import Any
from collections.abc import Generator, AsyncGenerator
from io import StringIO
import sys

from ai_chat_explorer.openai_modules import OpenAIProps, OpenAIClient, RequestContext
from ai_chat_explorer.db_modules import VectorDBItem, MainDB, EmbeddingData
from ai_chat_explorer.autogen_modules import AutoGenProps



request_context_name = "context"
openai_props_name = "openai_props"
vector_db_items_name = "vector_db_props"
autogen_props_name = "autogen_props"
chat_request_context_name = "chat_request_context"

chat_request_name = "chat_request"
chat_contatenate_request_name = "chat_contatenate_request"
token_count_request_name = "token_count_request"
autogen_request_name = "autogen_request"
vector_search_requests_name = "vector_search_requests"
embedding_request_name = "embedding_request"
excel_request_name = "excel_request"
file_request_name = "file_request"
web_request_name = "web_request"

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
                
                # strout,stderrorを元に戻す
                sys.stdout = sys.__stdout__
                sys.stderr = sys.__stderr__
                
                # resultにlogを追加して返す
                result["log"] = buffer.getvalue()
                # bufferをクリア
                buffer.truncate(0)

                json_string = json.dumps(result, ensure_ascii=False, indent=4)
                print(json_string)
                yield json_string

            except Exception as e:
                # エラーが発生した場合はエラーメッセージを出力
                import traceback
                traceback.print_exc()
                result = {}
                result["error"] = str(e)
            finally:
                # strout,stderrorを元に戻す
                sys.stdout = sys.__stdout__
                sys.stderr = sys.__stderr__
                # bufferをクリア
                buffer.truncate(0)


    return wrapper


########################
# parametar関連
########################
def get_chat_request_context_objects(request_dict: dict) -> RequestContext:
    '''
    {"chat_request_context": {}}の形式で渡される
    '''
    # chat_request_contextを取得
    chat_request_context_dict: dict[Any, Any] = request_dict.get(chat_request_context_name, None)
    if not chat_request_context_dict:
        raise ValueError("request_context is not set.")

    result = RequestContext(chat_request_context_dict)
    return result

def get_openai_objects(request_dict: dict) -> tuple[OpenAIProps, OpenAIClient]:
    '''
    {"openai_props": {}}の形式で渡される
    '''
    # OpenAIPorps, OpenAIClientを生成
    openai_props_dict = request_dict.get(openai_props_name, None)
    if not openai_props_dict:
        raise ValueError("openai_props is not set.")

    openai_props = OpenAIProps(openai_props_dict)
    client = OpenAIClient(openai_props)
    return openai_props, client

def get_autogen_objects(request_dict: dict) -> AutoGenProps:
    '''
    {"context": {"autogen_props": {}}}の形式で渡される
    '''
    # AutoGenPropsを生成
    props_dict = request_dict.get(autogen_props_name, None)
    if not props_dict:
        raise ValueError("autogen_props is not set")
    
    # get_openai_objectsを使ってOpenAIPropsを取得
    openai_props, _ = get_openai_objects(request_dict)

    # vector_db_itemsを取得
    vector_db_items = get_vector_search_requests_objects(request_dict)

    app_db_path = os.getenv("APP_DB_PATH", None)
    if not app_db_path:
        raise ValueError("APP_DB_PATH is not set.")
    autogen_props = AutoGenProps(app_db_path, props_dict, openai_props, vector_db_items)
    return autogen_props


def get_token_count_objects(request_dict: dict) -> dict:
    '''
    {"context": {"token_count_request": {}}}の形式で渡される
    '''

    # token_count_request_nameを取得
    token_count_request = request_dict.get(token_count_request_name, None)
    if not token_count_request:
        raise ValueError("token_count_request is not set")
    return token_count_request

def get_autogen_request_objects(request_dict: dict) -> dict:
    '''
    {"context": {"autogen_request": {}}}の形式で渡される
    '''
    # contextを取得
    request:dict = request_dict.get(autogen_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    return request

def get_vector_search_requests_objects(request_dict: dict) -> list[VectorDBItem]:
    '''
    {"vector_search_request": {}}の形式で渡される
    '''
    # contextを取得
    request:list[dict] = request_dict.get(vector_search_requests_name, None)
    if not request:
        raise ValueError("request is not set.")
    # MainDBを取得
    app_db_path = os.getenv("APP_DB_PATH", None)
    if not app_db_path:
        raise ValueError("APP_DB_PATH is not set.")
    main_db = MainDB(app_db_path)

    vector_db_items = []
    for item in request:
        # nameからVectorDBItemを取得
        name = item.get("Name", None)
        if not name:
            raise ValueError("Name is not set.")
        vector_db_item = main_db.get_vector_db_by_name(name)
        if not vector_db_item:
            raise ValueError(f"vector_db_item({name}) is not found.")
        # input_textとSearchKWArgを設定
        input_text = item.get("input_text", None)
        search_kwarg = item.get("SearchKWArg", None)
        if input_text:
            vector_db_item.input_text = input_text
        if search_kwarg:
            vector_db_item.search_kwarg = search_kwarg
        # vector_db_itemsに追加
        vector_db_items.append(vector_db_item)

    return vector_db_items

def get_embedding_request_objects(request_dict: dict) -> VectorDBItem:
    '''
    {"embedding_request": {}}の形式で渡される
    '''
    # contextを取得
    request: dict = request_dict.get(embedding_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    # MainDBを取得
    app_db_path = os.getenv("APP_DB_PATH", None)
    if not app_db_path:
        raise ValueError("APP_DB_PATH is not set.")
    main_db = MainDB(app_db_path)

    # nameからVectorDBItemを取得
    name = request.get("Name", None)
    if not name:
        raise ValueError("Name is not set.")
    vector_db_item = main_db.get_vector_db_by_name(name)
    if not vector_db_item:
        raise ValueError(f"vector_db_item({name}) is not found.")
    # Embeddingを取得
    embedding: dict = request.get("Embedding", None)
    if not embedding:
        raise ValueError("Embedding is not set.")
    
    # vector_db_itemにEmbeddingを設定
    vector_db_item.EmbeddingData = EmbeddingData(embedding)
    
    return vector_db_item


def get_excel_request_objects(request_dict: dict) -> tuple[str, dict]:
    '''
    {"context": {"excel_request": {}}}の形式で渡される
    '''
    # contextを取得
    request:dict = request_dict.get(excel_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    # file_pathとdata_jsonを取得
    file_path = request.get("file_path", None)
    data_json = request.get("data_json", None)

    return file_path, data_json

def get_file_request_objects(request_dict: dict) -> dict:
    '''
    {"context": {"file_request": {}}}の形式で渡される
    '''
    # contextを取得
    request:dict = request_dict.get(file_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    return request

def get_web_request_objects(request_dict: dict) -> dict:
    '''
    {"context": {"web_request": {}}}の形式で渡される
    '''
    # contextを取得
    request:dict = request_dict.get(web_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    return request
