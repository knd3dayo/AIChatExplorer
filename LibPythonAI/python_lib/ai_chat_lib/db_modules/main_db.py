import sqlite3
import json
from typing import List, Union
import uuid
import os

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

class ContentFoldersCatalog:

    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE "ContentFoldersCatalog" (
    "id" TEXT NOT NULL CONSTRAINT "PK_ContentFoldersCatalog" PRIMARY KEY,
    "folder_type_string" TEXT NOT NULL,
    "parent_id" TEXT NULL,
    "folder_name" TEXT NOT NULL,
    "description" TEXT NOT NULL,
    "extended_properties_json" TEXT NOT NULL
    "is_root_folder" INTEGER NOT NULL,
    )
    '''
    get_content_folder_requelsts_name = "content_folder_requests"

    @classmethod
    def get_content_folder_requelst_objects(cls, request_dict: dict) -> list["ContentFoldersCatalog"]:
        '''
        {"content_folder_requsts": [] }の形式で渡される
        '''
        # contextを取得
        content_folders: list[dict] = request_dict.get(cls.get_content_folder_requelsts_name, None)
        if not content_folders:
            raise ValueError("content_folder is not set.")
        
        # content_folderを生成
        result = []
        for item in content_folders:
            content_folder = ContentFoldersCatalog(item)
            result.append(content_folder)

        return result

    @classmethod
    def get_root_content_folders_api(cls):
        # request_jsonからrequestを作成
        # MainDBを取得
        main_db = MainDB()
        # ルートフォルダの情報を取得
        content_folders = main_db.get_root_content_folders()

        result = {}
        result["content_folders"] = [item.to_dict() for item in content_folders]
        return result
    
    @classmethod
    def get_content_folders_api(cls):
        # request_jsonからrequestを作成
        # MainDBを取得
        main_db = MainDB()
        # フォルダの情報を取得
        content_folders = main_db.get_content_folders()

        result = {}
        result["content_folders"] = [item.to_dict() for item in content_folders]
        return result
    
    @classmethod
    def get_content_folder_by_id_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # content_folder_idを取得
        content_folder_id = ContentFoldersCatalog.get_content_folder_requelst_objects(request_dict)[0].Id
        if not content_folder_id:
            raise ValueError("content_folder_id is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # フォルダの情報を取得
        content_folder = main_db.get_content_folder_by_id(content_folder_id)

        result: dict = {}
        if content_folder is not None:
            result["content_folder"] = content_folder.to_dict()
        return result
    
    @classmethod
    def update_content_folders_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # content_folderを取得
        content_folders = ContentFoldersCatalog.get_content_folder_requelst_objects(request_dict)
        if len(content_folders) == 0:
            raise ValueError("content_folder is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # フォルダの情報を更新
        for content_folder in content_folders:
            main_db.update_content_folder(content_folder)

        result: dict = {}
        return result
    
    @classmethod
    def delete_content_folders_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # content_folderを取得
        content_folders = ContentFoldersCatalog.get_content_folder_requelst_objects(request_dict)
        if len(content_folders) == 0:
            raise ValueError("content_folder is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # フォルダの情報を削除
        for content_folder in content_folders:
            main_db.delete_content_folder(content_folder)

        result: dict = {}
        return result

    @classmethod
    def get_content_folder_by_path_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # content_folder_pathを取得
        content_folder_path = request_dict.get("content_folder_path", None)
        if not content_folder_path:
            raise ValueError("content_folder_path is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # フォルダの情報を取得
        content_folder = main_db.get_content_folder_by_path(content_folder_path)

        result: dict = {}
        if content_folder is not None:
            result["content_folder"] = content_folder.to_dict()
        return result
    
    @classmethod
    def get_content_folder_path_by_id_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # content_folder_idを取得
        content_folder_id = ContentFoldersCatalog.get_content_folder_requelst_objects(request_dict)[0].Id
        if not content_folder_id:
            raise ValueError("content_folder_id is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # フォルダの情報を取得
        content_folder_path = main_db.get_content_folder_path_by_id(content_folder_id)

        result: dict = {}
        if content_folder_path is not None:
            result["content_folder_path"] = content_folder_path
        return result

    def __init__(self, content_folder_dict: dict):
        self.Id = content_folder_dict.get("id", "")
        self.FolderTypeString = content_folder_dict.get("folder_type_string", "")
        self.ParentId = content_folder_dict.get("parent_id", "")
        self.FolderName = content_folder_dict.get("folder_name", "")
        self.Description = content_folder_dict.get("description", "")
        self.ExtendedPropertiesJson = content_folder_dict.get("extended_properties_json", "")
        self.FolderPath = content_folder_dict.get("folder_path", "")
        IsRootFolder = content_folder_dict.get("is_root_folder", 0)
        # is_root_folderがintの場合
        if type(IsRootFolder) == int:
            self.IsRootFolder = bool(IsRootFolder)
        # is_root_folderがboolの場合
        elif type(IsRootFolder) == bool:
            self.IsRootFolder = IsRootFolder
        # is_root_folderがstrの場合
        elif type(IsRootFolder) == str and IsRootFolder.upper() == "TRUE":
            self.IsRootFolder = True
        else:
            self.IsRootFolder = False
 
    def to_dict(self) -> dict:
        
        result = {
            "id": self.Id,
            "folder_type_string": self.FolderTypeString,
            "parent_id": self.ParentId,
            "folder_name": self.FolderName,
            "description": self.Description,
            "extended_properties_json": self.ExtendedPropertiesJson,
        }
        if self.FolderPath:
            result["folder_path"] = self.FolderPath
        return result

class VectorDBItem:


    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE "VectorDBItems" (
        "id" TEXT NOT NULL CONSTRAINT "PK_VectorDBItems" PRIMARY KEY,
        "name" TEXT NOT NULL,
        "description" TEXT NOT NULL,
        "vector_db_url" TEXT NOT NULL,
        "is_use_multi_vector_retriever" INTEGER NOT NULL,
        "doc_store_url" TEXT NOT NULL,
        "vector_db_type" INTEGER NOT NULL,
        "collection_name" TEXT NOT NULL,
        "chunk_size" INTEGER NOT NULL,
        "default_search_result_limit" INTEGER NOT NULL,
        "is_enabled" INTEGER NOT NULL,
        "is_system" INTEGER NOT NULL
    )    
    '''
    # コレクションの指定がない場合はデフォルトのコレクション名を使用
    DEFAULT_COLLECTION_NAME = "ai_app_default_collection"
    FOLDER_CATALOG_COLLECTION_NAME = "ai_app_folder_catalog_collection"

    @classmethod
    def update_vector_db_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # vector_db_itemを取得
        vector_db_item = VectorDBItem.get_vector_db_item_object(request_dict)
        # vector_dbを更新
        main_db = MainDB()
        main_db.update_vector_db_item(vector_db_item)
        # 更新したVectorDBItemを返す
        result: dict = {}
        result["vector_db_item"] = vector_db_item.to_dict()
        return result

    @classmethod
    def delete_vector_db_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # vector_db_itemを取得
        vector_db_item = VectorDBItem.get_vector_db_item_object(request_dict)
        # vector_dbを削除
        main_db = MainDB()
        # ベクトルDBを削除する
        result: dict = main_db.delete_vector_db_item(vector_db_item)
        result["vector_db_item"] = vector_db_item.to_dict()
        return result

    @classmethod
    def get_vector_db_items_api(cls):
        # request_jsonからrequestを作成
        main_db = MainDB()
        # ベクトルDBの一覧を取得する
        vector_db_list = main_db.get_vector_db_items()

        result = {}
        result["vector_db_items"] = [item.to_dict() for item in vector_db_list]
        return result

    @classmethod
    def get_vector_db_item_by_id_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # vector_db_idを取得
        vector_db_id =  VectorDBItem.get_vector_db_item_object(request_dict).Id
        if not vector_db_id:
            raise ValueError("vector_db_id is not set")
        # idからVectorDBItemを取得
        main_db = MainDB()
        vector_db_item = main_db.get_vector_db_by_id(vector_db_id)

        result: dict = {}
        if vector_db_item is not None:
            result["vector_db_item"] = vector_db_item.to_dict()
        return result

    @classmethod
    def get_vector_db_item_by_name_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # vector_db_nameを取得
        vector_db_name = VectorDBItem.get_vector_db_item_object(request_dict).Name
        if not vector_db_name:
            raise ValueError("vector_db_name is not set")

        # nameからVectorDBItemを取得
        main_db = MainDB()
        vector_db = main_db.get_vector_db_by_name(vector_db_name)

        result: dict = {}
        if vector_db is not None:
            result["vector_db_item"] = vector_db.to_dict()
        return result

    vector_db_item_request_name = "vector_db_item_request"
    @classmethod
    def get_vector_db_item_object(cls, request_dict: dict) -> "VectorDBItem":
        '''
        {"vector_db_item_request": {}}の形式で渡される
        '''
        # vector_db_item_requestを取得
        vector_db_item_request: dict = request_dict.get(cls.vector_db_item_request_name, None)
        if not vector_db_item_request:
            raise ValueError("vector_db_item_request is not set.")

        # vector_db_itemを生成
        vector_db_item = VectorDBItem(vector_db_item_request)
        return vector_db_item    

    def __init__(self, vector_db_item_dict: dict):
        self.Id = vector_db_item_dict.get("id", "")
        if not self.Id:
            # UUIDを生成
            self.Id = str(uuid.uuid4())
        
        self.Name = vector_db_item_dict.get("name", "")
        self.Description = vector_db_item_dict.get("description", "")
        self.VectorDBURL = vector_db_item_dict.get("vector_db_url", "")
        value1 = vector_db_item_dict.get("is_use_multi_vector_retriever", 0)
        if value1 == 1 or value1 == True:
            self.IsUseMultiVectorRetriever = True
        elif (type(value1) == str and value1.upper() == "TRUE"):
            self.IsUseMultiVectorRetriever = True
        else:
            self.IsUseMultiVectorRetriever = False

        self.DocStoreURL = vector_db_item_dict.get("doc_store_url", "")
        self.VectorDBType = vector_db_item_dict.get("vector_db_type", 0)
        self.CollectionName = vector_db_item_dict.get("collection_name", "")
        self.ChunkSize = vector_db_item_dict.get("chunk_size", 0)
        self.DefaultSearchResultLimit = vector_db_item_dict.get("default_search_result_limit", 0)
        IsEnabled = vector_db_item_dict.get("is_enabled", 0)
        IsSystem = vector_db_item_dict.get("is_system", 0)
        # is_enabledがintの場合
        if type(IsEnabled) == int:
            self.IsEnabled = bool(IsEnabled)
        # is_enabledがboolの場合
        elif type(IsEnabled) == bool:
            self.IsEnabled = IsEnabled
        # is_enabledがstrの場合
        elif type(IsEnabled) == str and IsEnabled.upper() == "TRUE":
            self.IsEnabled = True
        else:
            self.IsEnabled = False
        # is_systemがintの場合
        if type(IsSystem) == int:
            self.IsSystem = bool(IsSystem)
        # is_systemがboolの場合
        elif type(IsSystem) == bool:
            self.IsSystem = IsSystem
        # is_systemがstrの場合
        elif type(IsSystem) == str and IsSystem.upper() == "TRUE":
            self.IsSystem = True
        else:
            self.IsSystem = False

        self.VectorDBTypeString :str = vector_db_item_dict.get("vector_db_type_string", "")
        if not self.VectorDBTypeString:
            if self.VectorDBType == 1:
                self.VectorDBTypeString = "Chroma"
            elif self.VectorDBType == 2:
                self.VectorDBTypeString = "PGVector"
            else:
                raise ValueError("VectorDBType must be 1 or 2")

        # system_message
        self.SystemMessage = vector_db_item_dict.get("system_message", self.Description)
        # FolderのID
        self.FolderId = vector_db_item_dict.get("folder_id", "")


    def to_dict(self) -> dict:
        return {
            "id": self.Id,
            "name": self.Name,
            "description": self.Description,
            "vector_db_url": self.VectorDBURL,
            "is_use_multi_vector_retriever": self.IsUseMultiVectorRetriever,
            "doc_store_url": self.DocStoreURL,
            "vector_db_type": self.VectorDBType,
            "vector_db_type_string": self.VectorDBTypeString,
            "collection_name": self.CollectionName,
            "chunk_size": self.ChunkSize,
            "default_search_result_limit": self.DefaultSearchResultLimit,
            "is_enabled": self.IsEnabled,
            "is_system": self.IsSystem,
        }
    

