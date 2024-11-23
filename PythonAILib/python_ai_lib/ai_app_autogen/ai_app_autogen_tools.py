import json

from typing import Annotated, Callable

import wikipedia
from selenium import webdriver
from selenium.webdriver.edge.service import Service
from selenium.webdriver.edge.options import Options
from webdriver_manager.microsoft import EdgeChromiumDriverManager

from ai_app_openai.ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps, VectorSearchParameter
from ai_app_langchain.langchain_vector_db import LangChainVectorDB
from ai_app_file.ai_app_file_util import FileUtil
from ai_app_autogen.ai_app_autogen_client import AutoGenProps


class AutoGenTools:
    def __init__(self, autogen_props: AutoGenProps, tools_dict: list[dict]):
        self.autogen_props: AutoGenProps = autogen_props
        self.tools: dict[str, tuple[Callable, str]] = {}

        if self.autogen_props.use_system_agent:
            self.tools.update(AutoGenToolGenerator.create_default_tools(autogen_props))

        self.tools.update(AutoGenToolGenerator.create_tools_dict(tools_dict))


    def add_tools(self, tools: dict[str, tuple[Callable, str]]):
        self.tools.update(tools)

 
    # 指定したディレクトリ内の*.jsonファイルを読み込み、関数を生成
    def load_tools(self, dirname: str):
        import os
        # ディレクトリ内のファイル一覧を取得
        files = os.listdir(dirname)
        # JSONファイルのみを抽出
        json_files = [f for f in files if f.endswith(".json")]
        # JSONファイルから関数を生成
        for json_file in json_files:
            with open(os.path.join(dirname, json_file), "r", encoding="utf-8") as f:
                json_str = f.read()
                name, func = self.__create_tool_from_json(json_str)
                # 関数名と関数生成関数のペアを登録
                self.tools[name] = func
    
    # JSON文字列から関数を生成        
    def __create_tool_from_json(self, json_str: str) -> tuple[Callable, str]:
        # JSON文字列を辞書に変換
        func_dict = json.loads(json_str)
        # name, description, content
        name = func_dict.get("name", "")
        description = func_dict.get("description", "")
        content = func_dict.get("content", "")
        # contentから関数を生成
        exec(content)
        func = locals()[name]
        # 関数と説明文を返すgeneratorとなる関数を返す
        def generator():
            return func, description
        return name, generator


from openpyxl import load_workbook
from ai_app_autogen.ai_app_autogen_client import AutoGenProps

