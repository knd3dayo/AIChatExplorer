import autogen 
from collections.abc import Generator
from typing import Any

from sample_autogen_props import AutoGenProps
from sample_autogen_agent import AutoGenAgentGenerator

class AutoGenGroupChatWrapper:

    # responseを格納するlist
    responses = []

    @classmethod
    def print_messages(cls, recipient, messages, sender, config): 
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
            
            # responseを格納
            cls.responses.append(response)

            return False, None  # required to ensure the agent communication flow continues

    @classmethod
    def run_group_chat(cls, initial_message: str, max_round: int) -> autogen.GroupChatManager:
        
        # defaultのエージェントを作成
        autogen_props = AutoGenProps()
        autogen_agents = AutoGenAgentGenerator.create_default_agents(autogen_props)

        # dict[str, tuple[ConversableAgent, str, dict]]から各Valueの1つめの要素を取得
        agents = [ v[0] for k, v in autogen_agents.items() ]

        # register_reply 
        for agent in agents:
            agent.register_reply([autogen.Agent, None], reply_func=cls.print_messages, config={"callback": None})


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

        return group_chat_manager

class AutoGenGroupChatGenerator:

    @classmethod
    def create_default_group_chat(cls) -> autogen.GroupChatManager:
        admin_name = "chat_admin_agent"


if __name__ == "__main__":
    import sys
    import getopt

    # 第2引数：最大ラウンド数　デフォルトは5
    max_round = 5
    output_file = None
    opts, args = getopt.getopt(sys.argv[1:], "df:i:o:")
    for opt, arg in opts:
        if opt == "-d":
            # デバッグ設定
            import logging 
            logging.basicConfig(level=logging.ERROR)
            from promptflow.tracing import start_trace
            # instrument OpenAI
            start_trace()
        elif opt == "-f":
            # 第1引数：入力用のテキストファイル
            input_file = arg
            with open(input_file, "r", encoding="utf-8") as f:
                initial_message = f.read()

        elif opt == "-i":
            max_round = int(arg)
        elif opt == "-o":
            output_file = arg

    AutoGenGroupChatWrapper.run_group_chat(initial_message, max_round=max_round)
     
    if output_file:
        with open(output_file, "w", encoding="utf-8") as f:
            for message in AutoGenGroupChatWrapper.responses:
                f.write(f"{message}\n")
