from dotenv import load_dotenv
import os, json
import base64
from mimetypes import guess_type

class OpenAIProps:
    def __init__(self, props_dict: dict):
        self.OpenAIKey:str = props_dict.get("OpenAIKey")
        self.OpenAICompletionModel:str = props_dict.get("OpenAICompletionModel")
        self.OpenAIEmbeddingModel:str = props_dict.get("OpenAIEmbeddingModel")
        self.OpenAIWhisperModel:str = props_dict.get("OpenAIWhisperModel")
        
        azure_openai_string = props_dict.get("AzureOpenAI", None)
        self.AzureOpenAI = azure_openai_string.upper() == "TRUE"

        self.AzureOpenAIEmbeddingVersion = props_dict.get("AzureOpenAIEmbeddingVersion", None)
        self.AzureOpenAICompletionVersion = props_dict.get("AzureOpenAICompletionVersion", None)
        self.AzureOpenAIWhisperVersion = props_dict.get("AzureOpenAIWhisperVersion", None)

        self.AzureOpenAIEndpoint = props_dict.get("AzureOpenAIEndpoint", None)
        self.OpenAICompletionBaseURL = props_dict.get("OpenAICompletionBaseURL", None)
        self.OpenAIEmbeddingBaseURL = props_dict.get("OpenAIEmbeddingBaseURL", None)
        self.OpenAIWhisperBaseURL = props_dict.get("OpenAIWhisperBaseURL", None)
        
        # AzureOpenAIEmbeddingVersionがNoneの場合は2024-02-01を設定する
        if self.AzureOpenAIEmbeddingVersion == None:
            self.AzureOpenAIEmbeddingVersion = "2024-02-01"
        # AzureOpenAICompletionVersionがNoneの場合は2024-02-01を設定する
        if self.AzureOpenAICompletionVersion == None:
            self.AzureOpenAICompletionVersion = "2024-02-01"
        # AzureOpenAIWhisperVersionがNoneの場合は2024-02-01を設定する
        if self.AzureOpenAIWhisperVersion == None:
            self.AzureOpenAIWhisperVersion = "2024-02-01"

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
    
    # OpenAIのWhisper用のパラメーター用のdictを作成する
    def create_openai_whisper_dict(self) -> dict:
        whisper_dict = {}
        whisper_dict["api_key"] = self.OpenAIKey
        if self.OpenAIWhisperBaseURL:
            whisper_dict["base_url"] = self.OpenAIWhisperBaseURL
        return whisper_dict

    # AzureOpenAIのWhisper用のパラメーター用のdictを作成する
    def create_azure_openai_whisper_dict(self) -> dict:
        whisper_dict = {}
        whisper_dict["api_key"] = self
        whisper_dict["api_version"] = self.AzureOpenAIWhisperVersion
        if self.OpenAIWhisperBaseURL:
            whisper_dict["base_url"] = self.OpenAIWhisperBaseURL
        else:
            whisper_dict["azure_endpoint"] = self.AzureOpenAIEndpoint
        
        return whisper_dict

# VectorDBのパラメーターを管理するクラス
class VectorDBProps:
    def __init__(self, props_dict: dict):
        # VectorStoreの設定
        self.VectorDBURL = props_dict.get("VectorDBURL")
        self.VectorDBTypeString = props_dict.get("VectorDBTypeString")
        self.Name = props_dict.get("VectorDBName", None)
        self.VectorDBDescription = props_dict.get("VectorDBDescription", None)
        # VectorDBDescriptionがNoneの場合は以下のデフォルト値を設定する
        if self.VectorDBDescription == None:
            self.VectorDBDescription = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。"
        
        # IsUseMultiVectorRetrieverがTrueの場合はMultiVectorRetrieverを使用する
        if props_dict.get("IsUseMultiVectorRetriever", "").upper() == "TRUE":
            self.IsUseMultiVectorRetriever = True
            # DocStoreの設定
            self.DocStoreURL = props_dict.get("DocStoreURL", None)
        else:
            self.IsUseMultiVectorRetriever = False
            self.DocStoreURL = None    
            

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

def env_to_props() -> OpenAIProps:
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
        "VectorDBName": os.getenv("VECTOR_DB_NAME"),
        "VectorDBURL": os.getenv("VECTOR_DB_URL"),
        "VectorDBTypeString": os.getenv("VECTOR_DB_TYPE_STRING"),
        "VectorDBDescription": os.getenv("VECTOR_DB_DESCRIPTION"),
        "IsUseMultiVectorRetriever": os.getenv("IS_USE_MULTI_VECTOR_RETRIEVER").upper() == "TRUE",
        "DocStoreURL": os.getenv("DOC_STORE_URL"),
    }
    vectorDBProps = VectorDBProps(props)
    return vectorDBProps

# Function to encode a local image into data URL 
def local_image_to_data_url(image_path) -> str:
    # Guess the MIME type of the image based on the file extension
    mime_type, _ = guess_type(image_path)
    if mime_type is None:
        mime_type = 'application/octet-stream'  # Default MIME type if none is found

    # Read and encode the image file
    with open(image_path, "rb") as image_file:
        base64_encoded_data = base64.b64encode(image_file.read()).decode('utf-8')

    # Construct the data URL
    return f"data:{mime_type};base64,{base64_encoded_data}"


    
# openai_chat用のパラメーターを作成する
def create_openai_chat_parameter_dict(model: str, messages_json: str, templature : float = None, json_mode : bool = False) -> dict:
    params = {}
    params["model"] = model
    params["messages"] = json.loads(messages_json)
    if templature:
        params["temperature"] = templature
    if json_mode:
        params["response_format"] = {"type": "json_object"}
    return params

# openai_chat_with_vision用のパラメーターを作成する
def create_openai_chat_with_vision_parameter_dict(model: str, prompt: str, image_file_name_list: list, templature : float = None, json_mode : bool = False, max_tokens = None) -> dict:
    # messagesの作成
    messages = []
    content = [{"type": "text", "text": prompt}]

    for image_file_name in image_file_name_list:
        image_data_url = local_image_to_data_url(image_file_name)
        content.append({"type": "image_url", "image_url": {"url": image_data_url}})

    role_user_dict = {"role": "user", "content": content}
    messages.append(role_user_dict)

    # 入力パラメーターの設定
    params = {}
    params["messages"] = messages
    params["model"] = model
    if templature:
        params["temperature"] = templature
    if json_mode:
        params["response_format"] = {"type": "json_object"}
    if max_tokens:
        params["max_tokens"] = max_tokens
    
    return params



