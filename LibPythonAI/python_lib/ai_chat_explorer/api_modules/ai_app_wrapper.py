import os, json
from typing import Any
from io import StringIO
import sys

from ai_chat_explorer.openai_modules import OpenAIClient

import ai_chat_explorer.api_modules.ai_app as ai_app

from ai_chat_explorer.api_modules.ai_app_util import *

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"
# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

########################
# ContentFolder関連
########################
def get_root_content_folders(request_json: str):
    # get_root_content_folderを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # content_foldersを取得
        content_folders = get_content_folder_requelst_objects(request_dict)
        
        
        # root_content_folderを取得
        root_content_folders: list[ContentFolder] = []
        for content_folder in content_folders:
            root_content_folder = ai_app.get_root_content_folder(content_folder.FolderTypeString)
            if root_content_folder is not None:
                root_content_folders.append(root_content_folder)

        
        result: dict = {}
        result["content_folders"] = [folder.to_dict() for folder in root_content_folders]
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

########################
# tag関連
########################
def get_tag_items(request_json: str):
    # get_tag_itemsを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        # request_dict: dict = json.loads(request_json)
        # tag_itemsを取得
        tag_items = ai_app.get_tag_items()
        
        result: dict = {}
        result["tag_items"] = [item.to_dict() for item in tag_items]
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_tag_items(request_json: str):
    # update_tag_itemsを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # tag_itemsを取得
        tag_items = get_tag_item_objects(request_dict)
        # tag_itemsを更新
        ai_app.update_tag_items(tag_items)

        result: dict = {}
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def delete_tag_items(request_json: str):
    # delete_tag_itemsを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # tag_itemsを取得
        tag_items = get_tag_item_objects(request_dict)
        # tag_itemsを削除
        ai_app.delete_tag_items(tag_items)

        result: dict = {}
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

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
        vector_db_items = get_vector_search_requests_objects(request_dict)
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
    # chat_requestを取得
    chat_request_dict = request_dict.get(chat_request_name, None)
    if not chat_request_dict:
        raise ValueError("chat_request is not set")
    
    # chat_request_dictのmessagesを取得
    messages = chat_request_dict.get("messages", None)
    if not messages:
        raise ValueError("messages is not set")
    # messagesのうち、role == userの最後の要素を取得
    last_message = [message for message in messages if message.get("role") == "user"][-1]
    # last_messageのcontent(リスト)を取得
    content_list = last_message.get("content", None)
    if not content_list:
        raise ValueError("content is not set")
    # content_listの要素の中で、typeがtextのものを取得
    text_list = [content for content in content_list if content.get("type") == "text"]
    # text_listの要素を結合 
    input_text = "\n".join([content.get("text") for content in text_list])
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

