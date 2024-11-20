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
from ai_app_file.ai_app_file_util import FileUtil, ExcelUtil


class AutoGenTools:
    def __init__(self, openai_props: OpenAIProps, vector_db_props_list: list[VectorDBProps]):
        self.OpenAIProps = openai_props
        self.VectorDBPropsList = vector_db_props_list
        # 関数名と関数生成関数のペアを保持する辞書
        self.function_generators = {
            "create_vector_search": self.create_vector_search,
            "search_wikipedia_ja": self.create_search_wikipedia_ja,
            "list_files_in_directory": self.create_list_files_in_directory,
            "extract_text_from_file": self.create_extract_text_from_file,
            "extract_webpage": self.create_extract_webpage_function,
            "get_html": self.create_get_html_function,
            "get_text_from_html": self.create_get_text_from_html_function,
            "get_urls_from_html": self.create_get_urls_from_html_function,
            "get_urls_from_text": self.create_get_urls_from_text_function,
            "save_tools": self.create_save_tools_function,
            "file_checker": self.create_check_file_function,
            "save_text_file": self.create_save_text_file_function
        }

    def create_vector_search(self) -> tuple[Callable[[str], list[str]], str]:
        def vector_search(query: Annotated[str, "検索対象の文字列"]) -> list[str]:
            params:VectorSearchParameter = VectorSearchParameter(self.OpenAIProps, self.VectorDBPropsList, query)
            result = LangChainVectorDB.vector_search(params)
            # resultからdocumentsを取得
            documents = result.get("documents",[])
            # documentsから各documentのcontentを取得
            result = [doc.get("content","") for doc in documents]
            return result

        return vector_search , "指定されたテキストをベクトル検索し、関連する文書を返す関数です。"

    def create_search_wikipedia_ja(self) -> tuple[Callable[[str, int], list[str]], str]:
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

    def create_list_files_in_directory(self) -> tuple[Callable[[str], list[str]], str]:
        def list_files_in_directory(directory_path: Annotated[str, "ディレクトリパス"]) -> list[str]:
            import os
            files = os.listdir(directory_path)
            return files
        return list_files_in_directory, "指定されたディレクトリ内のファイル一覧を返す関数です。"
        
    def create_extract_text_from_file(self) -> tuple[Callable[[str], str]]:
        # ファイルからテキストを抽出する関数を生成
        def extract_file(file_path: Annotated[str, "ファイルパス"]) -> str:
            # 一時ファイルからテキストを抽出
            text = FileUtil.extract_text_from_file(file_path)
            return text
        return extract_file, "指定されたファイルからテキストを抽出する関数です。"
    
    def create_check_file_function(self) -> tuple[Callable[[str], bool], str]:
        def check_file(file_path: Annotated[str, "ファイルパス"]) -> bool:
            # ファイルが存在するかチェック
            import os
            check_file = os.path.exists(file_path)
            return check_file
        
        return check_file, "指定されたファイルが存在するかチェックする関数です。"

    def create_extract_webpage_function(self) -> tuple[Callable[[str], list[str]], str]:
        def extract_webpage(url: Annotated[str, "テキストとリンク抽出対象のWebページのURL"]) -> Annotated[tuple[str, list[tuple[str, str]]], "ページテキスト,リンク(aタグのhref属性とリンクテキスト)のリスト"]:
            driver = self.create_web_driver()
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

    def create_get_html_function(self) -> tuple[Callable[[str], str], str]:
        def get_html(url: Annotated[str, "URL"]) -> str:
            driver = self.create_web_driver()
            driver.get(url)
            # ページが完全にロードされるのを待つ（必要に応じて明示的に待機条件を設定）
            driver.implicitly_wait(10)
            # ページ全体のHTMLを取得
            page_html = driver.page_source
            driver.close()
            return page_html
        return get_html, "指定されたURLのWebページのHTMLを取得する関数です。"

    # レンダリング結果のTextを取得する
    def create_get_text_from_html_function(self) -> tuple[Callable[[str], str], str]:
        def get_text_from_html(html: Annotated[str, "HTMLのソース"]) -> str:
            from bs4 import BeautifulSoup
            soup = BeautifulSoup(html, "html.parser")
            text = soup.get_text()
            return text
        return get_text_from_html , "指定されたHTMLのソースからテキストを抽出する関数です。"

    # リンク一覧を取得する
    def create_get_urls_from_html_function(self) -> tuple[Callable[[str], list[str]], str]:
        def get_urls_from_html(html: Annotated[str, "HTMLのソース"]) -> list[str]:
            # HTMLからURLを抽出
            from bs4 import BeautifulSoup
            soup = BeautifulSoup(html, "html.parser")
            urls = [a.get("href") for a in soup.find_all("a")]
            return urls
        return get_urls_from_html, "指定されたHTMLのソースからリンクを抽出する関数です。"

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
    
    def create_get_urls_from_text_function(self) -> tuple[Callable[[str], list[str]], str]:
        def get_urls_from_text(text: Annotated[str, "テキスト文字列"]) -> list[str]:
            # テキストからURLを抽出
            import re
            urls = re.findall(r"https?://\S+", text)
            return urls
        return get_urls_from_text, "指定されたテキストからURLを抽出する関数です。"

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
                self.function_generators[name] = func
    
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

    def create_save_text_file_function(self) -> tuple[Callable[[str, str, str], None], str]:
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


    def create_save_tools_function(self) -> tuple[Callable[[str, str, str], None], str]:
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


