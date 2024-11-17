import json

from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor
import autogen 

from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps
from ai_app_autogen_tools import AutoGenTools

class AutoGenProps:
    def __init__(self, openAIProps: OpenAIProps, work_dir_path: str):
        if openAIProps is None:
            raise ValueError("openAIProps is None")
        
        # 基本設定
        self.work_dir_path = work_dir_path
        self.llm_config = {}
        config_list = []
        llm_config_entry = {}

        llm_config_entry: dict = {}
        llm_config_entry["model"] = openAIProps.OpenAICompletionModel
        llm_config_entry["api_key"] = openAIProps.OpenAIKey

        # Create a local command line code executor.
        self.executor = LocalCommandLineCodeExecutor(
            timeout=10,  # Timeout for each code execution in seconds.
            work_dir=self.work_dir_path,  # Use the temporary directory to store the code files.
        )

        # AzureOpenAIの場合
        if openAIProps.AzureOpenAI:
            llm_config_entry["api_type"] = "azure"
            llm_config_entry["api_version"] = openAIProps.AzureOpenAICompletionVersion
            if openAIProps.OpenAICompletionBaseURL:
                llm_config_entry["base_url"] = openAIProps.OpenAICompletionBaseURL
            else:
                llm_config_entry["base_url"] = openAIProps.AzureOpenAIEndpoint
        
        # llm_configに追加
        config_list.append(llm_config_entry)
        self.llm_config["config_list"] = config_list