class AutoGenToolGenerator:
    def __init__(self, autogen_props: AutoGenProps):
        self.autogen_pros = autogen_props

    @classmethod
    def create_tool(cls, name: str, content: str):
        # contentから関数オブジェクトを作成する。
        exec(content)
        return locals()[name]

    @classmethod
    def create_tools_dict(cls, data_list: list[dict]) -> dict[str, tuple[Callable, str]]:
        function_dict = {}
        for data in data_list:
            name = data['name']
            description = data['description']
            content = data['content']

            # 関数を作成
            function_obj = cls.create_tool(name, content)
            # dictに格納
            function_dict[name] = (function_obj, description)

        # 結果を表示（必要に応じて）
        for name, (func, desc) in function_dict.items():
            print(f'Name: {name}, Description: {desc}, Function: {func}')
        
        return function_dict

    @classmethod
    def create_tools_dict_from_sheet(cls, excel_file) -> dict[str, tuple[Callable, str]]:
        # Excelファイルを開く
        wb = load_workbook(excel_file)
        sheet = wb['tools']

        # ヘッダー行から列名を取得
        headers = [cell.value for cell in sheet[1]]
        name_index = headers.index('name')
        description_index = headers.index('description')
        content_index = headers.index('content')
        human_input_mode_index = headers.index('human_input_mode')
        is_termination_msg_index = headers.index('is_termination_msg')
        code_execution_config_index = headers.index('code_execution_config')
        llm_config_index = headers.index('llm_config')
        system_message_index = headers.index('system_message')

        function_data_list = []

        # 2行目から最終行まで処理を行う
        for row in sheet.iter_rows(min_row=2, values_only=True):
            
            name = row[name_index]  # name列
            description = row[description_index]  # description列
            content = row[content_index]  # content列
            human_input_mode = row[human_input_mode_index]  # human_input_mode列
            is_termination_msg = row[is_termination_msg_index]  # is_termination_msg列
            code_execution_config = row[code_execution_config_index]  # code_execution_config列
            llm_config = row[llm_config_index]  # llm_config列
            system_message = row[system_message_index]  # system_message列

            # function_data_listに格納
            function_data_list.append({
                'name': name, 'description': description, 'content': content,
                'human_input_mode': human_input_mode, 'is_termination_msg': is_termination_msg,
                'code_execution_config': code_execution_config, 'llm_config': llm_config,
                'system_message': system_message
            })

        # create_tools_dictを呼び出し
        return cls.create_tools_dict(function_data_list)

    @classmethod        
    def create_default_tools(cls, autogen_props: AutoGenProps) -> dict[str, tuple[Callable, str]]:
        openai_props = autogen_props.openai_props
        vector_db_props_list = autogen_props.vector_db_items
        # デフォルトのツールを生成
        # 関数名と関数生成関数のペアを保持する辞書
        tools: dict[str, tuple[Callable, str]] = {
            "vector_search": cls.__create_vector_search(openai_props, vector_db_props_list),
            "search_wikipedia_ja": cls.__create_search_wikipedia_ja(),
            "list_files_in_directory": cls.__create_list_files_in_directory(),
            "extract_text_from_file": cls.__create_extract_text_from_file(),
            "extract_webpage": cls.__create_extract_webpage_function(),
            "save_tools": cls.__create_save_tools_function(),
            "check_file": cls.__create_check_file_function(),
            "save_text_file": cls.__create_save_text_file_function(),
            "search_duckduckgo": cls.__create_search_duckduckgo(),
            "get_current_time": cls.__create_get_current_time()
        }
        return tools

    @classmethod
    def __create_vector_search(cls, openai_props: OpenAIProps, vector_db_props_list: list[VectorDBProps]) -> tuple[Callable[[str], list[str]], str]:
        def vector_search(query: Annotated[str, "検索対象の文字列"]) -> list[str]:
            params:VectorSearchParameter = VectorSearchParameter(openai_props, vector_db_props_list, query)
            result = LangChainVectorDB.vector_search(params)
            # resultからdocumentsを取得
            documents = result.get("documents",[])
            # documentsから各documentのcontentを取得
            result = [doc.get("content","") for doc in documents]
            return result

        return vector_search , "指定されたテキストをベクトル検索し、関連する文書を返す関数です。"

    @classmethod
    def __create_search_wikipedia_ja(cls) -> tuple[Callable[[str, int], list[str]], str]:
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
        return search_wikipedia_ja, "指定されたキーワードでWikipediaを検索し、関連する記事を返す関数です。"

    @classmethod
    def __create_list_files_in_directory(cls) -> tuple[Callable[[str], list[str]], str]:
        def list_files_in_directory(directory_path: Annotated[str, "ディレクトリパス"]) -> list[str]:
            import os
            files = os.listdir(directory_path)
            return files
        return list_files_in_directory, "指定されたディレクトリ内のファイル一覧を返す関数です。"
        
    @classmethod
    def __create_extract_text_from_file(cls) -> tuple[Callable[[str], str]]:
        # ファイルからテキストを抽出する関数を生成
        def extract_file(file_path: Annotated[str, "ファイルパス"]) -> str:
            # 一時ファイルからテキストを抽出
            text = FileUtil.extract_text_from_file(file_path)
            return text
        return extract_file, "指定されたファイルからテキストを抽出する関数です。"
    
    @classmethod
    def __create_check_file_function(cls) -> tuple[Callable[[str], bool], str]:
        def check_file(file_path: Annotated[str, "ファイルパス"]) -> bool:
            # ファイルが存在するかチェック
            import os
            check_file = os.path.exists(file_path)
            return check_file
        
        return check_file, "指定されたファイルが存在するかチェックする関数です。"

    @classmethod
    def __create_extract_webpage_function(cls) -> tuple[Callable[[str], list[str]], str]:
        def extract_webpage(url: Annotated[str, "テキストとリンク抽出対象のWebページのURL"]) -> Annotated[tuple[str, list[tuple[str, str]]], "ページテキスト,リンク(aタグのhref属性とリンクテキスト)のリスト"]:
            driver = cls.__create_web_driver()
            # ページが完全にロードされるのを待つ（必要に応じて明示的に待機条件を設定）
            driver.implicitly_wait(10)
            # webページのHTMLを取得して、テキストとリンクを抽出
            driver.get(url)
            # ページ全体のHTMLを取得
            page_html = driver.page_source

            from bs4 import BeautifulSoup
            soup = BeautifulSoup(page_html, "html.parser")
            text = soup.get_text()
            # aタグのhref属性とテキストを取得
            urls : list[tuple[str, str]] = [(a.get("href"), a.get_text()) for a in soup.find_all("a")]
            driver.close()
            return text, urls
        return extract_webpage, "指定されたURLのWebページからテキストとリンクを抽出する関数です。"

    @classmethod
    def __create_web_driver(cls):
        # ヘッドレスモードのオプションを設定
        edge_options = Options()
        edge_options.add_argument("--headless")
        edge_options.add_argument("--disable-gpu")
        edge_options.add_argument("--no-sandbox")
        edge_options.add_argument("--disable-dev-shm-usage")

        # Edgeドライバをセットアップ
        driver = webdriver.Edge(service=Service(EdgeChromiumDriverManager().install()), options=edge_options)
        return driver

    # DuckDuckAgo APIを使用して、指定されたキーワードで検索し、関連する記事を返す関数を生成
    @classmethod
    def __create_search_duckduckgo(cls) -> tuple[Callable, str]:
        from duckduckgo_search import DDGS
        ddgs = DDGS()
        def search_duckduckgo(query: Annotated[str, "検索対象の文字列"], site: Annotated[str, "検索対象のサイト。県債対象サイトの指定がない場合は空文字"], num_results: Annotated[int, "表示する結果の最大数"]) ->  Annotated[list[tuple[str, str,str]], "(タイトル,URL,本文)のリスト"]:
            try:
                # DuckDuckGoの検索結果を取得
                if site:
                    query = f"{query} site:{site}"

                print(f"Search query: {query}")

                results_dict = ddgs.text(
                    keywords=query,            # 検索ワード
                    region='jp-jp',             # リージョン 日本は"jp-jp",指定なしの場合は"wt-wt"
                    safesearch='off',           # セーフサーチOFF->"off",ON->"on",標準->"moderate"
                    timelimit=None,             # 期間指定 指定なし->None,過去1日->"d",過去1週間->"w", 過去1か月->"m",過去1年->"y"
                    max_results=num_results     # 取得件数
                )

                resulst = []
                for result in results_dict:
                    # title, href, body
                    title = result.get("title", "")
                    href = result.get("href", "")
                    body = result.get("body", "")
                    print(f'Title: {title}, URL: {href}, Body: {body[:100]}')
                    resulst.append((title, href, body))

                return resulst
            except Exception as e:
                print(e)
                import traceback
                traceback.print_exc()
                return []
            
        return search_duckduckgo, "指定されたキーワードでDuckDuckGoを検索し、関連する記事を返す関数です。"

    @classmethod
    def __create_save_text_file_function(cls) -> tuple[Callable[[str, str, str], None], str]:
        def save_text_file(name: Annotated[str, "ファイル名"], dirname: Annotated[str, "ディレクトリ名"], text: Annotated[str, "保存するテキストデータ"]) -> Annotated[bool, "保存結果"]:
            # 指定したディレクトリに保存
            try:
                import os
                if not os.path.exists(dirname):
                    os.makedirs(dirname)
                path = os.path.join(dirname, name)
                with open(path , "w", encoding="utf-8") as f:
                    f.write(text)
                # ファイルの存在チェック    
                return os.path.exists(path)

            except Exception as e:
                print(e)
                return False

        return save_text_file, "テキストデータをファイルとして保存する関数です。"

    @classmethod
    def __create_save_tools_function(cls) -> tuple[Callable[[str, str, str], None], str]:
        def save_tools(name: Annotated[str, "関数名"], description: Annotated[str, "関数の説明"], code: Annotated[str, "関数のコード"], dirname: Annotated[str, "保存先ディレクトリ名"]) -> Annotated[bool, "保存結果"]:
            # JSON文字列を生成
            func_dict = {
                "name": name,
                "description": description,
                "content": code
            }
            # 指定したディレクトリに保存
            try:
                import os
                if not os.path.exists(dirname):
                    os.makedirs(dirname)
                filename = os.path.join(dirname, f"{name}.json")
                with open(filename , "w", encoding="utf-8") as f:
                    json.dump(func_dict, f, ensure_ascii=False, indent=4)
                return True
            except Exception as e:
                print(e)
                return False

        return save_tools, "PythonのコードをAutoGenのツール用のJSONファイルとして保存する関数です。"

    # 現在の時刻をyyyy/mm/dd (曜日) hh:mm:ss 形式で返す関数を生成   
    @classmethod
    def __create_get_current_time(cls) -> tuple[Callable, str]:
        from datetime import datetime
        def get_current_time() -> str:
            now = datetime.now()
            return now.strftime("%Y/%m/%d (%a) %H:%M:%S")
        return get_current_time, "現在の時刻をyyyy/mm/dd (曜日) hh:mm:ss 形式で返す関数です。"
