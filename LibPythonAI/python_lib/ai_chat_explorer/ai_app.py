import os, json
from typing import Any, Callable, AsyncGenerator, Union
import tempfile
import base64

from ai_chat_explorer.openai_modules import OpenAIProps, OpenAIClient, RequestContext
from ai_chat_explorer.langchain_modules import LangChainChatParameter, LangChainUtil, LangChainVectorDB, VectorSearchParameter
from ai_chat_explorer.file_modules import ExcelUtil, FileUtil
from ai_chat_explorer.autogen_modules import AutoGenProps
from ai_chat_explorer.db_modules import VectorDBItem, VectorSearchParameter

from selenium import webdriver
from selenium.webdriver.edge.service import Service
from selenium.webdriver.edge.options import Options
from webdriver_manager.microsoft import EdgeChromiumDriverManager
from typing import Annotated


########################
# ファイル関連
########################
# ファイルのMimeTypeを取得する
def get_mime_type(filename):
    return FileUtil.get_mime_type(filename)

# Excelのシート名一覧を取得する
def get_sheet_names(filename):
    return ExcelUtil.get_sheet_names(filename)

# Excelのシートのデータを取得する
def extract_text_from_sheet(filename, sheet_name):
    return ExcelUtil.extract_text_from_sheet(filename, sheet_name)


# ファイルからテキストを抽出する
def extract_text_from_file(filename:str) -> str:
    return FileUtil.extract_text_from_file(filename)

# base64形式のデータからテキストを抽出する
def extract_base64_to_text(base64_data:str, extension:str) -> str:
    # サイズが0の場合は空文字を返す
    if len(base64_data) == 0:
        return ""

    # base64からバイナリデータに変換
    base64_data_bytes = base64.b64decode(base64_data)

    # 拡張子の指定。extensionが空の場合は設定しない.空でない場合は"."を先頭に付与
    suffix = "" if extension == "" else "." + extension
    # base64データから一時ファイルを生成
    with tempfile.NamedTemporaryFile(mode="wb", delete=False, suffix=suffix) as temp:
        temp.write(base64_data_bytes)
        temp_path = temp.name
        temp.close()
        # 一時ファイルからテキストを抽出
        text = FileUtil.extract_text_from_file(temp_path)
        # 一時ファイルを削除
        os.remove(temp_path)
        return text

edge_driver_path: Union[str, None] = None

def _create_web_driver():
    # Edgeドライバをセットアップ
    # ヘッドレスモードのオプションを設定
    edge_options = Options()
    edge_options.add_argument("--incognito")
    edge_options.add_argument("--headless")
    edge_options.add_argument('--blink-settings=imagesEnabled=false')
    edge_options.add_argument('--disable-extensions')
    edge_options.add_argument('--disable-gpu')
    edge_options.add_argument('--no-sandbox')
    edge_options.add_argument('--disable-dev-shm-usage')
    edge_options.add_argument('--enable-chrome-browser-cloud-management')    
    global edge_driver_path
    # Edgeドライバをインストールし、インストール場所を取得
    if edge_driver_path is None:
        edge_driver_path = EdgeChromiumDriverManager().install()
        print(f"EdgeDriverのインストール場所: {edge_driver_path}")
    # Edgeドライバをセットアップ
    return webdriver.Edge(service=Service(edge_driver_path), options=edge_options)

def extract_webpage(url: Annotated[str, "URL of the web page to extract text and links from"]) -> Annotated[tuple[str, list[tuple[str, str]]], "Page text, list of links (href attribute and link text from <a> tags)"]:
    """
    This function extracts text and links from the specified URL of a web page.
    """
    # Edgeドライバをセットアップ
    web_driver = _create_web_driver()

    # Wait for the page to fully load (set explicit wait conditions if needed)
    web_driver.implicitly_wait(10)
    # Retrieve HTML of the web page and extract text and links
    web_driver.get(url)
    # Get the entire HTML of the page
    page_html = web_driver.page_source

    from bs4 import BeautifulSoup
    soup = BeautifulSoup(page_html, "html.parser")
    text = soup.get_text()
    sanitized_text = FileUtil.sanitize_text(text)
    # Retrieve href attribute and text from <a> tags
    urls: list[tuple[str, str]] = [(a.get("href"), a.get_text()) for a in soup.find_all("a")] # type: ignore
    web_driver.close()
    return sanitized_text, urls
    
