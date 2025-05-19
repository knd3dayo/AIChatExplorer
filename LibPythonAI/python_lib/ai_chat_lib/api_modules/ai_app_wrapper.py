import os, json

from ai_chat_lib.api_modules.ai_app_util import *
from ai_chat_lib.langchain_modules import  LangChainUtil
from ai_chat_lib.file_modules import ExcelUtil, FileUtil
from ai_chat_lib.web_modules import WebUtil
from ai_chat_lib.chat_modules import ChatUtil
from ai_chat_lib.db_modules import *
from ai_chat_lib.autogen_modules import AutoGenProps

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"
# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"
########################
# ContentFolders関連
########################
@capture_stdout_stderr
def get_root_content_folders():
    return ContentFoldersCatalog.get_root_content_folders_api()
@capture_stdout_stderr
def get_content_folders():
    return ContentFoldersCatalog.get_content_folders_api()
@capture_stdout_stderr
def get_content_folders_by_id(request_json: str):
    return ContentFoldersCatalog.get_content_folder_by_id_api(request_json)
@capture_stdout_stderr
def get_content_folder_by_path(request_json: str):
    return ContentFoldersCatalog.get_content_folder_by_path_api(request_json)
@capture_stdout_stderr
def update_content_folders(request_json: str):
    return ContentFoldersCatalog.update_content_folders_api(request_json)
@capture_stdout_stderr
def delete_content_folders(request_json: str):
    return ContentFoldersCatalog.delete_content_folders_api(request_json)

########################
# tag関連
########################
@capture_stdout_stderr
def get_tag_items(request_json: str):
    return TagItem.get_tag_items_api(request_json)

@capture_stdout_stderr
def update_tag_items(request_json: str):
    return TagItem.update_tag_items_api(request_json)

@capture_stdout_stderr
def delete_tag_items(request_json: str):
    return TagItem.delete_tag_items_api(request_json)

########################
# openai関連
########################
@capture_stdout_stderr_async
async def openai_chat_async(request_dict: dict) -> dict:
    return await ChatUtil.run_openai_chat_async_api(request_dict)

@capture_stdout_stderr
def get_token_count(request_json: str):
    return ChatUtil.get_token_count_api(request_json)

########################
# Autogen関連
########################
async def autogen_chat( request_json: str):
    yield AutoGenProps.autogen_chat_api(request_json)

@capture_stdout_stderr
def get_autogen_llm_config_list():
    return AutogentLLMConfig.get_autogen_llm_config_list_api()

@capture_stdout_stderr
def get_autogen_llm_config(request_json: str):
    return AutogentLLMConfig.get_autogen_llm_config_api(request_json)

@capture_stdout_stderr
def update_autogen_llm_config(request_json: str):
    return AutogentLLMConfig.update_autogen_llm_config_api(request_json)

@capture_stdout_stderr
def delete_autogen_llm_config(request_json: str):
    return AutogentLLMConfig.delete_autogen_llm_config_api(request_json)

@capture_stdout_stderr
def get_autogen_tool_list():
    return AutogenTools.get_autogen_tool_list_api()

@capture_stdout_stderr
def get_autogen_tool(request_json: str):
    return AutogenTools.get_autogen_tool_api(request_json)

@capture_stdout_stderr
def update_autogen_tool(request_json: str):
    return AutogenTools.update_autogen_tool_api(request_json)

@capture_stdout_stderr
def delete_autogen_tool(request_json: str):
    return AutogenTools.delete_autogen_tool_api(request_json)

@capture_stdout_stderr
def get_autogen_agent_list():
    return AutogenAgent.get_autogen_agent_list_api()

@capture_stdout_stderr
def get_autogen_agent(request_json: str):
    return AutogenAgent.get_autogen_agent_api(request_json)

@capture_stdout_stderr
def update_autogen_agent(request_json: str):
    return AutogenAgent.update_autogen_agent_api(request_json)

@capture_stdout_stderr
def delete_autogen_agent(request_json: str):
    return AutogenAgent.delete_autogen_agent_api(request_json)

