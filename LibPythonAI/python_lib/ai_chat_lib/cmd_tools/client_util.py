import json
import re
import os
from dotenv import load_dotenv
from typing import Optional
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

def update_normal_chat_request_by_envvars(json_template):
    load_dotenv()
    AZURE_OPENAI=os.environ.get("AZURE_OPENAI", "False").upper() == "TRUE"
    OPENAI_API_KEY=os.environ.get("OPENAI_API_KEY", None) 
    OPENAI_EMBEDDING_MODEL=os.environ.get("OPENAI_EMBEDDING_MODEL", None)
    OPENAI_COMPLETION_MODEL=os.environ.get("OPENAI_COMPLETION_MODEL", None)
    AZURE_OPENAI_ENDPOINT=os.environ.get("AZURE_OPENAI_ENDPOINT", None)
    AZURE_OPENAI_API_VERSION=os.environ.get("AZURE_OPENAI_API_VERSION", None)
    OPENAI_BASE_URL=os.environ.get("OPENAI_BASE_URL", None)

    if OPENAI_API_KEY is None:
        raise ValueError("OPENAI_API_KEY is not set.")
    if OPENAI_EMBEDDING_MODEL is None:
        raise ValueError("OPENAI_EMBEDDING_MODEL is not set.")
    if OPENAI_COMPLETION_MODEL is None:
        raise ValueError("OPENAI_COMPLETION_MODEL is not set.")

    # 環境変数から情報を取得する
    # openai_propsの設定
    json_template["openai_props"]["AzureOpenAI"] = AZURE_OPENAI
    json_template["openai_props"]["OpenAIKey"] = OPENAI_API_KEY
    json_template["openai_props"]["AzureOpenAIAPIVersion"] = AZURE_OPENAI_API_VERSION
    json_template["openai_props"]["AzureOpenAIEndpoint"] = AZURE_OPENAI_ENDPOINT
    json_template["openai_props"]["OpenAIBaseURL"] = OPENAI_BASE_URL


    # chat_requestの設定
    json_template["chat_request"]["model"] = OPENAI_COMPLETION_MODEL
    json_template["chat_request"]["messages"] = []

    # vector_search_requestsの解除
    json_template["vector_search_requests"] = None

def update_normal_chat_messages(role: str, message: str, json_template: dict):
    """
    Update the chat messages in the JSON template.
    :param role: Role of the message (user, assistant, system).
    :param message: Content of the message.
    :param json_template: JSON template to update.
    """
    # メッセージを追加する
    content = [ {"type": "text", "text": message} ]
    json_template["chat_request"]["messages"].append({"role": role, "content": content})
