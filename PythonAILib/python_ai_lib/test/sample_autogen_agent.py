
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from typing import Any, Callable

from sample_autogen_tools import AutoGenToolGenerator
from sample_autogen_props import AutoGenProps
        
from sample_autogen_props import AutoGenProps

class AutoGenAgentGenerator:

    @classmethod
    def create_default_agents(cls, autogen_props: AutoGenProps ) -> dict[str, tuple[ConversableAgent, str, dict]]:
        # create ddefault tools
        tools = AutoGenToolGenerator.create_default_tools(autogen_props)
        # Create an instance of AutoGenProps
        agents : dict[str, tuple[ConversableAgent, str, dict]] = {}
        agents["user_proxy"] = cls.__create_user_proxy(autogen_props, tools)
        agents["planner"] = cls.__create_planner(autogen_props, tools)
        agents["critic"] = cls.__create_critic(autogen_props, tools)
        agents["code_executor"] = cls.__create_code_executor(autogen_props, tools, True)
        agents["code_writer"] = cls.__create_code_writer(autogen_props, tools)
        agents["file_writer"] = cls.__create_file_writer(autogen_props, tools)
        agents["file_extractor"] = cls.__create_file_extractor(autogen_props, tools)
        agents["web_searcher"] = cls.__create_web_searcher(autogen_props, tools)
        agents["wikipeda_searcher"] = cls.__create_wikipedia_searcher(autogen_props, tools)
        agents["file_checker"] = cls.__create_file_checker(autogen_props, tools)
        agents["current_time"] = cls.__create_current_time(autogen_props, tools)

        return agents

    @classmethod
    def __create_user_proxy(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # タスクアサイナーを作成
        description = "ユーザーのリクエストを達成するためにプランナーと批評家と協力して実行計画を策定します。実行計画に基づき各エージェントと協力してタスクを完了させます。"
        user_proxy = autogen.UserProxyAgent(
            system_message="""
                あなたはユーザーの代理人です。ユーザーのリクエストを達成するためにプランナーと批評家と協力して実行計画を策定します。実行計画に基づき各エージェントと協力してタスクを完了させます。
                - まず、ユーザーのリクエストを達成するためにプランナーと批評家と協力して計画とタスクのリストを作成します。
                - 各エージェントにタスクを割り当て、そのタスクを実行します。
                - 各エージェントがタスクを実行して計画が達成されたら、[End Meeting]と返信します。
                - 追加の質問がなければ、[End Meeting]と返信します。
            """,
            name="user_proxy",
            human_input_mode="NEVER",
            code_execution_config=False,
            is_termination_msg=lambda msg: "end meeting" in msg["content"].lower(),
            description=description,
            llm_config=autogen_pros.create_llm_config()
        )

        # user_proxyに対してregister_from_execution()を設定
        for func, description in tools.values():
            print(f"register_for_execution: {func.__name__}")
            user_proxy.register_for_execution()(func)

        return user_proxy, description

    @classmethod
    def __create_planner(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # グループチャットのタスク割り当て
        description = "プランナー。計画を提案し、ユーザーの代理人(user_proxy)と批評家からのフィードバックに基づいてユーザーの代理の承認が得られるまで計画を修正します。"
        planner = autogen.AssistantAgent(
            system_message="""
            あなたはプランナーです。計画を提案し、ユーザーの代理人(user_proxy)と批評家からのフィードバックに基づいてユーザーの代理の承認が得られるまで計画を修正します。
            計画は、タスクのリスト、各タスクの責任者、タスクの期限、およびタスクの進捗状況を含む必要があります。
            計画立案には各エージェントの専門知識を活用してください。
            """,
            name="planner",
            human_input_mode="NEVER",
            code_execution_config=False,
            description=description,
            llm_config=autogen_pros.create_llm_config()
        )

        return planner, description

    @classmethod
    def __create_critic(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # グループチャットのタスク割り当て
        description = "批評家。他のエージェントの計画、主張、コードをダブルチェックし、フィードバックを提供します。計画に検証可能な情報（ソースURLなど）が含まれているかどうかを確認します。"
        critic = autogen.AssistantAgent(
            system_message="""批評家。他のエージェントの計画、主張、コードをダブルチェックし、フィードバックを提供します。
            計画に検証可能な情報（ソースURLなど）が含まれているかどうかを確認します。
            """,
            name="critic",
            human_input_mode="NEVER",
            code_execution_config=False,
            description=description,
            llm_config=autogen_pros.create_llm_config()
        )

        return critic, description


    @classmethod
    def __create_code_writer(cls, autogen_pros: AutoGenProps,  tools: dict[str, tuple[Callable, str]]):
        # コードライターとエグゼキューターを分けます。以下はLLMを使用するコード推論エージェントです。
        description = "ユーザーの指示に従ってPythonスクリプトを作成します。"
        code_writer_agent = ConversableAgent(
            "code_writer",
            system_message=f"""
                あなたはスクリプト開発者です。
                コードを書くと、自動的に外部アプリケーションで実行されます。
                ユーザーの指示に従ってコードを書きます。
                コードの実行結果はコードを投稿した後に自動的に表示されます。
                ただし、以下の条件を厳守してください：
                ルール：
                * コードブロック内にのみコードを提案してください。
                * スクリプトの実行結果がエラーの場合、エラーメッセージに基づいて対策を考え、再度修正コードを作成してください。
                * スクリプトの実行から得られる情報が不十分な場合、現在得られている情報に基づいて再度修正コードを作成してください。
                * あなたの最終目標はユーザーの指示であり、その目標を達成するために必要な限り多くの回数コードを作成および修正してください。
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )
        return code_writer_agent, description

    @classmethod
    def __create_file_writer(cls, autogen_pros: AutoGenProps,  tools: dict[str, tuple[Callable, str]]):
        # コードライターとエグゼキューターを分けます。以下はLLMを使用するコード推論エージェントです。
        description = "ユーザーの指示に従ってPythonでデータをファイルに保存します。"
        file_writer = ConversableAgent(
            "file_writer",
            system_message=f"""
                ユーザーの指示に従ってPythonでデータをファイルに保存します。
                デフォルトの保存場所は {autogen_pros.work_dir_path} です。
                ユーザーが保存場所を指定した場合は、その指定された場所にファイルを保存してください。
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )
        save_tools, description = tools["save_text_file"]
        # register_for_llm
        file_writer.register_for_llm(description=description)(save_tools)

        return file_writer, description

    @classmethod
    def __create_code_executor(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]], auto_execute_code: bool = False):
        # コードライターとエグゼキューターを分けます。以下はLLMを使用しないコード実行エージェントです。
        description = "コードライターが提供するコードを実行します。"
        code_execution_agent = ConversableAgent(
            "code_executor",
            system_message=f"""
                あなたはコード実行者です。
                コードライターが提供するコードを実行してください。
                コードの実行結果を表示してください。
                """,
            llm_config=False,
            code_execution_config={"executor": autogen_pros.create_code_executor()},
            description=description,
            human_input_mode="ALWAYS",
        )

        # コード実行者がコードを自動実行するかどうかを決定
        # auto_execute_codeがTrueの場合、human_input_modeを"NEVER"に設定
        if auto_execute_code:
            code_execution_agent.human_input_mode = "NEVER"

        return code_execution_agent, description

    # ファイル抽出機能を有効にする
    @classmethod
    def __create_file_extractor(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # ファイル抽出エージェント
        description = "ユーザーの指示に従ってファイルから情報を抽出します。"
        file_extractor = ConversableAgent(
            "file_extractor",
            system_message="""
                あなたはファイル抽出者です。ユーザーの指示に従ってファイルから情報を抽出します。
                提供された機能を使用して抽出結果を表示します。
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # エージェントに使用可能な機能の情報を登録
        extract_text_from_file, description = tools["extract_text_from_file"]
        file_extractor.register_for_llm(description=description)(extract_text_from_file)

        list_files_in_directory, description = tools["list_files_in_directory"]
        file_extractor.register_for_llm(description=description)(list_files_in_directory)

        return file_extractor, description

    @classmethod
    def __create_web_searcher(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # ウェブ検索エージェント
        description = "ウェブ上の文書を検索します。"
        web_searcher = ConversableAgent(
            "web_searcher",
            system_message="""
                あなたはウェブ検索者です。ユーザーの指示に従ってウェブ上で文書を検索します。
                - 提供されたsearch_duckduckgo関数を使用して情報を検索してください。
                - 必要な文書がリンク先にない場合は、さらにリンクされた情報を検索してください。
                - 必要な文書が見つかった場合は、extract_webpageを使って文書を取得し、ユーザーにテキストを提供してください。
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # 指定されたキーワードでAzureの文書を検索します。
        search_duckduckgo, description = tools["search_duckduckgo"]
        web_searcher.register_for_llm(description=description)(search_duckduckgo)

        # 指定されたURLからテキストとリンクを取得します。
        extract_webpage, description = tools["extract_webpage"]
        web_searcher.register_for_llm(description=description)(extract_webpage)

        return web_searcher, description

    @classmethod
    def __create_wikipedia_searcher(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        
        # ウェブ検索エージェント
        description = "指定されたURLから情報を取得します。"
        wipkipedia_searcher = ConversableAgent(
            "wipkipedia_searcher",
            system_message="""
                ユーザーの指示に従って指定されたURLから情報を取得します。
                - 指定されたURLからテキストとリンクを取得するために提供された関数を使用します。
                - ユーザーによってURLが提供されていない場合は、日本語版Wikipediaから関連するページを検索します。
                - リンクページに必要な文書がない場合は、さらにリンクされた情報を検索します。
                - 必要な文書が利用可能な場合は、そのテキストをユーザーに提供します。
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,   
            description=description,
            human_input_mode="NEVER",
        )

        # 利用可能な関数についての情報をエージェントに登録します

        # 日本語版Wikipediaから関連するページを検索します。
        search_wikipedia_ja, description = tools["search_wikipedia_ja"]
        wipkipedia_searcher.register_for_llm(description=description)(search_wikipedia_ja)

        # 指定されたURLからテキストとリンクを取得します。
        extract_webpage, description = tools["extract_webpage"]
        wipkipedia_searcher.register_for_llm(description=description)(extract_webpage)

        return wipkipedia_searcher, description

    @classmethod
    def __create_file_checker(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        
        # ファイルチェッカー
        description = "指定されたファイルが存在するかどうかを確認します。"
        file_checker = ConversableAgent(
            "file_checker",
            system_message="""
                ファイルチェッカー：指定されたファイルが存在するかどうかを確認します。
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # 指定されたURLからテキストとリンクを取得します。
        func, description = tools["check_file"]
        file_checker.register_for_llm(description=description)(func)
        return file_checker, description

    # 現在時刻を取得するエージェントを作成
    @classmethod
    def __create_current_time(cls, autogen_pros: AutoGenProps, tools: dict[str, tuple[Callable, str]]):
        # 現在時刻を取得するエージェント
        description = "現在時刻を取得します。"
        current_time = ConversableAgent(
            "current_time",
            system_message="""
                現在時刻を取得します。
                """,
            llm_config=autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # 現在時刻を取得する関数を登録
        func, description = tools["get_current_time"]
        current_time.register_for_llm(description=description)(func)
        return current_time, description
