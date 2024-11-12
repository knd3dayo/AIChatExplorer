from dotenv import load_dotenv
import os, json
from typing import Any
from typing import Annotated, Callable
from ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db_util import VectorDBProps, VectorSearchParameter
from ai_app_langchain_util import LangChainUtil
import wikipedia
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

    def create_extract_text_from_file(self) -> Callable[[str], str]:
        # ファイルからテキストを抽出する関数を生成
        def extract_file(file_path: Annotated[str, "ファイルパス"]) -> str:
            # 一時ファイルからテキストを抽出
            text = FileUtil.extract_text_from_file(file_path)
            return text
        return extract_file
    
    def create_get_html_function(self) -> Callable[[str], str]:
        def get_html(url: Annotated[str, "URL"]) -> str:
            # URLからHTMLを取得
            import requests
            try:
                response = requests.get(url)
                response.raise_for_status()  # ステータスコードが200番台でない場合、例外を発生させる
                return response.text
            except requests.exceptions.RequestException as e:
                print(f"Error retrieving {url}: {e}")
                return None
        return get_html

    def create_get_urls_from_html_function(self) -> Callable[[str], list[str]]:
        def get_urls_from_html(html: Annotated[str, "HTML文字列"]) -> list[str]:
            # HTMLからURLを抽出
            from bs4 import BeautifulSoup
            soup = BeautifulSoup(html, "html.parser")
            urls = [a.get("href") for a in soup.find_all("a")]
            return urls
        return get_urls_from_html
    
    def create_get_urls_from_text_function(self) -> Callable[[str], list[str]]:
        def get_urls_from_text(text: Annotated[str, "テキスト文字列"]) -> list[str]:
            # テキストからURLを抽出
            import re
            urls = re.findall(r"https?://\S+", text)
            return urls
        return get_urls_from_text
        