########################
# openai関連
########################
def run_openai_chat(openai_props: OpenAIProps, vector_db_items: list[VectorDBItem], request_context: RequestContext, request: dict) -> dict[str, str]:
    openai_client = OpenAIClient(openai_props)
    # ベクトル検索関数
    def vector_search(query: str) -> dict:
        from ai_chat_explorer.db_modules import VectorSearchParameter
        from ai_chat_explorer.langchain_modules.langchain_vector_db import LangChainVectorDB
        params:VectorSearchParameter = VectorSearchParameter(openai_props, vector_db_items, query)
        return LangChainVectorDB.vector_search(params)
    # vector_db_itemsが空の場合はNoneを設定
    vector_search_function: Union[Callable, None] = None if len(vector_db_items) == 0 else vector_search
    return openai_client.run_openai_chat(request_context, request, vector_search_function)

def openai_embedding(openai_props: OpenAIProps, input_text: str):
    openai_client = OpenAIClient(openai_props)
    return openai_client.openai_embedding(input_text)

def list_openai_models(openai_props: OpenAIProps):
    client = OpenAIClient(openai_props)
    return client.list_openai_models()

def get_token_count(openai_props: OpenAIProps, input_text: str):
    client = OpenAIClient(openai_props)
    return client.get_token_count(input_text)

########################
# autogen関連
########################
async def run_autogen_chat(autogen_props: AutoGenProps,input_text: str) -> AsyncGenerator:

    # run_group_chatを実行
    async for message in autogen_props.run_autogen_chat(input_text):
        yield message

########################
# langchain関連
########################

def vector_search(params:VectorSearchParameter) -> dict:
    result = LangChainVectorDB.vector_search(params)
    return result

def run_langchain_chat(openai_props: OpenAIProps, vector_db_items: list[VectorDBItem], params:LangChainChatParameter) -> dict:
    # langchan_chatを実行
    result = LangChainUtil.langchain_chat(openai_props, vector_db_items, params)
    return result

# vector db関連
def delete_collection(openai_props: OpenAIProps, vector_db_items: list[VectorDBItem]):
    # vector_db_itemsからVectorDBItemを取得
    # LangChainVectorDBを生成
    for vector_db_props in vector_db_items:
        vector_db = LangChainVectorDB.get_vector_db(openai_props, vector_db_props)
        # delete_collectionを実行
        vector_db.delete_collection()

def update_collection(openai_props: OpenAIProps, vector_db_items: list[VectorDBItem]):
    pass

def delete_embeddings(openai_props: OpenAIProps ,vector_db_props: VectorDBItem):
    vector_db = LangChainVectorDB.get_vector_db(openai_props, vector_db_props)
    for entry in vector_db_props.VectorMetadataList:
        vector_db.delete_document(entry.source_id)

def update_embeddings(openai_props: OpenAIProps ,vector_db_props: VectorDBItem):
    # LangChainVectorDBを生成
    vector_db = LangChainVectorDB.get_vector_db(openai_props, vector_db_props)
    for entry in vector_db_props.VectorMetadataList:
        vector_db.update_document(entry)

# export_to_excelを実行する
def export_to_excel(filePath, dataJson):
    # dataJsonをdictに変換
    data = json.loads(dataJson)
    # export_to_excelを実行
    print(data)
    ExcelUtil.export_to_excel(filePath, data.get("rows",[]))

# import_from_excelを実行する
def import_from_excel(filePath) -> dict:
    # import_to_excelを実行
    data = ExcelUtil.import_from_excel(filePath)
    # 結果用のdictを生成
    result = {}
    result["rows"] = data
    return result

# テスト用
def hello_world():
    return "Hello World"
