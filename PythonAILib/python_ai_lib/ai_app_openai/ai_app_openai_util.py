from dotenv import load_dotenv
import os, json
import base64
from mimetypes import guess_type
from typing import Any, Type
import tiktoken
import copy
# リクエストコンテキスト
class RequestContext:
    prompt_template_text_name = "prompt_template_text"
    chat_mode_name = "chat_mode" 
    summarize_prompt_text_name = "summarize_prompt_text"
    related_information_prompt_text_name = "related_information_prompt_text"
    def __init__(self, request_context_dict: dict):
        self.PromptTemplateText = request_context_dict.get(RequestContext.prompt_template_text_name, "")
        self.ChatMode = request_context_dict.get(RequestContext.chat_mode_name, "Normal")
        self.SplitMode = request_context_dict.get("split_mode", "None")
        self.SummarizePromptText = request_context_dict.get(RequestContext.summarize_prompt_text_name, "")
        self.RelatedInformationPromptText = request_context_dict.get(RequestContext.related_information_prompt_text_name, "")

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
    
    @staticmethod
    def create_openai_chat_parameter_dict_simple(model: str, prompt: str, templature : float =0.5, json_mode : bool = False) -> dict:
        # messagesの作成
        messages = []
        messages.append({"role": "user", "content": prompt})
        # 入力パラメーターの設定
        params : dict [ str, Any]= {}
        params["messages"] = messages
        params["model"] = model
        if templature:
            params["temperature"] = templature
        if json_mode:
            params["response_format"] = {"type": "json_object"}
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

    def split_message(self, message_list: list[str]) -> list[str]:
        # token_countが80KBを超える場合は分割する
        result_message_list = []
        temp_message_list = []
        total_token_count = 0
        for i in range(0, len(message_list)):
            message = message_list[i] + "\n"
            token_count = self.get_token_count(message)
            # total_token_count + token_countが80KBを超える場合はtemp_message_listをresult_message_listに追加する
            if total_token_count + token_count > 80000:
                result_message_list.append("\n".join(temp_message_list))
                temp_message_list = []
                total_token_count = 0
            temp_message_list.append(message)
            total_token_count += token_count
        # temp_message_listが空でない場合はresult_message_listに追加する
        if len(temp_message_list) > 0:
            result_message_list.append("\n".join(temp_message_list))
        # result_message_listを返す
        return result_message_list
    
    def pre_process_input(self, vector_search_function : callable, request_context:RequestContext, last_message_dict: dict) -> list[dict]:
        # "messages"の最後の要素を取得する       
        last_text_content_index = -1
        for i in range(0, len(last_message_dict["content"])):
            if last_message_dict["content"][i]["type"] == "text":
                last_text_content_index = i
                break
        # last_text_content_indexが-1の場合はエラーをraiseする
        if last_text_content_index == -1:
            raise ValueError("last_text_content_index is -1")
        # queryとして最後のtextを取得する
        original_last_message = last_message_dict["content"][last_text_content_index]["text"]
        # request_contextのSplitModeがNone以外の場合はoriginal_last_messageを改行毎にtokenをカウントして、
        # 80KBを超える場合は分割する
        # 結果はresult_messagesに格納する
        result_messages = []
        if request_context.SplitMode != "None":
            target_messages = self.split_message(original_last_message.split("\n"))
        else:
            target_messages = [original_last_message]

        for target_message in target_messages:
            # ベクトル検索用の文字列としてqueryにtarget_messageを設定する
            query = target_message
            # context_message 
            context_message = ""
            if len(request_context.PromptTemplateText) > 0:
                context_message = request_context.PromptTemplateText
            # chat_modeがNormal以外の場合はベクトル検索を実施
            if request_context.ChatMode != "Normal":
                vector_search_result = vector_search_function(query) 
                vector_search_results = [ document["content"] for document in vector_search_result["documents"]]
                context_message += request_context.RelatedInformationPromptText + "\n".join(vector_search_results)
            # last_messageをdeepcopyする
            result_last_message = copy.deepcopy(last_message_dict)
            # result_last_messageのcontentの最後の要素を更新する
            result_last_message["content"][last_text_content_index]["text"] = f"{context_message}\n{target_message}"
            # result_messagesに追加する
            result_messages.append(result_last_message)
        return result_messages

    def post_process_output(self, vector_search_function : callable, request_context: RequestContext, input_dict: dict, chat_result_dict_list: list[dict]) -> dict:
        # RequestContextのSplitModeがNormalSplitの場合はchat_result_dict_listのoutputを結合した文字列とtotal_tokensを集計した結果を返す
        if request_context.SplitMode == "NormalSplit":
            output = "\n".join([chat_result_dict["output"] for chat_result_dict in chat_result_dict_list])
            total_tokens = sum([chat_result_dict["total_tokens"] for chat_result_dict in chat_result_dict_list])
            return {"output": output, "total_tokens": total_tokens}
        
        # RequestContextのSplitModeがSplitAndSummarizeの場合はSummarize用のoutputを作成する
        if request_context.SplitMode == "SplitAndSummarize":
            summary_prompt_text = ""
            if len(request_context.PromptTemplateText) > 0:
                summary_prompt_text = f"""
                The following text is a document that was split into several parts, and based on the instructions of [{request_context.PromptTemplateText}], 
                the AI-generated responses were combined. 
                {request_context.PromptTemplateText}
                """

            else:
                summary_prompt_text = """
                The following text is a document that has been divided into several parts, with AI-generated responses combined.
                {request_context.PromptTemplateText}
                """
            summary_input =  summary_prompt_text + "\n".join([chat_result_dict["output"] for chat_result_dict in chat_result_dict_list])
            total_tokens = sum([chat_result_dict["total_tokens"] for chat_result_dict in chat_result_dict_list])
            # openai_chatの入力用のdictを作成する
            summary_input_dict = OpenAIProps.create_openai_chat_parameter_dict_simple(input_dict["model"], summary_input, input_dict.get("temperature", 0.5), input_dict.get("json_mode", False))
            # chatを実行する
            summary_result_dict = self.openai_chat(summary_input_dict)
            # total_tokensを更新する
            summary_result_dict["total_tokens"] = total_tokens + summary_result_dict["total_tokens"]
            return summary_result_dict
        
        # RequestContextのSplitModeがNoneの場合はoutput_dictの1つ目の要素を返す
        return chat_result_dict_list[0]
            
    def run_openai_chat(self, vector_search_function : callable, request_context: RequestContext ,input_dict: dict) -> dict:

        # pre_process_inputを実行する
        last_message_dict = input_dict["messages"][-1]
        pre_processed_input_list = self.pre_process_input(vector_search_function, request_context, last_message_dict)
        chat_result_dict_list = []

        for pre_processed_input in pre_processed_input_list:
            # input_dictのmessagesの最後の要素のみを取得する
            copied_input_dict = copy.deepcopy(input_dict)

            # split_modeがNone以外の場合はinput_dictのmessagesの最後の要素のみを取得する
            if request_context.SplitMode != "None":
                copied_input_dict["messages"] = [pre_processed_input]
            else:
                copied_input_dict["messages"][-1] = pre_processed_input

            chat_result_dict = self.openai_chat(copied_input_dict)
            # chat_result_dictをchat_result_dict_listに追加する
            chat_result_dict_list.append(chat_result_dict)

        # post_process_outputを実行する
        result_dict = self.post_process_output(vector_search_function, request_context, input_dict, chat_result_dict_list)
        return result_dict
    
    def openai_chat(self, input_dict: dict) -> dict:
        # openai.
        # RateLimitErrorが発生した場合はリトライする
        # リトライ回数は最大で3回
        # リトライ間隔はcount*30秒
        # リトライ回数が5回を超えた場合はRateLimitErrorをraiseする
        # リトライ回数が5回以内で成功した場合は結果を返す
        # OpenAIのchatを実行する
        print("chat input", json.dumps(input_dict, ensure_ascii=False, indent=2))

        client = self.get_completion_client()
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
        print("chat output", json.dumps(content, ensure_ascii=False, indent=2))
        return {"output": content, "total_tokens": total_tokens}


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
    