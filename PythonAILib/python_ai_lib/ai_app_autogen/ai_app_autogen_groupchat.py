
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from collections.abc import Generator
import queue
from typing import Any

from ai_app_autogen.ai_app_autogen_client import AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgents

class AutoGenGroupChat:
    def __init__(self, autoGentAgents: AutoGenAgents):
        
        self.autogen_agents = autoGentAgents

    def run_group_chat(self, initial_message: str, max_round: int, init_agent_tuple: tuple[ConversableAgent, str], agents_tuple: list[tuple[ConversableAgent, str]]) -> Generator[Any, None, None]:

        init_agent = init_agent_tuple[0]
        agents = [agent[0] for agent in agents_tuple]
        # register_reply 
        init_agent.register_reply( [autogen.Agent, None], reply_func=self.autogen_agents.print_messages_function, config={"callback": None})
        for agent in agents:
            agent.register_reply( [autogen.Agent, None], reply_func=self.autogen_agents.print_messages_function, config={"callback": None})


        # グループチャットを開始
        groupchat = autogen.GroupChat(
            admin_name="chat_admin_agent",
            agents=agents,
            messages=[], 
            # send_introductions=True,  
            max_round=max_round
        )

        group_chat_manager = autogen.GroupChatManager(
            groupchat=groupchat, 
            llm_config=self.autogen_agents.autogen_pros.create_llm_config(),
        )
        def start_chat():
            try:
                init_agent.initiate_chat(group_chat_manager, message=initial_message, max_turns=3)
            except Exception as e:
                print(f"Error: {e}")
                import traceback
                traceback.print_exc()
            finally:
                self.autogen_agents.finish()
        
        import threading
        thread = threading.Thread(target=start_chat)
        thread.start()

        return self.autogen_agents.get_messages()


