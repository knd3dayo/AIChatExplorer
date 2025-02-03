from dotenv import load_dotenv
import os, json
from ai_app_openai.ai_app_openai_util import OpenAIProps
import uuid
# VectorDBのパラメーターを管理するクラス
class VectorDBProps:

    # コレクションの指定がない場合はデフォルトのコレクション名を使用
    DEFAULT_COLLECTION_NAME = "ai_app_default_collection"

    source_id_name = "source_id"
    source_path_name = "source_path"
    git_repository_url_name = "git_repository_url"
    git_relative_path_name = "git_relative_path"
    image_url_name = "image_url"
    description_name = "description"
    system_message_name = "system_message"
    content_name = "content"
    folder_id_name = "folder_id"

    vector_db_url_name = "vector_db_url"
    vector_db_type_string_name = "vector_db_type_string"
    vector_db_name_name = "vector_db_name"
    vector_db_description_name = "vector_db_description"
    catalog_db_url_name = "catalog_db_url"
    chunk_size_name = "chunk_size"
    is_use_multi_vector_retriever_name = "is_use_multi_vector_retriever"
    doc_store_url_name = "doc_store_url"
    collection_name = "collection_name"
    search_kwargs_name = "search_kwargs"

    score_name = "score"

    def __init__(self, props_dict: dict):
        # AutoGenで使用するID
        self.id = str(uuid.uuid4())

        # VectorStoreの設定
        self.VectorDBURL: str = props_dict.get(VectorDBProps.vector_db_url_name, "")
        self.VectorDBTypeString :str = props_dict.get(VectorDBProps.vector_db_type_string_name, "")
        self.Name:str = props_dict.get(VectorDBProps.vector_db_name_name, "")
        self.VectorDBDescription:str = props_dict.get(VectorDBProps.vector_db_description_name, "")
        # VectorDBDescriptionがNoneの場合は以下のデフォルト値を設定する
        if not self.VectorDBDescription:
            self.VectorDBDescription = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"

        # DBのカタログ情報を保持するDBのURL
        self.CatalogDBURL = props_dict.get(VectorDBProps.catalog_db_url_name, "")
        if not self.CatalogDBURL:
            raise ValueError("catalog_db_url is not set.")
        
        # チャンクサイズ
        self.ChunkSize = props_dict.get(VectorDBProps.chunk_size_name, 500)

        # search_kwarg
        self.SearchKwarg = props_dict.get(VectorDBProps.search_kwargs_name, {})

        # description
        self.Description = props_dict.get(VectorDBProps.description_name, "")
        
        # system_message
        self.SystemMessage = props_dict.get(VectorDBProps.system_message_name, self.Description)

        # IsUseMultiVectorRetrieverがTrueの場合はMultiVectorRetrieverを使用する
        if props_dict.get(VectorDBProps.is_use_multi_vector_retriever_name, False) == True:
            self.IsUseMultiVectorRetriever = True
            # DocStoreの設定
            self.DocStoreURL = props_dict.get(VectorDBProps.doc_store_url_name, None)

        else:
            self.IsUseMultiVectorRetriever = False
            self.DocStoreURL = None    

        # Collectionの設定
        self.CollectionName = props_dict.get(VectorDBProps.collection_name, "")

        # FolderのID
        self.FolderID = props_dict.get(VectorDBProps.folder_id_name, "")

    def get_vector_db_dict(self) -> dict:
        vector_db_dict = {}
        vector_db_dict["name"] = self.Name
        vector_db_dict["vector_db_url"] = self.VectorDBURL
        vector_db_dict["description"] = self.VectorDBDescription
        vector_db_dict["system_message"] = self.SystemMessage
        vector_db_dict["vector_db_type_string"] = self.VectorDBTypeString
        vector_db_dict["collection_name"] = self.CollectionName
        vector_db_dict["doc_store_url"] = self.DocStoreURL
        return vector_db_dict

    @staticmethod
    def get_vector_db_settings() -> 'VectorDBProps':
        load_dotenv()
        props: dict = {
            "vector_db_name": os.getenv("VECTOR_DB_NAME"),
            "vector_db_url": os.getenv("VECTOR_DB_URL"),
            "vector_db_type_string": os.getenv("VECTOR_DB_TYPE_STRING"),
            "vector_db_description": os.getenv("VECTOR_DB_DESCRIPTION"),
            "vector_db_system_message": os.getenv("VECTOR_DB_SYSTEM_MESSAGE"),
            "is_use_multi_vector_retriever": os.getenv("IS_USE_MULTI_VECTOR_RETRIEVER","false").upper() == "TRUE",
            "doc_store_url": os.getenv("DOC_STORE_URL"),
            "collection_name": os.getenv("VECTOR_DB_COLLECTION_NAME"),
            # チャンクサイズ
            "chunk_size": int(os.getenv("ChunkSize", 1024)),
            
        }
        vectorDBProps = VectorDBProps(props)
        return vectorDBProps


class VectorSearchParameter:
    def __init__(self, openai_props: OpenAIProps = None, vector_db_props: list[VectorDBProps] = None, query: str = ""):

        # OpenAIPorpsを生成
        self.openai_props = openai_props

        # VectorDBPropsのリストを取得
        self.vector_db_props = vector_db_props

        #  openai_props, vector_db_items, query, search_kwargを設定する
        self.query = query

    @classmethod
    def from_json(cls, openai_props_json: str = "{}", vector_db_items_json: str = "{}", request_json: str = "{}"):
        params:VectorSearchParameter = VectorSearchParameter()
        # OpenAIPorpsを生成
        props = json.loads(openai_props_json)
        params.openai_props = OpenAIProps(props)

        # VectorDBPropsのリストを取得
        vector_db_items = json.loads(vector_db_items_json)
        params.vector_db_props = [VectorDBProps(item) for item in vector_db_items]

        #  openai_props, vector_db_items, query, search_kwargを設定する
        request: dict = json.loads(request_json)
        params.query = request.get("query", "")

        return params

class ContentUpdateOrDeleteRequestParams:
    def __init__(self, openai_props: OpenAIProps, vector_db_props_list: list[VectorDBProps], request_json: str):

        self.openai_props = openai_props
        self.vector_db_props_list = vector_db_props_list

        # request_jsonをdictに変換
        request: dict = json.loads(request_json)
        self.id = request[VectorDBProps.source_id_name]
        self.description = request.get(VectorDBProps.description_name, "")
        self.text = request[VectorDBProps.content_name]
        self.source_path = request.get(VectorDBProps.source_path_name, "")
        self.git_repository_url = request.get(VectorDBProps.git_repository_url_name, "")
        self.git_relative_path = request.get(VectorDBProps.git_relative_path_name, "")
        self.image_url = request.get(VectorDBProps.image_url_name, "")

    @classmethod
    def from_json(cls, props_json: str = "{}", vector_db_items_json: str = "{}", request_json: str = "{}"):
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)

        vector_db_items = json.loads(vector_db_items_json)

        vector_db_props_list = []
        for vector_db_item in vector_db_items:
            vector_db_props_list.append(VectorDBProps(vector_db_item))
        
        params:ContentUpdateOrDeleteRequestParams = ContentUpdateOrDeleteRequestParams(openai_props, vector_db_props_list, request_json)
        return params