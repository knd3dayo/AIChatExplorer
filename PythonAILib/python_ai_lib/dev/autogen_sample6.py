import os
from typing import Annotated
from dotenv import load_dotenv
import autogen  # type: ignore
import tempfile
import sys

from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor # type: ignore

sys.path.append(os.path.join(os.path.dirname(__file__), '..'))
from openai_props import OpenAIProps, env_to_props
from autogen_props import AutoGenProps

from io import StringIO
class Tee(StringIO):
    def __init__(self, name, mode):
        super().__init__()
        self.file = open(name, mode)
        self.stdout = sys.stdout
        sys.stdout = self
    def __del__(self):
        sys.stdout = self.stdout
        self.file.close()
    def write(self, data):
        self.file.write(data)
        self.stdout.write(data)
    def flush(self):
        self.file.flush()
# Create a temporary directory to store the code files.
temp_dir = tempfile.TemporaryDirectory()

# Create a local command line code executor.
executor = LocalCommandLineCodeExecutor(
    timeout=10,  # Timeout for each code execution in seconds.
    work_dir=temp_dir.name,  # Use the temporary directory to store the code files.
)

openAIProps: OpenAIProps = env_to_props()
autogenProps: AutoGenProps = AutoGenProps(openAIProps)

llm_config =autogenProps.llm_config


user_proxy = autogen.UserProxyAgent(
    system_message="""
        あなたはグループチャットの議題提供者です。
        グループチャットの管理者に議題を提示します。
        議題を提供した後は発言することはありません。
        議題を提供した後は、議事録と結論を提供されるまで待機します
        """,
    name="user_proxy",
    human_input_mode="NEVER",
    is_termination_msg=lambda msg: "[会議を終了]" in msg["content"].lower(),
    code_execution_config={"executor": executor},
    llm_config=llm_config,
    max_consecutive_auto_reply=5,

)

# グループチャットの管理者
chat_admin_agent = ConversableAgent(
    "chat-admin",
    system_message=f"""
        あなたはグループチャットの管理者です。
        他のエージェントに指示を出し、彼らが協力してタスクを完了するのを手伝います。
        グループチャットの内容をまとめて議事録(4000文字程度,各エージェントの発言の概要が時系列でわかるもの)と結論を出す責任があります。
        グループチャットでの結論が出ましたら、議事録と結論を議題提供者に伝えたのち、[会議を終了]と伝えてチャットを終了します。
        """
,
    llm_config=llm_config,
    code_execution_config=False,
    function_map=None,
    human_input_mode="NEVER",
)

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
        * あなたが作成するスクリプトは、{temp_dir}に保存されます。
        * スクリプトの実行結果がエラーである場合、エラー文を元に対策を考え、修正したコードを再度作成します。
        * スクリプトを実行した結果、情報を十分に得られなかった場合、現状得られた情報から修正したコードを再度作成します。
        * あなたはユーザーの指示を最終目標とし、これを満たす迄何度もコードを作成・修正します。
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
    system_message=f"""
        あなたはコード実行者です。
        コード作成者から提供されたコードを実行します。
        コードの実行結果を表示します。
        """,
    llm_config=False,
    
    code_execution_config={"executor": executor},
    human_input_mode="NEVER",
)

# Wikipedia検索者
wikipedia_searcher = ConversableAgent(
    "wikipedia-searcher",
    system_message="""
        あなたはWikipedia検索者です。ユーザーの指示に従いWikipediaで情報を検索します。
        提供された関数を用いて、検索した結果、タイトルと本文を表示します。
        関数を実行した結果がエラーとなった場合や不足機能がある場合は、コード作成者に修正を依頼してください。
        """,
    llm_config=llm_config,
    code_execution_config={"executor": executor},
    human_input_mode="NEVER",
)

# 関数を実行対象としてエージェントに登録する
@wikipedia_searcher.register_for_execution() 
# 利用可能な関数の情報をエージェントに登録する
@wikipedia_searcher.register_for_llm(description="Wikipedia(日本語版)から検索対象文字列に関連するページを検索します。検索結果からページのタイトルと本文を抽出してリストとしてページごとのリストとして返します")
def search_wikipedia_ja(query: Annotated[str, "検索対象の文字列"], num_results: Annotated[int, "表示する結果の最大数"]) -> list[str]:
    import wikipedia

    # Wikipediaの日本語版を使用
    wikipedia.set_lang("ja")
    
    # 検索結果を取得
    search_results = wikipedia.search(query, results=num_results)
    
    result_texts = []
    # 上位の結果を表示
    for i, title in enumerate(search_results):
    
        print(f"結果 {i + 1}: {title}")
        try:
            # ページの内容を取得
            page = wikipedia.page(title)
            print(page.content[:500])  # 最初の500文字を表示
            text = f"タイトル:\n{title}\n\n本文:\n{page.content}\n"
            result_texts.append(text)
        except wikipedia.exceptions.DisambiguationError as e:
            print(f"曖昧さ回避: {e.options}")
        except wikipedia.exceptions.PageError:
            print("ページが見つかりませんでした。")
        print("\n" + "-"*50 + "\n")
    return result_texts


groupchat = autogen.GroupChat(
    # 管理者エージェント
    admin_name="chat_admin_agent",
    agents=[user_proxy,chat_admin_agent, code_writer_agent, code_execution_agent, wikipedia_searcher], 
    messages=[], 
    send_introductions=True,  
    max_round=3
)

group_chat_manager = autogen.GroupChatManager(
    groupchat=groupchat, 
    llm_config=llm_config

)

user_message  = f"""
    ぽんちょろりん汁はどのようなものですか？
    """
user_proxy.initiate_chat(group_chat_manager, message=user_message, max_turns=3)
print(groupchat.messages)

