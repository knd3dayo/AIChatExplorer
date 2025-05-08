import sqlite3
import json
from typing import List, Union
import uuid
import os

# main_dbへのパスを取得
def get_main_db_path() -> str:
    app_data_path = os.getenv("APP_DATA_PATH", None)
    if not app_data_path:
        raise ValueError("APP_DATA_PATH is not set.")
    app_db_path = os.path.join(app_data_path, "main_db", "main.db")
    return app_db_path

def init_db(db_path: str):
        # MainDBを取得
    main_db = MainDB(db_path)
    # ContentFoldersテーブルを初期化
    main_db.init_content_folder_table()
    # TagItemsテーブルを初期化
    main_db.init_tag_item_table()
    # ContentItemsテーブルを初期化
    main_db.init_content_item_table()
    # VectorDBItemsテーブルを初期化
    main_db.init_vector_db_item_table()
    # autogen_llm_configsテーブルを初期化
    main_db.init_autogen_llm_config_table()
    # autogen_toolsテーブルを初期化
    main_db.init_autogen_tools_table()
    # autogen_agentsテーブルを初期化
    main_db.init_autogen_agents_table()
    # autogen_group_chatsテーブルを初期化
    main_db.init_autogen_group_chats_table()


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
 
    def to_dict(self) -> dict:
        return {
            "Id": self.Id,
            "FolderTypeString": self.FolderTypeString,
            "ParentId": self.ParentId,
            "FolderName": self.FolderName,
            "Description": self.Description,
            "ExtendedPropertiesJson": self.ExtendedPropertiesJson
        }

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
        "ChunkSize" INTEGER NOT NULL,
        "DefaultSearchResultLimit" INTEGER NOT NULL,
        "IsEnabled" INTEGER NOT NULL,
        "IsSystem" INTEGER NOT NULL
    )    
    '''
    def __init__(self, vector_db_item_dict: dict):
        self.Id = vector_db_item_dict.get("Id", "")
        if not self.Id:
            # UUIDを生成
            self.Id = str(uuid.uuid4())
        
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
                raise ValueError("VectorDBType must be 1 or 2")

        # vector_db_entries ContentUpdateOrDeleteRequestParamsのリスト
        metadata = vector_db_item_dict.get("Embedding" , None)
        self.EmbeddingData = EmbeddingData(metadata) if metadata else None
        # search_kwarg
        self.SearchKwargs = vector_db_item_dict.get("SearchKwargs", {})
        # system_message
        self.SystemMessage = vector_db_item_dict.get("SystemMessage", self.Description)
        # FolderのID
        self.FolderId = vector_db_item_dict.get("FolderId", "")

        # input_text
        self.input_text = vector_db_item_dict.get("input_text", None)
        # search_kwarg
        self.search_kwarg = vector_db_item_dict.get("SearchKwarg", None)

    def to_dict(self) -> dict:
        return {
            "Id": self.Id,
            "Name": self.Name,
            "Description": self.Description,
            "VectorDBURL": self.VectorDBURL,
            "IsUseMultiVectorRetriever": self.IsUseMultiVectorRetriever,
            "DocStoreURL": self.DocStoreURL,
            "VectorDBType": self.VectorDBType,
            "CollectionName": self.CollectionName,
            "CatalogDBURL": self.CatalogDBURL,
            "ChunkSize": self.ChunkSize,
            "DefaultSearchResultLimit": self.DefaultSearchResultLimit,
            "IsEnabled": self.IsEnabled,
            "IsSystem": self.IsSystem
        }
    
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

class EmbeddingData:
    def __init__(self, item: dict):

        # request_jsonをdictに変換
        self.source_id = item["source_id"]
        self.FolderId = item["FolderId"]
        self.description = item.get("description", "")
        self.content = item["content"]
        self.source_path = item.get("source_path", "")
        self.git_repository_url = item.get("git_repository_url", "")
        self.git_relative_path = item.get("git_relative_path", "")
        self.image_url = item.get("image_url", "")

class TagItem:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE "TagItems" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_TagItems" PRIMARY KEY,
    "Tag" TEXT NOT NULL,
    "IsPinned" INTEGER NOT NULL
    )
    '''
    def __init__(self, tag_item_dict: dict):
        self.Id = tag_item_dict.get("Id", "")
        if not self.Id:
            # UUIDを生成
            self.Id = str(uuid.uuid4())
        self.Tag = tag_item_dict.get("Tag", "")
        is_pinned = tag_item_dict.get("IsPinned", 0)
        if type(is_pinned) == int:
            self.IsPinned = bool(is_pinned)
        elif type(is_pinned) == bool:
            self.IsPinned = is_pinned
        elif type(is_pinned) == str and is_pinned.upper() == "TRUE":
            self.IsPinned = True
        else:
            self.IsPinned = False

    def to_dict(self) -> dict:
        return {
            "Id": self.Id,
            "Tag": self.Tag,
            "IsPinned": self.IsPinned
        }
    

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
        self.vector_db_items = agent_dict.get("vector_db_props", json.dumps([]))

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
    def init_content_folder_table(self):
        # ContentFoldersテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS ContentFolders (
                Id TEXT NOT NULL PRIMARY KEY,
                FolderTypeString TEXT NOT NULL,
                ParentId TEXT NULL,
                FolderName TEXT NOT NULL,
                Description TEXT NOT NULL,
                ExtendedPropertiesJson TEXT NOT NULL
            )
        ''')
        conn.commit()
        conn.close()
    
    def init_tag_item_table(self):
        # TagItemsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS TagItems (
                Id TEXT NOT NULL PRIMARY KEY,
                Tag TEXT NOT NULL,
                IsPinned INTEGER NOT NULL
            )
        ''')
        conn.commit()
        conn.close()


    def init_content_item_table(self):
        # ContentItemsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS ContentItems (
                Id TEXT NOT NULL PRIMARY KEY,
                FolderId TEXT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL,
                VectorizedAt TEXT NOT NULL,
                Content TEXT NOT NULL,
                Description TEXT NOT NULL,
                ContentType INTEGER NOT NULL,
                ChatMessagesJson TEXT NOT NULL,
                PromptChatResultJson TEXT NOT NULL,
                TagString TEXT NOT NULL,
                SourceApplicationName TEXT NOT NULL,
                SourceApplicationTitle TEXT NOT NULL,
                SourceApplicationID INTEGER NOT NULL,
                SourceApplicationPath TEXT NOT NULL,
                IsPinned INTEGER NOT NULL,
                DocumentReliability INTEGER NOT NULL,
                DocumentReliabilityReason TEXT NOT NULL,
                IsReferenceVectorDBItemsSynced INTEGER NOT NULL,
                CachedBase64String TEXT NOT NULL,
                ExtendedPropertiesJson TEXT NOT NULL
            )
        ''')
        conn.commit()
        conn.close()

    def init_vector_db_item_table(self):
        # VectorDBItemsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS VectorDBItems (
                Id TEXT NOT NULL PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                VectorDBURL TEXT NOT NULL,
                IsUseMultiVectorRetriever INTEGER NOT NULL,
                DocStoreURL TEXT NOT NULL,
                VectorDBType INTEGER NOT NULL,
                CollectionName TEXT NOT NULL,
                CatalogDBURL TEXT NOT NULL,
                ChunkSize INTEGER NOT NULL,
                DefaultSearchResultLimit INTEGER NOT NULL,
                IsEnabled INTEGER NOT NULL,
                IsSystem INTEGER NOT NULL
            )
        ''')
        conn.commit()
        conn.close()
        self.__init_default_vector_db_item()

    def __init_default_vector_db_item(self):
        # name="default"のVectorDBItemを取得
        vector_db_item = self.get_vector_db_by_name("default")
        # 存在しない場合は初期化処理
        if not vector_db_item:
            # VectorDBItemを作成
            params = {
                "Name": "default",
                "Description": "Application default vector db",
                "VectorDBType": 1,
                "VectorDBURL": os.path.join(os.getenv("APP_DATA_PATH", ""), "vector_db", "clipboard_vector_db"),
                "DocStoreURL": os.path.join(os.getenv("APP_DATA_PATH", ""), "vector_db", "clipboard_doc_store.db"),
                "IsUseMultiVectorRetriever": True,
                "IsEnable": True,
                "IsSystem": False,
                "CollectionName": "ai_app_default_collection",

            }
            vector_db_item = VectorDBItem(params)
            # VectorDBItemのプロパティを設定

            # MainDBに追加
            self.update_vector_db_item(vector_db_item)

        else:
            # 存在する場合は初期化処理を行わない
            print("VectorDBItem is already exists.")

    def init_autogen_llm_config_table(self):
        # autogen_llm_configsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS autogen_llm_configs (
                name TEXT PRIMARY KEY,
                api_type TEXT,
                api_version TEXT,
                model TEXT,
                api_key TEXT,
                base_url TEXT
            )
        ''')
        conn.commit()
        conn.close()

    def init_autogen_tools_table(self):
        # autogen_toolsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS autogen_tools (
                name TEXT PRIMARY KEY,
                path TEXT,
                description TEXT
            )
        ''')
        conn.commit()
        conn.close()

    def init_autogen_agents_table(self):
        # autogen_agentsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS autogen_agents (
                name TEXT PRIMARY KEY,
                description TEXT,
                system_message TEXT,
                code_execution BOOLEAN,
                llm_config_name TEXT,
                tool_names TEXT,
                vector_db_items TEXT
            )
        ''')
        conn.commit()
        conn.close()
        
    def init_autogen_group_chats_table(self):
        # autogen_group_chatsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS autogen_group_chats (
                name TEXT PRIMARY KEY,
                description TEXT,
                llm_config_name TEXT,
                agent_names TEXT
            )
        ''')
        conn.commit()
        conn.close()
    

    #########################################
    # ContentItem関連
    #########################################

    def get_content_folder(self, folder_id: str) -> Union[ContentFolder, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM ContentFolders WHERE Id=?", (folder_id,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None or len(row) == 0:
            return None

        folder_dict = dict(row)
        conn.close()

        return ContentFolder(folder_dict)
    
    def get_content_folders(self) -> List[ContentFolder]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row
        cur = conn.cursor()
        cur.execute("SELECT * FROM ContentFolders")
        rows = cur.fetchall()
        folders = [ContentFolder(dict(row)) for row in rows]
        conn.close()

        return folders
    
    # FolderTypeStringを指定してContentFolderのRootFolderを取得する
    def get_root_content_folder(self, folder_type_string: str) -> Union[ContentFolder, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM ContentFolders WHERE FolderTypeString=? AND ParentId IS NULL", (folder_type_string,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None or len(row) == 0:
            return None

        folder_dict = dict(row)
        conn.close()

        return ContentFolder(folder_dict)
    
    def update_content_folder(self, folder: ContentFolder):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        if self.get_content_folder(folder.Id) is None:
            cur.execute("INSERT INTO ContentFolders VALUES (?, ?, ?, ?, ?, ?)", (folder.Id, folder.FolderTypeString, folder.ParentId, folder.FolderName, folder.Description, folder.ExtendedPropertiesJson))
        else:
            cur.execute("UPDATE ContentFolders SET FolderTypeString=?, ParentId=?, FolderName=?, Description=?, ExtendedPropertiesJson=? WHERE Id=?", (folder.FolderTypeString, folder.ParentId, folder.FolderName, folder.Description, folder.ExtendedPropertiesJson, folder.Id))
        conn.commit()
        conn.close()

    def delete_content_folder(self, folder: ContentFolder):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("DELETE FROM ContentFolders WHERE Id=?", (folder.Id,))
        conn.commit()
        conn.close()

    ########################################
    # TagItem関連
    ########################################
    def get_tag_item(self, tag_id: str) -> Union[TagItem, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM TagItems WHERE Id=?", (tag_id,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None or len(row) == 0:
            return None

        tag_item_dict = dict(row)
        conn.close()

        return TagItem(tag_item_dict)
    
    def get_tag_items(self) -> List[TagItem]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM TagItems")
        rows = cur.fetchall()
        tag_items = [TagItem(dict(row)) for row in rows]
        conn.close()

        return tag_items
    
    def update_tag_item(self, tag_item: TagItem) -> TagItem:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        if self.get_tag_item(tag_item.Id) is None:
            cur.execute("INSERT INTO TagItems VALUES (?, ?, ?)", (tag_item.Id, tag_item.Tag, tag_item.IsPinned))
        else:
            cur.execute("UPDATE TagItems SET Tag=?, IsPinned=? WHERE Id=?", (tag_item.Tag, tag_item.IsPinned, tag_item.Id))
        conn.commit()
        conn.close()

        # 更新したTagItemを返す
        return tag_item
    
    def delete_tag_item(self, tag_item: TagItem):
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("DELETE FROM TagItems WHERE Id=?", (tag_item.Id,))
        conn.commit()
        conn.close()

    ########################################
    # VectorDBItem関連
    ########################################
    # Idを指定してVectorDBItemのdictを取得する
    def get_vector_db_item_dict_by_id(self, vector_db_item_id: str) -> Union[dict, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM VectorDBItems WHERE Id=?", (vector_db_item_id,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None or len(row) == 0:
            return None

        vector_db_item_dict = dict(row)
        conn.close()

        return vector_db_item_dict

    # Idを指定してVectorDBItemを取得する
    def get_vector_db_by_id(self, vector_db_item_id: str) -> Union[VectorDBItem, None]:
        vector_db_item_dict = self.get_vector_db_item_dict_by_id(vector_db_item_id)
        if vector_db_item_dict is None:
            return None
        return VectorDBItem(vector_db_item_dict)

    # nameを指定してVectorDBItemのdictを取得する
    def get_vector_db_item_dict_by_name(self, vector_db_item_name: str) -> Union[dict, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM VectorDBItems WHERE Name=?", (vector_db_item_name,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None or len(row) == 0:
            return None

        vector_db_item_dict = dict(row)
        conn.close()

        return vector_db_item_dict

    # Nameを指定してVectorDBItemを取得する
    def get_vector_db_by_name(self, vector_db_item_name: str) -> Union[VectorDBItem, None]:
        vector_db_item_dict = self.get_vector_db_item_dict_by_name(vector_db_item_name)
        if vector_db_item_dict is None:
            return None
        return VectorDBItem(vector_db_item_dict)
    
    def get_vector_db_items(self) -> List[VectorDBItem]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM VectorDBItems")
        rows = cur.fetchall()
        vector_db_items = [VectorDBItem(dict(row)) for row in rows]
        conn.close()

        return vector_db_items
    
    def update_vector_db_item(self, vector_db_item: VectorDBItem) -> VectorDBItem:
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
        # VectorDBTypeStringからVectorDBTypeを取得
        if vector_db_item.VectorDBTypeString == "Chroma":
            vector_db_item.VectorDBType = 1
        elif vector_db_item.VectorDBTypeString == "PGVector":
            vector_db_item.VectorDBType = 2
        else:
            raise ValueError("VectorDBType must be 1 or 2")

        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        if self.get_vector_db_by_id(vector_db_item.Id) is None:
            cur.execute("INSERT INTO VectorDBItems VALUES (?, ? , ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                         (vector_db_item.Id, vector_db_item.Name, vector_db_item.Description, 
                          vector_db_item.VectorDBURL, vector_db_item.IsUseMultiVectorRetriever, 
                          vector_db_item.DocStoreURL, vector_db_item.VectorDBType, 
                          vector_db_item.CollectionName, 
                          vector_db_item.ChunkSize, vector_db_item.DefaultSearchResultLimit, 
                          vector_db_item.IsEnabled, vector_db_item.IsSystem)
                          )
        else:
            cur.execute("UPDATE VectorDBItems SET Name=?, Description=?, VectorDBURL=?, IsUseMultiVectorRetriever=?, DocStoreURL=?, VectorDBType=?, CollectionName=?, ChunkSize=?, DefaultSearchResultLimit=?, IsEnabled=?, IsSystem=? WHERE Id=?",
                         (vector_db_item.Name, vector_db_item.Description, vector_db_item.VectorDBURL, 
                          vector_db_item.IsUseMultiVectorRetriever, vector_db_item.DocStoreURL, 
                          vector_db_item.VectorDBType, vector_db_item.CollectionName, 
                          vector_db_item.ChunkSize, 
                          vector_db_item.DefaultSearchResultLimit, vector_db_item.IsEnabled, 
                          vector_db_item.IsSystem, vector_db_item.Id)
                          )
        conn.commit()
        conn.close()

        # 更新したVectorDBItemを返す
        return vector_db_item

    def delete_vector_db_item(self, vector_db_item: VectorDBItem):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("DELETE FROM VectorDBItems WHERE Id=?", (vector_db_item.Id,))
        conn.commit()
        conn.close()

    #################################################
    # Autogen関連
    #################################################
    def get_autogen_llm_config(self, llm_config_name: str) -> Union[AutogentLLMConfig, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_llm_configs WHERE name=?", (llm_config_name,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None or len(row) == 0:
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
        if row is None or len(row) == 0:
            return None

        tool_dict = dict(row)
        conn.close()

        return AutogenTools(tool_dict)
    
    def get_autogen_tools(self) -> List[AutogenTools]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
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
        if row is None or len(row) == 0:
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
        if row is None or len(row) == 0:
            return None

        group_chat_dict = dict(row)
        conn.close()

        return AutogenGroupChat(group_chat_dict)