def get_autogen_llm_config_list(request_json: str):
    # get_autogen_llm_config_listを実行する関数を定義
    def func() -> dict:
        # autogen_propsからllm_config_listを取得
        llm_config_list = ai_app.get_autogen_llm_config_list()
        if not llm_config_list:
            raise ValueError("llm_config_list is not set")
        
        result: dict = {}
        result["llm_config_list"] = [config.to_dict() for config in llm_config_list]
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_autogen_llm_config(request_json: str):
    # get_autogen_llm_configを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからllm_configを取得
        llm_config = get_autogen_llm_config_object(request_dict)
        if not llm_config:
            raise ValueError("llm_config is not set")
        
        # ai_appからllm_configを取得
        llm_config_result = ai_app.get_autogen_llm_config(llm_config.name)
        result: dict = {}
        if llm_config_result:
            result["llm_config"] = llm_config_result.to_dict()

        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_autogen_llm_config(request_json: str):
    # update_autogen_llm_configを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからllm_configを取得
        llm_config = get_autogen_llm_config_object(request_dict)
        if not llm_config:
            raise ValueError("llm_config is not set")
        
        # autogen_propsを更新
        ai_app.update_autogen_llm_config(llm_config)

        result: dict = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def delete_autogen_llm_config(request_json: str):
    # delete_autogen_llm_configを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからllm_configを取得
        llm_config = get_autogen_llm_config_object(request_dict)
        if not llm_config:
            raise ValueError("llm_config is not set")
        
        # autogen_propsを削除
        ai_app.delete_autogen_llm_config(llm_config)

        result: dict = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_autogen_tool_list(request_json: str):
    # get_autogen_tools_listを実行する関数を定義
    def func() -> dict:
        # autogen_propsからtools_listを取得
        tools_list = ai_app.get_autogen_tool_list()
        if not tools_list:
            raise ValueError("tool_list is not set")
        
        result: dict = {}
        result["tool_list"] = [tool.to_dict() for tool in tools_list]
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_autogen_tool(request_json: str):
    # get_autogen_toolを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからtoolを取得
        tool = get_autogen_tool_object(request_dict)
        if not tool:
            raise ValueError("tool is not set")
        
        # ai_appからtoolを取得
        tool_result = ai_app.get_autogen_tool(tool.name)
        
        result: dict = {}
        if tool_result:
            result["tool"] = tool_result.to_dict()
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_autogen_tool(request_json: str):
    # update_autogen_toolを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからtoolを取得
        tool = get_autogen_tool_object(request_dict)
        if not tool:
            raise ValueError("tool is not set")
        
        # autogen_propsを更新
        ai_app.update_autogen_tool(tool)

        result: dict = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def delete_autogen_tool(request_json: str):
    # delete_autogen_toolを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからtoolを取得
        tool = get_autogen_tool_object(request_dict)
        if not tool:
            raise ValueError("tool is not set")
        
        # autogen_propsを削除
        ai_app.delete_autogen_tool(tool)

        result: dict = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_autogen_agent_list(request_json: str):
    # get_autogen_agent_listを実行する関数を定義
    def func() -> dict:
        # autogen_propsからagent_listを取得
        agent_list = ai_app.get_autogen_agent_list()
        if not agent_list:
            raise ValueError("agent_list is not set")
        
        result: dict = {}
        result["agent_list"] = [agent.to_dict() for agent in agent_list]
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_autogen_agent(request_json: str):
    # get_autogen_agentを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからagentを取得
        agent = get_autogen_agent_object(request_dict)
        if not agent:
            raise ValueError("agent is not set")
        
        # ai_appからagentを取得
        agent_result = ai_app.get_autogen_agent(agent.name)
        
        result: dict = {}
        if agent_result:
            result["agent"] = agent_result.to_dict()
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_autogen_agent(request_json: str):
    # update_autogen_agentを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからagentを取得
        agent = get_autogen_agent_object(request_dict)
        if not agent:
            raise ValueError("agent is not set")
        
        # autogen_propsを更新
        ai_app.update_autogen_agent(agent)

        result: dict = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def delete_autogen_agent(request_json: str):
    # delete_autogen_agentを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからagentを取得
        agent = get_autogen_agent_object(request_dict)
        if not agent:
            raise ValueError("agent is not set")
        
        # autogen_propsを削除
        ai_app.delete_autogen_agent(agent)

        result: dict = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_autogen_group_chat_list(request_json: str):
    # get_autogen_group_chat_listを実行する関数を定義
    def func() -> dict:
        # autogen_propsからgroup_chat_listを取得
        group_chat_list = ai_app.get_autogen_group_chat_list()
        if not group_chat_list:
            raise ValueError("group_chat_list is not set")
        
        result: dict = {}
        result["group_chat_list"] = [group_chat.to_dict() for group_chat in group_chat_list]
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def get_autogen_group_chat(request_json: str):
    # get_autogen_group_chatを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからgroup_chatを取得
        group_chat = get_autogen_group_chat_object(request_dict)
        if not group_chat:
            raise ValueError("group_chat is not set")
        # ai_appからgroup_chatを取得
        group_chat_result = ai_app.get_autogen_group_chat(group_chat.name)

        result: dict = {}
        if group_chat_result:
            result["group_chat"] = group_chat_result.to_dict()
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def update_autogen_group_chat(request_json: str):
    # update_autogen_group_chatを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからgroup_chatを取得
        group_chat = get_autogen_group_chat_object(request_dict)
        if not group_chat:
            raise ValueError("group_chat is not set")
        
        # autogen_propsを更新
        ai_app.update_autogen_group_chat(group_chat)

        result: dict = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

