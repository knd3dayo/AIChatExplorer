import os, sys, asyncio
from typing import Any
from dotenv import load_dotenv
# autogen
from autogen_ext.models.openai import OpenAIChatCompletionClient
from autogen_core.tools import FunctionTool
from autogen_agentchat.agents import AssistantAgent
from autogen_agentchat.base import TaskResult
from autogen_agentchat.messages import ChatMessage, AgentEvent, TextMessage
from autogen_agentchat.conditions import MaxMessageTermination, TextMentionTermination, TimeoutTermination
from autogen_agentchat.teams import SelectorGroupChat

# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

autogen_agents: list[AssistantAgent] = []


# 指定したnameのLLMConfigをDBから取得して、llm_configを返す    
def create_model_client() -> OpenAIChatCompletionClient:
    # .envファイルから環境変数を読み込む
    load_dotenv()
    api_key = os.getenv("OPENAI_API_KEY")
    if api_key is None:
        raise ValueError("API key is not set.")

    # print(f"autogen llm_config parameters:{parameters}")
    client = OpenAIChatCompletionClient(
        api_key=api_key,
        model="gpt-4o-mini",
    )
    return client

# 指定したnameのAgentをDBから取得して、Agentを返す
def create_agent(
        name: str, description: str, system_message:str, 
        model_client: OpenAIChatCompletionClient, tools: list[FunctionTool] = []) -> AssistantAgent:
    # ConversableAgent object用の引数辞書を作成
    params: dict[str, Any] = {}
    params["name"] = name
    params["description"] = description

    # code_executionがFalseの場合は、AssistantAgentを作成
    params["system_message"] = system_message
    # llm_config_nameが指定されている場合は、llm_config_dictを作成
    params["model_client"] = model_client
    if len(tools) > 0:
        params["tools"] = tools
    
    return AssistantAgent(**params)

def create_termination_condition(termination_msg: str, max_msg: int, timeout: int):
    # Define termination condition
    max_msg_termination = MaxMessageTermination(max_messages=max_msg)
    text_termination = TextMentionTermination(termination_msg)
    time_terminarion = TimeoutTermination(timeout)
    combined_termination = max_msg_termination | text_termination | time_terminarion
    return combined_termination

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

    # plannerとagent_callerによるグループチャットを作成
    chat = SelectorGroupChat(
            [planner, science_researcher, history_researcher, anime_researcher],
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