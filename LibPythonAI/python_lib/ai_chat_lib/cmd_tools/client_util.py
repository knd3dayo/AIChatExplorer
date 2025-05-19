import json
import re
import os
import httpx  # type: ignore

from dotenv import load_dotenv
from typing import Optional, Union, Any

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

def jsonc_load(file_path: str):
    """
    Load a JSON file with comments.
    :param file_path: Path to the JSON file.
    :return: Parsed JSON data.
    """
    with open(file_path, 'r', encoding='utf-8') as file:
        content = file.read()
        # Remove comments
        # remove single line comment
        content = re.sub(r'//.*?\n', '', content)
        # remove comment block
        content = re.sub(r'/\*.*?\*/', '', content, flags=re.DOTALL)
        return json.loads(content)

def jsonc_loads(json_str: str):
    """
    Load a JSON string with comments.
    :param json_str: JSON string.
    :return: Parsed JSON data.
    """
    # Remove comments
    # remove single line comment
    json_str = re.sub(r'//.*?\n', '', json_str)
    # remove comment block
    json_str = re.sub(r'/\*.*?\*/', '', json_str, flags=re.DOTALL)
    return json.loads(json_str)

def load_default_json_template() -> dict:
    """
    Load the default JSON template from a file.
    :return: Parsed JSON data.
    """
    # templateファイル[request_template.jsonc]を読み込む。ファイルはこのスクリプトと同じディレクトリにあるものとする。
    json_template = jsonc_load(os.path.join(os.path.dirname(__file__), "request_template.jsonc"))
    return json_template

def __update_openai_props_by_envvars(json_template):
    load_dotenv()
    AZURE_OPENAI=os.environ.get("AZURE_OPENAI", "False").upper() == "TRUE"
    OPENAI_API_KEY=os.environ.get("OPENAI_API_KEY", None) 
    OPENAI_COMPLETION_MODEL=os.environ.get("OPENAI_COMPLETION_MODEL", None)
    AZURE_OPENAI_ENDPOINT=os.environ.get("AZURE_OPENAI_ENDPOINT", None)
    AZURE_OPENAI_API_VERSION=os.environ.get("AZURE_OPENAI_API_VERSION", None)
    OPENAI_BASE_URL=os.environ.get("OPENAI_BASE_URL", None)

    if OPENAI_API_KEY is None:
        raise ValueError("OPENAI_API_KEY is not set.")
    if OPENAI_COMPLETION_MODEL is None:
        raise ValueError("OPENAI_COMPLETION_MODEL is not set.")

    # 環境変数から情報を取得する
    # openai_propsの設定
    json_template["openai_props"]["AzureOpenAI"] = AZURE_OPENAI
    json_template["openai_props"]["OpenAIKey"] = OPENAI_API_KEY
    json_template["openai_props"]["AzureOpenAIAPIVersion"] = AZURE_OPENAI_API_VERSION
    json_template["openai_props"]["AzureOpenAIEndpoint"] = AZURE_OPENAI_ENDPOINT
    json_template["openai_props"]["OpenAIBaseURL"] = OPENAI_BASE_URL

def create_vector_search_request_from_envvars( top_k: int = 10, folder_path: Optional[str] = None) -> dict:
    json_template = load_default_json_template()
    # 環境変数から情報を取得する
    # openai_propsの設定
    __update_openai_props_by_envvars(json_template)

    load_dotenv()
    VECTOR_DB_NAME=os.environ.get("VECTOR_DB_NAME", "default") 
    OPENAI_EMBEDDING_MODEL=os.environ.get("OPENAI_EMBEDDING_MODEL", None)
    if OPENAI_EMBEDDING_MODEL is None:
        raise ValueError("OPENAI_EMBEDDING_MODEL is not set.")

    # 環境変数から情報を取得する
    # vector_search_requestsの設定
    json_template["vector_search_requests"] = []
    request: dict[str, Any]= {}
    request["name"] = VECTOR_DB_NAME
    request["model"] = OPENAI_EMBEDDING_MODEL
    request["search_kwargs"] = {}
    request["search_kwargs"]["k"] = top_k
    if folder_path:
        request["search_kwargs"]["filter"] = {"folder_path": folder_path}
    json_template["vector_search_requests"].append(request)

    return json_template


