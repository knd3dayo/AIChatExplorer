from dotenv import load_dotenv
import os, json
import base64
from mimetypes import guess_type
from typing import Any, Union
import time
from openai import RateLimitError

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

class OpenAIProps:

    openai_props_name = "openai_props"
    @classmethod
    def get_openai_objects(cls, request_dict: dict) -> tuple["OpenAIProps", "OpenAIClient"]:
        '''
        {"openai_props": {}}の形式で渡される
        '''
        # OpenAIPorps, OpenAIClientを生成
        openai_props_dict = request_dict.get(cls.openai_props_name, None)
        if not openai_props_dict:
            raise ValueError("openai_props is not set.")

        openai_props = OpenAIProps(openai_props_dict)
        client = OpenAIClient(openai_props)
        return openai_props, client

    def __init__(self, props_dict: dict):
        
        self.OpenAIKey:str = props_dict.get("OpenAIKey", "")

        self.AzureOpenAI =props_dict.get("AzureOpenAI", False)
        if type(self.AzureOpenAI) == str:
            self.AzureOpenAI = self.AzureOpenAI.upper() == "TRUE"
            
        self.AzureOpenAIAPIVersion = props_dict.get("AzureOpenAIAPIVersion", None)
        self.AzureOpenAIEndpoint = props_dict.get("AzureOpenAIEndpoint", None)
        self.OpenAIBaseURL = props_dict.get("OpenAIBaseURL", None)

        # AzureOpenAICompletionVersionがNoneの場合は2024-02-01を設定する
        if self.AzureOpenAIAPIVersion == None:
            self.AzureOpenAIAPIVersion = "2024-02-01"

    # OpenAIのCompletion用のパラメーター用のdictを作成する
    def create_openai_dict(self) -> dict:
        completion_dict = {}
        completion_dict["api_key"] = self.OpenAIKey
        if self.OpenAIBaseURL:
            completion_dict["base_url"] = self.OpenAIBaseURL
        return completion_dict
        
    # AzureOpenAIのCompletion用のパラメーター用のdictを作成する
    def create_azure_openai_dict(self) -> dict:
        completion_dict = {}
        completion_dict["api_key"] = self.OpenAIKey
        if self.OpenAIBaseURL:
            completion_dict["base_url"] = self.OpenAIBaseURL
        else:
            completion_dict["azure_endpoint"] = self.AzureOpenAIEndpoint
            completion_dict["api_version"] = self.AzureOpenAIAPIVersion
        return completion_dict


    @staticmethod
    def env_to_props() -> 'OpenAIProps':
        load_dotenv()
        props: dict = {
            "OpenAIKey": os.getenv("OPENAI_API_KEY"),
            "OpenAICompletionModel": os.getenv("OPENAI_COMPLETION_MODEL"),
            "OpenAIEmbeddingModel": os.getenv("OPENAI_EMBEDDING_MODEL"),
            "AzureOpenAI": os.getenv("AZURE_OPENAI"),
            "AzureOpenAIAPIVersion": os.getenv("AZURE_OPENAI_API_VERSION"),
            "OpenAIBaseURL": os.getenv("OPENAI_BASE_URL"),
        }
        openAIProps = OpenAIProps(props)
        return openAIProps


    # Function to encode a local image into data URL 
    @staticmethod
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
    @staticmethod
    def create_openai_chat_parameter_dict(model: str, messages_json: str, temperature : float =0.5, json_mode : bool = False) -> dict:
        params : dict [ str, Any]= {}
        params["model"] = model
        params["messages"] = json.loads(messages_json)
        if temperature:
            params["temperature"] = str(temperature)
        if json_mode:
            params["response_format"]= {"type": "json_object"}
        return params
    
    @staticmethod
    def create_openai_chat_parameter_dict_simple(model: str, prompt: str, temperature : float =0.5, json_mode : bool = False) -> dict:
        # messagesの作成
        messages = []
        messages.append({"role": "user", "content": prompt})
        # 入力パラメーターの設定
        params : dict [ str, Any]= {}
        params["messages"] = messages
        params["model"] = model
        if temperature:
            params["temperature"] = temperature
        if json_mode:
            params["response_format"] = {"type": "json_object"}
        return params

    # openai_chat_with_vision用のパラメーターを作成する
    @staticmethod
    def create_openai_chat_with_vision_parameter_dict(model: str, prompt: str, image_file_name_list: list, temperature : float =0.5, json_mode : bool = False, max_tokens = None) -> dict:
        # messagesの作成
        messages = []
        content: list[dict [ str, Any]]  = [{"type": "text", "text": prompt}]

        for image_file_name in image_file_name_list:
            image_data_url = OpenAIProps.local_image_to_data_url(image_file_name)
            content.append({"type": "image_url", "image_url": {"url": image_data_url}})

        role_user_dict = {"role": "user", "content": content}
        messages.append(role_user_dict)

        # 入力パラメーターの設定
        params : dict [ str, Any]= {}
        params["messages"] = messages
        params["model"] = model
        if temperature:
            params["temperature"] = temperature
        if json_mode:
            params["response_format"] = {"type": "json_object"}
        if max_tokens:
            params["max_tokens"] = max_tokens
        
        return params

import json
from openai import AsyncOpenAI, AsyncAzureOpenAI, RateLimitError

class OpenAIClient:
    def __init__(self, props: OpenAIProps):
        
        self.props = props

    def get_completion_client(self) -> Union[AsyncOpenAI, AsyncAzureOpenAI]:
        
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_dict()
            return AsyncAzureOpenAI(
                **params
            )

        else:
            params =self.props.create_openai_dict()
            return AsyncOpenAI(
                **params
            )

    def get_embedding_client(self) -> Union[AsyncOpenAI, AsyncAzureOpenAI]:
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_dict()
            return AsyncAzureOpenAI(
                **params
            )
        else:
            params =self.props.create_openai_dict()
            return AsyncOpenAI(
                **params
            )

    async def list_openai_models_async(self) -> list[str]:
        
        client = self.get_completion_client()

        response = await client.models.list()

        # モデルのリストを取得する
        model_id_list = [ model.id for model in response.data]
        return model_id_list
 
