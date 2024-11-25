import autogen 
from collections.abc import Generator
from typing import Any

from sample_autogen_props import AutoGenProps
from sample_autogen_agent import AutoGenAgentGenerator

class AutoGenGroupChat:


    @classmethod
    def run_group_chat(cls, initial_message: str, max_round: int) -> Generator[Any, None, None]:
        
        # defaultのエージェントを作成
        autogen_props = AutoGenProps()
        autogen_agents = AutoGenAgentGenerator.create_default_agents(autogen_props)

        # dict[str, tuple[ConversableAgent, str, dict]]から各Valueの1つめの要素を取得
        agents = [ v[0] for k, v in autogen_agents.items() ]

        # init_agentを作成 autogen_agentのキーが"user_proxy"のものを取得
        init_agent = autogen_agents["user_proxy"][0]

        # グループチャットを開始
        groupchat = autogen.GroupChat(
            admin_name="chat_admin_agent",
            agents=agents,
            messages=[],
            max_round=max_round,
        )

        group_chat_manager = autogen.GroupChatManager(
            groupchat=groupchat,
            llm_config=autogen_props.create_llm_config(),
        )

        init_agent.initiate_chat(group_chat_manager, message=initial_message, max_turns=3)

if __name__ == "__main__":
    import sys
    # 第1引数：入力用のテキストファイル
    input_file = sys.argv[1]
    with open(input_file, "r") as f:
        initial_message = f.read()
    
    # 第2引数：最大ラウンド数　デフォルトは5
    max_round = 5
    if len(sys.argv) > 2:
        max_round = int(sys.argv[2])

    AutoGenGroupChat.run_group_chat(initial_message, max_round=max_round)
    