class VectorSearchRequest:

    vector_search_requests_name = "vector_search_requests"
    @classmethod
    def get_vector_search_requests_objects(cls, request_dict: dict) -> list["VectorSearchRequest"]:
        '''
        {"vector_search_request": {}}の形式で渡される
        '''
        # contextを取得
        request:list[dict] = request_dict.get(cls.vector_search_requests_name, None)
        if not request:
            logger.info("vector search request is not set. skipping.")
            return []

        vector_search_requests = []
        for item in request:
            # vector_search_requestsを生成
            vector_search_request = VectorSearchRequest(item)
            vector_search_requests.append(vector_search_request)
        return vector_search_requests

    def __init__(self, item: dict):
        self.name = item.get("name", "")
        self.query = item.get("query", "")
        self.model = item.get("model", "")
        self.vector_db_item = None
        # search_kwargsのアップデート
        self.search_kwargs = self.__update_search_kwargs(item.get("search_kwargs", {}))

    def __update_search_kwargs(self, kwargs: dict):
        # filterが存在する場合
        filter = kwargs.get("filter", None)
        if not filter:
            logger.debug("__update_search_kwargs: filter is not set.")
            return kwargs
        # folder_pathが指定されている場合はfolder_pathからfolder_idを取得する
        folder_path = filter.get("folder_path", None)
        if folder_path:
            logger.info(f"__update_search_kwargs: folder_path: {folder_path}")
            # MainDBを取得
            main_db = MainDB()
            # folder_pathからfolder_idを取得
            temp_folder_id = main_db.get_content_folder_path_by_id(folder_path)
            if temp_folder_id:
                kwargs["filter"]["folder_id"] = temp_folder_id
        
            del kwargs["filter"]["folder_path"]
        else:
            logger.info("__update_search_kwargs: folder_path is not set.")

        return kwargs

    def get_vector_db_item(self) -> VectorDBItem:
        if self.vector_db_item:
            return self.vector_db_item

        # nameからVectorDBItemを取得
        main_db = MainDB()
        vector_db_item = main_db.get_vector_db_by_name(self.name)
        if not vector_db_item:
            raise ValueError("VectorDBItem is not found.")
        return vector_db_item


    def to_dict(self) -> dict:
        return {
            "name": self.name,
            "query": self.query,
            "model": self.model,
            "search_kwargs": self.search_kwargs
        }
    

