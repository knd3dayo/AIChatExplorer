import os, json
from typing import Any
from ai_app_openai_util import OpenAIProps
from ai_app_vector_db_util import VectorDBProps
from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor
import autogen 
from ai_app_autogen_util import AutoGenProps, AutoGenTools

# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

class AutoGenGroupChatTest1:
    def __init__(self, llm_config: dict, executor: LocalCommandLineCodeExecutor, work_dir: str, auto_execute_code: bool = False):
        self.llm_config = llm_config
        self.executor = executor
        self.work_dir = work_dir
        self.__create_default_agents()

        # コード実行者が自動的にコードを実行するようにするかどうか
        self.auto_execute_code = auto_execute_code

        # 結果を出力するファイル
        self.output_file = None


    def __create_default_agents(self):
        # グループチャットの議題提供者
        self.user_proxy = autogen.UserProxyAgent(
            system_message="""
                あなたはグループチャットの議題提供者です。
                グループチャットの管理者に議題を提示します。
                """,
            name="user_proxy",
            human_input_mode="NEVER",
            is_termination_msg=lambda msg: "[会議を終了]" in msg["content"].lower(),
            code_execution_config=False,
            llm_config=self.llm_config,
        )
        # グループチャットの管理者
        self.chat_admin_agent = ConversableAgent(
            "chat-admin",
            system_message=f"""
                あなたはグループチャットの管理者です。
                他のエージェントに指示を出し、彼らが協力してタスクを完了するのを手伝います。
                グループチャットの内容をまとめて議事録(4000文字程度,各エージェントの発言の概要が時系列でわかるもの)と結論を出す責任があります。
                グループチャットでの結論が出ましたら、議事録と結論を議題提供者に伝えたのち、[会議を終了]と伝えてチャットを終了します。
                """
        ,
            llm_config=self.llm_config,
            code_execution_config=False,
            function_map=None,
            human_input_mode="NEVER",
        )
        # コード作成者
        self.code_writer_agent = None
        # コード実行者
        self.code_execution_agent = None
        # Web検索者
        self.web_searcher = None
        # ベクトル検索者
        self.vector_searcher = None
        # ファイル抽出者
        self.file_extractor = None

    def enable_code_writer_and_executor(self):
        # コードの作成者と実行者は分離する。以下はコード推論 Agent。LLM を持つ。
        self.code_writer_agent = ConversableAgent(
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
                * あなたが作成するスクリプトは、{self.work_dir}に保存されます。
                * pipインストール以外には、システムへの変更は行いません。
                * スクリプトの実行結果がエラーである場合、エラー文を元に対策を考え、修正したコードを再度作成します。
                * スクリプトを実行した結果、情報を十分に得られなかった場合、現状得られた情報から修正したコードを再度作成します。
                * あなたはユーザーの指示を最終目標とし、これを満たす迄何度もコードを作成・修正します。
                """
        ,
            llm_config=self.llm_config,
            code_execution_config=False,
            function_map=None,
            human_input_mode="NEVER",
        )

        # コードの作成者と実行者は分離する。以下はコード実行者 Agent。LLM は持たない。
        self.code_execution_agent = ConversableAgent(
            "code-execution",
            system_message=f"""
                あなたはコード実行者です。
                コード作成者から提供されたコードを実行します。
                コードの実行結果を表示します。
                """,
            llm_config=False,
            
            code_execution_config={"executor": self.executor},
            human_input_mode="NEVER",
        )
        # コード実行者が自動的にコードを実行するようにするかどうか
        # self.auto_execute_code == True の場合はhuman_input_mode="NEVER"とする
        if self.auto_execute_code:
            self.code_execution_agent.human_input_mode = "NEVER"
        else:
            self.code_execution_agent.human_input_mode = "ALWAYS" 

    def enable_web_searcher(self):
        
        # Web検索者
        self.web_searcher = ConversableAgent(
            "web-searcher",
            system_message="""
                あなたはWeb検索者です。ユーザーの指示に従いWebで情報を検索します。
                - 提供された関数を用いて、WikiPedia(日本語版)から情報を検索します。
                - 検索した結果テキストから公式ドキュメントへのリンクなどを抽出します。
                - 公式ドキュメントのHTMLを取得します。
                - ユーザーからの指示にマッチする情報をユーザーに提供します。
                """,
            llm_config=self.llm_config,
            code_execution_config=False,
            human_input_mode="NEVER",
        )

        # 利用可能な関数の情報をエージェントに登録する

        # Wikipedia(日本語版)から検索対象文字列に関連するページを検索します。
        search_web_ja = AutoGenTools().create_search_wikipedia_ja()
        self.web_searcher.register_for_llm(
                name="search_wikipedia_ja",
                description="""
                wikipedia(日本語版)から検索対象文字列に関連するページを検索します。
                検索結果からページのタイトルと本文を抽出してリストとしてページごとのリストとして返します
                """)(search_web_ja)
        self.user_proxy.register_for_execution(name="search_wikipedia_ja") (search_web_ja)

        # 指定されたURLのHTMLを取得します。
        get_html = AutoGenTools().create_get_html_function()
        self.web_searcher.register_for_llm(
                name="get_html",
                description="指定されたURLのHTMLを取得します。")(get_html)
        self.user_proxy.register_for_execution(name="get_html") (get_html)

        # テキストファイルからURLを抽出します。
        extract_urls_from_text = AutoGenTools().create_get_urls_from_text_function()
        self.web_searcher.register_for_llm(
                name="extract_urls_from_text",
                description="テキストファイルからURLを抽出します。")(extract_urls_from_text)
        self.user_proxy.register_for_execution(name="extract_urls_from_text") (extract_urls_from_text)

        # HTMLからURLを抽出します。
        extract_urls_from_html = AutoGenTools().create_get_urls_from_html_function()
        self.web_searcher.register_for_llm(
                name="extract_urls_from_html",
                description="HTMLからURLを抽出します。")(extract_urls_from_html)
        self.user_proxy.register_for_execution(name="extract_urls_from_html") (extract_urls_from_html)


    # ベクトル検索者を有効にする
    def enable_vector_searcher(self, openAIProps: OpenAIProps, vector_db_props_list: list[VectorDBProps]):
        # ベクトル検索者
        self.vector_searcher = ConversableAgent(
            "vector-searcher",
            system_message="""
                あなたはベクトル検索者です。ユーザーの指示に従い、ベクトルデータベースから情報を検索します。
                提供された関数を用いて、検索した結果を表示します。
                """,
            llm_config=self.llm_config,
            code_execution_config=False,
            human_input_mode="NEVER",
        )
        # 利用可能な関数の情報をエージェントに登録する
        vector_search = AutoGenTools().create_vector_search(openAIProps, vector_db_props_list)
        self.user_proxy.register_for_execution(name="vector_search") (vector_search)
        self.vector_searcher.register_for_llm(
                name="vector_search",
                description="ベクトルデータベースから検索対象文字列に関連するページを検索します。検索結果からページのタイトルと本文を抽出してリストとしてページごとのリストとして返します"
                )(vector_search)

    # ファイル抽出者を有効にする
    def enable_file_extractor(self):
        # ファイル抽出者
        self.file_extractor = ConversableAgent(
            "file-extractor",
            system_message="""
                あなたはファイル抽出者です。ユーザーの指示に従い、ファイルから情報を抽出します。
                提供された関数を用いて、抽出した結果を表示します。
                """,
            llm_config=self.llm_config,
            code_execution_config=False,
            human_input_mode="NEVER",
        )
        # 利用可能な関数の情報をエージェントに登録する
        extract_text_from_file = AutoGenTools().create_extract_text_from_file()
        self.user_proxy.register_for_execution(name="extract_text_from_file") (extract_text_from_file)
        self.file_extractor.register_for_llm(
                name="extract_text_from_file",
                description="ファイルからテキストを抽出します。"
                )(extract_text_from_file)


    def set_output_file(self, output_file: str):
        self.output_file = output_file

    def execute_group_chat(self, initial_message: str, max_round: int):
        # エージェントのうち、Noneでないものを指定
        agents: list[ConversableAgent] = [self.user_proxy, self.chat_admin_agent]
        for agent in [self.code_writer_agent, self.code_execution_agent, 
                      self.web_searcher, self.vector_searcher, self.file_extractor]:
            if agent:
                agents.append(agent)
        
        # グループチャットを開始
        groupchat = autogen.GroupChat(
            admin_name="chat_admin_agent",
            agents=agents,
            messages=[], 
            # send_introductions=True,  
            max_round=max_round
        )

        group_chat_manager = autogen.GroupChatManager(
            groupchat=groupchat, 
            llm_config=self.llm_config
        )
        self.user_proxy.initiate_chat(group_chat_manager, message=initial_message, max_turns=3)

        # 結果を出力するファイルが指定されている場合
        if self.output_file:
            with open(self.output_file, "w", encoding="utf-8") as f:
                f.write(json.dumps(groupchat.messages, ensure_ascii=False, indent=4))

        return groupchat

import os
import tempfile
import sys
import getopt

if __name__ == '__main__':

    # getoptsでオプション引数の解析
    # -p オプションでOpenAIプロパティファイル(JSON)を指定する
    # -v オプションでVectorDBプロパティファイル(JSON)を指定する
    # -o オプションで出力ファイルを指定する
    # -m オプションでメッセージを指定する
    message = None
    output_file = None
    props_file = None

    opts, args = getopt.getopt(sys.argv[1:], "m:o:p:")
    for opt, arg in opts:
        if opt == "-m":
            message = arg
        elif opt == "-o":
            output_file = arg
        elif opt == "-p":
            props_file = arg
    
    # プロパティファイル(JSON)を読み込む
    open_ai_props_dict = {}
    vector_db_props_dict = []
    request = {}

    if props_file:
        print(f"props_file:{props_file}")
        with open(props_file, "r", encoding="utf-8") as f:
            props_dict = json.load(f)
            open_ai_props_dict = props_dict.get("open_ai_props", {})
            open_ai_props = OpenAIProps(open_ai_props_dict)

            vector_db_props_dict = props_dict.get("vector_db_props", [])
            vector_db_props_list = [VectorDBProps(props) for props in vector_db_props_dict]

            request = props_dict.get("chat_request", {})

    else:
            open_ai_props: OpenAIProps = OpenAIProps.env_to_props()
            vector_db_props_list = [VectorDBProps.get_vector_db_settings()]

    input_text = ""
    # messageが指定されている場合は, messageを入力テキストとする
    if message:
        input_text = message
    else:
        # requestの[messages][0][content]の最後の要素を入力テキストとする
        messages = request.get("messages", [])
        if messages:
            last_content = messages[0].get("content",[])[-1]
            input_text = last_content.get("text", "")

    # メッセージが指定されていない場合は入力メッセージがない旨を表示して終了
    if not input_text:
        print("Input message is not specified.")
        sys.exit(1)

    # メッセージを表示
    print(f"Input message: {input_text}")

    # Create a temporary directory to store the code files.
    temp_dir = tempfile.TemporaryDirectory()

    # Create a local command line code executor.
    executor = LocalCommandLineCodeExecutor(
        timeout=10,  # Timeout for each code execution in seconds.
        work_dir=temp_dir.name,  # Use the temporary directory to store the code files.
    )

    autogenProps: AutoGenProps = AutoGenProps(open_ai_props)

    client = AutoGenGroupChatTest1(autogenProps.llm_config, executor, temp_dir.name)
    client.enable_code_writer_and_executor()
    client.enable_web_searcher()
    client.enable_vector_searcher(open_ai_props, vector_db_props_list)
    client.enable_file_extractor()

    if output_file:
        client.set_output_file(output_file)
        print(f"Output file: {output_file}")

    group_chat = client.execute_group_chat(input_text, 10)
    # Print the messages in the group chat.
    for message in group_chat.messages:
        # roleがuserまたはassistantの場合はrole, name, contentを表示
        if message["role"] in ["user", "assistant"]:
            print(f"role:[{message['role']}] name:[{message['name']}]")
            print ("------------------------------------------")
            print(f"content:{message['content']}\n")

    # Delete the temporary directory.
    temp_dir.cleanup()
    


