import importlib, json

class OpenAIUtil:
    def __init__(self):
        pass
    
    def init(self, openai_api_key, azure_openai, azure_openai_endpoint, completion_base_url=None, embedding_base_url=None):
        self.azure_openai = azure_openai
        self.openai_api_key = openai_api_key
        self.azure_openai_endpoint = azure_openai_endpoint
        self.completion_base_url = completion_base_url
        self.embedding_base_url = embedding_base_url
        self.completion, self.embedding = self.__create_openai_object()

    def init(self, props:dict):
        # propsからopenaiの設定を取得する
        self.openai_api_key = props.get("OpenAIKey", None)
        azure_openai_string = props.get("AzureOpenAI", "")
        # azure_openaiがTrueの場合、AzureOpenAIを使用する.azure_openai_stringを大文字にしてTRUEの場合はTrueに変換する
        self.azure_openai = azure_openai_string.upper() == "TRUE"
    
        self.azure_openai_endpoint = props.get("AzureOpenAIEndpoint", None)
        # azure_openai_endpointがNoneまたは空文字の場合は以下のBaseURLを使用する
        if not self.azure_openai_endpoint:
        
            # base_urlを取得する
            self.completion_base_url = props.get("OpenAICompletionBaseURL", None)
            self.embedding_base_url = props.get("OpenAIEmbeddingBaseURL", None)
        
        self.completion, self.embedding = self.__create_openai_object()

    def __create_openai_object(self):
        # OpenAIオブジェクトを作成
        if self.azure_openai:
            return self.__create_azure_openai_object()
        else:
            return self.__create_openai_object()
        
    def __create_azure_openai_object(self):
        openai = importlib.import_module("openai")

        params = {}
        params["api_key"] = self.openai_api_key
        params["api_key"] = self
        params["api_version"] = "2023-12-01-preview"
        if self.completion_base_url:
            params["base_url"] = self.completion_base_url
        else:
            params["azure_endpoint"] = self.azure_openai_endpoint

        completion = openai.AzureOpenAI(
            **params
        )
        if self.completion_base_url:
            params["base_url"] = self.embedding_base_url
        else:
            params["azure_endpoint"] = self.azure_openai_endpoint
            
        embedding = openai.AzureOpenAI(
            **params
        )
        return completion, embedding

    def __create_openai_object(self):
        openai = importlib.import_module("openai")
        completion = openai.OpenAI(
            api_key=self.openai_api_key,
            base_url = self.completion_base_url
        )
        embedding = openai.OpenAI(
            api_key=self.openai_api_key,
            base_url = self.embedding_base_url
        )
        return completion, embedding
    

def list_openai_models(props={}):
    
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil()
    openai_util.init(props)
    client = openai_util.completion

    response = client.models.list()

    # モデルのリストを取得する
    model_id_list = [ model.id for model in response.data]
    return model_id_list

def openai_json_chat(input_json, props={}):
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil()
    openai_util.init(props)
    client = openai_util.completion
    
    # chat_model_nameを取得する
    chat_model_name = props.get("OpenAICompletionModel", None)

    json_obj = json.loads(input_json)

    response = client.chat.completions.create(
        model=chat_model_name,
        messages=json_obj,
        response_format={"type": "json_object"}    
        )
    return response.choices[0].message.content
    

def openai_chat(input_json, props ={}):
    
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil()
    openai_util.init(props)
    client = openai_util.completion
    
    # chat_model_nameを取得する
    chat_model_name = props.get("OpenAICompletionModel", None)

    json_obj = json.loads(input_json)

    response = client.chat.completions.create(
        model=chat_model_name,
        messages=json_obj
    )
    return response.choices[0].message.content

def openai_embedding(input_text, props={}):
    
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil()
    openai_util.init(props)
    client = openai_util.embedding
    
    # embedding_model_nameを取得する
    embedding_model_name = props.get("OpenAIEmbeddingModel", None)
    
    response = client.embeddings.create(
        model=embedding_model_name,
        input=[input_text]
    )
    return response.data[0].embedding

if __name__ == "__main__":
    
    # テスト用のjson
    input_json_001 = """
    [
        {
            "role": "system",
            "content": "You are a helpful assistant."
        },
        {
            "role": "user",
            "content": "What is your name?"
        }
    ]
    """
    input_json_002 = """
    [
        {
            "role": "system",
            "content": "You are a helpful assistant."
        },
        {
            "role": "user",
            "content": "Please output json format. What is your name?"
        }
    ]
    """
        
    import env_to_props
    # envファイルからpropsを取得する
    props = env_to_props.get_props()

    # chatを実行
    result = openai_chat(input_json_001, props)
    print(result)
    # embeddingを実行
    result = openai_embedding("Hello, world", props)
    print(result)
    # chatを実行
    result = openai_json_chat(input_json_002, props)
    print(result)
    # list_openai_modelsを実行
    result = list_openai_models()
    print(result)
    