class EmbeddingData:


    embedding_request_name = "embedding_request"

    @classmethod
    def get_embedding_request_objects(cls, request_dict: dict) -> "EmbeddingData":
        '''
        {"embedding_request": {}}の形式で渡される
        '''
        # contextを取得
        request: dict = request_dict.get(cls.embedding_request_name, None)
        if not request:
            raise ValueError("request is not set.")
        # MainDBを取得
    
        return EmbeddingData(request)

    def __init__(self, item: dict):
    # nameからVectorDBItemを取得
        name = item.get("name", None)
        if not name:
            raise ValueError("name is not set.")
        
        model = item.get("model", None)
        if not model:
            raise ValueError("model is not set.")

        # request_jsonをdictに変換
        self.name = name
        self.model = model

        self.source_id = item["source_id"]
        self.FolderId = item["folder_id"]
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
    "id" TEXT NOT NULL CONSTRAINT "PK_TagItems" PRIMARY KEY,
    "tag" TEXT NOT NULL,
    "is_pinned" INTEGER NOT NULL
    )
    '''
    @classmethod
    def get_tag_item_objects(cls, request_dict: dict) -> list["TagItem"]:
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

    @classmethod
    def get_tag_items_api(cls, request_json: str):
        # request_jsonからrequestを作成
        # request_dict: dict = json.loads(request_json)
        # tag_itemsを取得
        # MainDBを取得
        main_db = MainDB()
        # タグの一覧を取得
        tag_items = main_db.get_tag_items()
        result: dict = {}
        result["tag_items"] = [item.to_dict() for item in tag_items]
        return result

    @classmethod
    def update_tag_items_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # tag_itemsを取得
        tag_items = TagItem.get_tag_item_objects(request_dict)
        # tag_itemsを更新
        # MainDBを取得
        main_db = MainDB()
        # タグを更新
        for tag_item in tag_items:
            main_db.update_tag_item(tag_item)

        result: dict = {}
        return result
    
    @classmethod
    def delete_tag_items_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # tag_itemsを取得
        tag_items = TagItem.get_tag_item_objects(request_dict)
        # MainDBを取得
        main_db = MainDB()
        # タグを削除
        for tag_item in tag_items:
            main_db.delete_tag_item(tag_item)

        result: dict = {}
        return result



    def __init__(self, tag_item_dict: dict):
        self.Id = tag_item_dict.get("id", "")
        if not self.Id:
            # UUIDを生成
            self.Id = str(uuid.uuid4())
        self.Tag = tag_item_dict.get("tag", "")
        is_pinned = tag_item_dict.get("is_pinned", 0)
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
            "id": self.Id,
            "tag": self.Tag,
            "is_pinned": self.IsPinned
        }
    

class AutogentLLMConfig:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE autogen_llm_configs (name TEXT, api_type TEXT, api_version TEXT, model TEXT, api_key TEXT, base_url TEXT)
    '''
    @classmethod
    def get_autogen_llm_config_list_api(cls):
        # autogen_propsからllm_config_listを取得
        # MainDBを取得
        main_db = MainDB()
        # LLMConfigの一覧を取得
        llm_config_list = main_db.get_autogen_llm_config_list()

        if not llm_config_list:
            raise ValueError("llm_config_list is not set")
        
        result = {}
        result["llm_config_list"] = [config.to_dict() for config in llm_config_list]
        return result

    @classmethod
    def get_autogen_llm_config_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからllm_configを取得
        llm_config = AutogentLLMConfig.get_autogen_llm_config_object(request_dict)
        if not llm_config:
            raise ValueError("llm_config is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # LLMConfigを取得
        llm_config_result = main_db.get_autogen_llm_config(llm_config.name)

        result: dict = {}
        if llm_config_result:
            result["llm_config"] = llm_config_result.to_dict()

        return result

    @classmethod
    def update_autogen_llm_config_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからllm_configを取得
        llm_config = AutogentLLMConfig.get_autogen_llm_config_object(request_dict)
        if not llm_config:
            raise ValueError("llm_config is not set")
        
        # autogen_propsを更新
        # MainDBを取得
        main_db = MainDB()
        # LLMConfigを更新
        main_db.update_autogen_llm_config(llm_config)

        result: dict = {}
        return result

    @classmethod
    def delete_autogen_llm_config_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからllm_configを取得
        llm_config = AutogentLLMConfig.get_autogen_llm_config_object(request_dict)
        if not llm_config:
            raise ValueError("llm_config is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # LLMConfigを削除
        main_db.delete_autogen_llm_config(llm_config)

        result: dict = {}
        return result

    autogen_llm_config_request_name = "autogen_llm_config_request"
    @classmethod
    def get_autogen_llm_config_object(cls, request_dict: dict) -> "AutogentLLMConfig":
        '''
        {"autogen_llm_config_request": {}}の形式で渡される
        '''
        # autogen_llm_config_requestを取得
        request:dict = request_dict.get(cls.autogen_llm_config_request_name, None)
        if not request:
            raise ValueError("request is not set.")
        result = AutogentLLMConfig(request)
        return result

    def __init__(self, llm_config_dict: dict):
        self.name = llm_config_dict.get("name", "")
        self.api_type = llm_config_dict.get("api_type", "")
        self.api_version = llm_config_dict.get("api_version", "")
        self.model = llm_config_dict.get("model", "")
        self.api_key = llm_config_dict.get("api_key", "")
        self.base_url = llm_config_dict.get("base_url", "")

    def to_dict(self) -> dict:
        return {
            "name": self.name,
            "api_type": self.api_type,
            "api_version": self.api_version,
            "model": self.model,
            "api_key": self.api_key,
            "base_url": self.base_url
        }

class AutogenTools:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE autogen_tools (name TEXT, path TEXT, description TEXT)    '''

    @classmethod
    def get_autogen_tool_list_api(cls, ):
        # autogen_propsからtools_listを取得
        # MainDBを取得
        main_db = MainDB()
        # AutogenToolsの一覧を取得
        tools_list = main_db.get_autogen_tool_list()
        
        result = {}
        result["tool_list"] = [tool.to_dict() for tool in tools_list]
        return result

    @classmethod
    def get_autogen_tool_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからtoolを取得
        tool = AutogenTools.get_autogen_tool_object(request_dict)
        if not tool:
            raise ValueError("tool is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # AutogenToolsを取得
        tool_result = main_db.get_autogen_tool(tool.name)
        
        result: dict = {}
        if tool_result:
            result["tool"] = tool_result.to_dict()
        return result

    @classmethod
    def update_autogen_tool_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからtoolを取得
        tool = AutogenTools.get_autogen_tool_object(request_dict)
        if not tool:
            raise ValueError("tool is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # AutogenToolsを更新
        main_db.update_autogen_tool(tool)

        result: dict = {}
        return result

    @classmethod
    def delete_autogen_tool_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからtoolを取得
        tool = AutogenTools.get_autogen_tool_object(request_dict)
        if not tool:
            raise ValueError("tool is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # AutogenToolsを削除
        main_db.delete_autogen_tool(tool)

        result: dict = {}
        return result


    autogen_tool_request_name = "autogen_tool_request"
    @classmethod
    def get_autogen_tool_object(cls, request_dict: dict) -> "AutogenTools":
        '''
        {"autogen_tool_request": {}}の形式で渡される
        '''
        # autogen_tool_requestを取得
        request:dict = request_dict.get(cls.autogen_tool_request_name, None)
        if not request:
            raise ValueError("request is not set.")
        # autogen_toolを生成
        autogen_tool = AutogenTools(request)
        return autogen_tool

    def __init__(self, tools_dict: dict):
        self.name = tools_dict.get("name", "")
        self.path = tools_dict.get("path", "")
        self.description = tools_dict.get("description", "")
    
    def to_dict(self) -> dict:
        return {
            "name": self.name,
            "path": self.path,
            "description": self.description
        }

class AutogenAgent:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE autogen_agents (
        name TEXT PRIMARY KEY, description TEXT, system_message TEXT, 
        code_execution INTEGER, llm_config_name TEXT, tool_names_json TEXT, vector_db_items_json TEXT)
    '''
    @classmethod
    def get_autogen_agent_list_api(cls):
        # MainDBを取得
        main_db = MainDB()
        # AutogenAgentの一覧を取得
        agent_list = main_db.get_autogen_agent_list()
        if not agent_list:
            raise ValueError("agent_list is not set")
        
        result = {}
        result["agent_list"] = [agent.to_dict() for agent in agent_list]
        return result

    @classmethod
    def get_autogen_agent_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからagentを取得
        agent = AutogenAgent.get_autogen_agent_object(request_dict)
        if not agent:
            raise ValueError("agent is not set")

        # MainDBを取得
        main_db = MainDB()
        # AutogenAgentを取得
        agent_result = main_db.get_autogen_agent(agent.name)

        result: dict = {}
        if agent_result:
            result["agent"] = agent_result.to_dict()
        return result

    @classmethod
    def update_autogen_agent_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからagentを取得
        agent = AutogenAgent.get_autogen_agent_object(request_dict)
        if not agent:
            raise ValueError("agent is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # AutogenAgentを更新
        main_db.update_autogen_agent(agent)

        result: dict = {}
        return result

    @classmethod
    def delete_autogen_agent_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからagentを取得
        agent = AutogenAgent.get_autogen_agent_object(request_dict)
        if not agent:
            raise ValueError("agent is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # AutogenAgentを削除
        main_db.delete_autogen_agent(agent)

        result: dict = {}
        return result

    autogen_agent_request_name = "autogen_agent_request"
    @classmethod
    def get_autogen_agent_object(cls, request_dict: dict) -> "AutogenAgent":
        '''
        {"autogen_agent_request": {}}の形式で渡される
        '''
        # autogen_agent_requestを取得
        request:dict = request_dict.get(cls.autogen_agent_request_name, None)
        if not request:
            raise ValueError("request is not set.")
        # autogen_agentを生成
        autogen_agent = AutogenAgent(request)
        return autogen_agent

    def __init__(self, agent_dict: dict):
        self.name = agent_dict.get("name", "")
        self.description = agent_dict.get("description", "")
        self.system_message = agent_dict.get("system_message", "")
        code_execution = agent_dict.get("code_execution", 0)
        # code_executionの値をboolに変換
        if type(code_execution) == int:
            if code_execution == 1:
                self.code_execution = True
            else:
                self.code_execution = False
        elif type(code_execution) == bool:
            self.code_execution = code_execution
        elif type(code_execution) == str and code_execution.upper() == "TRUE":
            self.code_execution = True
        else:
            self.code_execution = False

        self.llm_config_name = agent_dict.get("llm_config_name", "")
        self.tool_names = agent_dict.get("tool_names", "")
        self.vector_db_items = agent_dict.get("vector_db_items", [])

        self.tool_names_json = agent_dict.get("tool_names_json", None)
        if self.tool_names_json:
            # tool_names_jsonをdictに変換
            self.tool_names = json.loads(self.tool_names_json)

        self.vector_db_items_json = agent_dict.get("vector_db_items", None)
        if self.vector_db_items_json:
            # vector_db_items_jsonをdictに変換
            self.vector_db_items = json.loads(self.vector_db_items_json)

    def to_dict(self) -> dict:
        return {
            "name": self.name,
            "description": self.description,
            "system_message": self.system_message,
            "code_execution": self.code_execution,
            "llm_config_name": self.llm_config_name,
            "tool_names": self.tool_names,
            "vector_db_items": self.vector_db_items
        }
    
class AutogenGroupChat:
    '''
    以下のテーブル定義のデータを格納するクラス
    CREATE TABLE autogen_group_chats (name TEXT, description TEXT, llm_config_name TEXT, agent_names_json TEXT)
    '''
    @classmethod
    def get_autogen_group_chat_list_api(cls):
        # MainDBを取得
        main_db = MainDB()
        # AutogenGroupChatの一覧を取得
        group_chat_list = main_db.get_autogen_group_chat_list()

        if not group_chat_list:
            raise ValueError("group_chat_list is not set")
        
        result = {}
        result["group_chat_list"] = [group_chat.to_dict() for group_chat in group_chat_list]
        return result


    autogen_group_chat_request_name = "autogen_group_chat_request"
    @classmethod
    def get_autogen_group_chat_object(cls, request_dict: dict) -> "AutogenGroupChat":
        '''
        {"autogen_group_chat_request": {}}の形式で渡される
        '''
        # autogen_group_chat_requestを取得
        request:dict = request_dict.get(cls.autogen_group_chat_request_name, None)
        if not request:
            raise ValueError("request is not set.")
        # autogen_group_chatを生成
        autogen_group_chat = AutogenGroupChat(request)
        return autogen_group_chat

    @classmethod
    def get_autogen_group_chat_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからgroup_chatを取得
        group_chat = AutogenGroupChat.get_autogen_group_chat_object(request_dict)
        if not group_chat:
            raise ValueError("group_chat is not set")

        # MainDBを取得
        main_db = MainDB()
        # AutogenGroupChatを取得
        group_chat_result = main_db.get_autogen_group_chat(group_chat.name)

        result: dict = {}
        if group_chat_result:
            result["group_chat"] = group_chat_result.to_dict()
        return result

    @classmethod
    def update_autogen_group_chat_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからgroup_chatを取得
        group_chat = AutogenGroupChat.get_autogen_group_chat_object(request_dict)
        if not group_chat:
            raise ValueError("group_chat is not set")

        # MainDBを取得
        main_db = MainDB()
        # AutogenGroupChatを更新
        main_db.update_autogen_group_chat(group_chat)

        result: dict = {}
        return result

    @classmethod
    def delete_autogen_group_chat_api(cls, request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # autogen_propsからgroup_chatを取得
        group_chat = AutogenGroupChat.get_autogen_group_chat_object(request_dict)
        if not group_chat:
            raise ValueError("group_chat is not set")
        
        # MainDBを取得
        main_db = MainDB()
        # AutogenGroupChatを削除
        main_db.delete_autogen_group_chat(group_chat)

        result: dict = {}
        return result


    def __init__(self, group_chat_dict: dict):
        self.name = group_chat_dict.get("name", "")
        self.description = group_chat_dict.get("description", "")
        self.llm_config_name = group_chat_dict.get("llm_config_name", "")
        self.agent_names = group_chat_dict.get("agent_names", [])
        self.agent_names_json = group_chat_dict.get("agent_names_json", None)
        if self.agent_names_json:
            # agent_names_jsonをdictに変換
            self.agent_names = json.loads(self.agent_names_json)

    def to_dict(self) -> dict:
        return {
            "name": self.name,
            "description": self.description,
            "llm_config_name": self.llm_config_name,
            "agent_names": self.agent_names
        }

class MainDB:

    @classmethod
    def init(cls):
        # main_dbへのパスを取得
        app_db_path = MainDB.get_main_db_path()
        cls.__init_database(app_db_path)

    # main_dbへのパスを取得
    @classmethod
    def get_main_db_path(cls) -> str:
        app_data_path = os.getenv("APP_DATA_PATH", None)
        if not app_data_path:
            raise ValueError("APP_DATA_PATH is not set.")
        app_db_path = os.path.join(app_data_path, "server", "main_db", "server_main.db")

        return app_db_path

    @classmethod
    def __init_database(cls, app_db_path: str, upgrade: bool = False):

        # db_pathが存在しない場合は作成する
        if not os.path.exists(app_db_path):
            os.makedirs(os.path.dirname(app_db_path), exist_ok=True)
            conn = sqlite3.connect(app_db_path)
            conn.close()
            # データベースの初期化
            main_db = MainDB(app_db_path)
            # テーブルの初期化
            cls.__init_tables(main_db)

        if upgrade:
            # テーブルの初期化
            main_db = MainDB(app_db_path)
            cls.__init_tables(main_db)

    @classmethod
    def __init_tables(cls, main_db: "MainDB"):
        # DBPropertiesテーブルを初期化
        main_db.init_db_properties_table()
        # ContentFoldersテーブルを初期化
        main_db.__init_content_folder_catalog_table()
        # TagItemsテーブルを初期化
        main_db.__init_tag_item_table()
        # VectorDBItemsテーブルを初期化
        main_db.__init_vector_db_item_table()
        # autogen_llm_configsテーブルを初期化
        main_db.__init_autogen_llm_config_table()
        # autogen_toolsテーブルを初期化
        main_db.__init_autogen_tools_table()
        # autogen_agentsテーブルを初期化
        main_db.__init_autogen_agents_table()
        # autogen_group_chatsテーブルを初期化
        main_db.__init_autogen_group_chats_table()

    @classmethod
    def __update_database(cls, app_db_path: str):
        pass

    def __init__(self, db_path = ""):
        # db_pathが指定されている場合は、指定されたパスを使用する
        if db_path:
            self.db_path = db_path
        else:
            # db_pathが指定されていない場合は、環境変数から取得する
            self.db_path = MainDB.get_main_db_path()

        # データベースの初期化
        MainDB.__init_database(self.db_path)

    def init_db_properties_table(self):
        # DBPropertiesテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS DBProperties (
                id TEXT NOT NULL PRIMARY KEY,
                name TEXT NOT NULL,
                value TEXT NOT NULL
            )
        ''')
        # version = 1を追加
        cur.execute('''
            INSERT OR IGNORE INTO DBProperties (id, name, value) VALUES (?, ?, ?)
        ''', (str(uuid.uuid4()), "version", "1"))

        conn.commit()
        conn.close()

    def __init_content_folder_catalog_table(self):
        # ContentFoldersテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS ContentFoldersCatalog (
                id TEXT NOT NULL PRIMARY KEY,
                folder_type_string TEXT NOT NULL,
                parent_id TEXT NULL,
                folder_name TEXT NOT NULL,
                description TEXT NOT NULL,
                extended_properties_json TEXT NOT NULL
            )
        ''')
        conn.commit()
        conn.close()
    
    def __init_tag_item_table(self):
        # TagItemsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS TagItems (
                id TEXT NOT NULL PRIMARY KEY,
                tag TEXT NOT NULL,
                is_pinned INTEGER NOT NULL
            )
        ''')
        conn.commit()
        conn.close()

    def __init_vector_db_item_table(self):
        # VectorDBItemsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS VectorDBItems (
                id TEXT NOT NULL PRIMARY KEY,
                name TEXT NOT NULL,
                description TEXT NOT NULL,
                vector_db_url TEXT NOT NULL,
                is_use_multi_vector_retriever INTEGER NOT NULL,
                doc_store_url TEXT NOT NULL,
                vector_db_type INTEGER NOT NULL,
                collection_name TEXT NOT NULL,
                chunk_size INTEGER NOT NULL,
                default_search_result_limit INTEGER NOT NULL,
                is_enabled INTEGER NOT NULL,
                is_system INTEGER NOT NULL
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
                "name": "default",
                "description": "Application default vector db",
                "vector_db_url": os.path.join(os.getenv("APP_DATA_PATH", ""), "server", "vector_db", "default_vector_db"),
                "is_use_multi_vector_retriever": True,
                "doc_store_url": f'sqlite:///{os.path.join(os.getenv("APP_DATA_PATH", ""), "server", "vector_db", "default_doc_store.db")}',
                "vector_db_type": 1,
                "collection_name": "ai_app_default_collection",
                "chunk_size": 4096,
                "default_search_result_limit": 10,
                "is_enable": True,
                "is_system": False,
            }
            vector_db_item = VectorDBItem(params)
            # VectorDBItemのプロパティを設定

            # MainDBに追加
            self.update_vector_db_item(vector_db_item)

        else:
            # 存在する場合は初期化処理を行わない
            logger.info("VectorDBItem is already exists.")

    def __init_autogen_llm_config_table(self):
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

    def __init_autogen_tools_table(self):
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

    def __init_autogen_agents_table(self):
        # autogen_agentsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS autogen_agents (
                name TEXT PRIMARY KEY,
                description TEXT,
                system_message TEXT,
                code_execution INTEGER,
                llm_config_name TEXT,
                tool_names_json TEXT,
                vector_db_items_json TEXT
            )
        ''')
        conn.commit()
        conn.close()
        
    def __init_autogen_group_chats_table(self):
        # autogen_group_chatsテーブルが存在しない場合は作成する
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute('''
            CREATE TABLE IF NOT EXISTS autogen_group_chats (
                name TEXT PRIMARY KEY,
                description TEXT,
                llm_config_name TEXT,
                agent_names_json TEXT
            )
        ''')
        conn.commit()
        conn.close()
    
    #########################################
    # DBProperties関連
    #########################################
    def get_db_properties(self) -> dict:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM DBProperties")
        rows = cur.fetchall()
        db_properties = {row["name"]: row["value"] for row in rows}
        conn.close()

        return db_properties
    
    def get_db_property(self, name: str) -> Union[str, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM DBProperties WHERE name=?", (name,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None or len(row) == 0:
            return None

        db_property_dict = dict(row)
        conn.close()

        return db_property_dict["value"]
    
    def update_db_property(self, name: str, value: str):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        if self.get_db_property(name) is None:
            cur.execute("INSERT INTO DBProperties (id, name, value) VALUES (?, ?, ?)", (str(uuid.uuid4()), name, value))
        else:
            cur.execute("UPDATE DBProperties SET value=? WHERE name=?", (value, name))
        conn.commit()
        conn.close()

    def delete_db_property(self, name: str):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("DELETE FROM DBProperties WHERE name=?", (name,))
        conn.commit()
        conn.close()


    #########################################
    # ContentFolder関連
    #########################################
    # idを指定して、idとfolder_nameとparent_idを取得する.再帰的に親フォルダを辿り、folderのパスを生成する
    def get_content_folder_path_by_id(self, folder_id: str) -> str:
        def get_folder_name_recursively(folder_id: str, paths: list[str]) -> list[str]:
            # データベースへ接続
            with sqlite3.connect(self.db_path) as conn:
                conn.row_factory = sqlite3.Row
                cur = conn.cursor()
                cur.execute("SELECT * FROM ContentFoldersCatalog WHERE id=?", (folder_id,))
                row = cur.fetchone()

                # データが存在しない場合はNoneを返す
                if row is None:
                    logger.info(f"Folder with id {folder_id} not found.")
                    return paths
                
                logger.info(f"Folder with id {folder_id} found.")

                folder_dict = dict(row)
                folder_name = folder_dict.get("folder_name", "")
                parent_id = folder_dict.get("parent_id", "")

                logger.info(f"Folder name: {folder_name}, Parent id: {parent_id}")

                # 親フォルダが存在する場合は再帰的に取得する
                if parent_id:
                    paths = get_folder_name_recursively(parent_id, paths)

                paths.append(folder_name)
            return paths

        # フォルダのパスを取得する
        paths = get_folder_name_recursively(folder_id, [])
        # フォルダのパスを生成する
        folder_path = "/".join(reversed(paths))
        logger.info(f"get_content_folder_path_by_id: Folder path: {folder_path}")
        return folder_path

    # pathを指定して、pathにマッチするエントリーを再帰的に辿り、folderを取得する
    def get_content_folder_by_path(self, folder_path: str) -> Union[ContentFoldersCatalog, None]:
        # フォルダのパスを分割する
        folder_names = folder_path.split("/")
        # フォルダのパスを取得する
        folder_id = None
        for folder_name in folder_names:
            # データベースへ接続
            with sqlite3.connect(self.db_path) as conn:
                conn.row_factory = sqlite3.Row
                cur = conn.cursor()
                cur.execute("SELECT * FROM ContentFoldersCatalog WHERE folder_name=? AND parent_id=?", (folder_name, folder_id))
                row = cur.fetchone()

                # データが存在しない場合はNoneを返す
                if row is None:
                    return None

                folder_dict = dict(row)
                folder_id = folder_dict.get("id", "")

        return self.get_content_folder_by_id(folder_id)

    # FolderTypeStringを指定してContentFolderのRootFolderを取得する
    def get_root_content_folders(self) -> Union[ContentFoldersCatalog, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM ContentFoldersCatalog WHERE parent_id IS NULL")
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None or len(row) == 0:
            return None

        folder_dict = dict(row)
        conn.close()

        return ContentFoldersCatalog(folder_dict)
    
    def get_content_folders(self) -> List[ContentFoldersCatalog]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row
        cur = conn.cursor()
        cur.execute("SELECT * FROM ContentFoldersCatalog")
        rows = cur.fetchall()
        folders = [ContentFoldersCatalog(dict(row)) for row in rows]
        conn.close()

        return folders

    def get_content_folder_by_id(self, folder_id: Union[str, None]) -> Union[ContentFoldersCatalog, None]:
        if folder_id is None:
            return None

        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM ContentFoldersCatalog WHERE id=?", (folder_id,))
        row = cur.fetchone()

        # データが存在しない場合はNoneを返す
        if row is None or len(row) == 0:
            return None

        folder_dict = dict(row)
        conn.close()

        return ContentFoldersCatalog(folder_dict)

    def update_content_folder(self, folder: ContentFoldersCatalog):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        if self.get_content_folder_by_id(folder.Id) is None:
            logger.info(f"ContentFolder {folder.FolderName} is not exists. Create new folder.")
            cur.execute("INSERT INTO ContentFoldersCatalog VALUES (?, ?, ?, ?, ?, ?)", (folder.Id, folder.FolderTypeString, folder.ParentId, folder.FolderName, folder.Description, folder.ExtendedPropertiesJson))
        else:
            logger.info(f"ContentFolder {folder.FolderName} is exists. Update folder.")
            cur.execute("UPDATE ContentFoldersCatalog SET folder_type_string=?, parent_id=?, folder_name=?, description=?, extended_properties_json=? WHERE Id=?", (folder.FolderTypeString, folder.ParentId, folder.FolderName, folder.Description, folder.ExtendedPropertiesJson, folder.Id))
        conn.commit()
        conn.close()

    def delete_content_folder(self, folder: ContentFoldersCatalog):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("DELETE FROM ContentFolders WHERE id=?", (folder.Id,))
        conn.commit()
        conn.close()


    ########################################
    # TagItem関連
    ########################################
    def get_tag_item(self, tag_id: str) -> Union[TagItem, None]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM TagItems WHERE id=?", (tag_id,))
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
            cur.execute("UPDATE TagItems SET tag=?, is_pinned=? WHERE id=?", (tag_item.Tag, tag_item.IsPinned, tag_item.Id))
        conn.commit()
        conn.close()

        # 更新したTagItemを返す
        return tag_item
    
    def delete_tag_item(self, tag_item: TagItem):
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("DELETE FROM TagItems WHERE id=?", (tag_item.Id,))
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
        cur.execute("SELECT * FROM VectorDBItems WHERE id=?", (vector_db_item_id,))
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
        cur.execute("SELECT * FROM VectorDBItems WHERE name=?", (vector_db_item_name,))
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
    
    # folder_idを指定してパスを取得する
    def get_vector_db_item_path(self, vector_db_item_id: str) -> str:
        vector_db_item = self.get_vector_db_by_id(vector_db_item_id)
        if vector_db_item is None:
            raise ValueError("VectorDBItem not found")
        return vector_db_item.VectorDBURL
    
    def update_vector_db_item(self, vector_db_item: VectorDBItem) -> VectorDBItem:
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
            cur.execute("UPDATE VectorDBItems SET name=?, description=?, vector_db_url=?, is_use_multi_vector_retriever=?, doc_store_url=?, vector_db_type=?, collection_name=?, chunk_size=?, default_search_result_limit=?, is_enabled=?, is_system=? WHERE id=?",
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
    def get_autogen_llm_config_list(self) -> List[AutogentLLMConfig]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_llm_configs")
        rows = cur.fetchall()
        llm_configs = [AutogentLLMConfig(dict(row)) for row in rows]
        conn.close()

        return llm_configs
    
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
    
    def update_autogen_llm_config(self, llm_config: AutogentLLMConfig):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        if self.get_autogen_llm_config(llm_config.name) is None:
            cur.execute("INSERT INTO autogen_llm_configs VALUES (?, ?, ?, ?, ?, ?)", (llm_config.name, llm_config.api_type, llm_config.api_version, llm_config.model, llm_config.api_key, llm_config.base_url))
        else:
            cur.execute("UPDATE autogen_llm_configs SET api_type=?, api_version=?, model=?, api_key=?, base_url=? WHERE name=?", (llm_config.api_type, llm_config.api_version, llm_config.model, llm_config.api_key, llm_config.base_url, llm_config.name))
        conn.commit()
        conn.close()

    def delete_autogen_llm_config(self, llm_config: AutogentLLMConfig):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("DELETE FROM autogen_llm_configs WHERE name=?", (llm_config.name,))
        conn.commit()
        conn.close()

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
    
    def get_autogen_tool_list(self) -> List[AutogenTools]:
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
        conn.close()

    def delete_autogen_tool(self, tool: AutogenTools):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("DELETE FROM autogen_tools WHERE name=?", (tool.name,))
        conn.commit()
        conn.close()

    #################################################
    # AutogenAgent関連
    #################################################
    def get_autogen_agent_list(self) -> List[AutogenAgent]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_agents")
        rows = cur.fetchall()
        agents = [AutogenAgent(dict(row)) for row in rows]
        conn.close()

        return agents

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
    
    def update_autogen_agent(self, agent: AutogenAgent):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        if self.get_autogen_agent(agent.name) is None:
            cur.execute("INSERT INTO autogen_agents VALUES (?, ?, ?, ?, ?, ?, ?)", 
                        (agent.name, agent.description, agent.system_message, 
                         agent.code_execution, agent.llm_config_name, 
                         json.dumps(agent.tool_names, ensure_ascii=False), 
                         json.dumps(agent.vector_db_items,ensure_ascii=False)))
        else:
            cur.execute("UPDATE autogen_agents SET description=?, system_message=?, code_execution=?, llm_config_name=?, tool_names_json=?, vector_db_items_json=? WHERE name=?", 
                        (agent.description, agent.system_message, 
                         agent.code_execution, agent.llm_config_name, 
                         json.dumps(agent.tool_names, ensure_ascii=False), 
                         json.dumps(agent.vector_db_items,ensure_ascii=False), 
                         agent.name))
        conn.commit()
        conn.close()
    
    def delete_autogen_agent(self, agent: AutogenAgent):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("DELETE FROM autogen_agents WHERE name=?", (agent.name,))
        conn.commit()
        conn.close()
    #################################################
    # AutogenGroupChat関連
    #################################################
    def get_autogen_group_chat_list(self) -> List[AutogenGroupChat]:
        conn = sqlite3.connect(self.db_path)
        conn.row_factory = sqlite3.Row 
        cur = conn.cursor()
        cur.execute("SELECT * FROM autogen_group_chats")
        rows = cur.fetchall()
        group_chats = [AutogenGroupChat(dict(row)) for row in rows]
        conn.close()

        return group_chats  

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
    
    def update_autogen_group_chat(self, group_chat: AutogenGroupChat):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        if self.get_autogen_group_chat(group_chat.name) is None:
            cur.execute("INSERT INTO autogen_group_chats VALUES (?, ?, ?, ?)", 
                        (group_chat.name, group_chat.description, group_chat.llm_config_name, 
                         json.dumps(group_chat.agent_names, ensure_ascii=False)))
        else:
            cur.execute("UPDATE autogen_group_chats SET description=?, llm_config_name=?, agent_names_json=? WHERE name=?", 
                        (group_chat.description, group_chat.llm_config_name, 
                         json.dumps(group_chat.agent_names, ensure_ascii=False), group_chat.name))
        conn.commit()
        conn.close()

    def delete_autogen_group_chat(self, group_chat: AutogenGroupChat):
        conn = sqlite3.connect(self.db_path)
        cur = conn.cursor()
        cur.execute("DELETE FROM autogen_group_chats WHERE name=?", (group_chat.name,))
        conn.commit()
        conn.close()