import os
from typing import Annotated
from dotenv import load_dotenv
import autogen  # type: ignore
import tempfile
import sys

from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor # type: ignore

sys.path.append(os.path.join(os.path.dirname(__file__), '..'))
from ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db_props import VectorDBProps
from ai_app_autogen_util import AutoGenProps

# Create a temporary directory to store the code files.
temp_dir = tempfile.TemporaryDirectory()

# Create a local command line code executor.
executor = LocalCommandLineCodeExecutor(
    timeout=10,  # Timeout for each code execution in seconds.
    work_dir=temp_dir.name,  # Use the temporary directory to store the code files.
)

openAIProps: OpenAIProps = OpenAIProps.env_to_props()
autogenProps: AutoGenProps = AutoGenProps(openAIProps)

llm_config =autogenProps.llm_config

# コード作成者と実行者のチャットを開始
import sys
# 推測対象のファイルがあるディレクトリを絶対パスで指定
dname = os.path.abspath(sys.argv[1])
print(f"ディレクトリ名: {dname}")


# コードの作成者と実行者は分離する。以下はコード推論 Agent。LLM を持つ。
code_writer_agent = ConversableAgent(
    "code-writer",
    system_message=f"""
        あなたはPython開発者です。
        あなたがコードを書くと自動的に外部のアプリケーション上で実行されます。
        ユーザーの指示に従ってあなたはコードを書きます。
        コードの実行結果は、あなたがコードを投稿した後に自動的に表示されます。
        ただし、次の条件を厳密に遵守する必要があります。
        ルール:
        * コードブロック内でのみコードを提案します。
        * 必要に応じてpipパッケージをインストールします。
        * pipインストール以外には、システムへの変更は行いません。また、外部のファイルへのアクセスは読み取りのみ許可されます。
        * あなたが作成するスクリプトは、{dname}に保存されます。
        * スクリプトの実行結果がエラーである場合、エラー文を元に対策を考え、修正したコードを再度作成します。
        * スクリプトを実行した結果、情報を十分に得られなかった場合、現状得られた情報から修正したコードを再度作成します。
        * あなたはユーザーの指示を最終目標とし、これを満たす迄何度もコードを作成・修正します。
        * 目的を達成した場合、[終了]とユーザーに伝え、チャットを終了します。
        """
,
    llm_config=llm_config,
    code_execution_config=False,
    function_map=None,
    human_input_mode="NEVER",
)

# コードの作成者と実行者は分離する。以下はコード実行者 Agent。LLM は持たない。
code_execution_agent = ConversableAgent(
    "code-execution",
    llm_config=False,
    is_termination_msg=lambda msg: "[終了]" in msg["content"].lower(),
    code_execution_config={"executor": executor},
    human_input_mode="NEVER",
)

user_message  = f"""
    {dname}にはとあるアプリケーションのソースコードがあります。
    そのソースコードのファイルの内容から何のアプリケーションかを推測してください。
    確認対象は*.pyファイルのみです。
    コード作成者から[終了]と伝えられるまで、推測を続けてください。
    """
    
code_execution_agent.initiate_chat(code_writer_agent, message=user_message, max_turns=10)