@capture_stdout_stderr
def get_autogen_group_chat_list():
    return AutogenGroupChat.get_autogen_group_chat_list_api()

@capture_stdout_stderr
def get_autogen_group_chat(request_json: str):
    return AutogenGroupChat.get_autogen_group_chat_api(request_json)

@capture_stdout_stderr
def update_autogen_group_chat(request_json: str):
    return AutogenGroupChat.update_autogen_group_chat_api(request_json)

@capture_stdout_stderr
def delete_autogen_group_chat(request_json: str):
    return AutogenGroupChat.delete_autogen_group_chat_api(request_json)

########################
# ベクトルDB関連
########################
# vector_db_itemを更新する
@capture_stdout_stderr
def update_vector_db(request_json: str):
    return VectorDBItem.update_vector_db_api(request_json)

# vector_db_itemを削除する
@capture_stdout_stderr
def delete_vector_db(request_json: str):
    return VectorDBItem.delete_vector_db_api(request_json)

# vector_dbのリストを取得する
@capture_stdout_stderr
def get_vector_db_items():
    return VectorDBItem.get_vector_db_items_api()

# get_vector_db_item_by_idを実行する
@capture_stdout_stderr
def get_vector_db_item_by_id(request_json: str):
    return VectorDBItem.get_vector_db_item_by_id_api(request_json)

# get_vector_db_item_by_nameを実行する
@capture_stdout_stderr
def get_vector_db_item_by_name(request_json: str):
    return VectorDBItem.get_vector_db_item_by_name_api(request_json)

@capture_stdout_stderr
def vector_search(request_json: str):
    return LangChainUtil.vector_search_api(request_json)

@capture_stdout_stderr
def update_collection(request_json: str):
    return LangChainUtil.update_collection_api(request_json)

@capture_stdout_stderr
def delete_collection(request_json: str):
    return LangChainUtil.delete_collection_api(request_json)

# ベクトルDBのインデックスをフォルダ単位で削除する
@capture_stdout_stderr
def delete_embeddings_by_folder(request_json: str):
    return LangChainUtil.delete_embeddings_by_folder_api(request_json)

# ベクトルDBのインデックスを削除する
@capture_stdout_stderr
def delete_embeddings(request_json: str):
    return LangChainUtil.delete_embeddings_api(request_json)

# ベクトルDBのコンテンツインデックスを更新する
@capture_stdout_stderr_async
async def update_embeddings(request_json: str) -> dict:
    return await LangChainUtil.update_embeddings_api(request_json)

########################
# ファイル関連
########################
# ファイルのMimeTypeを取得する
@capture_stdout_stderr
def get_mime_type(request_json: str):
    return FileUtil.get_mime_type_api(request_json)

# Excelのシート名一覧を取得する
@capture_stdout_stderr
def get_sheet_names(request_json: str):
    return ExcelUtil.get_sheet_names_api(request_json)

# Excelのシートのデータを取得する
@capture_stdout_stderr
def extract_excel_sheet(request_json: str):
    return ExcelUtil.extract_excel_sheet_api(request_json)

# ファイルからテキストを抽出する
@capture_stdout_stderr_async
async def extract_text_from_file_async(request_json: str) -> dict:
    return await FileUtil.extract_text_from_file_async_api(request_json)

# base64形式のデータからテキストを抽出する
@capture_stdout_stderr_async
async def extract_base64_to_text_async(request_json: str):
    return await FileUtil.extract_base64_to_text_async_api(request_json)

@capture_stdout_stderr
def extract_webpage(request_json: str):
    return WebUtil.extract_webpage_api(request_json)

# export_to_excelを実行する
@capture_stdout_stderr
def export_to_excel(request_json: str):
    return ExcelUtil.export_to_excel_api(request_json)

# import_from_excelを実行する
@capture_stdout_stderr
def import_from_excel(request_json: str):
    return ExcelUtil.import_from_excel_api(request_json)

# テスト用
def hello_world():
    return {"output": "Hello, World!"}
