from dotenv import load_dotenv
import os, json
from ai_app_openai.ai_app_openai_util import OpenAIProps

# VectorDBのパラメーターを管理するクラス
class VectorDBProps:
    def __init__(self, props_dict: dict):
        # VectorStoreの設定
        self.VectorDBURL: str = props_dict.get("VectorDBURL", "")
        self.VectorDBTypeString :str = props_dict.get("VectorDBTypeString", "")
        self.Name:str = props_dict.get("VectorDBName", "")
        self.VectorDBDescription:str = props_dict.get("VectorDBDescription", "")
        # VectorDBDescriptionがNoneの場合は以下のデフォルト値を設定する
        if not self.VectorDBDescription:
            self.VectorDBDescription = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"
        
        # チャンクサイズ
        self.ChunkSize = props_dict.get("ChunkSize", 500)
        # ベクトル検索時の検索結果上限数
        self.MaxSearchResults = props_dict.get("MaxSearchResults", 10)

        # IsUseMultiVectorRetrieverがTrueの場合はMultiVectorRetrieverを使用する
        if props_dict.get("IsUseMultiVectorRetriever", False) == True:
            self.IsUseMultiVectorRetriever = True
            # DocStoreの設定
            self.DocStoreURL = props_dict.get("DocStoreURL", None)
            # MultiVectorDocChunkSize
            self.MultiVectorDocChunkSize = props_dict.get("MultiVectorDocChunkSize", 10000)

        else:
            self.IsUseMultiVectorRetriever = False
            self.DocStoreURL = None    
            self.MultiVectorDocChunkSize = -1
            

        # Collectionの設定
        self.CollectionName = props_dict.get("CollectionName", None)

    def get_vector_db_dict(self) -> dict:
        vector_db_dict = {}
        vector_db_dict["name"] = self.Name
        vector_db_dict["vector_db_url"] = self.VectorDBURL
        vector_db_dict["description"] = self.VectorDBDescription
        vector_db_dict["vector_db_type_string"] = self.VectorDBTypeString
        vector_db_dict["collection_name"] = self.CollectionName
        vector_db_dict["doc_store_url"] = self.DocStoreURL
        return vector_db_dict

    @staticmethod
    def get_vector_db_settings() -> 'VectorDBProps':
        load_dotenv()
        props: dict = {
            "VectorDBName": os.getenv("VECTOR_DB_NAME"),
            "VectorDBURL": os.getenv("VECTOR_DB_URL"),
            "VectorDBTypeString": os.getenv("VECTOR_DB_TYPE_STRING"),
            "VectorDBDescription": os.getenv("VECTOR_DB_DESCRIPTION"),
            "IsUseMultiVectorRetriever": os.getenv("IS_USE_MULTI_VECTOR_RETRIEVER","false").upper() == "TRUE",
            "DocStoreURL": os.getenv("DOC_STORE_URL"),
            "CollectionName": os.getenv("VECTOR_DB_COLLECTION_NAME"),
            # チャンクサイズ
            "ChunkSize": int(os.getenv("ChunkSize", 500)),
            # マルチベクトルリトリーバーの場合のドキュメントチャンクサイズ
            "MultiVectorDocChunkSize": int(os.getenv("MultiVectorDocChunkSize", 500)),
            # ベクトル検索時の検索結果上限数
            "MaxSearchResults": int(os.getenv("MaxSearchResults", 10))
            
        }
        vectorDBProps = VectorDBProps(props)
        return vectorDBProps


class VectorSearchParameter:
    def __init__(self, openai_props: OpenAIProps = None, vector_db_props: list[VectorDBProps] = None, query: str = "", search_kwarg: dict = {}):

        # OpenAIPorpsを生成
        self.openai_props = openai_props

        # VectorDBPropsのリストを取得
        self.vector_db_props = vector_db_props

        #  openai_props, vector_db_items, query, search_kwargを設定する
        self.query = query
        self.search_kwarg = search_kwarg

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
        params.search_kwarg = request.get("search_kwarg", {})

        return params

class ContentUpdateOrDeleteRequestParams:
    def __init__(self, openai_props: OpenAIProps, vector_db_props_list: list[VectorDBProps], request_json: str):

        self.openai_props = openai_props
        self.vector_db_props_list = vector_db_props_list

        # request_jsonをdictに変換
        request: dict = json.loads(request_json)
        self.text = request["content"]
        self.source = request["id"]
        self.source_url = ""
        self.description = request.get("description", "")
        self.reliability = request.get("reliability", 0)
        self.mode = request.get("mode", "")
        self.image_url = request.get("image_url", "")

    @classmethod
    def from_content_or_image_json(cls, props_json: str = "{}", vector_db_items_json: str = "{}", request_json: str = "{}"):
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)

        vector_db_items = json.loads(vector_db_items_json)

        vector_db_props_list = []
        for vector_db_item in vector_db_items:
            vector_db_props_list.append(VectorDBProps(vector_db_item))
        
        params:ContentUpdateOrDeleteRequestParams = ContentUpdateOrDeleteRequestParams(openai_props, vector_db_props_list, request_json)
        return params

class FileUpdateOrDeleteRequestParams:
    def __init__(self, openai_props: OpenAIProps, vector_db_props_list: list[VectorDBProps], request_json: str):
        
        self.openai_props = openai_props

        self.vector_db_props_list = vector_db_props_list
        
        # request_jsonをdictに変換
        # request_jsonをdictに変換
        request: dict = json.loads(request_json)
        self.document_root = request["WorkDirectory"]
        self.relative_path = request["RelativePath"]
        self.source_url = request["RepositoryURL"]
        self.description = request.get("description", "")
        self.reliability = request.get("reliability", 0)
        self.mode = request.get("mode", "")
    
    @classmethod
    def from_json(cls, props_json: str = "{}", vector_db_items_json: str = "{}", request_json: str = "{}"):
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)

        vector_db_items = json.loads(vector_db_items_json)
        vector_db_props_list = []
        for vector_db_item in vector_db_items:
            vector_db_props_list.append(VectorDBProps(vector_db_item))
        
        params: FileUpdateOrDeleteRequestParams = FileUpdateOrDeleteRequestParams(openai_props, vector_db_props_list, request_json)
        return params

