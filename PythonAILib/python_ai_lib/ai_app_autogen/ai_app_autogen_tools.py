import json

from typing import Annotated, Callable
from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor # type: ignore

import wikipedia
from selenium import webdriver
from selenium.webdriver.edge.service import Service
from selenium.webdriver.edge.options import Options
from webdriver_manager.microsoft import EdgeChromiumDriverManager

from ai_app_openai.ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps, VectorSearchParameter
from ai_app_langchain.ai_app_langchain_util import LangChainUtil
from ai_app_langchain.langchain_vector_db import LangChainVectorDB
from ai_app_file.ai_app_file_util import FileUtil, ExcelUtil


class AutoGenTools:
    def __init__(self):
        pass

    def create_vector_search(self, openai_props: OpenAIProps, vector_db_props_list: list[VectorDBProps]) -> Callable[[str], list[str]]:
        def vector_search(query: Annotated[str, "検索対象の文字列"]) -> list[str]:
            params:VectorSearchParameter = VectorSearchParameter(openai_props, vector_db_props_list, query)
            result = LangChainVectorDB.vector_search(params)
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

    def create_list_files_in_directory(self) -> Callable[[str], list[str]]:
        def list_files_in_directory(directory_path: Annotated[str, "ディレクトリパス"]) -> list[str]:
            import os
            files = os.listdir(directory_path)
            return files
        return list_files_in_directory
        
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
        
