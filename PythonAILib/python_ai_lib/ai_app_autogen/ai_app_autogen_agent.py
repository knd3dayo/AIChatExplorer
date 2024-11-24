
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from collections.abc import Generator
from typing import Any

from ai_app_autogen.ai_app_autogen_tools import AutoGenTools, AutoGenToolGenerator
from ai_app_autogen.ai_app_autogen_client import AutoGenProps
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps

class AutoGenAgents:

    def __init__(self, autogen_props: AutoGenProps, autogen_tools: AutoGenTools, agents_dict: list[dict], auto_execute_code: bool = False):
        
        self.autogen_props = autogen_props
        self.work_dir_path = autogen_props.work_dir_path
        # コード実行者が自動的にコードを実行するようにするかどうか
        # self.auto_execute_code = auto_execute_code
        self.auto_execute_code = True
        
        self.autogen_tools = autogen_tools

        self.agents : dict[str, tuple[ConversableAgent, str]] = {}
        if self.autogen_props.use_system_agent:
            print("Using system agents")
            self.agents.update(AutoGenAgentGenerator.create_default_agents(self.autogen_props, self.autogen_tools))
        
        for agent_dict in agents_dict:
            self.agents.update(AutoGenAgentGenerator.create_agents_dict(self.autogen_tools, agent_dict))

        self.temp_dir = None


    def add_agents(self, agents: dict[str, tuple[ConversableAgent, str]]):
        self.agents.update(agents)

    # エージェントの終了処理
    def finish(self):
        if self.temp_dir:
            self.temp_dir.cleanup()

from openpyxl import load_workbook
from autogen import ConversableAgent, UserProxyAgent
from ai_app_autogen.ai_app_autogen_client import AutoGenProps

