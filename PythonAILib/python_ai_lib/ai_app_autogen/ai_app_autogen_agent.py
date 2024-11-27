
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from collections.abc import Generator
from typing import Any, Callable

from ai_app_autogen.ai_app_autogen_tools import AutoGenTools, AutoGenToolGenerator
from ai_app_autogen.ai_app_autogen_props import AutoGenProps
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps

class AutoGenAgents:

    def __init__(self, autogen_props: AutoGenProps, autogen_tools: AutoGenTools, agents_dict: list[dict], auto_execute_code: bool = False):
        
        self.autogen_props = autogen_props
        self.work_dir_path = autogen_props.work_dir_path
        # コード実行者が自動的にコードを実行するようにするかどうか
        # self.auto_execute_code = auto_execute_code
        self.auto_execute_code = True
        
        self.autogen_tools = autogen_tools

        self.agents : dict[str, tuple[ConversableAgent, str]] = {}
        print("Default agents")
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

from autogen import ConversableAgent, UserProxyAgent
from ai_app_autogen.ai_app_autogen_props import AutoGenProps

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
            agent_dict[name] = (agent_obj, description, data)

        # 結果を表示（必要に応じて）
        for name, (agent, desc, _ ) in agent_dict.items():
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
        agents["file_operator"] = cls.__create_file_operator(autogen_props, tools)
        agents["vector_searcher"] = cls.__create_vector_searcher(autogen_props, tools)
        agents["web_searcher"] = cls.__create_web_searcher(autogen_props, tools)
        agents["current_time"] = cls.__create_current_time(autogen_props, tools)
        agents["planner"] = cls.__create_planner(autogen_props, tools)
        agents["critic"] = cls.__create_critic(autogen_props, tools)

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

        # create definition dict
        definition = cls.create_definiton(
            name="user_proxy",
            description=description,
            system_message="",
            type="userproxy",
            human_input_mode="NEVER",
            is_termination_msg="end meeting",
            code_execution_config=False,
            llm_config=True,
            tools=",".join([func.__name__ for func, description in autogen_tools.tools.values()])
        )

        return user_proxy, description, definition

    @classmethod
    def __create_planner(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # Task assigner for group chat
        description = "Planner. Suggest a plan. Revise the plan based on feedback from admin and critic, until admin approval. "
        planner = autogen.AssistantAgent(
            system_message="""Planner. Suggest a plan. Revise the plan based on feedback from admin and critic, 
            until admin approval. 
            """,
            name="planner",
            human_input_mode="NEVER",
            code_execution_config=False,
            description=description,
            llm_config=autogen_pros.create_llm_config()
        )

        # Setting register_from_execution() for planner
        for func, description in tools.values():
            print(f"register_for_execution: {func.__name__}")
            planner.register_for_execution()(func)

        return planner, description

    @classmethod
    def __create_critic(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # Task assigner for group chat
        description = "Critic. Double check plan, claims, code from other agents and provide feedback. Check whether the plan includes adding verifiable info such as source URL."
        critic = autogen.AssistantAgent(
            system_message="""Critic. Double check plan, claims, code from other agents and provide feedback. 
            Check whether the plan includes adding verifiable info such as source URL.
            """,
            name="critic",
            human_input_mode="NEVER",
            code_execution_config=False,
            description=description,
            llm_config=autogen_pros.create_llm_config()
        )

        # Setting register_from_execution() for planner
        for func, description in tools.values():
            print(f"register_for_execution: {func.__name__}")
            critic.register_for_execution()(func)

        return critic, description

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
        # create definition dict
        definition = cls.create_definiton(
            name="code_writer",
            description=description,
            system_message="",
            type="assistant",
            human_input_mode="NEVER",
            is_termination_msg="end meeting",
            code_execution_config=False,
            llm_config=True,
            tools=",".join([func.__name__ for func, description in autogen_tools.tools.values()])
        )

        return code_writer_agent, description, definition


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

        # create definition dict
        definition = cls.create_definiton(
            name="code_executor",
            description=description,
            system_message="",
            type="assistant",
            human_input_mode="NEVER",
            is_termination_msg="end meeting",
            code_execution_config=True,
            llm_config=False,
            tools=",".join([func.__name__ for func, description in autogen_tools.tools.values()])
        )

        return code_execution_agent, description, definition

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

        # create definition dict
        definition = cls.create_definiton(
            name="vector_searcher",
            description=description,
            system_message="",
            type="assistant",
            human_input_mode="NEVER",
            is_termination_msg="end meeting",
            code_execution_config=False,
            llm_config=True,
            tools=",".join([func.__name__ for func, description in autogen_tools.tools.values()])
        )

        return vector_searcher, description, definition

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

        # token消費量が多くなるのでコメントアウト
        # Searches for relevant pages from the Japanese version of Wikipedia.
        # search_wikipedia_ja, description = autogen_tools.tools["search_wikipedia_ja"]
        # web_searcher.register_for_llm(description=description)(search_wikipedia_ja)

        # create definition dict
        definition = cls.create_definiton(
            name="web_searcher",
            description=description,
            system_message="",
            type="assistant",
            human_input_mode="NEVER",
            is_termination_msg="end meeting",
            code_execution_config=False,
            llm_config=True,
            tools=",".join([func.__name__ for func, description in autogen_tools.tools.values()])
        )

        return web_searcher, description, definition

    # Enable File Extractor
    @classmethod
    def __create_file_operator(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # File Extractor
        description = "File operator. Ex. Write text file. Extracts information from files according to the user's instructions."
        file_operator = ConversableAgent(
            "file_operator",
            system_message="""
                You are a file operator. 
                - You extract information from files according to the user's instructions.
                Use the provided function to display the extraction results.
                - Saves data to a file in Python according to the user's instructions.
                The default save location is {autogen_pros.work_dir_path}.
                If the user specifies a save location, save the file to the specified location.
                - list directory files
                - File Checker: Checks whether the specified file exists.
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # Register the information of available functions to the agent
        extract_text_from_file, description = tools["extract_text_from_file"]
        file_operator.register_for_llm(description=description)(extract_text_from_file)

        list_files_in_directory, description = tools["list_files_in_directory"]
        file_operator.register_for_llm(description=description)(list_files_in_directory)

        save_tools, description = tools["save_text_file"]
        # register_for_llm
        file_operator.register_for_llm(description=description)(save_tools)
        # Retrieves text and links from the specified URL.
        func, description = tools["check_file"]
        file_operator.register_for_llm(description=description)(func)

        # create definition dict
        definition = cls.create_definiton(
            name="file_operator",
            description=description,
            system_message="",
            type="assistant",
            human_input_mode="NEVER",
            is_termination_msg="end meeting",
            code_execution_config=False,
            llm_config=True,
            tools=",".join([func.__name__ for func, description in tools.values()]) 
        )

        return file_operator, description, definition

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

        # create definition dict
        definition = cls.create_definiton(
            name="current_time",
            description=description,
            system_message="",
            type="assistant",
            human_input_mode="NEVER",
            is_termination_msg="end meeting",
            code_execution_config=False,
            llm_config=True,
            tools=",".join([func.__name__ for func, description in autogen_tools.tools.values()])
        )

        return current_time, description, definition
