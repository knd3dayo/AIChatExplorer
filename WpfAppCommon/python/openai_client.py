import json
import base64
from mimetypes import guess_type
from openai import OpenAI, AzureOpenAI
from openai_props import OpenAIProps

class OpenAIClient:
    def __init__(self, props: OpenAIProps):
        
        self.props = props

    def get_completion_client(self):
        
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_completion_dict()
            result = AzureOpenAI(
                **params
            )

        else:
            params =self.props.create_openai_completion_dict()
            result = OpenAI(
                **params
            )
            return result

    def get_embedding_client(self):
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_embedding_dict()
            result = AzureOpenAI(
                **params
            )
        else:
            params =self.props.create_openai_embedding_dict()
            result = OpenAI(
                **params
            )
        return result
        

    # Function to encode a local image into data URL 
    def local_image_to_data_url(self, image_path) -> str:
        # Guess the MIME type of the image based on the file extension
        mime_type, _ = guess_type(image_path)
        if mime_type is None:
            mime_type = 'application/octet-stream'  # Default MIME type if none is found

        # Read and encode the image file
        with open(image_path, "rb") as image_file:
            base64_encoded_data = base64.b64encode(image_file.read()).decode('utf-8')

        # Construct the data URL
        return f"data:{mime_type};base64,{base64_encoded_data}"


    def list_openai_models(self) -> list[str]:
        
        client = self.get_completion_client()

        response = client.models.list()

        # モデルのリストを取得する
        model_id_list = [ model.id for model in response.data]
        return model_id_list

    def run_openai_chat(self, input_dict: dict):
        # OpenAIのchatを実行する
        client = self.get_completion_client()
        model = self.props.OpenAICompletionModel
            
        response = client.chat.completions.create(
            **input_dict
            )
        return response.choices[0].message.content
    
    def openai_chat(self, input_json: str, json_mode: bool = False, temperature=None):
        # 入力パラメーターの設定
        json_obj = json.loads(input_json)
        input_dict = {}
        input_dict["messages"] = json_obj
        input_dict["model"] = self.props.OpenAICompletionModel

        # json_modeがTrueの場合、response_formatを設定する
        if json_mode:
            input_dict["response_format"] = {"type": "json_object"}

        return self.run_openai_chat(input_dict)

    def openai_chat_with_vision(self, prompt: str, image_file_name_list:list, temperature=None, json_mode=False):
        # 入力パラメーターの設定
        input_list = []
        
        # image_file_name_listが空の場合は"[]"を返す
        if len(image_file_name_list) == 0:
            return "[]"
        
        content = [{"type": "text", "text": prompt}]

        for image_file_name in image_file_name_list:
            image_data_url = self.local_image_to_data_url(image_file_name)
            content.append({"type": "image_url", "image_url": {"url": image_data_url}})

        role_system_dict = {"role": "system", "content": "You are a helpful assistant."}
        role_user_dict = {"role": "user", "content": content}
        input_list.append(role_system_dict)
        input_list.append(role_user_dict)

        input_dict = {}
        # azure_openaiがTrueの場合、max_tokensを設定する
        if props.get("AzureOpenAI", "").upper() == "TRUE":
            input_dict["max_tokens"] = 4096
        # json_modeがTrueの場合、response_formatを設定する
        if json_mode:
            input_dict["response_format"] = {"type": "json_object"}
        input_dict["messages"] = input_list

        # chat_model_nameを取得する
        chat_model_name = props.get("OpenAICompletionModel", None)
        input_dict["model"] = chat_model_name
        if temperature:
            input_dict["temperature"] = temperature

        return self.run_openai_chat(input_dict)
    
    def openai_embedding(self, input_text: str):
        
        # OpenAIのchatを実行する
        client = self.get_embedding_client()
        
        # embedding_model_nameを取得する
        embedding_model_name = self.props.OpenAIEmbeddingModel
        
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
    
    
    import openai_props
    # envファイルからpropsを取得する
    props = openai_props.get_props()
    openai_client = OpenAIClient(props)

    # GPT4-Vのテスト
    image_data_url = openai_client.local_image_to_data_url("../TestData/extract_test.png")
    input_json_obj_003 =[]
    content = [
        {"type": "text", "text": "画像に「Hello World!」という文字列が含まれている場合Yes,そうでない場合はNoと答えてください"},
        {"type": "image_url", "image_url": {"url": image_data_url}}
        ]
    role_system_dict = {"role": "system", "content": "You are a helpful assistant."}
    role_user_dict = {"role": "user", "content": content}
    input_json_obj_003.append(role_system_dict)
    input_json_obj_003.append(role_user_dict)
    input_json_003 = json.dumps(input_json_obj_003)

    # chatを実行
    result = openai_client.openai_chat(input_json_001)
    print(result)
    # embeddingを実行
    result = openai_client.openai_embedding("Hello, world")
    print(result)
    # json_modeのchatを実行
    result = openai_client.openai_chat(input_json_002, True)
    print(result)
    # list_openai_modelsを実行
    result = openai_client.list_openai_models()
    print(result)
    
    # gpt4-vのchatを実行
    result = openai_client.openai_chat(input_json_003)
    print(result)

    