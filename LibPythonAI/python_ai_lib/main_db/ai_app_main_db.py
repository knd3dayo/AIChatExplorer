import sqlite3
import json
from typing import List

class ContentFolder:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE "ContentFolders" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_ContentFolders" PRIMARY KEY,
    "FolderTypeString" TEXT NOT NULL,
    "ParentId" TEXT NULL,
    "IsRootFolder" INTEGER NOT NULL,
    "FolderName" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "ContentOutputFolderPrefix" TEXT NOT NULL,
    "ExtendedPropertiesJson" TEXT NOT NULL
)
    '''
    def __init__(self, id: str, content_folder_dict: dict):
        self.id = id
        self.folder_type_string = content_folder_dict.get("folder_type_string", "")
        self.parent_id = content_folder_dict.get("parent_id", "")
        self.is_root_folder = content_folder_dict.get("is_root_folder", 0)
        self.folder_name = content_folder_dict.get("folder_name", "")
        self.description = content_folder_dict.get("description", "")
        self.content_output_folder_prefix = content_folder_dict.get("content_output_folder_prefix", "")
        self.extended_properties_json = content_folder_dict.get("extended_properties_json", "")
 

class ContentItem:
    '''
    以下のテーブル定義のデータを格納するクラス
    "Id" TEXT NOT NULL CONSTRAINT "PK_ContentItems" PRIMARY KEY,
    "FolderId" TEXT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL,
    "VectorizedAt" TEXT NOT NULL,
    "Content" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "ContentType" INTEGER NOT NULL,
    "ChatMessagesJson" TEXT NOT NULL,
    "PromptChatResultJson" TEXT NOT NULL,
    "TagString" TEXT NOT NULL,
    "SourceApplicationName" TEXT NOT NULL,
    "SourceApplicationTitle" TEXT NOT NULL,
    "SourceApplicationID" INTEGER NOT NULL,
    "SourceApplicationPath" TEXT NOT NULL,
    "IsPinned" INTEGER NOT NULL,
    "DocumentReliability" INTEGER NOT NULL,
    "DocumentReliabilityReason" TEXT NOT NULL,
    "IsReferenceVectorDBItemsSynced" INTEGER NOT NULL,
    "CachedBase64String" TEXT NOT NULL,
    "ExtendedPropertiesJson" TEXT NOT NULL
    '''
    def __init__(self, content_item_dict: dict):

        self.id = content_item_dict.get("id", "")
        self.folder_id = content_item_dict.get("folder_id", "")
        self.created_at = content_item_dict.get("created_at", "")
        self.updated_at = content_item_dict.get("updated_at", "")
        self.vectorized_at = content_item_dict.get("vectorized_at", "")
        self.content = content_item_dict.get("content", "")
        self.description = content_item_dict.get("description", "")
        self.content_type = content_item_dict.get("content_type", 0)
        self.chat_messages_json = content_item_dict.get("chat_messages_json", "")
        self.prompt_chat_result_json = content_item_dict.get("prompt_chat_result_json", "")
        self.tag_string = content_item_dict.get("tag_string", "")
        self.source_application_name = content_item_dict.get("source_application_name", "")
        self.source_application_title = content_item_dict.get("source_application_title", "")
        self.source_application_id = content_item_dict.get("source_application_id", 0)
        self.source_application_path = content_item_dict.get("source_application_path", "")
        self.is_pinned = content_item_dict.get("is_pinned", 0)
        self.document_reliability = content_item_dict.get("document_reliability", 0)
        self.document_reliability_reason = content_item_dict.get("document_reliability_reason", "")
        self.is_reference_vector_db_items_synced = content_item_dict.get("is_reference_vector_db_items_synced", 0)
        self.cached_base64_string = content_item_dict.get("cached_base64_string", "")
        self.extended_properties_json = content_item_dict.get("extended_properties_json", "")


class VectorDBItem:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE "VectorDBItems" (
        "Id" TEXT NOT NULL CONSTRAINT "PK_VectorDBItems" PRIMARY KEY,
        "Name" TEXT NOT NULL,
        "Description" TEXT NOT NULL,
        "VectorDBURL" TEXT NOT NULL,
        "IsUseMultiVectorRetriever" INTEGER NOT NULL,
        "DocStoreURL" TEXT NOT NULL,
        "VectorDBType" INTEGER NOT NULL,
        "CollectionName" TEXT NOT NULL,
        "CatalogDBURL" TEXT NOT NULL,
        "ChunkSize" INTEGER NOT NULL,
        "DefaultSearchResultLimit" INTEGER NOT NULL,
        "IsEnabled" INTEGER NOT NULL,
        "IsSystem" INTEGER NOT NULL
    )    
    '''
    def __init__(self, vector_db_item_dict: dict):
        self.id = vector_db_item_dict.get("id", "")
        self.name = vector_db_item_dict.get("name", "")
        self.description = vector_db_item_dict.get("description", "")
        self.vector_db_url = vector_db_item_dict.get("vector_db_url", "")
        self.is_use_multi_vector_retriever = vector_db_item_dict.get("is_use_multi_vector_retriever", 0)
        self.doc_store_url = vector_db_item_dict.get("doc_store_url", "")
        self.vector_db_type = vector_db_item_dict.get("vector_db_type", 0)
        self.collection_name = vector_db_item_dict.get("collection_name", "")
        self.catalog_db_url = vector_db_item_dict.get("catalog_db_url", "")
        self.chunk_size = vector_db_item_dict.get("chunk_size", 0)
        self.default_search_result_limit = vector_db_item_dict.get("default_search_result_limit", 0)
        self.is_enabled = vector_db_item_dict.get("is_enabled", 0)
        self.is_system = vector_db_item_dict.get("is_system", 0)


