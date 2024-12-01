import autogen 
from autogen import ConversableAgent
from collections.abc import Generator
from typing import Any

from sample_autogen_props import AutoGenProps
from sample_autogen_agent import AutoGenAgentGenerator, AutoGenAgentWrapper
from sample_autogen_tools import AutoGenToolGenerator, AutoGenToolWrapper

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
        
        # AutoGenPropsのインスタンスを生成
        autogen_props = AutoGenProps()
        # デフォルトのエージェントのリストを生成
        default_tool_wrappers: list[AutoGenToolWrapper] = AutoGenToolGenerator.create_default_tools()
        # デフォルトのエージェントのリストを生成
        default_agent_wrappers: list[AutoGenAgentWrapper] = AutoGenAgentGenerator.create_default_agents(autogen_props, default_tool_wrappers)

        # init_agent_name
        init_agent_name = "user_proxy"
        # autogen_agents
        agents = [agent.create_agent(autogen_props, default_tool_wrappers) for agent in default_agent_wrappers ]

        # init_agent_nameに一致するエージェントを取得
        init_agent_wrappers = [agent for agent in default_agent_wrappers if agent.name == init_agent_name]

        if len(init_agent_wrappers) == 0:
            raise ValueError(f"init_agent not found: {init_agent_name}")

        init_agent: ConversableAgent = init_agent_wrappers[0].create_agent(autogen_props, default_tool_wrappers)

        # register_reply 
        for agent in agents:
            agent.register_reply([autogen.Agent, None], reply_func=cls.print_messages, config={"callback": None})

        # agentのリストを表示
        for agent in agents:
            print(f"agent:{agent.name} is ready.")

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
