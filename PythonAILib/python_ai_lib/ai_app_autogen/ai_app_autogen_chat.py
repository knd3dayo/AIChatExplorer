import threading
from autogen import ConversableAgent
import autogen 
from collections.abc import Generator
from typing import Any
import queue

from ai_app_autogen.ai_app_autogen_props import AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgentWrapper

class AuoGentChatBase:
    def __init__(self):
        # 途中のメッセージを格納するキュー
        self.message_queue = queue.Queue()
        # 終了フラグ
        self.finished = False
        
        self.print_messages_function = self.create_print_messages_function()

    # キューからメッセージを取得 yiled で返す
    def get_messages(self) -> Generator[Any, None, None]:
        while True:
            try:
                if self.finished:
                    print("finished")
                    break
                message = self.message_queue.get(timeout=5)
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


class AutoGenGroupChatWrapper(AuoGentChatBase):
    def __init__(self, init_agent : ConversableAgent, agents: list[ConversableAgent]):

        super().__init__()
        self.init_agent = init_agent
        self.agents = agents

        # register_reply 
        for agent in self.agents:
            agent.register_reply([autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # agentのリストを表示
        for agent in self.agents:
            print(f"agent:{agent.name} is ready.")


    def run_chat(self, initial_message: str, max_turns: int, max_round: int) -> Generator[Any, None, None]:

        # グループチャットを開始
        groupchat = autogen.GroupChat(
            admin_name="chat_admin_agent",
            agents=self.agents,
            messages=[],
            max_round=max_round,
        )

        group_chat_manager = autogen.GroupChatManager(
            groupchat=groupchat,
            is_termination_msg = lambda msg: "end meeting" in msg["content"].lower()
        )

        def start_chat():
            try:
                self.init_agent.initiate_chat(group_chat_manager, message=initial_message, max_turns=max_turns)
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

    @classmethod
    def create_default_chats(cls, autogen_props: AutoGenProps, agent_wrappers: list[AutoGenAgentWrapper]) -> list["AutoGenGroupChatWrapper"]:
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

class AutoGenNormalChatWrapper(AuoGentChatBase):
    def __init__(self, init_agent : ConversableAgent, agents: list[ConversableAgent]):

        super().__init__()
        self.init_agent = init_agent
        self.agents = agents

        # register_reply 
        for agent in self.agents:
            agent.register_reply([autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # agentのリストを表示
        for agent in self.agents:
            print(f"agent:{agent.name} is ready.")


    def run_chat(self, initial_message: str, max_turns: int) -> Generator[Any, None, None]:

        # receiver_agentを設定
        # agentsからinit_agentを削除
        agents = [agent for agent in self.agents if agent.name != self.init_agent.name]
        reciever_agent = agents[0]

        def start_chat():
            try:
                self.init_agent.initiate_chat(reciever_agent, message=initial_message, max_turns=max_turns)
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

