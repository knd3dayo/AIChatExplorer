from autogen import ConversableAgent, UserProxyAgent, GroupChatManager, GroupChat, GroupChatManager
from ai_app_autogen.ai_app_autogen_client import AutoGenProps


class AgentGenGroupChatGenerator:
    def __init__(self, autogen_pros: AutoGenProps, autogen_tools: dict, autogen_agents: dict):
        self.autogen_pros = autogen_pros
        self.autogen_tools = autogen_tools
        self.autogen_agents = autogen_agents


    def run_group_chat(self,  admin_name: str, agents: list[ConversableAgent], max_round :int ) -> GroupChatManager:
        # グループチャットを開始
        groupchat = GroupChat(
            admin_name=admin_name,
            agents=agents,
            messages=[], 
            # send_introductions=True,  
            max_round=max_round
        )

        group_chat_manager = GroupChatManager(
            groupchat=groupchat, 
            llm_config=self.autogen_pros.create_llm_config(),
        )
        return group_chat_manager

import sys
from ai_app_autogen.ai_app_autogen_tools import AutoGenToolGenerator
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgentGenerator

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python script.py <excel_file>")
        sys.exit(1)

    excel_file = sys.argv[1]
    autogen_props = AutoGenProps()
    autogen_def_tool = AutoGenToolGenerator(autogen_props)
    tools_dict = autogen_def_tool.create_tools_dict_from_sheet(excel_file)
    autogen_def_agent = AutoGenAgentGenerator(autogen_props, tools_dict)
    agents_dict = autogen_def_agent.create_agents_dict_from_sheet(excel_file)

    print(tools_dict)
    print(agents_dict)
