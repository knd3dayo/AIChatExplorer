import threading
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from collections.abc import Generator
from typing import Any

from ai_app_autogen.ai_app_autogen_client import AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgents, AutoGenAgentGenerator
from ai_app_autogen.ai_app_autogen_tools import AutoGenTools

class AutoGenGroupChat:
    @classmethod
    def create_default_group_chat(cls, autogen_props: AutoGenProps) -> "AutoGenGroupChat":
        autogen_group_chat = AutoGenGroupChat("default") 
        agents: dict = AutoGenAgentGenerator.create_default_agents(autogen_props)
        init_agent = agents["user_proxy"]
        autogen_group_chat.set_init_agent(init_agent)
        autogen_group_chat.set_agents(agents)

        return autogen_group_chat
    
    def __init__(self, autogen_props: AutoGenProps):
        self.autogen_props = autogen_props
        group_chat_dict = autogen_props.group_chat_dict
        self.name = group_chat_dict.get("name", "default")
        tools_dict = group_chat_dict.get("tools", {})
        self.autogen_tools = AutoGenTools(self.autogen_props, tools_dict)

        agents_dict = group_chat_dict.get("agents", {})
        self.autogen_agents = AutoGenAgents(self.autogen_props, agents_dict)

        init_agent_name = group_chat_dict.get("init_agent", "user_proxy")
        self.init_autogent_agent = self.autogen_agents.agents.get(init_agent_name, None)
        if self.init_autogent_agent is None:
            raise ValueError(f"init_agent not found: {init_agent_name}")

    def run_group_chat(self, initial_message: str, max_round: int) -> Generator[Any, None, None]:
        
        init_agent = self.init_autogent_agent[0]
        agents = [agent[0] for agent in self.autogen_agents.agents.values()]
        # register_reply 
        init_agent.register_reply([autogen.Agent, None], reply_func=self.autogen_agents.print_messages_function, config={"callback": None})
        for agent in agents:
            agent.register_reply([autogen.Agent, None], reply_func=self.autogen_agents.print_messages_function, config={"callback": None})

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
                self.autogen_agents.finish()

        thread = threading.Thread(target=start_chat)
        thread.start()
        # thread.join()  # スレッドが完了するまで待機

        return self.autogen_agents.get_messages()
