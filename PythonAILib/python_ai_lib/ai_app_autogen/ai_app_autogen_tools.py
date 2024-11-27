import json

from typing import Annotated, Callable

import wikipedia
from selenium import webdriver
from selenium.webdriver.edge.service import Service
from selenium.webdriver.edge.options import Options
from webdriver_manager.microsoft import EdgeChromiumDriverManager

from ai_app_openai.ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps, VectorSearchParameter
from ai_app_langchain.langchain_vector_db import LangChainVectorDB
from ai_app_file.ai_app_file_util import FileUtil
from ai_app_autogen.ai_app_autogen_props import AutoGenProps


class AutoGenTools:
    def __init__(self, autogen_props: AutoGenProps, tools_dict: list[dict]):
        self.autogen_props: AutoGenProps = autogen_props
        self.tools: dict[str, tuple[Callable, str]] = {}

        self.tools.update(AutoGenToolGenerator.create_default_tools(autogen_props))

        self.tools.update(AutoGenToolGenerator.create_tools_from_definition(tools_dict))


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
from ai_app_autogen.ai_app_autogen_props import AutoGenProps

class AutoGenToolGenerator:
    def __init__(self, autogen_props: AutoGenProps):
        self.autogen_pros = autogen_props

    @classmethod
    def create_tool(cls, name: str, content: str):
        # contentから関数オブジェクトを作成する。
        exec(content)
        return locals()[name]

    @classmethod
    def create_tools_from_definition(cls, data_list: list[dict]) -> dict[str, tuple[Callable, str]]:
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
    def create_definition_from_tools(cls, tools: dict[str, tuple[Callable, str]]) -> list[dict]:
        import inspect
        data_list = []
        for name, (func, desc) in tools.items():
            data = {
                "name": name,
                "description": desc,
                "content": inspect.getsource(func)
            }
            data_list.append(data)
        return data_list

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
        def vector_search(query: Annotated[str, "String to search for"]) -> list[str]:
            params: VectorSearchParameter = VectorSearchParameter(openai_props, vector_db_props_list, query)
            result = LangChainVectorDB.vector_search(params)
            # Retrieve documents from result
            documents = result.get("documents", [])
            # Extract content of each document from documents
            result = [doc.get("content", "") for doc in documents]
            return result

        return vector_search, "This function performs a vector search on the specified text and returns the related documents."

    @classmethod
    def __create_search_wikipedia_ja(cls) -> tuple[Callable[[str, str, int], list[str]], str]:
        def search_wikipedia_ja(query: Annotated[str, "String to search for"], lang: Annotated[str, "Language of Wikipedia"], num_results: Annotated[int, "Maximum number of results to display"]) -> list[str]:

            # Use the Japanese version of Wikipedia
            wikipedia.set_lang(lang)
            
            # Retrieve search results
            search_results = wikipedia.search(query, results=num_results)
            
            result_texts = []
            # Display the top results
            for i, title in enumerate(search_results):
            
                print(f"Result {i + 1}: {title}")
                try:
                    # Retrieve the content of the page
                    page = wikipedia.page(title)
                    print(page.content[:500])  # Display the first 500 characters
                    text = f"Title:\n{title}\n\nContent:\n{page.content}\n"
                    result_texts.append(text)
                except wikipedia.exceptions.DisambiguationError as e:
                    print(f"Disambiguation: {e.options}")
                except wikipedia.exceptions.PageError:
                    print("Page not found.")
                print("\n" + "-"*50 + "\n")
            return result_texts
        return search_wikipedia_ja, "This function searches Wikipedia with the specified keywords and returns related articles."

    @classmethod
    def __create_list_files_in_directory(cls) -> tuple[Callable[[str], list[str]], str]:
        def list_files_in_directory(directory_path: Annotated[str, "Directory path"]) -> list[str]:
            import os
            files = os.listdir(directory_path)
            return files
        return list_files_in_directory, "This function returns a list of files in the specified directory."

    @classmethod
    def __create_extract_text_from_file(cls) -> tuple[Callable[[str], str]]:
        # Generates a function to extract text from a file
        def extract_file(file_path: Annotated[str, "File path"]) -> str:
            # Extract text from a temporary file
            text = FileUtil.extract_text_from_file(file_path)
            return text
        return extract_file, "This function extracts text from the specified file."

    @classmethod
    def __create_check_file_function(cls) -> tuple[Callable[[str], bool], str]:
        def check_file(file_path: Annotated[str, "File path"]) -> bool:
            # Check if the file exists
            import os
            check_file = os.path.exists(file_path)
            return check_file
        
        return check_file, "This function checks if the specified file exists."

    @classmethod
    def __create_extract_webpage_function(cls) -> tuple[Callable[[str], list[str]], str]:
        def extract_webpage(url: Annotated[str, "URL of the web page to extract text and links from"]) -> Annotated[tuple[str, list[tuple[str, str]]], "Page text, list of links (href attribute and link text from <a> tags)"]:
            driver = cls.__create_web_driver()
            # Wait for the page to fully load (set explicit wait conditions if needed)
            driver.implicitly_wait(10)
            # Retrieve HTML of the web page and extract text and links
            driver.get(url)
            # Get the entire HTML of the page
            page_html = driver.page_source

            from bs4 import BeautifulSoup
            soup = BeautifulSoup(page_html, "html.parser")
            text = soup.get_text()
            # Retrieve href attribute and text from <a> tags
            urls: list[tuple[str, str]] = [(a.get("href"), a.get_text()) for a in soup.find_all("a")]
            driver.close()
            return text, urls
        return extract_webpage, "This function extracts text and links from the specified URL of a web page."

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

    # Generate a function to search with the specified keywords using the DuckDuckGo API and return related articles
    @classmethod
    def __create_search_duckduckgo(cls) -> tuple[Callable, str]:
        from duckduckgo_search import DDGS
        ddgs = DDGS()
        def search_duckduckgo(query: Annotated[str, "String to search for"], site: Annotated[str, "Site to search within. Leave blank if no site is specified"], num_results: Annotated[int, "Maximum number of results to display"]) -> Annotated[list[tuple[str, str, str]], "(Title, URL, Body) list"]:
            try:
                # Retrieve DuckDuckGo search results
                if site:
                    query = f"{query} site:{site}"

                print(f"Search query: {query}")

                results_dict = ddgs.text(
                    keywords=query,            # Search keywords
                    region='jp-jp',            # Region. For Japan: "jp-jp", No specific region: "wt-wt"
                    safesearch='off',          # Safe search OFF->"off", ON->"on", Moderate->"moderate"
                    timelimit=None,            # Time limit. None for no limit, past day->"d", past week->"w", past month->"m", past year->"y"
                    max_results=num_results    # Number of results to retrieve
                )

                results = []
                for result in results_dict:
                    # title, href, body
                    title = result.get("title", "")
                    href = result.get("href", "")
                    body = result.get("body", "")
                    print(f'Title: {title}, URL: {href}, Body: {body[:100]}')
                    results.append((title, href, body))

                return results
            except Exception as e:
                print(e)
                import traceback
                traceback.print_exc()
                return []
            
        return search_duckduckgo, "This function searches DuckDuckGo with the specified keywords and returns related articles."

    @classmethod
    def __create_save_text_file_function(cls) -> tuple[Callable[[str, str, str], None], str]:
        def save_text_file(name: Annotated[str, "File name"], dirname: Annotated[str, "Directory name"], text: Annotated[str, "Text data to save"]) -> Annotated[bool, "Save result"]:
            # Save in the specified directory
            try:
                import os
                if not os.path.exists(dirname):
                    os.makedirs(dirname)
                path = os.path.join(dirname, name)
                with open(path, "w", encoding="utf-8") as f:
                    f.write(text)
                # Check if the file exists
                return os.path.exists(path)
            except Exception as e:
                print(e)
                return False

        return save_text_file, "This function saves text data as a file."

    @classmethod
    def __create_save_tools_function(cls) -> tuple[Callable[[str, str, str], None], str]:
        def save_tools(name: Annotated[str, "Function name"], description: Annotated[str, "Function description"], code: Annotated[str, "Function code"], dirname: Annotated[str, "Directory name for saving"]) -> Annotated[bool, "Save result"]:
            # Generate JSON string
            func_dict = {
                "name": name,
                "description": description,
                "content": code
            }
            # Save in the specified directory
            try:
                import os
                if not os.path.exists(dirname):
                    os.makedirs(dirname)
                filename = os.path.join(dirname, f"{name}.json")
                with open(filename, "w", encoding="utf-8") as f:
                    json.dump(func_dict, f, ensure_ascii=False, indent=4)
                return True
            except Exception as e:
                print(e)
                return False

        return save_tools, "This function saves Python code as a JSON file for AutoGen tools."

    # Generate a function that returns the current time in the format yyyy/mm/dd (Day) hh:mm:ss
    @classmethod
    def __create_get_current_time(cls) -> tuple[Callable, str]:
        from datetime import datetime
        def get_current_time() -> str:
            now = datetime.now()
            return now.strftime("%Y/%m/%d (%a) %H:%M:%S")
        return get_current_time, "This function returns the current time in the format yyyy/mm/dd (Day) hh:mm:ss."
