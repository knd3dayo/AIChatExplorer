import os
from typing import Annotated
from dotenv import load_dotenv
import autogen  # type: ignore
import tempfile

from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor # type: ignore

# Create a temporary directory to store the code files.
temp_dir = tempfile.TemporaryDirectory()

# Create a local command line code executor.
executor = LocalCommandLineCodeExecutor(
    timeout=10,  # Timeout for each code execution in seconds.
    work_dir=temp_dir.name,  # Use the temporary directory to store the code files.
)
load_dotenv()

llm_config = {
    "config_list": [{
        "model": "gpt-4o-mini", "api_key": os.environ["OPENAI_API_KEY"], }],
}

assistant = autogen.AssistantAgent(
    name="assistant",
    system_message="""タスクを解く際、提供された関数に役立つものがある場合、それ利用して下さい。
        最終的な解答を提示した後は「タスク完了」というメッセージを出力してください。""",
    llm_config=llm_config,
    code_execution_config={"executor": executor},
    max_consecutive_auto_reply=5,
)

user_proxy = autogen.UserProxyAgent(
    name="user_proxy",
    is_termination_msg=lambda x: x.get("content", "") and x.get("content", "").rstrip().endswith("タスク完了"),
    human_input_mode="NEVER",
    llm_config=llm_config,
    code_execution_config={"executor": executor},
    max_consecutive_auto_reply=5,

)

# 関数を実行対象としてエージェントに登録する
@user_proxy.register_for_execution() 
# 利用可能な関数の情報をエージェントに登録する
@assistant.register_for_llm(description="摂氏から華氏へ変換する関数")
def celsius_to_fahrenheit(celsius: Annotated[float, "温度（摂氏）"]) -> float:
    return (celsius * 9/5) + 32

user_proxy.initiate_chat(assistant, message="摂氏15度を華氏に変換して下さい。")
