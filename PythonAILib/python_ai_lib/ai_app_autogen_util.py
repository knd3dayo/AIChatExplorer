import json

from typing import Annotated, Callable
from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor
import autogen 

import wikipedia
from selenium import webdriver
from selenium.webdriver.edge.service import Service
from selenium.webdriver.edge.options import Options
from webdriver_manager.microsoft import EdgeChromiumDriverManager

from ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db_util import VectorDBProps, VectorSearchParameter
from ai_app_langchain_util import LangChainUtil
from ai_app_file_util import FileUtil, ExcelUtil

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
            llm_config_entry["api_version"] = openAIProps.AzureOpenAICompletionVersion
            if openAIProps.OpenAICompletionBaseURL:
                llm_config_entry["base_url"] = openAIProps.OpenAICompletionBaseURL
            else:
                llm_config_entry["base_url"] = openAIProps.AzureOpenAIEndpoint
        
        # llm_configに追加
        config_list.append(llm_config_entry)
        self.llm_config["config_list"] = config_list

class AutoGenAgents:
    def __init__(self, llm_config: dict, executor: LocalCommandLineCodeExecutor, work_dir: str, auto_execute_code: bool = False):
        self.llm_config = llm_config
        self.executor = executor
        self.work_dir = work_dir
        self.__create_default_agents()

        # コード実行者が自動的にコードを実行するようにするかどうか
        self.auto_execute_code = auto_execute_code

        # 結果を出力するファイル
        self.output_file = None

        self.autogen_tools = AutoGenTools()


    def __create_default_agents(self):
        # グループチャットの議題提供者
        self.user_proxy = autogen.UserProxyAgent(
            system_message="""
                あなたはグループチャットの議題提供者です。
                グループチャットの管理者に議題を提示します。
                グループチャットの管理者から会議を終了する旨のメッセージが届いたら、[会議を終了]と返信します。
                """,
            name="user_proxy",
            human_input_mode="NEVER",
            is_termination_msg=lambda msg: "[会議を終了]" in msg["content"].lower(),
            code_execution_config=False,
            llm_config=self.llm_config,
        )
        # グループチャットの管理者
        self.chat_admin_agent = autogen.UserProxyAgent(
            name="chat-admin",
            system_message=f"""
                あなたはグループチャットの管理者です。
                他のエージェントに指示を出し、彼らが協力してタスクを完了するのを手伝います。
                グループチャットの内容をまとめて議事録(4000文字程度,各エージェントの発言の概要が時系列でわかるもの)と結論を出す責任があります。
                グループチャットでの結論が出ましたら、議事録と結論を議題提供者に伝えたのち、[会議を終了]と返信します。
                """
        ,
            llm_config=self.llm_config,
            code_execution_config=False,
            is_termination_msg=lambda msg: "[会議を終了]" in msg["content"].lower(),
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
        # Azure関連のドキュメント検索者
        self.azure_document_searcher = None

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
        self.chat_admin_agent.register_for_execution(name="vector_search") (vector_search)
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
        extract_text_from_file = self.autogen_tools.create_extract_text_from_file()
        params = self.autogen_tools.create_extract_text_from_file_params()
        self.chat_admin_agent.register_for_execution(name="extract_text_from_file") (extract_text_from_file)
        self.file_extractor.register_for_llm(**params)(extract_text_from_file)


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

        # 利用可能な関数の情報をエージェントに登録する

        # Wikipedia(日本語版)から検索対象文字列に関連するページを検索します。
        search_wikipedia_ja = self.autogen_tools.create_search_wikipedia_ja()
        self.web_searcher.register_for_llm( 
            **self.autogen_tools.create_search_wipedia_ja_params()
            )(search_wikipedia_ja)
        self.chat_admin_agent.register_for_execution(name="search_wikipedia_ja") (search_wikipedia_ja)

        # 指定されたURLのテキストとリンクを取得します。
        extract_webpage = AutoGenTools().create_extract_webpage_function()
        self.web_searcher.register_for_llm(
                name="extract_webpage",
                description="Seleniumを使用して指定されたURLのHTMLソースを取得したのち、テキストとリンク一覧を抽出します。")(extract_webpage)
        self.chat_admin_agent.register_for_execution(name="extract_webpage") (extract_webpage)


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

        # 指定されたURLのテキストとリンクを取得します。
        extract_webpage = AutoGenTools().create_extract_webpage_function()
        self.azure_document_searcher.register_for_llm(
                name="extract_webpage",
                description="Seleniumを使用して指定されたURLのHTMLソースを取得したのち、テキストとリンク一覧を抽出します。")(extract_webpage)
        self.chat_admin_agent.register_for_execution(name="extract_webpage") (extract_webpage)

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

    def create_search_wipedia_ja_params(self) -> dict:
        params = {}
        params["name"]="search_wikipedia_ja",
        params["description"]="""
            wikipedia(日本語版)から検索対象文字列に関連するページを検索します。
            検索結果からページのタイトルと本文を抽出してリストとしてページごとのリストとして返します
            """
        return params

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

    def create_extract_text_from_file_params(self) -> dict:
        params = {}
        #   name="extract_text_from_file",
        #   description="ファイルからテキストを抽出します。"
        params["name"]="extract_text_from_file",
        params["description"]="ファイルからテキストを抽出します。"
        return params
        
    def create_extract_text_from_file(self) -> Callable[[str], str]:
        # ファイルからテキストを抽出する関数を生成
        def extract_file(file_path: Annotated[str, "ファイルパス"]) -> str:
            # 一時ファイルからテキストを抽出
            text = FileUtil.extract_text_from_file(file_path)
            return text
        return extract_file
    
    def create_extract_webpage_function(self) -> Callable[[str], list[str]]:
        def extract_webpage(url: Annotated[str, "テキストとリンク抽出対象のWebページのURL"]) -> Annotated[tuple[str, list[str]], "テキストとリンク"]:
            driver = self.create_web_driver()
            # webページのHTMLを取得して、テキストとリンクを抽出
            driver.get(url)
            # ページが完全にロードされるのを待つ（必要に応じて明示的に待機条件を設定）
            driver.implicitly_wait(10)
            # ページ全体のHTMLを取得
            page_html = driver.page_source

            from bs4 import BeautifulSoup
            soup = BeautifulSoup(page_html, "html.parser")
            text = soup.get_text()
            urls = [a.get("href") for a in soup.find_all("a")]
            driver.close()
            return text, urls
        return extract_webpage

    def create_get_html_function(self) -> Callable[[str], str]:
        def get_html(url: Annotated[str, "URL"]) -> str:
            driver = self.create_web_driver()
            driver.get(url)
            # ページが完全にロードされるのを待つ（必要に応じて明示的に待機条件を設定）
            driver.implicitly_wait(10)
            # ページ全体のHTMLを取得
            page_html = driver.page_source
            driver.close()
            return page_html
        return get_html

    # レンダリング結果のTextを取得する
    def create_get_text_from_html_function(self) -> Callable[[str], str]:
        def get_text_from_html(html: Annotated[str, "HTMLのソース"]) -> str:
            from bs4 import BeautifulSoup
            soup = BeautifulSoup(html, "html.parser")
            text = soup.get_text()
            return text
        return get_text_from_html    

    # リンク一覧を取得する
    def create_get_urls_from_html_function(self) -> Callable[[str], list[str]]:
        get_urls_from_html = self.create_get_urls_from_html_function()
        def get_urls_from_html(html: Annotated[str, "HTMLのソース"]) -> list[str]:
            # HTMLからURLを抽出
            from bs4 import BeautifulSoup
            soup = BeautifulSoup(html, "html.parser")
            urls = [a.get("href") for a in soup.find_all("a")]
            return urls
        return get_urls_from_html

    def create_web_driver(self):
        # ヘッドレスモードのオプションを設定
        edge_options = Options()
        edge_options.add_argument("--headless")
        edge_options.add_argument("--disable-gpu")
        edge_options.add_argument("--no-sandbox")
        edge_options.add_argument("--disable-dev-shm-usage")

        # Edgeドライバをセットアップ
        driver = webdriver.Edge(service=Service(EdgeChromiumDriverManager().install()), options=edge_options)
        return driver
    
    def create_get_urls_from_text_function(self) -> Callable[[str], list[str]]:
        def get_urls_from_text(text: Annotated[str, "テキスト文字列"]) -> list[str]:
            # テキストからURLを抽出
            import re
            urls = re.findall(r"https?://\S+", text)
            return urls
        return get_urls_from_text
        
