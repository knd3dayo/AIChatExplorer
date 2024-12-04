from autogen.coding import LocalCommandLineCodeExecutor
import venv
from ai_app_openai.ai_app_openai_util import OpenAIProps

class AutoGenProps:
    def __init__(self, openai_props: OpenAIProps,  props_dict: dict):
        # OpenAIProps
        self.openai_props = openai_props

        # work_dir
        work_dir = props_dict.get("work_dir", None)
        if work_dir is None:
            raise ValueError("work_dir is None")
        self.work_dir_path = work_dir

        # venv_path
        self.venv_path = props_dict.get("venv_path", None)

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
    
    # TODO Agent毎に設定できるようにする
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
    
    # TODO Agent毎に設定できるようにする
    def create_code_executor(self):
        params = {}
        params["timeout"] = 120
        params["work_dir"] = self.work_dir_path
        if self.venv_path:
            env_builder = venv.EnvBuilder(with_pip=True)
            virtual_env_context = env_builder.ensure_directories(self.venv_path)
            params["virtual_env_context"] = virtual_env_context
            
        # Create a local command line code executor.
        print(f"work_dir_path:{self.work_dir_path}")
        executor = LocalCommandLineCodeExecutor(
            **params
        )

        return executor

    # TODO Agent毎に設定できるようにする    
    def create_code_executor_config(self):
        return {"executor": self.create_code_executor()}