def prepare_vector_search_request(request_json_file: Union[str, None], message: Union[str, None], search_result_count, vector_db_folder: str) -> dict:
    """
    ベクトル検索リクエストを準備する関数
    :param request_json_file: JSONファイルのパス
    :param message: メッセージ
    :param search_result_count: 検索結果の数
    :param vector_db_folder: ベクトルDBの検索対象フォルダ
    :return: リクエスト辞書
    """
    
    if request_json_file:
        # JSONファイルからリクエストを作成する
        request_dict = create_normal_chat_request_from_json_file(request_json_file)
    else:
        # 環境変数からリクエストを作成する
        request_dict = create_vector_search_request_from_envvars(search_result_count, vector_db_folder)

    return request_dict

def add_normal_chat_message(role: str, message: str, json_template: dict):
    """
    Add a message to the chat request in the JSON template.
    :param role: Role of the message (user, assistant, system).
    :param message: Content of the message.
    :param json_template: JSON template to update.
    """
    # メッセージを追加する
    content = [ {"type": "text", "text": message} ]
    json_template["chat_request"]["messages"].append({"role": role, "content": content})

def clear_normal_chat_messages(json_template: dict):
    """
    Clear the chat messages in the JSON template.
    :param json_template: JSON template to update.
    """
    # メッセージをクリアする
    json_template["chat_request"]["messages"] = []

def create_normal_chat_request_from_json_file(request_json_file: str) -> dict:
    """
    JSONファイルからリクエストを作成する関数
    :param request_json_file: JSONファイルのパス
    :return: リクエスト辞書
    """
    with open(request_json_file, "r", encoding="utf-8") as f:
        request_json = f.read()
        request_dict = json.loads(request_json)
    return request_dict

def create_normal_chat_request_from_envvars() -> dict:
    json_template = load_default_json_template()
    # 環境変数から情報を取得する
    # openai_propsの設定
    __update_openai_props_by_envvars(json_template)

    # chat_requestの設定
    load_dotenv()
    OPENAI_COMPLETION_MODEL=os.environ.get("OPENAI_COMPLETION_MODEL", None)
    json_template["chat_request"]["model"] = OPENAI_COMPLETION_MODEL
    json_template["chat_request"]["messages"] = []

    # vector_search_requestsの解除
    json_template["vector_search_requests"] = None


    return json_template

def prepare_normal_chat_request(request_json_file: Union[str, None], interactive_mode: bool, message: Union[str, None]) -> dict:
    """
    リクエストを準備する関数
    :param request_json_file: JSONファイルのパス
    :param interactive_mode: インタラクティブモードかどうか
    :param message: メッセージ
    :return: リクエスト辞書
    """
    
    if request_json_file:
        # JSONファイルからリクエストを作成する
        request_dict = create_normal_chat_request_from_json_file(request_json_file)
    else:
        # 環境変数からリクエストを作成する
        request_dict = create_normal_chat_request_from_envvars()

    if interactive_mode and message:
        # インタラクティブモードの場合はメッセージをクリアする
        clear_normal_chat_messages(request_dict)
        # メッセージを設定する
        add_normal_chat_message("user", message, request_dict)

    return request_dict

async def send_request(request_dict: dict, api_endpoint: str) -> dict:
    """
    リクエストを送信する関数
    :param request_dict: リクエスト辞書
    :param api_base: APIのURL
    :return: レスポンス辞書
    """

    # APIリクエスト 現時点では認証はなし
    headers = {
        "Content-Type": "application/json",
        "Accept": "application/json",
    }
    # APIリクエスト 現時点では認証はなし
    headers = {
        "Content-Type": "application/json",
        "Accept": "application/json",
    }

    # APIリクエスト送信
    async with httpx.AsyncClient() as client:
        logger.debug(f"Request data: {request_dict}")
        response = await client.post(api_endpoint, headers=headers, json=request_dict, timeout=180)
        logger.debug(f"Response data: {response.text}")

    # レスポンスの取得
    if response.status_code != 200:
        raise ValueError(f"API request failed with status code {response.status_code}")
    
    # レスポンスのJSONをdictionaryに変換
    response_dict = response.json()  # response.json() は非同期ではない

    return response_dict

