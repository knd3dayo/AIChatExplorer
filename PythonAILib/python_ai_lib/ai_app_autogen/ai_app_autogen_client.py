import json

from autogen.coding import LocalCommandLineCodeExecutor

from ai_app_openai.ai_app_openai_util import OpenAIProps

class AutoGenProps:
    def __init__(self, openAIProps: OpenAIProps, work_dir_path: str):
        if openAIProps is None:
            raise ValueError("openAIProps is None")
        # work_dir_pathがNoneの場合はエラー
        if work_dir_path is None:
            raise ValueError("work_dir_path is None")
        # 基本設定
        self.work_dir_path = work_dir_path
        self.OpenAIProps = openAIProps


    def create_llm_config(self):
        config_list = []
        llm_config_entry = {}

        llm_config_entry: dict = {}
        llm_config_entry["model"] = self.OpenAIProps.OpenAICompletionModel
        llm_config_entry["api_key"] = self.OpenAIProps.OpenAIKey

        # AzureOpenAIの場合
        if self.OpenAIProps.AzureOpenAI:
            llm_config_entry["api_type"] = "azure"
            llm_config_entry["api_version"] = self.OpenAIProps.AzureOpenAICompletionVersion
            if self.OpenAIProps.OpenAICompletionBaseURL:
                llm_config_entry["base_url"] = self.OpenAIProps.OpenAICompletionBaseURL
            else:
                llm_config_entry["base_url"] = self.OpenAIProps.AzureOpenAIEndpoint
        
        # llm_configに追加
        config_list.append(llm_config_entry)
        llm_config = {}
        llm_config["config_list"] = config_list

        return llm_config