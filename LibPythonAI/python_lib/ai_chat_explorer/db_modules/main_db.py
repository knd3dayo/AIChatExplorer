import sqlite3
import json
from typing import List, Union
import uuid

class ContentFolder:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE "ContentFolders" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_ContentFolders" PRIMARY KEY,
    "FolderTypeString" TEXT NOT NULL,
    "ParentId" TEXT NULL,
    "FolderName" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "ExtendedPropertiesJson" TEXT NOT NULL
)
    '''
    def __init__(self, content_folder_dict: dict):
        self.Id = content_folder_dict.get("Id", "")
        self.FolderTypeString = content_folder_dict.get("FolderTypeString", "")
        self.ParentId = content_folder_dict.get("ParentId", "")
        self.FolderName = content_folder_dict.get("FolderName", "")
        self.Description = content_folder_dict.get("Description", "")
        self.ExtendedPropertiesJson = content_folder_dict.get("ExtendedPropertiesJson", "")
 

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

        self.Id = content_item_dict.get("Id", "")
        self.FolderId = content_item_dict.get("FolderId", "")
        self.CreatedAt = content_item_dict.get("CreatedAt", "")
        self.UpdatedAt = content_item_dict.get("UpdatedAt", "")
        self.VectorizedAt = content_item_dict.get("VectorizedAt", "")
        self.Content = content_item_dict.get("Content", "")
        self.Description = content_item_dict.get("Description", "")
        self.ContentType = content_item_dict.get("ContentType", 0)
        self.ChatMessagesJson = content_item_dict.get("ChatMessagesJson", "")
        self.PromptChatResultJson = content_item_dict.get("PromptChatResultJson", "")
        self.TagString = content_item_dict.get("TagString", "")
        self.SourceApplicationName = content_item_dict.get("SourceApplicationName", "")
        self.SourceApplicationTitle = content_item_dict.get("SourceApplicationTitle", "")
        self.SourceApplicationID = content_item_dict.get("SourceApplicationID", 0)
        self.SourceApplicationPath = content_item_dict.get("SourceApplicationPath", "")
        self.IsPinned = content_item_dict.get("IsPinned", 0)
        self.DocumentReliability = content_item_dict.get("DocumentReliability", 0)
        self.DocumentReliabilityReason = content_item_dict.get("DocumentReliabilityReason", "")
        self.IsReferenceVectorDBItemsSynced = content_item_dict.get("IsReferenceVectorDBItemsSynced", 0)
        self.CachedBase64String = content_item_dict.get("CachedBase64String", "")
        self.ExtendedPropertiesJson = content_item_dict.get("ExtendedPropertiesJson", "")


class VectorDBItem:

    # コレクションの指定がない場合はデフォルトのコレクション名を使用
    DEFAULT_COLLECTION_NAME = "ai_app_default_collection"
    FOLDER_CATALOG_COLLECTION_NAME = "ai_app_folder_catalog_collection"

    source_id_name = "source_id"
    source_path_name = "source_path"
    git_repository_url_name = "git_repository_url"
    git_relative_path_name = "git_relative_path"
    image_url_name = "image_url"
    description_name = "description"
    system_message_name = "system_message"
    content_name = "content"
    folder_id_name = "folder_id"


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
        self.Id = vector_db_item_dict.get("Id", str(uuid.uuid4()))
        self.Name = vector_db_item_dict.get("Name", "")
        self.Description = vector_db_item_dict.get("Description", "")
        self.VectorDBURL = vector_db_item_dict.get("VectorDBURL", "")
        value1 = vector_db_item_dict.get("IsUseMultiVectorRetriever", 0)
        if value1 == 1 or value1 == True:
            self.IsUseMultiVectorRetriever = True
        elif (type(value1) == str and value1.upper() == "TRUE"):
            self.IsUseMultiVectorRetriever = True
        else:
            self.IsUseMultiVectorRetriever = False

        self.DocStoreURL = vector_db_item_dict.get("DocStoreURL", "")
        self.VectorDBType = vector_db_item_dict.get("VectorDBType", 0)
        self.CollectionName = vector_db_item_dict.get("CollectionName", "")
        self.CatalogDBURL = vector_db_item_dict.get("CatalogDBURL", "")
        self.ChunkSize = vector_db_item_dict.get("ChunkSize", 0)
        self.DefaultSearchResultLimit = vector_db_item_dict.get("DefaultSearchResultLimit", 0)
        self.IsEnabled = vector_db_item_dict.get("IsEnabled", 0)
        self.IsSystem = vector_db_item_dict.get("IsSystem", 0)

        self.VectorDBTypeString :str = vector_db_item_dict.get("VectorDBTypeString", "")
        if not self.VectorDBTypeString:
            if self.VectorDBType == 1:
                self.VectorDBTypeString = "Chroma"
            elif self.VectorDBType == 2:
                self.VectorDBTypeString = "PGVector"
            else:
                raise ValueError("VectorDBType must be 0 or 1")


        # vector_db_entries ContentUpdateOrDeleteRequestParamsのリスト
        self.VectorMetadataList = [VectorMetadata(entry) for entry in vector_db_item_dict.get("VectorMetadataList", [])]

        # search_kwarg
        self.SearchKwargs = vector_db_item_dict.get("SearchKwargs", {})

        # system_message
        self.SystemMessage = vector_db_item_dict.get("SystemMessage", self.Description)

        # FolderのID
        self.FolderId = vector_db_item_dict.get("FolderId", "")

    @staticmethod
    def get_vector_db_env_variables() -> 'VectorDBItem':
        from dotenv import load_dotenv
        import os
        load_dotenv()
        props: dict = {
            "Name": os.getenv("VECTOR_DB_NAME"),
            "VectorDBUrl": os.getenv("VECTOR_DB_URL"),
            "VectorDBTypeString": os.getenv("VECTOR_DB_TYPE_STRING"),
            "VectorDBDescription": os.getenv("VECTOR_DB_DESCRIPTION"),
            "IsUseMultiVectorRetriever": os.getenv("IS_USE_MULTI_VECTOR_RETRIEVER","false").upper() == "TRUE",
            "DocStoreUrl": os.getenv("DOC_STORE_URL"),
            "CollectionName": os.getenv("VECTOR_DB_COLLECTION_NAME"),
            # チャンクサイズ
            "ChunkSize": int(os.getenv("ChunkSize", 1024)),
            
        }
        return  VectorDBItem(props)

class VectorMetadata:
    def __init__(self, vector_db_entry: dict):

        # request_jsonをdictに変換
        self.source_id = vector_db_entry["source_id"]
        self.description = vector_db_entry.get("description", "")
        self.content = vector_db_entry["content"]
        self.source_path = vector_db_entry.get("source_path", "")
        self.git_repository_url = vector_db_entry.get("git_repository_url", "")
        self.git_relative_path = vector_db_entry.get("git_relative_path", "")
        self.image_url = vector_db_entry.get("image_url", "")

class VectorSearchParameter:
    from ai_chat_explorer.openai_modules import OpenAIProps
    def __init__(self, 
                 openai_props: Union[OpenAIProps, None] = None, 
                 vector_db_props: list[VectorDBItem] = [], 
                 query: str = ""):

        # OpenAIPorpsを生成
        self.openai_props = openai_props

        # VectorDBItemのリストを取得
        self.vector_db_props = vector_db_props

        #  openai_props, vector_db_items, query, search_kwargを設定する
        self.query = query

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

    def get_content_folder(self, folder_id: str) -> Union[ContentFolder, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM ContentFolders WHERE Id=?", (folder_id,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None:
            return None

        folder_dict = dict(row)
        conn.close()

        return ContentFolder(folder_dict)
    
    def get_content_folders(self) -> List[ContentFolder]:
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("SELECT * FROM ContentFolders")
        rows = cur.fetchall()
        folders = [ContentFolder(dict(row)) for row in rows]
        conn.close()

        return folders

    def get_vector_db_item(self, vector_db_item_id: str) -> Union[VectorDBItem, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM VectorDBItems WHERE Id=?", (vector_db_item_id,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None:
            return None
        # debug
        print(dict(row))
        vector_db_item_dict = dict(row)
        conn.close()

        return VectorDBItem(vector_db_item_dict)
    
    def get_vector_db_items(self) -> List[VectorDBItem]:
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("SELECT * FROM VectorDBItems")
        rows = cur.fetchall()
        vector_db_items = [VectorDBItem(dict(row)) for row in rows]
        conn.close()

        return vector_db_items


    def get_autogen_llm_config(self, llm_config_name: str) -> Union[AutogentLLMConfig, None]:
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
    
    def get_autogen_tool(self, tool_name: str) -> Union[AutogenTools, None]:
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

    def get_autogen_agent(self, agent_name: str) -> Union[AutogenAgent, None]:
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


    def get_autogen_group_chat(self, group_chat_name: str) -> Union[AutogenGroupChat, None]:
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