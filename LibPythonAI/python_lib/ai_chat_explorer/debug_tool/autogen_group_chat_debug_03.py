import os, sys, asyncio
from typing import Annotated
# autogen
from autogen_core.tools import FunctionTool
from autogen_agentchat.agents import AssistantAgent
from autogen_agentchat.base import TaskResult
from autogen_agentchat.messages import ChatMessage, AgentEvent, TextMessage
from autogen_agentchat.teams import SelectorGroupChat
from ai_chat_explorer.debug_tool.autogen_group_chat_debug_02 import create_model_client, create_agent, create_termination_condition

# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

autogen_agents: list[AssistantAgent] = []

# エージェント一覧を取得する関数
def list_agents() -> Annotated[list[dict[str, str]], "List of registered agents, each containing 'name' and 'description'"]:
    """
    This function retrieves a list of registered agents.
    """
    agent_descption_list = []
    for agent in autogen_agents:
        agent_descption_list.append({"name": agent.name, "description": agent.description})
    return agent_descption_list

# execute_agent
# エージェントを実行する関数
async def execute_agent(
        agent_name: Annotated[str, "Agent name"], initial_message: Annotated[str, "Input text"],
        ) -> Annotated[str, "Output text"]:
    """
    This function executes the specified agent with the input text and returns the output text.
    First argument: agent name, second argument: input text.
    - Agent name: Specify the name of the agent as the Python function name.
    - Input text: The text data to be processed by the agent.
    """
    # agent_nameに対応するエージェントを取得
    agent_list = [item for item in autogen_agents if item.name == agent_name]
    agent = agent_list[0] if len(agent_list) > 0 else None

    if agent is None:
        return "The specified agent does not exist."

    output_text = ""
    # run_agent関数を使用して、エージェントを実行
    async for message in agent.run_stream(task=initial_message):
        if type(message) == TaskResult:
            break
        if type(message) == ChatMessage or type(message) == AgentEvent or type(message) == TextMessage:
            message_str = f"{message.source}: {message.content}"
            output_text += message_str + "\n"

    return output_text


async def main(input_message: str):
    # モデルクライアントを作成
    model_client = create_model_client()
    
    # テスト用エージェントを作成
    science_researcher = create_agent(
        name="science_researcher",
        description="科学知識に関する質問に答えるエージェント",
        system_message="あなたは科学研究者です。科学に関する質問に答えることができます。",
        model_client=model_client,
    )
    history_researcher = create_agent(
        name="history_researcher",
        description="歴史に関する質問に答えるエージェント",
        system_message="あなたは歴史研究者です。歴史に関する質問に答えることができます。",
        model_client=model_client,
    )
    anime_researcher = create_agent(
        name="anime_researcher",
        description="アニメに関する質問に答えるエージェント",
        system_message="あなたはアニメ研究者です。アニメに関する質問に答えることができます。",
        model_client=model_client,
    )
    # エージェント呼び出しエージェントを作成
    agent_caller = create_agent(
        name="agent_caller",
        description="他のエージェントを呼び出すエージェント",
        system_message=""""
        list_agentsで呼び出し可能なエージェント一覧を取得します。そして、
        ユーザーの要求にマッチする適切なエージェントを呼び出すことができます。
        """,
        model_client=model_client,
        tools=[
            FunctionTool(execute_agent, execute_agent.__doc__, name = "execute_agent"), # type: ignore
            FunctionTool(list_agents, list_agents.__doc__ ,name = "list_agents") # type: ignore
        ] 
    )
    # plannerエージェント
    planner = create_agent(
        name="planner",
        description="ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成しますト",
        system_message=""""
        ユーザーの要求を達成するための計画を考えて、各エージェントと協力して要求を達成します
        - ユーザーの要求を達成するための計画を作成してタスク一覧を作成します。
        - タスクの割り当てに問題ないか？もっと効率的な計画およびタスク割り当てがないか？については対象エージェントに確認します。
        - 計画に基づき、対象のエージェントにタスクを割り当てます。
        - 計画作成が完了したら[計画作成完了]と返信してください
        その後、計画に基づきタスクを実行します。全てのタスクが完了したら、[TERMINATE]と返信してください。
        """,
        model_client=model_client,
    )
    # エージェントをリストに追加
    global autogen_agents
    autogen_agents.append(science_researcher)
    autogen_agents.append(history_researcher)
    autogen_agents.append(anime_researcher)
    # autogen_agents.append(agent_caller)
    # autogen_agents.append(planner)

    # plannerとagent_callerによるグループチャットを作成
    chat = SelectorGroupChat(
            [planner, agent_caller],
            model_client=model_client,
            termination_condition=create_termination_condition("[TERMINATE]", 10, 120)
            )

    # グループチャットを実行
    stream = chat.run_stream(task=input_message)
    async for message in stream:
        if type(message) == TaskResult:
            break
        if type(message) == ChatMessage or type(message) == AgentEvent or type(message) == TextMessage:
            message_str = f"{message.source}: {message.content}"
            print(message_str)

if __name__ == '__main__':
    asyncio.run(main("光速について教えてください。plannerはエージェントからの回答を要約してください。"))