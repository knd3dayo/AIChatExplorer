import json
import queue
from typing import Any, Generator
from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor # type: ignore
import autogen 

from ai_app_autogen.ai_app_autogen_tools import AutoGenTools
from ai_app_autogen.ai_app_autogen_client import AutoGenProps

class AutoGenAgents:
    def __init__(self, autgenProps: AutoGenProps, auto_execute_code: bool = False, vector_db_props_list = []):
        self.llm_config = autgenProps.llm_config
        self.executor = autgenProps.executor
        self.work_dir = autgenProps.work_dir_path

        self.print_messages_function = self.__create_print_messages_function()

        self.autogen_tools = AutoGenTools(autgenProps.OpenAIProps, vector_db_props_list)

        # agents
        self.agents = []

        self.__create_default_agents()

        # コード実行者が自動的にコードを実行するようにするかどうか
        self.auto_execute_code = auto_execute_code

        # 結果を出力するファイル
        self.output_file = None


        # 途中のメッセージを格納するキュー
        self.message_queue = queue.Queue()

        # 終了フラグ
        self.finished = False


    def set_output_file(self, output_file: str):
        self.output_file = output_file

    def execute_group_chat(self, initial_message: str, max_round: int):

        # グループチャットを開始
        groupchat = autogen.GroupChat(
            admin_name="chat_admin_agent",
            agents=self.agents,
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

    def __create_print_messages_function(self):
        def print_messages(recipient, messages, sender, config): 
            if "callback" in config and  config["callback"] is not None:
                callback = config["callback"]
                callback(sender, recipient, messages[-1])
            response = f"Messages sent to: {recipient.name} | num messages: {len(messages)}\n{messages[-1]}" 
            # queueにresponseを追加
            self.message_queue.put(response)

            return False, None  # required to ensure the agent communication flow continues
        
        return print_messages

    # エージェントの終了処理
    def finish(self):
        self.finished = True
        self.message_queue.put(None)
    # キューからメッセージを取得 yiled で返す
    def get_messages(self) -> Generator[Any, None, None]:
        while True:
            if self.finished:
                break
            message = self.message_queue.get()
            yield message, False
        
        return None, True

    # キューから取り出したメッセージを表示
    def print_messages(self):
        for message, is_last_message in self.get_messages():
            if message is None:
                break
            print(message)

    def __create_default_agents(self):
        # グループチャットの議題提供者
        self.user_proxy = autogen.UserProxyAgent(
            system_message="""
                あなたはグループチャットの議題提供者です。
                グループチャットの管理者に議題を提示します。
                グループチャットの管理者から会議を終了する旨のメッセージが届いたら、グループチャットの管理者に[会議を終了]と返信します。
                """,
            name="user_proxy",
            human_input_mode="NEVER",
            is_termination_msg=lambda msg: "会議を終了" in msg["content"].lower(),
            code_execution_config=False,
            llm_config=self.llm_config,
        )
        # register_reply 
        self.user_proxy.register_reply( [autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # user_proxyのregister_from_execution()を設定
        for generator_func in self.autogen_tools.function_generators.values():
            func, description = generator_func()
            self.user_proxy.register_for_execution()(func)

        # agentsに追加
        self.agents.append(self.user_proxy)

    def enable_code_writer(self):
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
                * あなたが作成するスクリプトは、{self.work_dir}に保存されます。
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
        # register_reply 
        self.code_writer_agent.register_reply( [autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # agentsに追加
        self.agents.append(self.code_writer_agent)

    def enable_autogen_tool_writer(self):
        # コードの作成者と実行者は分離する。以下はコード推論 Agent。LLM を持つ。
        self.autogen_tool_writer = ConversableAgent(
            "autogen-tool-writer",
            system_message=f"""
                あなたはAutoGenツール作成者です。ユーザーの指示に従い、AutoGenツール用のPython関数と説明を作成します。
                AutoGenツール用の関数は、Annotatedデコレータ(from typing import Annotated)を使って型ヒントを指定してください。
                作成して関数と説明はsave_tools関数によりjsonとして{self.work_dir}に保存します。
                """
        ,
            llm_config=self.llm_config,
            code_execution_config=False,
            function_map=None,
            human_input_mode="NEVER",
        )
        # register_reply 
        self.autogen_tool_writer.register_reply( [autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})
        save_tools, description = self.autogen_tools.create_save_tools_function()
        # register_for_llm
        self.autogen_tool_writer.register_for_llm(description=description)(save_tools)

        # agentsに追加
        self.agents.append(self.autogen_tool_writer)

    def enable_code_executor(self):
        # コードの作成者と実行者は分離する。以下はコード実行者 Agent。LLM は持たない。
        self.code_execution_agent = ConversableAgent(
            "code-execution",
            system_message=f"""
                あなたはコード実行者です。
                コード作成者から提供されたコードを実行します。
                必要に応じてpipパッケージをインストールします。
                pipインストール以外には、システムへの変更は行いません。
                コードの実行結果を表示します。
                """,
            llm_config=False,
            
            code_execution_config={"executor": self.executor},
            human_input_mode="NEVER",
        )
        # register_reply 
        self.code_execution_agent.register_reply( [autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # コード実行者が自動的にコードを実行するようにするかどうか
        # self.auto_execute_code == True の場合はhuman_input_mode="NEVER"とする
        if self.auto_execute_code:
            self.code_execution_agent.human_input_mode = "NEVER"
        else:
            self.code_execution_agent.human_input_mode = "ALWAYS" 

        # agentsに追加
        self.agents.append(self.code_execution_agent)

    # ベクトル検索者を有効にする
    def enable_vector_searcher(self):
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
        # register_reply 
        self.vector_searcher.register_reply( [autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # 利用可能な関数の情報をエージェントに登録する
        vector_search, description = self.autogen_tools.create_vector_search()
        self.vector_searcher.register_for_llm(description=description)(vector_search)

        # agentsに追加
        self.agents.append(self.vector_searcher)

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
        # register_reply 
        self.file_extractor.register_reply( [autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # 利用可能な関数の情報をエージェントに登録する
        extract_text_from_file, description = self.autogen_tools.create_extract_text_from_file()
        self.file_extractor.register_for_llm (description=description)(extract_text_from_file)

        list_files_in_directory,  description = self.autogen_tools.create_list_files_in_directory()
        self.file_extractor.register_for_llm (description=description)(list_files_in_directory)

        # agentsに追加
        self.agents.append(self.file_extractor)

    def enable_web_searcher(self):
        
        # Web検索者
        self.web_searcher = ConversableAgent(
            "web-searcher",
            system_message="""
                あなたは汎用のWeb検索者です。ユーザーの指示に従いWebで情報を検索します。
                - 提供された関数を用いて、WikiPedia(日本語版)から情報を検索します。
                - 検索した結果テキストから公式ドキュメントへのリンクなどを抽出します。
                - 公式ドキュメントのHTMLを取得します。
                - ユーザーからの指示にマッチする情報をユーザーに提供します。
                """,
            llm_config=self.llm_config,
            code_execution_config=False,
            human_input_mode="NEVER",
        )
        # register_reply 
        self.web_searcher.register_reply( [autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # 利用可能な関数の情報をエージェントに登録する

        # Wikipedia(日本語版)から検索対象文字列に関連するページを検索します。
        search_wikipedia_ja, description = self.autogen_tools.create_search_wikipedia_ja()
        self.web_searcher.register_for_llm(description=description)(search_wikipedia_ja)

        # 指定されたURLのテキストとリンクを取得します。
        extract_webpage, description = self.autogen_tools.create_extract_webpage_function()
        self.web_searcher.register_for_llm(description=description)(extract_webpage)

        # agentsに追加
        self.agents.append(self.web_searcher)

    def enable_azure_document_searcher(self):
        
        # Web検索者
        self.azure_document_searcher = ConversableAgent(
            "azure-document-searcher",
            system_message="""
                あなたはAzure関連のドキュメント検索者です。ユーザーの指示に従いWeb上のAzure関連のドキュメントを検索します。
                - 提供された関数を用いて、https://learn.microsoft.com/ja-jp/azure/?product=popularからリンクされた情報を検索します。
                - リンク先に必要なドキュメントがない場合はさらにリンクされた情報を検索します。
                - 必要なドキュメントがあった場合はドキュメントのテキストをユーザーに提供します。
                """,
            llm_config=self.llm_config,
            code_execution_config=False,
            human_input_mode="NEVER",
        )

        # register_reply 
        self.azure_document_searcher.register_reply( [autogen.Agent, None], reply_func=self.print_messages_function, config={"callback": None})

        # 指定されたURLのテキストとリンクを取得します。
        extract_webpage, description = self.autogen_tools.create_extract_webpage_function()
        self.azure_document_searcher.register_for_llm(description=description)(extract_webpage)

        # agentsに追加
        self.agents.append(self.azure_document_searcher)
