
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from typing import Any, Callable
import uuid
from ai_app_autogen.ai_app_autogen_tools import AutoGenToolWrapper
from ai_app_autogen.ai_app_autogen_props import AutoGenProps
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps

class AutoGenAgentWrapper:

    def __init__(self, name: str, description: str, system_message: str, 
                 type_value: str, human_input_mode: str, termination_msg: str, 
                 code_execution: bool, llm_execution: bool, 
                 tool_names_for_execution: list[str], tool_names_for_llm: list[str], vector_db_props_list: list[VectorDBProps] =[]):
        self.name = name
        self.description = description
        self.system_message = system_message
        self.type_value = type_value
        self.human_input_mode = human_input_mode
        self.termination_msg = termination_msg
        self.code_execution = code_execution
        self.llm_execution = llm_execution
        self.tool_names_for_execution = tool_names_for_execution
        self.tool_names_for_llm = tool_names_for_llm
        self.vector_db_props_list = vector_db_props_list

    def create_agent(self, autogen_props: AutoGenProps, tool_wrappers: list[AutoGenToolWrapper]):
        params = {}
        params['name'] = self.name
        params['description'] = self.description
        params['system_message'] = self.system_message

        params['human_input_mode'] = self.human_input_mode if self.human_input_mode else "NEVER"
        # is_termination_msgが指定されていない場合はNone, それ以外の場合はtermination_msgを設定
        if self.termination_msg:
            params["is_termination_msg"] = lambda msg: self.termination_msg in msg["content"].lower()

        # code_executionを大文字にしたものがTRUEの場合はauto_gen_pros.create_code_executor()を呼び出し
        # それ以外の場合はFalseを設定
        params['code_execution_config'] = autogen_props.create_code_executor_config() if self.code_execution else False
        # llm_configを設定 llmをお大文字にしたものがTRUEの場合はauto_gen_pros.create_llm_config()を呼び出し
        # それ以外の場合はNoneを設定
        params['llm_config'] = autogen_props.create_llm_config() if self.llm_execution else False

        agent = None
        if self.type_value == 'userproxy':
            # userproxyの場合
            agent: UserProxyAgent = UserProxyAgent( **params)
        elif self.type_value == 'assistant':
            # assistantの場合
            agent: ConversableAgent = ConversableAgent( **params)
        else:
            raise ValueError(f"Unknown agent type: {self.type_value}")

    
        for tool_name in self.tool_names_for_execution:
            tool_wrapper = next((tool for tool in tool_wrappers if tool.name == tool_name), None)
            if tool_wrapper:
                agent.register_for_execution(name=tool_wrapper.name)(tool_wrapper.tool)
        
        for tool_name in self.tool_names_for_llm:
            tool_wrapper = next((tool for tool in tool_wrappers if tool.name == tool_name), None)
            if tool_wrapper:
                agent.register_for_llm(name=tool_wrapper.name, description=tool_wrapper.description)(tool_wrapper.tool)


        return agent
        

    @classmethod
    def create_wrapper(cls, data: dict) -> "AutoGenAgentWrapper":
        return cls(
            name=data["name"],
            description=data["description"],
            system_message=data["system_message"],
            type=data["type"],
            human_input_mode=data["human_input_mode"],
            is_termination_msg=data["is_termination_msg"],
            code_execution_config=data["code_execution_config"],
            llm_config=data["llm_config"],
            tools=["tools"]
        )
    @classmethod
    def create_wrapper_list(cls, data: list[dict]) -> list["AutoGenAgentWrapper"]:
        return [cls.create_wrapper(d) for d in data]

    @classmethod
    def create_dict(cls, agent: "AutoGenAgentWrapper") -> dict:
        return {
            "name": agent.name,
            "description": agent.description,
            "system_message": agent.system_message,
            "type_value": agent.type_value,
            "human_input_mode": agent.human_input_mode,
            "termination_msg": agent.termination_msg,
            "code_execution": agent.code_execution,
            "llm_execution": agent.llm_execution,
            "tool_names_for_execution": agent.tool_names_for_execution,
            "tool_names_for_llm": agent.tool_names_for_llm
        }
    
    @classmethod
    def create_dict_list(cls, agents: list["AutoGenAgentWrapper"]) -> list[dict]:
        return [cls.create_dict(agent) for agent in agents]


from autogen import ConversableAgent, UserProxyAgent
from ai_app_autogen.ai_app_autogen_props import AutoGenProps

