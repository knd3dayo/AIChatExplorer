import threading
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from collections.abc import Generator
from typing import Any
import queue

from ai_app_autogen.ai_app_autogen_props import AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgentGenerator, AutoGentAgentWrapper
from ai_app_autogen.ai_app_autogen_tools import AutoGenToolGenerator, AutoGenToolWrapper


class AutoGenGroupChat:
    def __init__(self, autogen_props: AutoGenProps ):
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
        agent_name_string = group_chat_dict.get("agent_names", "")
        self.agent_names = []
        if agent_name_string != "":
            self.agent_names = agent_name_string.split(",")

        print (f"agent_names: {self.agent_names}")
        
        self.print_messages_function = self.create_print_messages_function()

        # 途中のメッセージを格納するキュー
        self.message_queue = queue.Queue()
        # 終了フラグ
        self.finished = False

        # autogen_propsからtoolsを取得
        self.tool_wrappers: list[AutoGenToolWrapper] = AutoGenToolWrapper.create_wrapper_list(autogen_props.tools_list)
        # tool_wrappersにデフォルトのツールを追加
        default_tool_wrappers = AutoGenToolGenerator.create_default_tools(self.autogen_props)
        # 既に追加されている場合は追加しない
        for default_tool_wrapper in default_tool_wrappers:
            if default_tool_wrapper not in self.tool_wrappers:
                self.tool_wrappers.append(default_tool_wrapper)

        # autogen_propsからagentsを取得
        self.agent_wrappers: list[AutoGentAgentWrapper] = AutoGentAgentWrapper.create_wrapper_list(autogen_props.agents_list)
        # agent_wrappersにデフォルトのエージェントを追加
        default_agent_wrappers = AutoGenAgentGenerator.create_default_agents(self.autogen_props, self.tool_wrappers)
        # 既に追加されている場合は追加しない
        for default_agent_wrapper in default_agent_wrappers:
            if default_agent_wrapper not in self.agent_wrappers:
                self.agent_wrappers.append(default_agent_wrapper)


    def run_group_chat(self, initial_message: str, max_round: int) -> Generator[Any, None, None]:



        # エージェントのリストを取得
        if len(self.agent_names) > 0:
            # agent_wrappersの中からagent_namesに含まれていないエージェントを削除
            agent_wrappers = [agent for agent in agent_wrappers if agent.name in self.agent_names]

        # エージェントのリストを生成
        agents: list[ConversableAgent] = [agent.create_agent(self.autogen_props, tool_wrappers) for agent in agent_wrappers]

        # init_agent_nameに一致するエージェントを取得
        init_agent_wrappers = [agent for agent in agent_wrappers if agent.name == self.init_agent_name]

        if len(init_agent_wrappers) == 0:
            raise ValueError(f"init_agent not found: {self.init_agent_name}")

        init_agent: ConversableAgent = init_agent_wrappers[0].create_agent(self.autogen_props, tool_wrappers)

        # register_reply 
        for agent in agents:
            agent.register_reply([autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # グループチャットを開始
        groupchat = autogen.GroupChat(
            admin_name="chat_admin_agent",
            agents=agents,
            messages=[],
            max_round=max_round,
        )

        group_chat_manager = autogen.GroupChatManager(
            groupchat=groupchat
        )

        def start_chat():
            try:
                init_agent.initiate_chat(group_chat_manager, message=initial_message, max_turns=3)
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
            if self.finished:
                break
            try:
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

class AutoGenGroupChatGenerator:
    @classmethod
    def create_default_group_chat(cls, autogen_props: AutoGenProps) -> AutoGenGroupChat:
        return AutoGenGroupChat(autogen_props)

    @staticmethod
    def create_group_chat(autogen_props: AutoGenProps, group_chat_dict: dict) -> AutoGenGroupChat:
        autogen_props.group_chat_dict = group_chat_dict
        return AutoGenGroupChat(autogen_props)
    
    