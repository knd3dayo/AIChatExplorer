from dotenv import load_dotenv
import os

class OpenAIProps:
    def __init__(self, props_dict: dict):
        self.OpenAIKey:str = props_dict.get("OpenAIKey")
        self.OpenAICompletionModel:str = props_dict.get("OpenAICompletionModel")
        self.OpenAIEmbeddingModel:str = props_dict.get("OpenAIEmbeddingModel")
        
        azure_openai_string = props_dict.get("AzureOpenAI", None)
        self.AzureOpenAI = azure_openai_string.upper() == "TRUE"

        self.AzureOpenAIEmbeddingVersion = props_dict.get("AzureOpenAIEmbeddingVersion", None)
        self.AzureOpenAICompletionVersion = props_dict.get("AzureOpenAICompletionVersion", None)
        self.AzureOpenAIEndpoint = props_dict.get("AzureOpenAIEndpoint", None)
        self.OpenAICompletionBaseURL = props_dict.get("OpenAICompletionBaseURL", None)
        self.OpenAIEmbeddingBaseURL = props_dict.get("OpenAIEmbeddingBaseURL", None)
        # AzureOpenAIEmbeddingVersionがNoneの場合は2024-02-01を設定する
        if self.AzureOpenAIEmbeddingVersion == None:
            self.AzureOpenAIEmbeddingVersion = "2024-02-01"
        # AzureOpenAICompletionVersionがNoneの場合は2024-02-01を設定する
        if self.AzureOpenAICompletionVersion == None:
            self.AzureOpenAICompletionVersion = "2024-02-01"
        # AzureOpenAIEndpointがNoneの場合、OpenAICompletionBaseURLまたはOpenAIEmbeddingBaseURLがNoneの場合はValueErrorを発生させる
        if self.AzureOpenAI:
            if self.AzureOpenAIEndpoint == None:
                if self.OpenAICompletionBaseURL == None and self.OpenAIEmbeddingBaseURL == None:
                    raise ValueError("AzureOpenAIEndpoint is None")

    # OpenAIのCompletion用のパラメーター用のdictを作成する
    def create_openai_completion_dict(self) -> dict:
        completion_dict = {}
        completion_dict["api_key"] = self.OpenAIKey
        if self.OpenAICompletionBaseURL:
            completion_dict["base_url"] = self.OpenAICompletionBaseURL
        return completion_dict
        
    # AzureOpenAIのCompletion用のパラメーター用のdictを作成する
    def create_azure_openai_completion_dict(self) -> dict:
        completion_dict = {}
        completion_dict["api_key"] = self.OpenAIKey
        completion_dict["api_version"] = self.AzureOpenAICompletionVersion
        if self.OpenAICompletionBaseURL:
            completion_dict["base_url"] = self.OpenAICompletionBaseURL
        else:
            completion_dict["azure_endpoint"] = self.AzureOpenAIEndpoint
        return completion_dict
        
    # OpenAIのEmbedding用のパラメーター用のdictを作成する
    def create_openai_embedding_dict(self) -> dict:
        embedding_dict = {}
        embedding_dict["api_key"] = self.OpenAIKey
        if self.OpenAIEmbeddingBaseURL:
            embedding_dict["base_url"] = self.OpenAIEmbeddingBaseURL
        return embedding_dict
        
    # AzureOpenAIのEmbedding用のパラメーター用のdictを作成する
    def create_azure_openai_embedding_dict(self) -> dict:
        embedding_dict = {}
        embedding_dict["api_key"] = self.OpenAIKey
        embedding_dict["api_version"] = self.AzureOpenAIEmbeddingVersion
        if self.OpenAIEmbeddingBaseURL:
            embedding_dict["base_url"] = self.OpenAIEmbeddingBaseURL
        else:
            embedding_dict["azure_endpoint"] = self.AzureOpenAIEndpoint
        return embedding_dict


# VectorDBのパラメーターを管理するクラス
class VectorDBProps:
    def __init__(self, props_dict: dict):
        self.VectorDBURL = props_dict.get("VectorDBURL")
        self.VectorDBTypeString = props_dict.get("VectorDBTypeString")

        self.Name = props_dict.get("Name", None)
        self.CollectionName = props_dict.get("CollectionName", None)
        self.VectorDBDescription = props_dict.get("VectorDBDescription", None)
        # VectorDBDescriptionがNoneの場合は以下のデフォルト値を設定する
        if self.VectorDBDescription == None:
            self.VectorDBDescription = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"
    
    def get_vector_db_dict(self) -> dict:
        vector_db_dict = {}
        vector_db_dict["name"] = self.Name
        vector_db_dict["vector_db_url"] = self.VectorDBURL
        vector_db_dict["description"] = self.VectorDBDescription
        vector_db_dict["vector_db_type_string"] = self.VectorDBTypeString
        return vector_db_dict

def get_props() -> OpenAIProps:
    load_dotenv()
    props: dict = {
        "OpenAIKey": os.getenv("OPENAI_API_KEY"),
        "OpenAICompletionModel": os.getenv("OPENAI_COMPLETION_MODEL"),
        "OpenAIEmbeddingModel": os.getenv("OPENAI_EMBEDDING_MODEL"),
        "AzureOpenAI": os.getenv("AZURE_OPENAI"),
        "OpenAICompletionBaseURL": os.getenv("OPENAI_COMPLETION_BASE_URL"),
        "OpenAIEmbeddingBaseURL": os.getenv("OPENAI_EMBEDDING_BASE_URL"),
    }
    openAIProps = OpenAIProps(props)
    return openAIProps


def get_vector_db_settings() -> VectorDBProps:
    load_dotenv()
    props: dict = {
        "VectorDBURL": os.getenv("VECTOR_DB_URL"),
        "VectorDBTypeString": os.getenv("VECTOR_DB_TYPE_STRING"),
        "VectorDBDescription": os.getenv("VECTOR_DB_DESCRIPTION"),
    }
    vectorDBProps = VectorDBProps(props)
    return vectorDBProps
    
