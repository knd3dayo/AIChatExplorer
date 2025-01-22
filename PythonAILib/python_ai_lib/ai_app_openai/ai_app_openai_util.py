from dotenv import load_dotenv
import os, json
import base64
from mimetypes import guess_type
from typing import Any, Type
import tiktoken

# リクエストコンテキスト
class RequestContext:
    prompt_template_text_name = "prompt_template_text"
    chat_mode_name = "chat_mode" 
    def __init__(self, request_context_dict: dict):
        self.PromptTemplateText = request_context_dict.get(RequestContext.prompt_template_text_name, "")
        self.ChatMode = request_context_dict.get(RequestContext.chat_mode_name, "Normal")
        self.SplitMode = request_context_dict.get("split_mode", "None")

class OpenAIProps:
    def __init__(self, props_dict: dict):
        
        self.OpenAIKey:str = props_dict.get("OpenAIKey", "")
        self.OpenAICompletionModel:str = props_dict.get("OpenAICompletionModel", "")
        self.OpenAIEmbeddingModel:str = props_dict.get("OpenAIEmbeddingModel", "")
        self.OpenAIWhisperModel:str = props_dict.get("OpenAIWhisperModel" , "")
        self.OpenAITranscriptionModel:str = props_dict.get("OpenAITranscriptionModel", "")

        self.AzureOpenAI =props_dict.get("AzureOpenAI", False)
        if type(self.AzureOpenAI) == str:
            self.AzureOpenAI = self.AzureOpenAI.upper() == "TRUE"
            
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

    @staticmethod
    def env_to_props() -> 'OpenAIProps':
        load_dotenv()
        props: dict = {
            "OpenAIKey": os.getenv("OPENAI_API_KEY"),
            "OpenAICompletionModel": os.getenv("OPENAI_COMPLETION_MODEL"),
            "OpenAIEmbeddingModel": os.getenv("OPENAI_EMBEDDING_MODEL"),
            "AzureOpenAI": os.getenv("AZURE_OPENAI"),
            "AzureOpenAICompletionVersion": os.getenv("AZURE_OPENAI_API_VERSION"),
            "OpenAICompletionBaseURL": os.getenv("OPENAI_COMPLETION_BASE_URL"),
            "OpenAIEmbeddingBaseURL": os.getenv("OPENAI_EMBEDDING_BASE_URL"),
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
    def create_openai_chat_parameter_dict(model: str, messages_json: str, templature : float =0.5, json_mode : bool = False) -> dict:
        params : dict [ str, Any]= {}
        params["model"] = model
        params["messages"] = json.loads(messages_json)
        if templature:
            params["temperature"] = str(templature)
        if json_mode:
            params["response_format"]= {"type": "json_object"}
        return params

    # openai_chat_with_vision用のパラメーターを作成する
    @staticmethod
    def create_openai_chat_with_vision_parameter_dict(model: str, prompt: str, image_file_name_list: list, templature : float =0.5, json_mode : bool = False, max_tokens = None) -> dict:
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
        if templature:
            params["temperature"] = templature
        if json_mode:
            params["response_format"] = {"type": "json_object"}
        if max_tokens:
            params["max_tokens"] = max_tokens
        
        return params

from typing import Tuple
import json
from openai import OpenAI, AzureOpenAI, RateLimitError
import time

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

    def get_whisper_client(self):
        if (self.props.AzureOpenAI):
            params = self.props.create_azure_openai_whisper_dict()
            result = AzureOpenAI(
                **params
            )
        else:
            params =self.props.create_openai_whisper_dict()
            result = OpenAI(
                **params
            )
        return result

    def list_openai_models(self) -> list[str]:
        
        client = self.get_completion_client()

        response = client.models.list()

        # モデルのリストを取得する
        model_id_list = [ model.id for model in response.data]
        return model_id_list

    def run_openai_chat(self, request_context: RequestContext ,input_dict: dict) -> dict:
        # OpenAIのchatを実行する
        client = self.get_completion_client()
        # AzureOpenAIの場合はmax_tokensとstream=Falseを設定する
        if self.props.AzureOpenAI:
            input_dict["max_tokens"] = 4096
            input_dict["stream"] = False

        # "messages"の最後の要素を取得する       
        last_message = input_dict["messages"][-1]
        for i in range(0, len(last_message["content"])):
            if last_message["content"][i]["type"] == "text":
                original_text = last_message["content"][i]["text"]
                last_message["content"][i]["text"] = f"{request_context.PromptTemplateText}\n{original_text}"
                break
        # messagesの最後の要素を更新する
        input_dict["messages"][-1] = last_message

        if request_context.SplitMode != "None":
            # input_dictのmessagesの最後の要素のみを取得する
            last_message = input_dict["messages"][-1]
            # input_dictのmessagesを更新する
            input_dict["messages"] = [last_message]

        # openai.
        # RateLimitErrorが発生した場合はリトライする
        # リトライ回数は最大で3回
        # リトライ間隔はcount*30秒
        # リトライ回数が5回を超えた場合はRateLimitErrorをraiseする
        # リトライ回数が5回以内で成功した場合は結果を返す
        count = 0
        while count < 3:
            try:
                response = client.chat.completions.create(
                    **input_dict
                )
                break
            except RateLimitError as e:
                count += 1
                # rate limit errorが発生した場合はリトライする旨を表示。英語
                print(f"RateLimitError has occurred. Retry after {count*30} seconds.")
                time.sleep(count*30)
                if count == 5:
                    raise e
                            
        # token情報を取得する
        total_tokens = response.usage.total_tokens
        # contentを取得する
        content = response.choices[0].message.content
        # dictにして返す
        return {"output": content, "total_tokens": total_tokens}
    
    def openai_chat(self, input_json: str, json_mode: bool = False, temperature=None) -> dict:
        # 入力パラメーターの設定
        model = self.props.OpenAICompletionModel
        params = OpenAIProps.create_openai_chat_parameter_dict(model, input_json, temperature, json_mode)
        return self.run_openai_chat(params)

    def openai_chat_with_vision(self, prompt: str, image_file_name_list:list, temperature=None, json_mode=False) -> dict:
        
        # AzureOpenAIの場合はmax_tokensを設定する
        if self.props.AzureOpenAI:
            max_tokens = 4096
        else:
            max_tokens = None

        # openai_chat_with_vision用のパラメーターを作成する
        params = OpenAIProps.create_openai_chat_with_vision_parameter_dict(self.props.OpenAICompletionModel, prompt, image_file_name_list, temperature, json_mode, max_tokens)

        return self.run_openai_chat(params)
    
    def openai_embedding(self, input_text: str):
        
        # OpenAIのchatを実行する
        client = self.get_embedding_client()
        
        # embedding_model_nameを取得する
        embedding_model_name = self.props.OpenAIEmbeddingModel
        
        # RateLimitErrorが発生した場合はリトライする
        # リトライ回数は最大で3回
        # リトライ間隔はcount*30秒
        # リトライ回数が5回を超えた場合はRateLimitErrorをraiseする
        # リトライ回数が5回以内で成功した場合は結果を返す
        count = 0
        while count < 3:
            try:
                response = client.embeddings.create(
                    model=embedding_model_name,
                    input=[input_text]
                )
                break
            except RateLimitError as e:
                count += 1
                # rate limit errorが発生した場合はリトライする旨を表示。英語
                print(f"RateLimitError has occurred. Retry after {count*30} seconds.")
                time.sleep(count*30)
                if count == 5:
                    raise e

        return response.data[0].embedding
    
    def openai_transcription(self, audit_file_path: str):
        # OpenAIのtranscriptionを実行する
        client = self.get_whisper_client()
        
        with open(audit_file_path, "rb") as f:
            response = client.audio.transcriptions.create(
                model=self.props.OpenAITranscriptionModel,
                file=f,
                response_format="verbose_json"
            )
        return json.loads(response.model_dump_json())

    def get_token_count(self, input_text: str) -> int:
        # completion_modelに対応するencoderを取得する
        encoder = tiktoken.encoding_for_model(self.props.OpenAICompletionModel)
        # token数を取得する
        return len(encoder.encode(input_text))
    