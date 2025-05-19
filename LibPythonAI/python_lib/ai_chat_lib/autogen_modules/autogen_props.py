from typing import Any, Union, Callable
import os
import sys
from io import StringIO
import venv
# json
import json
# Generator
from typing import AsyncGenerator


# autogen
from autogen_ext.models.openai import OpenAIChatCompletionClient, AzureOpenAIChatCompletionClient
from autogen_core.tools import FunctionTool
from autogen_core import CancellationToken
from autogen_agentchat.agents import AssistantAgent, CodeExecutorAgent
from autogen_ext.code_executors.local import LocalCommandLineCodeExecutor
from autogen_agentchat.conditions import MaxMessageTermination, TextMentionTermination, TimeoutTermination
from autogen_agentchat.teams import SelectorGroupChat
from autogen_agentchat.base import TaskResult
from autogen_agentchat.messages import ChatMessage, AgentEvent, TextMessage

from ai_chat_lib.db_modules import VectorDBItem, MainDB, VectorSearchRequest

# openai_props
from ai_chat_lib.openai_modules import OpenAIProps

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)


class AutoGenProps:

    autogen_props_name = "autogen_props"
    @classmethod
    def get_autogen_objects(cls, request_dict: dict) -> "AutoGenProps":
        '''
        {"context": {"autogen_props": {}}}の形式で渡される
        '''
        # AutoGenPropsを生成
        props_dict = request_dict.get(cls.autogen_props_name, None)
        if not props_dict:
            raise ValueError("autogen_props is not set")
        
        # get_openai_objectsを使ってOpenAIPropsを取得
        openai_props, _ = OpenAIProps.get_openai_objects(request_dict)

        # vector_db_itemsを取得
        vector_search_requests = VectorSearchRequest.get_vector_search_requests_objects(request_dict)

        app_db_path = MainDB.get_main_db_path()
        autogen_props = AutoGenProps(app_db_path, props_dict, openai_props, vector_search_requests)
        return autogen_props


    CHAT_TYPE_GROUP_NAME = "group"
    CHAT_TYPE_NORMAL_NAME = "normal"

    session_tokens: dict[str, bool] = {}
    # session_tokenを登録する
    @classmethod
    def register_session_token(cls, session_token: str):
        cls.session_tokens[session_token] = True

    # session_tokenを削除する
    @classmethod
    def remove_session_token(cls, session_token: str) -> bool:
        logger.debug(f"remove_session_token: {session_token}")
        logger.debug(cls.session_tokens)
        if session_token in cls.session_tokens:
            cls.session_tokens.pop(session_token)
            return True
        return False

    def __init__(self, app_db_path: str ,props_dict: dict, openai_props: OpenAIProps, vector_search_requests:list[VectorSearchRequest]):

        # session_token
        self.session_token = props_dict.get("session_token", None)

        # autogen_db_path
        autogen_db_path = app_db_path
        if autogen_db_path is None:
            raise ValueError("autogen_db_path is None")
        self.autogen_db_path = autogen_db_path

        # work_dir
        work_dir = props_dict.get("work_dir", None)
        if work_dir is None:
            raise ValueError("work_dir is None")
        self.work_dir_path = work_dir

        # tool_dir
        self.tool_dir_path = props_dict.get("tool_dir", None)
        if self.tool_dir_path is None:
            raise ValueError("tool_dir is None")

        # venv_path
        self.venv_path = os.getenv("VIRTUAL_ENV", None)

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
        self.vector_search_requests = vector_search_requests

        # main_vector_db
        self.main_vector_db_id = props_dict.get("main_vector_db_id", None)
        if self.main_vector_db_id is None:
            raise ValueError("main_vector_db_id is None")
        
        # chat_history_folder_id
        self.chat_history_folder_id = props_dict.get("chat_history_folder_id", None)

        # default_tools absoulte path
        import ai_chat_lib.autogen_modules.default_tools as default_tools
        self.default_tools_path = default_tools.__file__

        # chat_object
        self.chat_object: Union[SelectorGroupChat, None] = None

        # quit_flag 
        self.quit_flag = False

        # __prepare_autogen_chat
        self.__prepare_autogen_chat()

    def quit(self):
        self.quit_flag = True

    # clear_agents
    def clear_agents(self):
        self.agents = []


    # 指定したnameのGroupChatをDBから取得して、GroupChatを返す
    def __prepare_autogen_chat(self):
        if self.chat_type == self.CHAT_TYPE_NORMAL_NAME:
            self.__prepare_autogen_agent_chat()
        elif self.chat_type == self.CHAT_TYPE_GROUP_NAME:
            self.__prepare_autogen_group_chat()
        else:
            raise ValueError(f"chat_type:{self.chat_type} is not supported")

    def __prepare_autogen_agent_chat(self):

        agent = self.__load_agent(self.chat_name, self.openai_props)
        logger.debug(f"agent:{agent.name}")

        self.chat_object = agent

    def __prepare_autogen_group_chat(self):
        # MainDBを作成
        main_db = MainDB(self.autogen_db_path)
        # chat_objectを取得
        chat_dict = main_db.get_autogen_group_chat(self.chat_name)

        # agent_namesを取得
        agent_names = chat_dict.agent_names
        agents = []
        for agent_name in agent_names:
            agent = self.__load_agent(agent_name, self.openai_props)
            agents.append(agent)

        # vector_search_agentsがある場合は、agentsに追加
        vector_search_agents = self.__create_vector_search_agent_list(self.openai_props, self.vector_db_prop_list)
        agents.extend(vector_search_agents)

        # エージェント名一覧を表示
        for agent in agents:
            logger.debug(f"agent:{agent.name}")

        # termination_conditionを作成
        termination_condition = self.__create_termination_condition(self.terminate_msg, self.max_msg, self.timeout)

        # SelectorGroupChatを作成
        chat = SelectorGroupChat(
            agents, 
            model_client=self.__load_client(chat_dict.llm_config_name), 
            termination_condition=termination_condition,
            )
        
        self.chat_object = chat

    # vector_search_agentsを準備する。vector_db_props_listを受け取り、vector_search_agentsを作成する
    def __create_vector_search_agent_list(self, openai_props: OpenAIProps, vector_search_requests:list[VectorSearchRequest]):
        vector_search_agents = []
        for request in vector_search_requests:
            vector_search_agent = self.__create_vector_search_agent(openai_props, request)
            vector_search_agents.append(vector_search_agent)
        
        return vector_search_agents

    # 指定したopenai_propsとvector_db_propsを使って、VectorSearchAgentを作成する
    def __create_vector_search_agent(self, openai_props: OpenAIProps, vector_search_request: VectorSearchRequest):
        import uuid
        params: dict[str, Any] = {}
        id = str(uuid.uuid4()).replace('-', '_')

        # main_dbを作成
        main_db = MainDB()
        # vector_db_propsを取得
        vector_db_props = main_db.get_vector_db_by_name(vector_search_request.name)
        if vector_db_props is None:
            raise ValueError(f"VectorDBItem not found for name: {vector_search_request.name}")

        params["name"] = f"vector_searcher_{id}"
        params["description"] = vector_db_props.Description
        params["system_message"] = vector_db_props.SystemMessage
        # defaultのllm_config_nameを使って、model_clientを作成
        params["model_client"] = self.__load_client("default")
        # vector_search_toolを作成
        from ai_chat_lib.autogen_modules.vector_db_tools import create_vector_search_tool
        func = create_vector_search_tool(openai_props, [vector_search_request])
        func_tool = FunctionTool(func, description=f"Vector Search Tool for {vector_db_props.Description}", name=f"vector_search_tool_{id}")
        params["tools"] = [func_tool]

        return AssistantAgent(**params)

    # 指定したnameのAgentをDBから取得して、Agentを返す
    def __load_agent(self, name: str, openai_props: OpenAIProps) -> Union[AssistantAgent, CodeExecutorAgent, None]:
        main_db = MainDB(self.autogen_db_path)
        agent_dict = main_db.get_autogen_agent(name)
        if not agent_dict:
            return None
        # ConversableAgent object用の引数辞書を作成
        params: dict[str, Any] = {}
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
            params["model_client"] = self.__load_client(agent_dict.llm_config_name)

            # tool_namesが指定されている場合は、tool_dictを作成
            tool_dict_list = []
            for tool_name in agent_dict.tool_names.split(","):
                if not tool_name:
                    continue
                logger.debug(f"tool_name:{tool_name}")
                func_tool = self.__create_tool(tool_name)
                tool_dict_list.append(func_tool)
            # vector_db_itemsが指定されている場合は、vector_db_items用のtoolを作成
            # vector_search_toolを作成
            from ai_chat_lib.autogen_modules.vector_db_tools import create_vector_search_tool
            import uuid
            for vector_db_item in json.loads(agent_dict.vector_db_items):
                id = str(uuid.uuid4()).replace('-', '_')
                func = create_vector_search_tool(openai_props, [vector_db_item])
                vector_db_props = VectorDBItem(vector_db_item)
                func_tool = FunctionTool(
                    func, description=f"Vector Search Tool for {vector_db_props.Description}", 
                    name=f"vector_search_tool_{id}",
                    global_imports=[" from typing import Annotated"]
                    )
                tool_dict_list.append(func_tool)

            params["tools"] = tool_dict_list
            return AssistantAgent(**params)

    def load_function(self,file_path:str, name: str) -> Callable:
        """
        指定されたファイルから関数を読み込む
        引数:
        - file_path: ファイルパス
        - name: 関数名
        戻り値: 読み込んだ関数
        """
        
        # importlibで{name}を読み込む
        import importlib.util
        spec = importlib.util.spec_from_file_location(name, file_path) # type: ignore
        module = importlib.util.module_from_spec(spec) # type: ignore
        spec.loader.exec_module(module) # type: ignore
        func = getattr(module, name)
        # func内でautogen_propsを使えるようにする
        func.__globals__["autogen_props"] = self

        return func
    def __create_tool(self, name: str):
        main_db = MainDB(self.autogen_db_path)
        tool_dict = main_db.get_autogen_tool(name)
        if not tool_dict:
            raise ValueError (f"Tool {name} not found in the database.")
 
        # nameの関数を取得
        func = self.load_function(tool_dict.path, tool_dict.name)

        return FunctionTool(
            func, description=tool_dict.description, 
            name=tool_dict.name,
            global_imports=[" from typing import Annotated"]
         )
    
    # 指定したnameのLLMConfigをDBから取得して、llm_configを返す    
    def __load_client(self, name: str) -> Union[OpenAIChatCompletionClient, AzureOpenAIChatCompletionClient]:
        main_db = MainDB(self.autogen_db_path)
        llm_config_entry = main_db.get_autogen_llm_config(name)
        if not llm_config_entry:
            raise ValueError (f"LLMConfig {name} not found in the database.")

        client: Union[OpenAIChatCompletionClient , AzureOpenAIChatCompletionClient, None] = None
        parameters:dict[str, Any] = {}
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
            # logger.debug(f"autogen llm_config parameters:{parameters}")
            client = AzureOpenAIChatCompletionClient(**parameters)
        else:
            # base_urlが指定されている場合は、parametersのbase_urlにbase_urlを設定
            if llm_config_entry.base_url != "":
                parameters["base_url"] = llm_config_entry.base_url
    
            # logger.debug(f"autogen llm_config parameters:{parameters}")
            client = OpenAIChatCompletionClient(**parameters)

        return client

    def __create_code_executor(self):
        params = {}
        params["work_dir"] = self.work_dir_path
        logger.debug(f"work_dir_path:{self.work_dir_path}")
        if self.venv_path:
            env_builder = venv.EnvBuilder(with_pip=True)
            virtual_env_context = env_builder.ensure_directories(self.venv_path)
            params["virtual_env_context"] = virtual_env_context
            logger.debug(f"venv_path:{self.venv_path}")
            
        # Create a local command line code executor.
        executor = LocalCommandLineCodeExecutor(
            **params
        )
        return executor

    def __create_termination_condition(self, termination_msg: str, max_msg: int, timeout: int):
        from functools import reduce
        termination_list: list = []
        # Define termination condition
        if termination_msg:
            termination_list.append(TextMentionTermination(termination_msg))
        if max_msg > 0:
            termination_list.append(MaxMessageTermination(max_messages=max_msg))
        if timeout > 0:
            termination_list.append(TimeoutTermination(timeout))
        # Combine termination conditions using | operator
        if termination_list:
            combined_termination = reduce(lambda x, y: x | y, termination_list)
        else:
            combined_termination = None  # or some default termination condition if needed

        return combined_termination

    # create_agent
    def create_agent(self,
            name: str, description: str, system_message:str, 
            tools: list[FunctionTool] = [], handoffs=[] ) -> AssistantAgent:
        # AssistantAgentの引数用の辞書を作成
        params: dict[str, Any] = {}
        params["name"] = name
        params["description"] = description

        # code_executionがFalseの場合は、AssistantAgentを作成
        params["system_message"] = system_message
        # llm_config_nameが指定されている場合は、llm_config_dictを作成
        params["model_client"] = self.__load_client("default")
        if len(tools) > 0:
            params["tools"] = tools
        if len(handoffs) > 0:
            params["handoffs"] = handoffs

        return AssistantAgent(**params)    

    # 指定した名前のエージェントを実行する
    async def run_agent(self, agent_name: str, initial_message: str) -> AsyncGenerator:
        # agent_nameのAgentを作成
        agent = self.__load_agent(agent_name, self.openai_props)
        if agent is None:
            raise ValueError(f"Agent {agent_name} not found in the database.")

        # session_tokenを登録
        AutoGenProps.register_session_token(self.session_token)
        cancel_token: CancellationToken = CancellationToken()
        async for message in agent.run_stream(task=initial_message, cancellation_token=cancel_token):
            # session_tokensにsesson_tokenがない場合は、処理を中断
            if AutoGenProps.session_tokens.get(self.session_token) is None:
                logger.debug("request cancel")
                cancel_token.cancel()    
                break
            if type(message) == TaskResult:
                break
            if type(message) == ChatMessage or type(message) == AgentEvent or type(message) == TextMessage:
                message_str = f"{message.source}: {message.content}" # type: ignore
                yield message_str
    
    # 指定したinitial_messageを使って、GroupChatを実行する
    async def run_autogen_chat(self, initial_message: str) -> AsyncGenerator:
        if self.chat_object is None:
            raise ValueError("chat_object is None")

        # session_tokenを登録
        AutoGenProps.register_session_token(self.session_token)
        cancel_token: CancellationToken = CancellationToken()

        async for message in self.chat_object.run_stream(task=initial_message, cancellation_token=cancel_token):
            # session_tokensにsesson_tokenがない場合は、処理を中断
            if AutoGenProps.session_tokens.get(self.session_token) is None:
                logger.debug("request cancel")
                cancel_token.cancel()    
                break
            if type(message) == TaskResult:
                break
            if type(message) == ChatMessage or type(message) == AgentEvent or type(message) == TextMessage:
                message_str = f"{message.source}: {message.content}" # type: ignore
                yield message_str

    chat_request_name = "chat_request"
    @classmethod
    async def autogen_chat_api(cls, request_json: str) -> AsyncGenerator:

        result = None # 初期化
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        autogen_props = AutoGenProps.get_autogen_objects( request_dict)
        # chat_requestを取得
        chat_request_dict = request_dict.get(cls.chat_request_name, None)
        if not chat_request_dict:
            raise ValueError("chat_request is not set")
        
        # chat_request_dictのmessagesを取得
        messages = chat_request_dict.get("messages", None)
        if not messages:
            raise ValueError("messages is not set")
        # messagesのうち、role == userの最後の要素を取得
        last_message = [message for message in messages if message.get("role") == "user"][-1]
        # last_messageのcontent(リスト)を取得
        content_list = last_message.get("content", None)
        if not content_list:
            raise ValueError("content is not set")
        # content_listの要素の中で、typeがtextのものを取得
        text_list = [content for content in content_list if content.get("type") == "text"]
        # text_listの要素を結合 
        input_text = "\n".join([content.get("text") for content in text_list])
        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer

        # run_group_chatを実行
        async for message in autogen_props.run_autogen_chat(input_text):
            if not message:
                break
            # dictを作成
            result = {"message": message }
            # resultにlogを追加して返す
            result["log"] = buffer.getvalue()
            json_string = json.dumps(result, ensure_ascii=False, indent=4)
            # bufferをクリア
            buffer.truncate(0)
        
            yield json_string

        # strout,stderrorを元に戻す
        sys.stdout = sys.__stdout__
        sys.stderr = sys.__stderr__