class AutoGenAgentGenerator:

    # Enable Vector Searcher
    @classmethod
    def create_vector_search_agents(cls, vector_db_props_list: list[VectorDBProps]) -> AutoGenAgentWrapper:
        agent_wrappers = []
        for vector_db_props in vector_db_props_list:
            # Vector Searcher
            name = "vector_searcher_" +  vector_db_props.id
            description = vector_db_props.VectorDBDescription
            system_message=vector_db_props.VectorDBDescription
            agent_wrapper = AutoGenAgentWrapper(
                name=name,
                description=description,
                system_message=system_message,
                type_value="assistant",
                human_input_mode="NEVER",
                termination_msg=None,
                code_execution=False,
                llm_execution=True,
                tool_names_for_execution=[],
                tool_names_for_llm=["vector_search_" + vector_db_props.id],
            )
            agent_wrappers.append(agent_wrapper)
        return agent_wrappers

    @classmethod
    def create_default_agents(cls, autogen_props: AutoGenProps, tool_wrappers: list[AutoGenToolWrapper]) -> list[AutoGenAgentWrapper]:

        agent_wrappers = []
        agent_wrappers.append(cls.__create_user_proxy_wrapper(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_code_executor(autogen_props, tool_wrappers, True))
        agent_wrappers.append(cls.__create_code_writer(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_file_operator(autogen_props, tool_wrappers))
        # vector_searcherは別途実装
        # agent_wrappers.append(cls.__create_vector_searcher(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_web_searcher(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_current_time(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_planner(autogen_props, tool_wrappers))
        # agent_wrappers.append(cls.__create_critic(autogen_props, tool_wrappers))

        return agent_wrappers

    @classmethod
    def create_vector_searcher_agents(cls, vector_db_props_list: list[VectorDBProps]) -> list[AutoGenAgentWrapper]:
        agent_wrappers = []
        for vector_db_props in vector_db_props_list:
            agent_wrappers.append(cls.create_vector_searcher(vector_db_props))
        return agent_wrappers

    @classmethod
    def __create_user_proxy_wrapper(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper: 
        # Task assigner for group chat
        name="user_proxy"
        description = "Creates a list of tasks to achieve the user's request and assigns tasks to each agent."
        system_message="""
            Executes tasks in collaboration with each agent to achieve the user's request.
            - First, create a plan and a list of tasks to achieve the user's request.
            - Assign tasks to each agent and execute the tasks.
            - When the plan is achieved by executing the tasks by each agent, reply with [End Meeting].
            - If there are no additional questions, reply with [End Meeting].
            """
        description=description
        
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="userproxy",
            human_input_mode="NEVER",
            termination_msg="end meeting",
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[tool.name for tool in autogen_tools],
            tool_names_for_llm=[]
        )
        return agent_wrapper

    @classmethod
    def __create_planner(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # Task assigner for group chat
        description = "Planner. Suggest a plan. Revise the plan based on feedback from admin and critic, until admin approval. "
        system_message="""Planner. Suggest a plan. Revise the plan based on feedback from admin and critic, 
        until admin approval. 
        """
        name="planner"
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[tool.name for tool in autogen_tools],
            tool_names_for_llm=[]
        )
        return agent_wrapper

    @classmethod
    def __create_critic(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # Task assigner for group chat
        description = "Critic. Double check plan, claims, code from other agents and provide feedback. Check whether the plan includes adding verifiable info such as source URL."
        system_message="""Critic. Double check plan, claims, code from other agents and provide feedback. 
        Check whether the plan includes adding verifiable info such as source URL.
        """
        name="critic"
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[tool.name for tool in autogen_tools],
            tool_names_for_llm=[]
        )
        return agent_wrapper


    @classmethod
    def __create_code_writer(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # Separate the code writer and executor. Below is the code inference Agent with LLM.
        description = "Creates Python scripts according to the user's instructions."
        name = "code_writer"
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
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=True,
            llm_execution=False,
            tool_names_for_execution=[],
            tool_names_for_llm=[]
        )
        return agent_wrapper


    @classmethod
    def __create_code_executor(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper], auto_execute_code: bool = False) -> AutoGenAgentWrapper:
        # Separate the code writer and executor. Below is the code executor Agent without LLM.
        description = "Executes the code provided by the code writer."
        name = "code_executor"
        system_message=f"""
            You are a code executor.
            Execute the code provided by the code writer.
            Display the execution results of the code.
            """
        if auto_execute_code:
            human_input_mode = "NEVER"
        else:
            human_input_mode = "ALWAYS"

        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode= human_input_mode,
            termination_msg=None,
            code_execution=True,
            llm_execution=False,
            tool_names_for_execution=[],
            tool_names_for_llm=[]
        )
        return agent_wrapper

    @classmethod
    def __create_web_searcher(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # Web Searcher
        description = "Searches documents on the web."
        name = "web_searcher"
        system_message="""
            You are a web searcher. You search for documents on the web according to the user's instructions.
            - Use the provided search_duckduckgo function to search for information.
            - If the required document is not at the link destination, search for further linked information.
            - If the required document is found, retrieve the document with extract_webpage and provide the text to the user.
            """
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[],
            tool_names_for_llm=["search_duckduckgo", "extract_webpage"]
        )

        # token消費量が多くなるのでコメントアウト
        # Searches for relevant pages from the Japanese version of Wikipedia.
        # search_wikipedia_ja, description = autogen_tools.tools["search_wikipedia_ja"]
        # web_searcher.register_for_llm(description=description)(search_wikipedia_ja)

        return agent_wrapper


    # Enable File Extractor
    @classmethod
    def __create_file_operator(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # File Extractor
        description = "File operator. Ex. Write text file. Extracts information from files according to the user's instructions."
        name = "file_operator"
        system_message=f"""
            You are a file operator. 
            - You extract information from files according to the user's instructions.
            Use the provided function to display the extraction results.
            - Saves data to a file in Python according to the user's instructions.
            The default save location is {autogen_pros.work_dir_path}.
            If the user specifies a save location, save the file to the specified location.
            - list directory files
            - File Checker: Checks whether the specified file exists.
            """
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[],
            tool_names_for_llm=["extract_text_from_file", "list_files_in_directory", "save_text_file", "check_file"]
        )

        return agent_wrapper

    # Create an agent to get the current time
    @classmethod
    def __create_current_time(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # Agent to get the current time
        description = "Retrieves the current time."
        name = "current_time"
        system_message="""
            Retrieves the current time.
            """
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[],
            tool_names_for_llm=["get_current_time"]
        )

        return agent_wrapper

