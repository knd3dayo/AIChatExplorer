from dotenv import load_dotenv
import os, json
from typing import Any
from typing import Annotated, Callable
from ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db_util import VectorDBProps, VectorSearchParameter
from ai_app_langchain_util import LangChainUtil
import wikipedia

class AutoGenProps:
    def __init__(self, openAIProps: OpenAIProps):
        if openAIProps is None:
            raise ValueError("openAIProps is None")
        
        # 基本設定
        self.llm_config = {}
        config_list = []
        llm_config_entry = {}

        llm_config_entry: dict = {}
        llm_config_entry["model"] = openAIProps.OpenAICompletionModel
        llm_config_entry["api_key"] = openAIProps.OpenAIKey

        # AzureOpenAIの場合
        if openAIProps.AzureOpenAI:
            llm_config_entry["api_type"] = "azure"
            llm_config_entry["version"] = openAIProps.AzureOpenAICompletionVersion
            if openAIProps.OpenAICompletionBaseURL:
                llm_config_entry["base_url"] = openAIProps.OpenAICompletionBaseURL
            else:
                llm_config_entry["base_url"] = openAIProps.AzureOpenAIEndpoint
        
        # llm_configに追加
        config_list.append(llm_config_entry)
        self.llm_config["config_list"] = config_list

class AutoGenTools:
    def __init__(self):
        pass

    def create_vector_search(self, openai_props: OpenAIProps, vector_db_props_list: list[VectorDBProps]) -> Callable[[str], list[str]]:
        def vector_search(query: Annotated[str, "検索対象の文字列"]) -> list[str]:
            params:VectorSearchParameter = VectorSearchParameter(openai_props, vector_db_props_list, query)
            result = LangChainUtil.run_vector_search(params)
            # resultからdocumentsを取得
            documents = result.get("documents",[])
            # documentsから各documentのcontentを取得
            result = [doc.get("content","") for doc in documents]
            return result

        return vector_search

    def create_search_wikipedia_ja(self) -> Callable[[str, int], list[str]]:
        def search_wikipedia_ja(query: Annotated[str, "検索対象の文字列"], num_results: Annotated[int, "表示する結果の最大数"]) -> list[str]:

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
        return search_wikipedia_ja

from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor # type: ignore
import autogen  # type: ignore

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
            code_execution_config={"executor": self.executor},
            llm_config=self.llm_config,
            max_consecutive_auto_reply=5,
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
        # Wikipedia検索者
        self.wikipedia_searcher = None
        # ベクトル検索者
        self.vector_searcher = None

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
                * pipインストール以外には、システムへの変更は行いません。また、外部のファイルへのアクセスは読み取りのみ許可されます。
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
        # コード実行者が自動的にコードを実行するようにするかどうか
        # self.auto_execute_code == True の場合はhuman_input_mode="NEVER"とする
        if self.auto_execute_code:
            self.code_writer_agent.human_input_mode = "NEVER"
        else:
            self.code_writer_agent.human_input_mode = "ALWAYS" 

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

    def enable_wikipedia_searcher(self):
        
        # Wikipedia検索者
        self.wikipedia_searcher = ConversableAgent(
            "wikipedia-searcher",
            system_message="""
                あなたはWikipedia検索者です。ユーザーの指示に従いWikipediaで情報を検索します。
                提供された関数を用いて、検索した結果、タイトルと本文を表示します。
                """,
            llm_config=self.llm_config,
            code_execution_config={"executor": self.executor},
            human_input_mode="NEVER",
        )
        # 利用可能な関数の情報をエージェントに登録する
        search_wikipedia_ja = AutoGenTools().create_search_wikipedia_ja()
        self.wikipedia_searcher.register_for_llm(
                name="search_wikipedia_ja",
                description="Wikipedia(日本語版)から検索対象文字列に関連するページを検索します。検索結果からページのタイトルと本文を抽出してリストとしてページごとのリストとして返します"
                )(search_wikipedia_ja)
        self.user_proxy.register_for_execution(name="search_wikipedia_ja") (search_wikipedia_ja)

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
            code_execution_config={"executor": self.executor},
            human_input_mode="NEVER",
        )
        # 利用可能な関数の情報をエージェントに登録する
        vector_search = AutoGenTools().create_vector_search(openAIProps, vector_db_props_list)
        self.user_proxy.register_for_execution(name="vector_search") (vector_search)
        self.vector_searcher.register_for_llm(
                name="vector_search",
                description="ベクトルデータベースから検索対象文字列に関連するページを検索します。検索結果からページのタイトルと本文を抽出してリストとしてページごとのリストとして返します"
                )(vector_search)

    def set_output_file(self, output_file: str):
        self.output_file = output_file

    def execute_group_chat(self, initial_message: str, max_round: int):
        # エージェントのうち、Noneでないものを指定
        agents: list[ConversableAgent] = [self.user_proxy, self.chat_admin_agent]
        for agent in [self.code_writer_agent, self.code_execution_agent, self.wikipedia_searcher, self.vector_searcher]:
            if agent:
                agents.append(agent)
        
        # グループチャットを開始
        groupchat = autogen.GroupChat(
            admin_name="chat_admin_agent",
            agents=agents,
            messages=[], 
            send_introductions=True,  
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

