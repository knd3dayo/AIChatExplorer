import threading
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from collections.abc import Generator
from typing import Any
import queue

from ai_app_autogen.ai_app_autogen_props import AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgentGenerator, AutoGenAgentWrapper
from ai_app_autogen.ai_app_autogen_tools import AutoGenToolGenerator, AutoGenToolWrapper
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps

class AutoGenGroupChat:
    def __init__(self, autogen_props: AutoGenProps , vector_db_items: list[VectorDBProps] = []):
        self.autogen_props = autogen_props
        # group_chat_dict
        group_chat_dict = autogen_props.group_chat_dict
        
        # name
        self.name = group_chat_dict.get("name", None)
        if self.name is None:
            raise ValueError("name is None")

        # description
        self.description = group_chat_dict.get("description", None)
        if self.description is None:
            raise ValueError("description is None")

        # init_agent_name
        self.init_agent_name = group_chat_dict.get("init_agent_name", None)
        if self.init_agent_name is None:
            raise ValueError("init_agent_name is None")

        # agent_names
        self.agent_names =  group_chat_dict.get("agent_names", [])

        agent_names_string = ",".join(self.agent_names)
        print (f"agent_names: {agent_names_string}")
        
        self.print_messages_function = self.create_print_messages_function()

        # 途中のメッセージを格納するキュー
        self.message_queue = queue.Queue()
        # 終了フラグ
        self.finished = False

        # autogen_propsからtoolsを取得
        self.tool_wrappers: list[AutoGenToolWrapper] = AutoGenToolWrapper.create_wrapper_list(autogen_props.tools_list)
        # tool_wrappersにデフォルトのツールを追加
        default_tool_wrappers = AutoGenToolGenerator.create_default_tools()
        
        # VectorDBPropsがある場合はAutoGenToolGeneratorのcreate_vector_search_toolsを呼び出す
        if len(vector_db_items) > 0:
            default_tool_wrappers += AutoGenToolGenerator.create_vector_search_tools(self.autogen_props.openai_props, vector_db_items)

        # 既に追加されている場合は追加しない
        for default_tool_wrapper in default_tool_wrappers:
            if default_tool_wrapper not in self.tool_wrappers:
                self.tool_wrappers.append(default_tool_wrapper)

        # autogen_propsからagentsを取得
        self.agent_wrappers: list[AutoGenAgentWrapper] = AutoGenAgentWrapper.create_wrapper_list(autogen_props.agents_list)
        # agent_wrappersにデフォルトのエージェントを追加
        default_agent_wrappers = AutoGenAgentGenerator.create_default_agents(self.autogen_props, self.tool_wrappers)

        # 既に追加されている場合は追加しない
        for default_agent_wrapper in default_agent_wrappers:
            if default_agent_wrapper not in self.agent_wrappers:
                self.agent_wrappers.append(default_agent_wrapper)

        # VectorDBPropsがある場合はAutoGenAgentGeneratorのcreate_vector_search_agentsを呼び出す
        self.vector_search_agents = []
        if len(vector_db_items) > 0:
            self.vector_search_agents = AutoGenAgentGenerator.create_vector_search_agents(vector_db_items)

        # エージェントのリストを生成
        self.agents: list[ConversableAgent] = [agent.create_agent(self.autogen_props, self.tool_wrappers) for agent in self.agent_wrappers if agent.name in self.agent_names]

        # ベクトルDB検索用のagentを追加
        for vector_search_agent in self.vector_search_agents:
            self.agents.append(vector_search_agent.create_agent(self.autogen_props, self.tool_wrappers))
        
        # init_agent_nameに一致するエージェントを取得
        init_agent_wrappers = [agent for agent in self.agent_wrappers if agent.name == self.init_agent_name]

        if len(init_agent_wrappers) == 0:
            raise ValueError(f"init_agent not found: {self.init_agent_name}")

        self.init_agent: ConversableAgent = init_agent_wrappers[0].create_agent(self.autogen_props, self.tool_wrappers)

        # register_reply 
        for agent in self.agents:
            agent.register_reply([autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # agentのリストを表示
        for agent in self.agents:
            print(f"agent:{agent.name} is ready.")



    def run_group_chat(self, initial_message: str, vector_db_items: list[VectorDBProps], max_round: int) -> Generator[Any, None, None]:

        # グループチャットを開始
        groupchat = autogen.GroupChat(
            admin_name="chat_admin_agent",
            agents=self.agents,
            messages=[],
            max_round=max_round,
        )

        group_chat_manager = autogen.GroupChatManager(
            groupchat=groupchat
        )

        def start_chat():
            try:
                self.init_agent.initiate_chat(group_chat_manager, message=initial_message, max_turns=3)
            except Exception as e:
                print(f"Error: {e}")
                import traceback
                traceback.print_exc()
            finally:
                self.finish()

        thread = threading.Thread(target=start_chat)
        thread.start()
        # thread.join()  # スレッドが完了するまで待機

        return self.get_messages()

    # キューからメッセージを取得 yiled で返す
    def get_messages(self) -> Generator[Any, None, None]:
        while True:
            try:
                if self.finished:
                    print("finished")
                    break
                message = self.message_queue.get(timeout=1)
                yield message, False
            except queue.Empty:
                continue
        
        return None, True

    # キューから取り出したメッセージを表示
    def print_messages(self):
        for message, is_last_message in self.get_messages():
            if message is None:
                break
            print(message)

    def create_print_messages_function(self):
        def print_messages(recipient, messages, sender, config): 
            if "callback" in config and  config["callback"] is not None:
                callback = config["callback"]
                callback(sender, recipient, messages[-1])

            # Print the messages in the group chat.
            # roleがuserまたはassistantの場合はrole, name, contentを表示
            message = messages[-1]
            header = f"role:[{message['role']}] name:[{message['name']}]\n------------------------------------------\n"
            content = f"{message['content']}\n"
            if message["role"] in ["user", "assistant"]:
                response = f"Messages sent to: {recipient.name} | num messages: {len(messages)}\n{header}{content}"
            else:
                response = f"Messages sent to: {recipient.name} | num messages: {len(messages)}\n{header}" 
            # queueにresponseを追加
            self.message_queue.put(response)

            return False, None  # required to ensure the agent communication flow continues
        
        return print_messages

    # エージェントの終了処理
    def finish(self):
        self.finished = True
        self.message_queue.put(None)


    @classmethod
    def _create_group_chat_dict(cls, name: str, description: str, init_agent_name: str, agent_names: list[str]) -> dict:
        group_chat_dict = {
            "name": name,
            "description": description,
            "init_agent_name": init_agent_name,
            "agent_names": agent_names
        }
        return group_chat_dict


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


class AutoGenGroupChatGenerator:
    @classmethod
    def create_default_chats(cls, autogen_props: AutoGenProps, agent_wrappers: list[AutoGenAgentWrapper]) -> list[AutoGenGroupChat]:
        agent_names = [agent_wrapper.name for agent_wrapper in agent_wrappers]
        default_group_chat = {
            "name": "default",
            "description": "default",
            "init_agent_name": "user_proxy",
            "agent_names": agent_names
        }
        vector_search_group_chat = {
            "name": "vector_search",
            "description": "vector_search",
            "init_agent_name": "user_proxy",
            "agent_names": ["user_proxy", "planner"]
        }

        return [default_group_chat, vector_search_group_chat]
