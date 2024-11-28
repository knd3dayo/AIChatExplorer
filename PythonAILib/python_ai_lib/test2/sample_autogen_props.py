from autogen.coding import LocalCommandLineCodeExecutor
from dotenv import load_dotenv
import datetime
import os

class AutoGenProps:
    def __init__(self,):
        load_dotenv()

        # 作業用フォルダ：yyyy-mm-dd-hh-mm-ss
        self.work_dir_path = datetime.datetime.now().strftime("%Y-%m-%d-%H-%M-%S")
        
        # openai_api_key
        self.openai_api_key = os.getenv("OPENAI_API_KEY", None)
        if self.openai_api_key is None:
            raise ValueError("OPENAI_API_KEY is None")
        # openai_model OPENAI_COMPLETION_MODEL
        self.openai_model = os.getenv("OPENAI_COMPLETION_MODEL", None)
        if self.openai_model is None:
            raise ValueError("OPENAI_COMPLETION_MODEL is None")

        # base_url OPENAI_COMPLETION_BASE_URL
        self.base_url = os.getenv("OPENAI_COMPLETION_BASE_URL", None)
            
        # azure_openai
        self.azure_openai = os.getenv("AZURE_OPENAI", "False").lower() == "true"

        if self.azure_openai:
            # azure_api_version
            self.azure_api_version = os.getenv("AZURE_OPENAI_API_VERSION", None)


    def create_llm_config(self):
        config_list = []
        llm_config_entry = {}

        llm_config_entry: dict = {}
        llm_config_entry["model"] = self.openai_model
        llm_config_entry["api_key"] = self.openai_api_key

        # base_urlが設定されている場合
        if self.base_url:
            llm_config_entry["base_url"] = self.base_url

        # AzureOpenAIの場合
        if self.azure_openai:
            llm_config_entry["api_type"] = "azure"
            llm_config_entry["api_version"] = self.azure_api_version
        
        # llm_configに追加
        config_list.append(llm_config_entry)
        llm_config = {}
        llm_config["config_list"] = config_list
        llm_config["cache_seed"] = None

        return llm_config
    
    def create_code_executor(self):
        # Create a local command line code executor.
        print(f"work_dir_path:{self.work_dir_path}")
        executor = LocalCommandLineCodeExecutor(
            timeout=120,  # Timeout for each code execution in seconds.
            work_dir=self.work_dir_path,  # Use the temporary directory to store the code files.
        )
        return executor
