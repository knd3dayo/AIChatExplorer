import venv
from typing import Any
# queue
from queue import Queue
# asyncio
import asyncio

# sqlite3
import sqlite3
# autogen
from autogen_ext.models.openai import OpenAIChatCompletionClient, AzureOpenAIChatCompletionClient
from autogen_core.tools import FunctionTool
from autogen_agentchat.agents import AssistantAgent, CodeExecutorAgent
from autogen_ext.code_executors.local import LocalCommandLineCodeExecutor
from autogen_agentchat.conditions import MaxMessageTermination, TextMentionTermination, TimeoutTermination
from autogen_agentchat.teams import SelectorGroupChat
from autogen_agentchat.base import TaskResult

# vector_db_props
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps

# openai_props
from ai_app_openai.ai_app_openai_util import OpenAIProps

class AutoGenProps:

    llm_configs_table_name = "llm_configs"
    agents_table_name = "agents"
    tools_table_name = "tools"
    group_chats_table_name = "group_chats"

    def __init__(self, props_dict: dict):

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

        # group_chat
        self.chat_dict = props_dict.get("group_chat", None)

        if self.chat_dict is None:
            raise ValueError("chat_dict is None")
        
        # terminate_msg
        self.terminate_msg = props_dict.get("terminate_msg", "TERMINATE")

        # max_msg
        self.max_msg = props_dict.get("max_msg", 15)

        # timeout
        self.timeout = props_dict.get("timeout", 120)

        # default_tools absoulte path
        import ai_app_autogen.default_tools as default_tools
        self.default_tools_path = default_tools.__file__
        # vector_search_agents
        self.vector_search_agents = []

    # openai_propsとvector_db_props_listを受け取り、vector_search_agentsを作成する
    def prepare_vector_search_agents(self, openai_props: OpenAIProps, vector_db_props_list: list[VectorDBProps]):
        for vector_db_props in vector_db_props_list:
            vector_search_agent = self.__create_vector_search_agent(openai_props, vector_db_props)
            self.vector_search_agents.append(vector_search_agent)

    # vector_search_agentsをクリアする
    def clear_vector_search_agents(self):
        self.vector_search_agents.clear()

    # 指定したinitial_messageを使って、GroupChatを実行する
    def run_group_chat(self, initial_message: str, result_queue: Queue):
        task = self.__create_run_group_chat_task(initial_message, result_queue)
        asyncio.run(task)

    async def __create_run_group_chat_task(self, initial_message: str, result_queue: Queue):
        if not initial_message:
            raise ValueError("initial_message is None")
        group_chat = self.__create_group_chat(self.chat_dict["name"])
        async for message in group_chat.run_stream(task=initial_message):
            if type(message) == TaskResult:
                result_queue.put(None)
                break
            message_str = f"{message.source}: {message.content}"
            result_queue.put(message_str)

    # 指定したnameのGroupChatをDBから取得して、GroupChatを返す
    def __create_group_chat(self, name: str):
        # sqlite3のDBを開く
        conn = sqlite3.connect(self.autogen_db_path)
        cursor = conn.cursor()
        cursor.execute(f"SELECT * FROM {self.group_chats_table_name} WHERE name = ?", (name,))
        row = cursor.fetchone()
        # データが存在しない場合は例外を発生させる
        if row is None:
            raise ValueError(f"GroupChat:{name} is not found.")
        # DBから取得したデータをchat_dictに変換する
        # なお、group_chatsのテーブル定義は以下の通り
        # "CREATE TABLE IF NOT EXISTS group_chats (name TEXT, description TEXT, llm_config_name TEXT, agent_names TEXT)"
        chat_dict = {}
        chat_dict["name"] = row[0]
        chat_dict["description"] = row[1]
        chat_dict["llm_config_name"] = row[2]
        chat_dict["agent_names"] = row[3]
        conn.close()

        # agent_namesを取得
        agent_names = chat_dict["agent_names"].split(",")
        agents = []
        for agent_name in agent_names:
            agent = self.__create_agent(agent_name)
            agents.append(agent)

        # self.vector_search_agentsがある場合は、agentsに追加
        if self.vector_search_agents:
            agents.extend(self.vector_search_agents)

        # エージェント名一覧を表示
        for agent in agents:
            print(f"agent:{agent.name}")

        # termination_conditionを作成
        termination_condition = self.__create_termination_condition(self.terminate_msg, self.max_msg, self.timeout)

        # SelectorGroupChatを作成
        chat = SelectorGroupChat(
            agents, 
            model_client=self.__create_client(chat_dict["llm_config_name"]), 
            termination_condition=termination_condition,
            )
        
        return chat

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
    def __create_agent(self, name: str):
        # sqlite3のDBを開く
        conn = sqlite3.connect(self.autogen_db_path)
        cursor = conn.cursor()
        cursor.execute(f"SELECT * FROM {self.agents_table_name} WHERE name = ?", (name,))
        row = cursor.fetchone()
        # データが存在しない場合は例外を発生させる
        if row is None:
            raise ValueError(f"Agent:{name} is not found.")
        # DBから取得したデータをagent_dictに変換する
        # なお、agentsのテーブル定義は以下の通り
        # "CREATE TABLE IF NOT EXISTS agents (name TEXT PRIMARY KEY, description TEXT, system_message TEXT, code_execution BOOLEAN, llm_config_name TEXT, tool_names TEXT)"
        agent_dict = {}
        agent_dict["name"] = row[0]
        agent_dict["description"] = row[1]
        agent_dict["system_message"] = row[2]
        agent_dict["code_execution"] = row[3]
        agent_dict["llm_config_name"] = row[4]
        agent_dict["tool_names"] = row[5]
        conn.close()
        # ConversableAgent object用の引数辞書を作成
        params = {}
        params["name"] = agent_dict["name"]
        params["description"] = agent_dict["description"]
        # code_executionがTrueの場合は、CodeExecutionAgentを作成
        if agent_dict["code_execution"]:
            code_executor = self.__create_code_executor()
            params["code_executor"] = code_executor
            return CodeExecutorAgent(**params)

        else:
            # code_executionがFalseの場合は、AssistantAgentを作成
            params["system_message"] = agent_dict["system_message"]
            # llm_config_nameが指定されている場合は、llm_config_dictを作成
            params["model_client"] = self.__create_client(agent_dict["llm_config_name"])

            # tool_namesが指定されている場合は、tool_dictを作成
            if agent_dict["tool_names"]:
                tool_names = agent_dict["tool_names"].split(",")
                tool_dict_list = []
                for tool_name in tool_names:
                    print(f"tool_name:{tool_name}")
                    func_tool = self.__create_tool(tool_name)
                    tool_dict_list.append(func_tool)

                params["tools"] = tool_dict_list

            return AssistantAgent(**params)

    def __create_tool(self, name: str):
        # sqlite3のDBを開く
        conn = sqlite3.connect(self.autogen_db_path)
        cursor = conn.cursor()
        cursor.execute(f"SELECT * FROM {self.tools_table_name} WHERE name = ?", (name,))
        row = cursor.fetchone()
        # データが存在しない場合は例外を発生させる
        if row is None:
            raise ValueError(f"Tool:{name} is not found.")
        # DBから取得したデータをtool_dictに変換する
        # なお、toolsのテーブル定義は以下の通り
        # "CREATE TABLE IF NOT EXISTS tools (name TEXT, path TEXT, description TEXT)"
        tool_dict = {}
        tool_dict["name"] = row[0]
        tool_dict["path"] = row[1]
        tool_dict["description"] = row[2]
        conn.close()
        
        print (f"tool_dict:{tool_dict}")

        # source_pathからファイルを読み込む
        with open(tool_dict["path"], "r", encoding="utf-8") as f:
            content = f.read()
        locals_copy = {}
        globals_copy = {}
        globals_copy["autogen_props"] = self
        # contentから関数オブジェクトを作成する。
        exec(content, globals_copy, locals_copy)

        # nameの関数を取得
        tool_dict["func"] = locals_copy[name]

        return FunctionTool(tool_dict["func"], description=tool_dict["description"], name=tool_dict["name"])
    
    # 指定したnameのLLMConfigをDBから取得して、llm_configを返す    
    def __create_client(self, name: str):
        # sqlite3のDBを開く
        conn = sqlite3.connect(self.autogen_db_path)
        cursor = conn.cursor()
        cursor.execute(f"SELECT * FROM {self.llm_configs_table_name} WHERE name = ?", (name,))
        row = cursor.fetchone()
        # データが存在しない場合は例外を発生させる
        if row is None:
            raise ValueError(f"LLMConfig:{name} is not found.")
        # DBから取得したデータをllm_configに変換する
        # なお、llm_configsのテーブル定義は以下の通り
        # CREATE TABLE IF NOT EXISTS llm_configs (name TEXT, api_type TEXT, api_version TEXT, model TEXT, api_key TEXT, base_url TEXT)
        llm_config_entry: dict = {}
        llm_config_entry["api_type"] = row[1]
        llm_config_entry["api_version"] = row[2]
        llm_config_entry["model"] = row[3]
        llm_config_entry["api_key"] = row[4]
        llm_config_entry["base_url"] = row[5]
        conn.close() 

        client = None
        parameters = {}
        parameters["api_key"] = llm_config_entry["api_key"]
        if llm_config_entry["api_type"] == "azure":
            # parametersのapi_versionにapi_versionを設定
            parameters["api_version"] = llm_config_entry["api_version"]
            # parametersのazure_endpointにbase_urlを設定
            parameters["azure_endpoint"] = llm_config_entry["base_url"]
            # parametersのazure_deploymentにmodelを設定
            parameters["azure_deployment"] = llm_config_entry["model"]
            # print(f"autogen llm_config parameters:{parameters}")
            client = AzureOpenAIChatCompletionClient(**parameters)
        else:
            # parametersのmodelにmodelを設定
            parameters["model"] = llm_config_entry["model"]
            # base_urlが指定されている場合は、parametersのbase_urlにbase_urlを設定
            if llm_config_entry["base_url"] != "":
                parameters["base_url"] = llm_config_entry["base_url"]
    
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