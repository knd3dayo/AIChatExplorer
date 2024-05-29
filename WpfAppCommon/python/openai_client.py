import json
from openai import OpenAI, AzureOpenAI
from openai_props import OpenAIProps, create_openai_chat_parameter_dict, create_openai_chat_with_vision_parameter_dict

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

    def run_openai_chat(self, input_dict: dict) -> str:
        # OpenAIのchatを実行する
        client = self.get_completion_client()
        model = self.props.OpenAICompletionModel
            
        response = client.chat.completions.create(
            **input_dict
            )
        return response.choices[0].message.content
    
    def openai_chat(self, input_json: str, json_mode: bool = False, temperature=None) -> str:
        # 入力パラメーターの設定
        model = self.props.OpenAICompletionModel
        params = create_openai_chat_parameter_dict(model, input_json, temperature, json_mode)
        return self.run_openai_chat(params)

    def openai_chat_with_vision(self, prompt: str, image_file_name_list:list, temperature=None, json_mode=False):
        
        # AzureOpenAIの場合はmax_tokensを設定する
        if self.props.AzureOpenAI:
            max_tokens = 4096
        else:
            max_tokens = None

        # openai_chat_with_vision用のパラメーターを作成する
        params = create_openai_chat_with_vision_parameter_dict(self.props.OpenAICompletionModel, prompt, image_file_name_list, temperature, json_mode, max_tokens)

        return self.run_openai_chat(params)
    
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