class AutogentLLMConfig:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE autogen_llm_configs (name TEXT, api_type TEXT, api_version TEXT, model TEXT, api_key TEXT, base_url TEXT)
    '''
    def __init__(self, llm_config_dict: dict):
        self.name = llm_config_dict.get("name", "")
        self.api_type = llm_config_dict.get("api_type", "")
        self.api_version = llm_config_dict.get("api_version", "")
        self.model = llm_config_dict.get("model", "")
        self.api_key = llm_config_dict.get("api_key", "")
        self.base_url = llm_config_dict.get("base_url", "")

class AutogenTools:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE autogen_tools (name TEXT, path TEXT, description TEXT)    '''
    def __init__(self, tools_dict: dict):
        self.name = tools_dict.get("name", "")
        self.path = tools_dict.get("path", "")
        self.description = tools_dict.get("description", "")

class AutogenAgent:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE autogen_agents (name TEXT PRIMARY KEY, description TEXT, system_message TEXT, code_execution BOOLEAN, llm_config_name TEXT, tool_names TEXT, vector_db_items TEXT)
    '''
    def __init__(self, agent_dict: dict):
        self.name = agent_dict.get("name", "")
        self.description = agent_dict.get("description", "")
        self.system_message = agent_dict.get("system_message", "")
        self.code_execution = agent_dict.get("code_execution", False)
        self.llm_config_name = agent_dict.get("llm_config_name", "")
        self.tool_names = agent_dict.get("tool_names", "")
        self.vector_db_items = agent_dict.get("vector_db_items", json.dumps([]))

class AutogenGroupChat:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE autogen_group_chats (name TEXT, description TEXT, llm_config_name TEXT, agent_names TEXT)
    '''
    def __init__(self, group_chat_dict: dict):
        self.name = group_chat_dict.get("name", "")
        self.description = group_chat_dict.get("description", "")
        self.llm_config_name = group_chat_dict.get("llm_config_name", "")
        self.agent_names_str = group_chat_dict.get("agent_names", "")
        self.agent_names = self.agent_names_str.split(",")

class MainDB:
    def __init__(self, db_path):
        self.db_path = db_path

    def get_autogen_llm_config(self, llm_config_name: str) -> AutogentLLMConfig:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_llm_configs WHERE name=?", (llm_config_name,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None:
            return None

        llm_config_dict = dict(row)
        conn.close()

        return AutogentLLMConfig(llm_config_dict)
    
    def get_autogen_tool(self, tool_name: str) -> AutogenTools:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_tools WHERE name=?", (tool_name,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None:
            return None

        tool_dict = dict(row)
        conn.close()

        return AutogenTools(tool_dict)
    
    def get_autogen_tools(self) -> List[AutogenTools]:
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_tools")
        rows = cur.fetchall()
        tools = [AutogenTools(dict(row)) for row in rows]
        conn.close()

        return tools

    def update_autogen_tool(self, tool: AutogenTools):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        if self.get_autogen_tool(tool.name) is None:
            cur.execute("INSERT INTO autogen_tools VALUES (?, ?, ?)", (tool.name, tool.path, tool.description))
        else:
            cur.execute("UPDATE autogen_tools SET path=?, description=? WHERE name=?", (tool.path, tool.description, tool.name))
        conn.commit()

    def get_autogen_agent(self, agent_name: str) -> AutogenAgent:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_agents WHERE name=?", (agent_name,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None:
            return None
        agent_dict = dict(row)
        conn.close()

        return AutogenAgent(agent_dict)

    def get_autogen_agents(self) -> List[AutogenAgent]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_agents")
        rows = cur.fetchall()
        agents = [AutogenAgent(dict(row)) for row in rows]
        conn.close()

        return agents
    
    def update_autogen_agent(self, agent: AutogenAgent):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        if self.get_autogen_agent(agent.name) is None:
            cur.execute("INSERT INTO autogen_agents VALUES (?, ?, ?, ?, ?, ?, ?)", (agent.name, agent.description, agent.system_message, agent.code_execution, agent.llm_config_name, agent.tool_names, agent.vector_db_items))
        else:
            cur.execute("UPDATE autogen_agents SET description=?, system_message=?, code_execution=?, llm_config_name=?, tool_names=?, vector_db_items=? WHERE name=?", (agent.description, agent.system_message, agent.code_execution, agent.llm_config_name, agent.tool_names, agent.vector_db_items, agent.name))
        conn.commit()


    def get_autogen_group_chat(self, group_chat_name: str) -> AutogenGroupChat:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_group_chats WHERE name=?", (group_chat_name,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None:
            return None

        group_chat_dict = dict(row)
        conn.close()

        return AutogenGroupChat(group_chat_dict)