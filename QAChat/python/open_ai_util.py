import importlib, json

class OpenAIUtil:
    def __init__(self, azure_openai, openai_api_key, completion_base_url=None, embedding_base_url=None):
        self.azure_openai = azure_openai
        self.openai_api_key = openai_api_key
        self.completion_base_url = completion_base_url
        self.embedding_base_url = embedding_base_url
        self.client = self._create_openai_object()

    def _create_openai_object(self):
        # OpenAIオブジェクトを作成
        openai = importlib.import_module("openai")
        if self.azure_openai:
            client = openai.AzureOpenAI(
                api_key=self.openai_api_key,
                api_version="2023-12-01-preview",
                base_url = self.completion_base_url
                
            )
        else:
            client = openai.OpenAI(
                api_key=self.openai_api_key,
                base_url = self.embedding_base_url
            )
        return client

    

def openai_json_chat(input_json, props={}):
    # propsからopenaiの設定を取得する
    openai_api_key = props.get("OpenAIKey", None)
    chat_model_name = props.get("OpenAICompletionModel", None)
    azure_openai_string = props.get("AzureOpenAI", False)
    completion_base_url = props.get("OpenAICompletionBaseURL", None)
    embedding_base_url = props.get("OpenAIEmbeddingBaseURL", None)
    
    # azure_openaiがTrueの場合、AzureOpenAIを使用する.azure_openai_stringを大文字にしてTRUEの場合はTrueに変換する
    
    azure_openai = azure_openai_string.upper() == "TRUE"
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil(azure_openai, openai_api_key, completion_base_url, embedding_base_url)
    json_obj = json.loads(input_json)
    client = openai_util.client
    response = client.chat.completions.create(
        model=chat_model_name,
        messages=json_obj,
        response_format={"type": "json_object"}    
        )
    return response.choices[0].message.content
    

def openai_chat(input_json, props ={}):
    # propsからopenaiの設定を取得する
    openai_api_key = props.get("OpenAIKey", None)
    chat_model_name = props.get("OpenAICompletionModel", None)
    azure_openai_string = props.get("AzureOpenAI", False)
    # azure_openaiがTrueの場合、AzureOpenAIを使用する.azure_openai_stringを大文字にしてTRUEの場合はTrueに変換する
    azure_openai = azure_openai_string.upper() == "TRUE"
    # base_urlを取得する
    completion_base_url = props.get("OpenAICompletionBaseURL", None)
    embedding_base_url = props.get("OpenAIEmbeddingBaseURL", None)
    
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil(azure_openai, openai_api_key, completion_base_url, embedding_base_url)
    json_obj = json.loads(input_json)
    client = openai_util.client
    response = client.chat.completions.create(
        model=chat_model_name,
        messages=json_obj
    )
    return response.choices[0].message.content

def openai_embedding(input_text, props={}):
    # propsからopenaiの設定を取得する
    openai_api_key = props.get("OpenAIKey", None)
    chat_model_name = props.get("OpenAIEmbeddingModel", None)
    azure_openai_string = props.get("AzureOpenAI", False)
    # azure_openaiがTrueの場合、AzureOpenAIを使用する.azure_openai_stringを大文字にしてTRUEの場合はTrueに変換する
    azure_openai = azure_openai_string.upper() == "TRUE"
    # base_urlを取得する
    completion_base_url = props.get("OpenAICompletionBaseURL", None)
    embedding_base_url = props.get("OpenAIEmbeddingBaseURL", None)
    
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil(azure_openai, openai_api_key, completion_base_url, embedding_base_url)
    client = openai_util.client
    
    response = client.embeddings.create(
        model=chat_model_name,
        input=input_text
    )
    return response.data[0].embedding