class AutoGenAgentGenerator:

    @classmethod
    def create_agent_from_definition(cls, autogen_tools: AutoGenTools, params: dict) -> ConversableAgent:
        autogen_props: AutoGenProps = autogen_tools.autogen_props

        # human_input_modeが指定されていない場合はデフォルト値を設定
        human_input_mode = params.get('human_input_mode', None)
        params['human_input_mode'] = human_input_mode if human_input_mode else "NEVER"
        # is_termination_msgが指定されていない場合はNone, それ以外の場合はtermination_msgを設定
        termination_msg = params.get('is_termination_msg', None)
        if termination_msg:
            is_termination_msg = lambda msg: termination_msg in msg["content"].lower()
            params['is_termination_msg'] = is_termination_msg

        # code_executionを大文字にしたものがTRUEの場合はauto_gen_pros.create_code_executor()を呼び出し
        # それ以外の場合はFalseを設定
        code_execution = params.get('code_execution_config', None)
        params['code_execution_config'] = autogen_props.create_code_executor() if code_execution.upper() == "TRUE" else False
        # llm_configを設定 llmをお大文字にしたものがTRUEの場合はauto_gen_pros.create_llm_config()を呼び出し
        # それ以外の場合はNoneを設定
        llm = params.get('llm_config', None)
        params['llm_config'] = autogen_props.create_llm_config() if llm.upper() == "TRUE" else None

        type_value = params.get('type', None)
        agent = None
        if type_value == 'userproxy':
            # userproxyの場合
            agent: UserProxyAgent = UserProxyAgent( **params)
        elif type_value == 'assistant':
            # assistantの場合
            agent: ConversableAgent = ConversableAgent( **params)
        else:
            raise ValueError(f"Unknown agent type: {type_value}")

        # agentにツールを設定
        # tools :str からtools_listに変換
        tool_name_text = params.get('tools', None)
        if tool_name_text:
            tool_names = tool_name_text.split(',')
            for tool_name in tool_names:
                # self.autogen_toolsからtool_nameを指定して関数と説明を取得
                function_obj, description = autogen_tools.get(tool_name, (None, None))
                if function_obj is None:
                    raise ValueError(f"Unknown tool: {tool_name}")
                # agentにツールを設定
                agent.register_for_llm(name=tool_name, description=description)(function_obj)
        
        return agent

    @classmethod
    def create_agents_dict(cls, autogen_tools: AutoGenTools, data_list: list[dict]) -> dict:
        agent_dict = {}
        for data in data_list:
            name = data['name']
            description = data['description']
            # エージェントを作成
            agent_obj = cls.create_agent_from_definition(autogen_tools, data)
            # dictに格納
            agent_dict[name] = (agent_obj, description)

        # 結果を表示（必要に応じて）
        for name, (agent, desc) in agent_dict.items():
            print(f'Name: {name}, Description: {desc}, Agent: {agent}')
        
        return agent_dict

    @classmethod
    def create_definiton(
        cls, name: str, description: str, system_message: str, type: str, human_input_mode: str, 
        is_termination_msg: str, code_execution_config: bool, llm_config: bool, tools: str) -> dict:
        return {
            "name": name,
            "description": description,
            "type": type,
            "human_input_mode": human_input_mode,
            "is_termination_msg": is_termination_msg,
            "code_execution_config": code_execution_config,
            "llm_config": llm_config,
            "tools": tools
        }

    @classmethod
    def create_default_agents(cls, autogen_props: AutoGenProps, tools: AutoGenTools) -> dict[str, tuple[ConversableAgent, str, dict]]:

        agents : dict[str, tuple[ConversableAgent, str, dict]] = {}
        agents["user_proxy"] = cls.__create_user_proxy(autogen_props, tools)
        agents["code_executor"] = cls.__create_code_executor(autogen_props, tools, True)
        agents["code_writer"] = cls.__create_code_writer(autogen_props, tools)
        agents["file_writer"] = cls.__create_file_writer(autogen_props, tools)
        agents["vector_searcher"] = cls.__create_vector_searcher(autogen_props, tools)
        agents["file_extractor"] = cls.__create_file_extractor(autogen_props, tools)
        agents["web_searcher"] = cls.__create_web_searcher(autogen_props, tools)
        agents["wikipeda_searcher"] = cls.__create_wikipedia_searcher(autogen_props, tools)
        agents["azure_document_searcher"] = cls.__create_azure_document_searcher(autogen_props, tools)
        agents["file_checker"] = cls.__create_file_checker(autogen_props, tools)
        agents["current_time"] = cls.__create_current_time(autogen_props, tools)

        return agents

    @classmethod
    def __create_user_proxy(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        # Task assigner for group chat
        description = "Creates a list of tasks to achieve the user's request and assigns tasks to each agent."
        user_proxy = autogen.UserProxyAgent(
            system_message="""
                Executes tasks in collaboration with each agent to achieve the user's request.
                - First, create a plan and a list of tasks to achieve the user's request.
                - Assign tasks to each agent and execute the tasks.
                - When the plan is achieved by executing the tasks by each agent, reply with [End Meeting].
                - If there are no additional questions, reply with [End Meeting].
                """,
            name="user_proxy",
            human_input_mode="NEVER",
            code_execution_config=False,
            is_termination_msg=lambda msg: "end meeting" in msg["content"].lower(),
            description=description,
            llm_config=autogen_pros.create_llm_config()
        )

        # Setting register_from_execution() for user_proxy
        for func, description in autogen_tools.tools.values():
            print(f"register_for_execution: {func.__name__}")
            user_proxy.register_for_execution()(func)

        return user_proxy, description

    @classmethod
    def __create_code_writer(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        # Separate the code writer and executor. Below is the code inference Agent with LLM.
        description = "Creates Python scripts according to the user's instructions."
        code_writer_agent = ConversableAgent(
            "code-writer",
            system_message=f"""
                You are a script developer.
                When you write code, it is automatically executed on an external application.
                You write code according to the user's instructions.
                The execution result of the code is automatically displayed after you post the code.
                However, you must strictly adhere to the following conditions:
                Rules:
                * Only propose code within code blocks.
                * If the execution result of the script is an error, consider measures based on the error message and create revised code again.
                * If the information obtained from the execution of the script is insufficient, create revised code again based on the currently obtained information.
                * Your ultimate goal is the user's instructions, and you will create and revise code as many times as necessary to meet this goal.
                """
        ,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        return code_writer_agent, description

    @classmethod
    def __create_file_writer(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        # Separate the code writer and executor. Below is the code inference Agent with LLM.
        description = "Saves data to a file in Python according to the user's instructions."
        file_writer = ConversableAgent(
            "file-writer",
            system_message=f"""
                Saves data to a file in Python according to the user's instructions.
                The default save location is {autogen_pros.work_dir_path}.
                If the user specifies a save location, save the file to the specified location.
                """
        ,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )
        save_tools, description = autogen_tools.tools["save_text_file"]
        # register_for_llm
        file_writer.register_for_llm(description=description)(save_tools)

        return file_writer, description

    @classmethod
    def __create_code_executor(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools, auto_execute_code: bool = False):
        # Separate the code writer and executor. Below is the code executor Agent without LLM.
        description = "Executes the code provided by the code writer."
        code_execution_agent = ConversableAgent(
            "code-execution",
            system_message=f"""
                You are a code executor.
                Execute the code provided by the code writer.
                Display the execution results of the code.
                """,
            llm_config=False,
            code_execution_config={"executor": autogen_pros.create_code_executor()},
            description=description,
            human_input_mode="NEVER",
        )

        # Determine whether the code executor automatically executes the code
        # If self.auto_execute_code == True, set human_input_mode to "NEVER"
        if auto_execute_code:
            code_execution_agent.human_input_mode = "NEVER"
        else:
            code_execution_agent.human_input_mode = "ALWAYS"

        return code_execution_agent, description

    # Enable Vector Searcher
    @classmethod
    def __create_vector_searcher(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        # Vector Searcher
        description = "Searches information from the vector database according to the user's instructions."
        vector_searcher = ConversableAgent(
            "vector-searcher",
            system_message="""
                You are a vector searcher. You search for information from the vector database according to the user's instructions.
                Use the provided function to display the search results.
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # Register the information of available functions to the agent
        vector_search, description = autogen_tools.tools["vector_search"]
        vector_searcher.register_for_llm(description=description)(vector_search)

        return vector_searcher, description

    # Enable File Extractor
    @classmethod
    def __create_file_extractor(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        # File Extractor
        description = "Extracts information from files according to the user's instructions."
        file_extractor = ConversableAgent(
            "file-extractor",
            system_message="""
                You are a file extractor. You extract information from files according to the user's instructions.
                Use the provided function to display the extraction results.
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # Register the information of available functions to the agent
        extract_text_from_file, description = autogen_tools.tools["extract_text_from_file"]
        file_extractor.register_for_llm(description=description)(extract_text_from_file)

        list_files_in_directory, description = autogen_tools.tools["list_files_in_directory"]
        file_extractor.register_for_llm(description=description)(list_files_in_directory)

        return file_extractor, description

    @classmethod
    def __create_web_searcher(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        # Web Searcher
        description = "Searches documents on the web."
        web_searcher = ConversableAgent(
            "azure-document-searcher",
            system_message="""
                You are a web searcher. You search for documents on the web according to the user's instructions.
                - Use the provided search_duckduckgo function to search for information.
                - If the required document is not at the link destination, search for further linked information.
                - If the required document is found, retrieve the document with extract_webpage and provide the text to the user.
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # Search for Azure documents with the specified keywords.
        search_duckduckgo, description = autogen_tools.tools["search_duckduckgo"]
        web_searcher.register_for_llm(description=description)(search_duckduckgo)

        # Retrieve the text and links from the specified URL.
        extract_webpage, description = autogen_tools.tools["extract_webpage"]
        web_searcher.register_for_llm(description=description)(extract_webpage)

        return web_searcher, description

    @classmethod
    def __create_wikipedia_searcher(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        
        # Web Searcher
        description = "Retrieves information from the specified URL."
        wipkipedia_searcher = ConversableAgent(
            "wipkipedia-searcher",
            system_message="""
                Retrieves information from the specified URL according to the user's instructions.
                - Uses the provided function to retrieve text and links from the specified URL.
                - If no URL is provided by the user, searches for relevant pages from the Japanese version of Wikipedia.
                - If necessary documents are not available on the linked page, searches further linked information.
                - Provides the text of the necessary documents to the user if they are available.
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,   
            description=description,
            human_input_mode="NEVER",
        )

        # Register information about available functions to the agent

        # Searches for relevant pages from the Japanese version of Wikipedia.
        search_wikipedia_ja, description = autogen_tools.tools["search_wikipedia_ja"]
        wipkipedia_searcher.register_for_llm(description=description)(search_wikipedia_ja)

        # Retrieves text and links from the specified URL.
        extract_webpage, description = autogen_tools.tools["extract_webpage"]
        wipkipedia_searcher.register_for_llm(description=description)(extract_webpage)

        return wipkipedia_searcher, description

    @classmethod
    def __create_azure_document_searcher(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        
        # Web Searcher
        description = "Searches for Azure-related documents."
        azure_document_searcher = ConversableAgent(
            "azure-document-searcher",
            system_message="""
                You are an Azure document searcher. Searches for Azure-related documents on the web according to user instructions.
                - Uses the provided search_duckduckgo function to search within site:https://learn.microsoft.com/en-us/azure.
                - If necessary documents are not available on the linked page, searches further linked information.
                - Retrieves the necessary documents with extract_webpage and provides the text to the user.
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # Searches for Azure documents with the specified keywords.
        search_duckduckgo, description = autogen_tools.tools["search_duckduckgo"]
        azure_document_searcher.register_for_llm(description=description)(search_duckduckgo)

        # Retrieves text and links from the specified URL.
        extract_webpage, description = autogen_tools.tools["extract_webpage"]
        azure_document_searcher.register_for_llm(description=description)(extract_webpage)

        return azure_document_searcher, description

    @classmethod
    def __create_file_checker(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        
        # File Checker
        description = "Checks whether the specified file exists."
        file_checker = ConversableAgent(
            "file_checker",
            system_message="""
                File Checker: Checks whether the specified file exists.
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # Retrieves text and links from the specified URL.
        func, description = autogen_tools.tools["check_file"]
        file_checker.register_for_llm(description=description)(func)

        return file_checker, description

    # Create an agent to get the current time
    @classmethod
    def __create_current_time(cls, autogen_pros: AutoGenProps, autogen_tools: AutoGenTools):
        # Agent to get the current time
        description = "Retrieves the current time."
        current_time = ConversableAgent(
            "current_time",
            system_message="""
                Retrieves the current time.
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # Register the function to get the current time
        func, description = autogen_tools.tools["get_current_time"]
        current_time.register_for_llm(description=description)(func)

        return current_time, description
