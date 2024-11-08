from dotenv import load_dotenv
import os, json
from typing import Any
from openai_props import OpenAIProps

class AutoGenProps:
    def __init__(self, openAIProps: OpenAIProps):
        if openAIProps is None:
            raise ValueError("openAIProps is None")
        
        # 基本設定
        self.llm_config = {}
        config_list = []
        llm_config_entry = {}

        llm_config_entry: dict = {}
        llm_config_entry["model"] = openAIProps.OpenAICompletionModel
        llm_config_entry["api_key"] = openAIProps.OpenAIKey

        # AzureOpenAIの場合
        if openAIProps.AzureOpenAI:
            llm_config_entry["api_type"] = "azure"
            llm_config_entry["version"] = openAIProps.AzureOpenAICompletionVersion
            if openAIProps.OpenAICompletionBaseURL:
                llm_config_entry["base_url"] = openAIProps.OpenAICompletionBaseURL
            else:
                llm_config_entry["base_url"] = openAIProps.AzureOpenAIEndpoint
        
        # llm_configに追加
        config_list.append(llm_config_entry)
        self.llm_config["config_list"] = config_list
