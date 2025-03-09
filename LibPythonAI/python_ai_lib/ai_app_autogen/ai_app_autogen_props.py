import venv
from typing import Any
# queue
from queue import Queue, Empty
# asyncio
import asyncio
# json
import json
# sqlite3
import sqlite3
# time
import time
# Generator
from typing import Generator

# autogen
from autogen_ext.models.openai import OpenAIChatCompletionClient, AzureOpenAIChatCompletionClient
from autogen_core.tools import FunctionTool
from autogen_agentchat.agents import AssistantAgent, CodeExecutorAgent
from autogen_ext.code_executors.local import LocalCommandLineCodeExecutor
from autogen_agentchat.conditions import MaxMessageTermination, TextMentionTermination, TimeoutTermination
from autogen_agentchat.teams import SelectorGroupChat
from autogen_agentchat.base import TaskResult

# vector_db_props
from ai_app_langchain import VectorDBProps

# openai_props
from ai_app_openai import OpenAIProps

# main_db
from main_db import MainDB, AutogenAgent, AutogenGroupChat, AutogenTools, AutogentLLMConfig

class AutoGenProps:

    CHAT_TYPE_GROUP_NAME = "group"
    CHAT_TYPE_NORMAL_NAME = "normal"

    def __init__(self, props_dict: dict, openai_props: OpenAIProps, vector_db_prop_list:list[VectorDBProps]):

        # autogen_db_path
        autogen_db_path = props_dict.get("autogen_db_path", None)
        if autogen_db_path is None:
            raise ValueError("autogen_db_path is None")
        self.autogen_db_path = autogen_db_path

        # work_dir
        work_dir = props_dict.get("work_dir", None)
        if work_dir is None:
            raise ValueError("work_dir is None")
        self.work_dir_path = work_dir

        # venv_path
        self.venv_path = props_dict.get("venv_path", None)

        # chat_tpe
        self.chat_type = props_dict.get("chat_type", None)
        
        # chat_name
        self.chat_name = props_dict.get("chat_name", None)

        # terminate_msg
        self.terminate_msg = props_dict.get("terminate_msg", "TERMINATE")

        # max_msg
        self.max_msg = props_dict.get("max_msg", 15)

        # timeout
        self.timeout = props_dict.get("timeout", 120)

        # openai_props
        self.openai_props = openai_props

        # vector_db_prop_list
        self.vector_db_prop_list = vector_db_prop_list

        # default_tools absoulte path
        import ai_app_autogen.default_tools as default_tools
        self.default_tools_path = default_tools.__file__

        # chat_object
        self.chat_object: SelectorGroupChat = None

        self.quit_flag = False

        # __prepare_autogen_chat
        self.__prepare_autogen_chat()

    def quit(self):
        self.quit_flag = True

    # clear_agents
    def clear_agents(self):
        self.agents = []

    # 指定した名前のエージェントを実行する
    def run_agent(self, agent_name: str, input_text: str, result_queue: Queue) -> Generator:
        # agent_nameのAgentを作成
        agent = self.__create_agent(agent_name, self.openai_props)
        task = self.__create_run_autogen_chat_task(agent, input_text, result_queue)

        # agentを実行
        import threading

        def run_task():
            try:
                asyncio.run(task)
            except Exception as e:
                print(f"run_task error:{e}")
                self.quit()
                raise e

        thread = threading.Thread(target=run_task)
        thread.start()

        # _get_messageを使って、result_queueからメッセージを取得して、Generatorを返す
        try:
            yield from self._get_message(result_queue)
        except Exception as e:
            self.quit()
            raise e
    
    # 指定したinitial_messageを使って、GroupChatを実行する
    def run_autogen_chat(self, initial_message: str, result_queue: Queue) -> Generator:
        task = self.__create_run_autogen_chat_task(self.chat_object, initial_message, result_queue)

        import threading

        def run_task():
            try:
                asyncio.run(task)
            except Exception as e:
                print(f"run_task error:{e}")
                self.quit()
                raise e

        thread = threading.Thread(target=run_task)
        thread.start()

        # _get_messageを使って、result_queueからメッセージを取得して、Generatorを返す
        try:
            yield from self._get_message(result_queue)
        except Exception as e:
            self.quit()
            raise e

    # 指定したnameのGroupChatをDBから取得して、GroupChatを返す
    def __prepare_autogen_chat(self):
        if self.chat_type == self.CHAT_TYPE_NORMAL_NAME:
            self.__prepare_autogen_agent_chat()
        elif self.chat_type == self.CHAT_TYPE_GROUP_NAME:
            self.__prepare_autogen_group_chat()
        else:
            raise ValueError(f"chat_type:{self.chat_type} is not supported")

    def __prepare_autogen_agent_chat(self):

        agent = self.__create_agent(self.chat_name, self.openai_props)
        print(f"agent:{agent.name}")

        self.chat_object = agent

    def __prepare_autogen_group_chat(self):
        # MainDBを作成
        main_db = MainDB(self.autogen_db_path)
        # chat_objectを取得
        chat_dict = main_db.get_autogen_group_chat(self.chat_name)

        # agent_namesを取得
        agent_names = chat_dict.agent_names.split(",")
        agents = []
        for agent_name in agent_names:
            agent = self.__create_agent(agent_name, self.openai_props)
            agents.append(agent)

        # vector_search_agentsがある場合は、agentsに追加
        vector_search_agents = self.__create_vector_search_agent_list(self.openai_props, self.vector_db_prop_list)
        agents.extend(vector_search_agents)

        # エージェント名一覧を表示
        for agent in agents:
            print(f"agent:{agent.name}")

        # termination_conditionを作成
        termination_condition = self.__create_termination_condition(self.terminate_msg, self.max_msg, self.timeout)

        # SelectorGroupChatを作成
        chat = SelectorGroupChat(
            agents, 
            model_client=self.__create_client(chat_dict.llm_config_name), 
            termination_condition=termination_condition,
            )
        
        self.chat_object = chat


    def _get_message(self, result_queue: Queue) -> Generator:
        while True:
            retry_count = 0
            while retry_count < 10:
                if self.quit_flag:
                    self.quit_flag = False
                    return None
                try:
                    message = result_queue.get(block=True, timeout=5)
                    if message:
                        yield message
                    else:
                        return None
                except Empty:
                    retry_count += 1
                    if retry_count > 10:
                        return None
                    

    async def __create_run_autogen_chat_task(self, chat_object: SelectorGroupChat, initial_message: str, result_queue: Queue):
        if not initial_message:
            raise ValueError("initial_message is None")
        if not result_queue:
            raise ValueError("result_queue is None")
        if not self.chat_object:
            raise ValueError("chat_object is not prepared")
        
        async for message in chat_object.run_stream(task=initial_message):
            if self.quit_flag:
                self.quit_flag = False
                return
            
            if type(message) == TaskResult:
                result_queue.put(None)
                break
            message_str = f"{message.source}: {message.content}"
            result_queue.put(message_str)
            # result_queue.put(message)

    # vector_search_agentsを準備する。vector_db_props_listを受け取り、vector_search_agentsを作成する
    def __create_vector_search_agent_list(self, openai_props: OpenAIProps, vector_db_prop_list:list[VectorDBProps]):
        vector_search_agents = []
        for vector_db_props in vector_db_prop_list:
            vector_search_agent = self.__create_vector_search_agent(openai_props, vector_db_props)
            vector_search_agents.append(vector_search_agent)
        
        return vector_search_agents

    # 指定したopenai_propsとvector_db_propsを使って、VectorSearchAgentを作成する
    def __create_vector_search_agent(self, openai_props: OpenAIProps, vector_db_props: VectorDBProps):
        import uuid
        params = {}
        id = str(uuid.uuid4()).replace('-', '_')

        params["name"] = f"vector_searcher_{id}"
        params["description"] = vector_db_props.Description
        params["system_message"] = vector_db_props.SystemMessage
        # defaultのllm_config_nameを使って、model_clientを作成
        params["model_client"] = self.__create_client("default")
        # vector_search_toolを作成
        from ai_app_autogen.vector_db_tools import create_vector_search_tool
        func = create_vector_search_tool(openai_props, [vector_db_props])
        func_tool = FunctionTool(func, description=f"Vector Search Tool for {vector_db_props.Description}", name=f"vector_search_tool_{id}")
        params["tools"] = [func_tool]

        return AssistantAgent(**params)

    # 指定したnameのAgentをDBから取得して、Agentを返す
    def __create_agent(self, name: str, openai_props: OpenAIProps):
        main_db = MainDB(self.autogen_db_path)
        agent_dict = main_db.get_autogen_agent(name)
        # ConversableAgent object用の引数辞書を作成
        params = {}
        params["name"] = agent_dict.name
        params["description"] = agent_dict.description

        # code_executionがTrueの場合は、CodeExecutionAgentを作成
        if agent_dict.code_execution:
            code_executor = self.__create_code_executor()
            params["code_executor"] = code_executor
            return CodeExecutorAgent(**params)

        else:
            # code_executionがFalseの場合は、AssistantAgentを作成
            params["system_message"] = agent_dict.system_message
            # llm_config_nameが指定されている場合は、llm_config_dictを作成
            params["model_client"] = self.__create_client(agent_dict.llm_config_name)

            # tool_namesが指定されている場合は、tool_dictを作成
            tool_dict_list = []
            if agent_dict.tool_names:
                tool_names = agent_dict.tool_names.split(",")
                for tool_name in tool_names:
                    print(f"tool_name:{tool_name}")
                    func_tool = self.__create_tool(tool_name)
                    tool_dict_list.append(func_tool)
            # vector_db_itemsが指定されている場合は、vector_db_items用のtoolを作成
            # vector_search_toolを作成
            from ai_app_autogen.vector_db_tools import create_vector_search_tool
            import uuid
            for vector_db_item in json.loads(agent_dict.vector_db_items_json):
                id = str(uuid.uuid4()).replace('-', '_')
                func = create_vector_search_tool(openai_props, [vector_db_item])
                func_tool = FunctionTool(func, description=f"Vector Search Tool for {vector_db_item.Description}", name=f"vector_search_tool_{id}")
                tool_dict_list.append(func_tool)
            params["tools"] = tool_dict_list
            return AssistantAgent(**params)

    def __create_tool(self, name: str):
        main_db = MainDB(self.autogen_db_path)
        tool_dict = main_db.get_autogen_tool(name)

        # source_pathからファイルを読み込む
        with open(tool_dict.path, "r", encoding="utf-8") as f:
            content = f.read()
        locals_copy = {}
        globals_copy = {}
        globals_copy["autogen_props"] = self
        # contentから関数オブジェクトを作成する。
        exec(content, globals_copy, locals_copy)

        # nameの関数を取得
        func = locals_copy[name]

        return FunctionTool(func, description=tool_dict.description, name=tool_dict.name)
    
    # 指定したnameのLLMConfigをDBから取得して、llm_configを返す    
    def __create_client(self, name: str):
        main_db = MainDB(self.autogen_db_path)
        llm_config_entry = main_db.get_autogen_llm_config(name)
 
        client = None
        parameters = {}
        parameters["api_key"] = llm_config_entry.api_key
        # parametersのmodelにmodelを設定
        parameters["model"] = llm_config_entry.model
        if llm_config_entry.api_type == "azure":
            # parametersのapi_versionにapi_versionを設定
            parameters["api_version"] = llm_config_entry.api_version
            # parametersのazure_endpointにbase_urlを設定
            parameters["azure_endpoint"] = llm_config_entry.base_url
            # parametersのazure_deploymentにmodelを設定
            parameters["azure_deployment"] = llm_config_entry.model
            # print(f"autogen llm_config parameters:{parameters}")
            client = AzureOpenAIChatCompletionClient(**parameters)
        else:
            # base_urlが指定されている場合は、parametersのbase_urlにbase_urlを設定
            if llm_config_entry.base_url != "":
                parameters["base_url"] = llm_config_entry.base_url
    
            # print(f"autogen llm_config parameters:{parameters}")
            client = OpenAIChatCompletionClient(**parameters)

        return client

    def __create_code_executor(self):
        params = {}
        params["work_dir"] = self.work_dir_path
        print(f"work_dir_path:{self.work_dir_path}")
        if self.venv_path:
            env_builder = venv.EnvBuilder(with_pip=True)
            virtual_env_context = env_builder.ensure_directories(self.venv_path)
            params["virtual_env_context"] = virtual_env_context
            print(f"venv_path:{self.venv_path}")
            
        # Create a local command line code executor.
        executor = LocalCommandLineCodeExecutor(
            **params
        )
        return executor

    def __create_termination_condition(self, termination_msg: str, max_msg: int, timeout: int):
        # Define termination condition
        max_msg_termination = MaxMessageTermination(max_messages=max_msg)
        text_termination = TextMentionTermination(termination_msg)
        time_terminarion = TimeoutTermination(timeout)
        combined_termination = max_msg_termination | text_termination | time_terminarion
        return combined_termination