def delete_autogen_group_chat(request_json: str):
    # delete_autogen_group_chatを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからgroup_chatを取得
        group_chat = get_autogen_group_chat_object(request_dict)
        if not group_chat:
            raise ValueError("group_chat is not set")
        
        # autogen_propsを削除
        ai_app.delete_autogen_group_chat(group_chat)

        result: dict = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
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
        from ai_chat_explorer.langchain_modules.langchain_util import LangChainChatParameter
        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)
        # ChatRequestContextからVectorDBItemを生成
        vector_db_items = get_vector_search_requests_objects(request_dict)

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
# vector_db_itemを更新する
def update_vector_db(request_json: str):
    # update_vector_dbを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # vector_db_itemを取得
        vector_db_item = get_vector_db_item_object(request_dict)
        # vector_dbを更新
        ai_app.update_vector_db(vector_db_item)

        result: dict = {}
        result["vector_db_item"] = vector_db_item.to_dict()
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# vector_db_itemを削除する
def delete_vector_db(request_json: str):
    # delete_vector_dbを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # vector_db_itemを取得
        vector_db_item = get_vector_db_item_object(request_dict)
        # vector_dbを削除
        ai_app.delete_vector_db(vector_db_item)

        result: dict = {}
        result["vector_db_item"] = vector_db_item.to_dict()
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# vector_dbのリストを取得する
def get_vector_db_items():
    # get_vector_db_listを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        # vector_db_listを取得
        vector_db_list = ai_app.get_vector_db_items()
        
        result: dict = {}
        result["vector_db_items"] = [item.to_dict() for item in vector_db_list]
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# get_vector_db_item_by_idを実行する
def get_vector_db_item_by_id(request_json: str):
    # get_vector_db_by_idを実行する関数を定義
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # vector_db_idを取得
        vector_db_id =  get_vector_db_item_object(request_dict).Id
        if not vector_db_id:
            raise ValueError("vector_db_id is not set")
        # vector_dbを取得
        vector_db = ai_app.get_vector_db_by_id(vector_db_id)

        result: dict = {}
        if vector_db is not None:
            result["vector_db_item"] = vector_db.to_dict()
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()
# get_vector_db_item_by_nameを実行する
def get_vector_db_item_by_name(request_json: str):
    def func() -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # vector_db_nameを取得
        vector_db_name = get_vector_db_item_object(request_dict).Name
        if not vector_db_name:
            raise ValueError("vector_db_name is not set")
        # vector_dbを取得
        vector_db = ai_app.get_vector_db_by_name(vector_db_name)
        
        result: dict = {}
        if vector_db is not None:
            result["vector_db_item"] = vector_db.to_dict()
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
        # queryを取得
        vector_search_requests: list[VectorDBItem] = get_vector_search_requests_objects(request_dict)

        result = ai_app.vector_search(openai_props, vector_search_requests)
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
        vector_db_item = get_embedding_request_objects(request_dict)

        ai_app.update_collection(openai_props, vector_db_item)

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
        vector_db_item = get_embedding_request_objects(request_dict)
        
        ai_app.delete_collection(openai_props, vector_db_item)

        return {}

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行して結果のJSONを返す
    return wrapper()

# ベクトルDBのインデックスをフォルダ単位で削除する
def delete_embeddings_by_folder(request_json: str):
    def func () -> dict:
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)

        # ChatRequestContextからOpenAIPorps, OpenAIClientを生成
        openai_props, _ = get_openai_objects(request_dict)

        # embedding_requestを取得
        vector_db_item = get_embedding_request_objects(request_dict)
        ai_app.delete_embeddings_by_folder(openai_props, vector_db_item)

        return {}

    # strout, stderrをキャプチャするラッパー関数を生成
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

        # embedding_requestを取得
        vector_db_item = get_embedding_request_objects(request_dict)
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
        # embedding_requestを取得
        vector_db_item: VectorDBItem = get_embedding_request_objects(request_dict)
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
