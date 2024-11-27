
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from typing import Any, Callable

from sample_autogen_tools import AutoGenToolGenerator
from sample_autogen_props import AutoGenProps
        
from sample_autogen_props import AutoGenProps

class AutoGenAgentGenerator:

    @classmethod
    def create_default_agents(cls, autogen_props: AutoGenProps ) -> dict[str, tuple[ConversableAgent, str, dict]]:
        # create ddefault tools
        tools = AutoGenToolGenerator.create_default_tools()
        # Create an instance of AutoGenProps
        agents : dict[str, tuple[ConversableAgent, str, dict]] = {}
        agents["user_proxy"] = cls.__create_user_proxy(autogen_props, tools)
        agents["code_executor"] = cls.__create_code_executor(autogen_props, tools, True)
        agents["code_writer"] = cls.__create_code_writer(autogen_props, tools)
        agents["file_oerator"] = cls.__create_file_operator(autogen_props, tools)
        agents["web_searcher"] = cls.__create_web_searcher(autogen_props, tools)
        agents["current_time"] = cls.__create_current_time(autogen_props, tools)
        agents["planner"] = cls.__create_planner(autogen_props, tools)
        # agents["critic"] = cls.__create_critic(autogen_props, tools)

        return agents

    @classmethod
    def __create_user_proxy(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # Task assigner for group chat
        description = "A human admin. Interact with the planner to discuss the plan. Plan execution needs to be approved by this admin."
        user_proxy = autogen.UserProxyAgent(
            system_message="""
                A human admin. Interact with the planner to discuss the plan. Plan execution needs to be approved by this admin.
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
        for func, description in tools.values():
            print(f"register_for_execution: {func.__name__}")
            user_proxy.register_for_execution()(func)

        return user_proxy, description

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

        return critic, description

    @classmethod
    def __create_code_writer(cls, autogen_pros: AutoGenProps,  tools: dict[str, tuple[Callable, str]]):
        # Separate the code writer and executor. Below is the code inference Agent with LLM.
        description = "Creates Python scripts according to the user's instructions."
        code_writer_agent = ConversableAgent(
            "code_writer",
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
    def __create_code_executor(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]], auto_execute_code: bool = False):
        # Separate the code writer and executor. Below is the code executor Agent without LLM.
        description = "Executes the code provided by the code writer."
        code_execution_agent = ConversableAgent(
            "code_executor",
            system_message=f"""
                You are a code executor.
                Execute the code provided by the code writer.
                Display the execution results of the code.
                """,
            llm_config=False,
            code_execution_config={"executor": autogen_pros.create_code_executor()},
            description=description,
            human_input_mode="ALWAYS",
        )

        # Determine whether the code executor automatically executes the code
        # If auto_execute_code == True, set human_input_mode to "NEVER"
        if auto_execute_code:
            code_execution_agent.human_input_mode = "NEVER"


        return code_execution_agent, description

    # Enable File Extractor
    @classmethod
    def __create_file_operator(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # File Extractor
        description = "File operator. Ex. Write text file. Extracts information from files according to the user's instructions."
        file_operator = ConversableAgent(
            "file_operator",
            system_message=f"""
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
        return file_operator, description

    @classmethod
    def __create_web_searcher(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # Web Searcher
        description = "Searches documents on the web."
        web_searcher = ConversableAgent(
            "web_searcher",
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
        search_duckduckgo, description = tools["search_duckduckgo"]
        web_searcher.register_for_llm(description=description)(search_duckduckgo)

        # Retrieve the text and links from the specified URL.
        extract_webpage, description = tools["extract_webpage"]
        web_searcher.register_for_llm(description=description)(extract_webpage)

        # token消費量が多くなるのでコメントアウト
        # Searches for relevant pages from the Japanese version of Wikipedia.
        # search_wikipedia_ja, description = tools["search_wikipedia_ja"]
        # web_searcher.register_for_llm(description=description)(search_wikipedia_ja)

        return web_searcher, description

    # Create an agent to get the current time
    @classmethod
    def __create_current_time(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
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
        func, description = tools["get_current_time"]
        current_time.register_for_llm(description=description)(func)
        return current_time, description
