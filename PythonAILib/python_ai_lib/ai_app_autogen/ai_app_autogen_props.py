from autogen.coding import LocalCommandLineCodeExecutor

from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps

class AutoGenProps:
    def __init__(self, openai_props: OpenAIProps, vector_db_items: list[VectorDBProps],  props_dict: dict):
        # OpenAIProps
        self.openai_props = openai_props
        # VectorDBPropsのリスト
        self.vector_db_items = vector_db_items

        # work_dir
        work_dir = props_dict.get("work_dir", None)
        if work_dir is None:
            raise ValueError("work_dir is None")
        self.work_dir_path = work_dir

        # use_system_agent
        self.use_system_agent: bool = props_dict.get("use_system_agent", False)

        # group_chat
        group_chat_dict = props_dict.get("group_chat", None)
        if group_chat_dict is None:
            raise ValueError("group_chat is None")
        self.group_chat_dict = group_chat_dict

        # tools
        self.tools_list = group_chat_dict.get("tools", [])
        # agents
        self.agents_list = group_chat_dict.get("agents", [])
        
    def create_llm_config(self):
        config_list = []
        llm_config_entry = {}

        llm_config_entry: dict = {}
        llm_config_entry["model"] = self.openai_props.OpenAICompletionModel
        llm_config_entry["api_key"] = self.openai_props.OpenAIKey

        # AzureOpenAIの場合
        if self.openai_props.AzureOpenAI:
            llm_config_entry["api_type"] = "azure"
            llm_config_entry["api_version"] = self.openai_props.AzureOpenAICompletionVersion
            if self.openai_props.OpenAICompletionBaseURL:
                llm_config_entry["base_url"] = self.openai_props.OpenAICompletionBaseURL
            else:
                llm_config_entry["base_url"] = self.openai_props.AzureOpenAIEndpoint
        
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
    
    def create_code_executor_config(self):
        return {"executor": self.create_code_executor()}