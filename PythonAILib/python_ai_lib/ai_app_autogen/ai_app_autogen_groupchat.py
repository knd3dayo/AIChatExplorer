import threading
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from collections.abc import Generator
from typing import Any
import queue

from ai_app_autogen.ai_app_autogen_client import AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgents, AutoGenAgentGenerator
from ai_app_autogen.ai_app_autogen_tools import AutoGenTools

class AutoGenGroupChat:
    def __init__(self, autogen_props: AutoGenProps):
        self.autogen_props = autogen_props
        group_chat_dict = autogen_props.group_chat_dict
        self.name = group_chat_dict.get("name", "default")
        agent_names_str = group_chat_dict.get("agent_names", "")
        agent_names = agent_names_str.split(",")

        init_agent_name = group_chat_dict.get("init_agent", "user_proxy")


        default_autogen_tools = AutoGenTools(self.autogen_props, autogen_props.tools_list)
        default_autogen_agents = AutoGenAgents(self.autogen_props, default_autogen_tools, autogen_props.agents_list)

        # nameがdefaultの場合はデフォルトのエージェントを使用
        if self.name == "default":
            self.autogen_agents = default_autogen_agents
        else:
            self.autogen_agents = []
            for agent_name in agent_names:
                agent = default_autogen_agents.agents.get(agent_name, None)
                if agent is None:
                    raise ValueError(f"agent not found: {agent_name}")
                self.autogen_agents.append(agent)

        self.init_autogent_agent = self.autogen_agents.agents.get(init_agent_name, None)

        if self.init_autogent_agent is None:
            raise ValueError(f"init_agent not found: {init_agent_name}")

        self.print_messages_function = self.create_print_messages_function()


        # 途中のメッセージを格納するキュー
        self.message_queue = queue.Queue()
        # 終了フラグ
        self.finished = False


    def run_group_chat(self, initial_message: str, max_round: int) -> Generator[Any, None, None]:
        
        init_agent = self.init_autogent_agent[0]
        agents = [agent[0] for agent in self.autogen_agents.agents.values()]
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
            groupchat=groupchat,
            llm_config=self.autogen_agents.autogen_props.create_llm_config(),
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
        self.autogen_agents.finish()
