import os, json
from typing import Any
from collections.abc import Generator, AsyncGenerator
from io import StringIO
import sys

from ai_chat_explorer.openai_modules import OpenAIProps, OpenAIClient, RequestContext
from ai_chat_explorer.db_modules import VectorDBItem, TagItem, MainDB, EmbeddingData, ContentFolder, init_db, get_main_db_path, VectorSearchRequest
from ai_chat_explorer.db_modules import AutogentLLMConfig, AutogenTools, AutogenAgent, AutogenGroupChat
from ai_chat_explorer.autogen_modules import AutoGenProps



openai_props_name = "openai_props"
vector_db_item_request_name = "vector_db_item_request"
autogen_props_name = "autogen_props"
chat_request_context_name = "chat_request_context"
get_content_folder_requelsts_name = "content_folder_requests"
chat_request_name = "chat_request"
chat_contatenate_request_name = "chat_contatenate_request"
token_count_request_name = "token_count_request"
vector_search_requests_name = "vector_search_requests"
embedding_request_name = "embedding_request"
excel_request_name = "excel_request"
file_request_name = "file_request"
web_request_name = "web_request"
autogen_llm_config_request_name = "autogen_llm_config_request"
autogen_tool_request_name = "autogen_tool_request"
autogen_agent_request_name = "autogen_agent_request"
autogen_group_chat_request_name = "autogen_group_chat_request"

# アプリケーション初期化時に呼び出される関数
def init_app() -> None:
    init_db(get_main_db_path())

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
def capture_stdout_stderr_async(func):
    async def wrapper(*args, **kwargs) -> str:
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

            result = await func(*args, **kwargs)
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

    return  wrapper

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
def get_content_folder_requelst_objects(request_dict: dict) -> list[ContentFolder]:
    '''
    {"content_folder_requsts": [] }の形式で渡される
    '''
    # contextを取得
    content_folders: list[dict] = request_dict.get(get_content_folder_requelsts_name, None)
    if not content_folders:
        raise ValueError("content_folder is not set.")
    
    # content_folderを生成
    result = []
    for item in content_folders:
        content_folder = ContentFolder(item)
        result.append(content_folder)

    return result


def get_tag_item_objects(request_dict: dict) -> list[TagItem]:
    '''
    {"tag_item_requests": []}の形式で渡される
    '''
    # contextを取得
    tag_items: list[dict] = request_dict.get("tag_item_requests", None)
    if not tag_items:
        raise ValueError("tag_items is not set.")
    
    # TagItemを生成
    tag_items_list = []
    for item in tag_items:
        tag_item = TagItem(item)
        tag_items_list.append(tag_item)

    return tag_items_list
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
    vector_search_requests = get_vector_search_requests_objects(request_dict)

    app_db_path = get_main_db_path()
    autogen_props = AutoGenProps(app_db_path, props_dict, openai_props, vector_search_requests)
    return autogen_props

def get_autogen_llm_config_object(request_dict: dict) -> AutogentLLMConfig:
    '''
    {"autogen_llm_config_request": {}}の形式で渡される
    '''
    # autogen_llm_config_requestを取得
    request:dict = request_dict.get(autogen_llm_config_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    result = AutogentLLMConfig(request)
    return result

def get_autogen_tool_object(request_dict: dict) -> AutogenTools:
    '''
    {"autogen_tool_request": {}}の形式で渡される
    '''
    # autogen_tool_requestを取得
    request:dict = request_dict.get(autogen_tool_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    # autogen_toolを生成
    autogen_tool = AutogenTools(request)
    return autogen_tool

def get_autogen_agent_object(request_dict: dict) -> AutogenAgent:
    '''
    {"autogen_agent_request": {}}の形式で渡される
    '''
    # autogen_agent_requestを取得
    request:dict = request_dict.get(autogen_agent_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    # autogen_agentを生成
    autogen_agent = AutogenAgent(request)
    return autogen_agent

def get_autogen_group_chat_object(request_dict: dict) -> AutogenGroupChat:
    '''
    {"autogen_group_chat_request": {}}の形式で渡される
    '''
    # autogen_group_chat_requestを取得
    request:dict = request_dict.get(autogen_group_chat_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    # autogen_group_chatを生成
    autogen_group_chat = AutogenGroupChat(request)
    return autogen_group_chat

def get_token_count_objects(request_dict: dict) -> dict:
    '''
    {"context": {"token_count_request": {}}}の形式で渡される
    '''

    # token_count_request_nameを取得
    token_count_request = request_dict.get(token_count_request_name, None)
    if not token_count_request:
        raise ValueError("token_count_request is not set")
    return token_count_request


def get_vector_db_item_object(request_dict: dict) -> VectorDBItem:
    '''
    {"vector_db_item_request": {}}の形式で渡される
    '''
    # vector_db_item_requestを取得
    vector_db_item_request: dict = request_dict.get(vector_db_item_request_name, None)
    if not vector_db_item_request:
        raise ValueError("vector_db_item_request is not set.")

    # vector_db_itemを生成
    vector_db_item = VectorDBItem(vector_db_item_request)
    return vector_db_item    

def get_vector_search_requests_objects(request_dict: dict) -> list[VectorSearchRequest]:
    '''
    {"vector_search_request": {}}の形式で渡される
    '''
    # contextを取得
    request:list[dict] = request_dict.get(vector_search_requests_name, None)
    if not request:
        print("request is not set.", file=sys.stderr)
        return []

    vector_search_requests = []
    for item in request:
        # vector_search_requestsを生成
        vector_search_request = VectorSearchRequest(item)
        vector_search_requests.append(vector_search_request)
    return vector_search_requests


def get_embedding_request_objects(request_dict: dict) -> EmbeddingData:
    '''
    {"embedding_request": {}}の形式で渡される
    '''
    # contextを取得
    request: dict = request_dict.get(embedding_request_name, None)
    if not request:
        raise ValueError("request is not set.")
    # MainDBを取得
 
    return EmbeddingData(request)

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
