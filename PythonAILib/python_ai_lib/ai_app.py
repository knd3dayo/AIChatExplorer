import os, json
from typing import Any, Tuple, Generator
import tempfile
import base64

from ai_app_openai import OpenAIProps, OpenAIClient
from ai_app_vector_db import VectorDBProps, VectorSearchParameter, ContentUpdateOrDeleteRequestParams
from ai_app_langchain import LangChainChatParameter, LangChainUtil, LangChainVectorDB
from ai_app_file import ExcelUtil, FileUtil
from ai_app_autogen import AutoGenGroupChatWrapper, AutoGenProps, AutoGenNormalChatWrapper

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
    # 拡張子の指定。extensionが空の場合は設定しない.空でない場合は"."を先頭に付与
    suffix = "" if extension == "" else "." + extension
    # base64データから一時ファイルを生成
    with tempfile.NamedTemporaryFile(mode="wb", delete=False, suffix=suffix) as temp:
        # base64からバイナリデータに変換
        base64_data_bytes = base64.b64decode(base64_data)
        temp.write(base64_data_bytes)
        temp_path = temp.name
        temp.close()
        # 一時ファイルからテキストを抽出
        text = FileUtil.extract_text_from_file(temp_path)
        # 一時ファイルを削除
        os.remove(temp_path)
        return text

def extract_webpage(url: Annotated[str, "URL of the web page to extract text and links from"]) -> Annotated[tuple[str, list[tuple[str, str]]], "Page text, list of links (href attribute and link text from <a> tags)"]:
    """
    This function extracts text and links from the specified URL of a web page.
    """
    # ヘッドレスモードのオプションを設定
    edge_options = Options()
    edge_options.add_argument("--headless")
    edge_options.add_argument("--disable-gpu")
    edge_options.add_argument("--no-sandbox")
    edge_options.add_argument("--disable-dev-shm-usage")

    # Edgeドライバをセットアップ
    driver = webdriver.Edge(service=Service(EdgeChromiumDriverManager().install()), options=edge_options)
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
    
########################
# openai関連
########################
def run_openai_chat(openai_props: OpenAIProps, request: dict) -> Tuple[str, str]:
    openai_client = OpenAIClient(openai_props)
    return openai_client.run_openai_chat(request)

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
def run_autogen_group_chat(autogen_props: AutoGenProps, vector_db_items: list[VectorDBProps] ,input_text: str) -> Generator[Any, None, None]:
    # autogen_propsからAgentsを取得
    from ai_app_autogen.ai_app_autogen_util import AutoGenChatUtil
    autogen_util = AutoGenChatUtil(autogen_props, vector_db_items)
    # AutoGenGroupChatを生成

    autogen_chat = AutoGenGroupChatWrapper(autogen_util.init_agent, autogen_util.agents)
    # run_group_chatを実行
    result = autogen_chat.run_chat(input_text, 2, 10)
    return result

def run_autogen_normal_chat(autogen_props: AutoGenProps, vector_db_items: list[VectorDBProps] ,input_text: str) -> Generator[Any, None, None]:
    # autogen_propsからAgentsを取得
    from ai_app_autogen.ai_app_autogen_util import AutoGenChatUtil
    autogen_util = AutoGenChatUtil(autogen_props, vector_db_items)
    # AutoGenGroupChatを生成

    autogen_chat = AutoGenNormalChatWrapper(autogen_util.init_agent, autogen_util.agents)
    # run_group_chatを実行
    result = autogen_chat.run_chat(input_text, 2)
    return result

########################
# VectorDBCatalog関連
########################
from ai_app_vector_db.ai_app_vector_db_catalog import VectorDBCatalog
def get_catalogs(catalb_db_url: str, vector_db_url: str) -> list[dict]:
    vector_db_catalog = VectorDBCatalog(catalb_db_url)
    result_list = vector_db_catalog.get_catalogs(vector_db_url)
    return result_list

def get_catalog(catalb_db_url: str, vector_db_url: str, collection: str) -> dict:
    vector_db_catalog = VectorDBCatalog(catalb_db_url)
    result_dict = vector_db_catalog.get_catalog(vector_db_url, collection)
    return result_dict

def update_catalog(catalb_db_url: str, vector_db_url: str, collection: str, description: str):
    vector_db_catalog = VectorDBCatalog(catalb_db_url)
    vector_db_catalog.update_catalog(vector_db_url, collection, description)

def delete_catalog(catalb_db_url: str, vector_db_url: str, collection: str):
    vector_db_catalog = VectorDBCatalog(catalb_db_url)
    id = vector_db_catalog.get_catalog(vector_db_url, collection).get("id", None)
    if id is not None:
        vector_db_catalog.delete_catalog(id)

########################
# langchain関連
########################

def vector_search(params:VectorSearchParameter) -> dict:
    result = LangChainVectorDB.vector_search(params)
    return result

def run_langchain_chat(openai_props: OpenAIProps, vector_db_items: list[VectorDBProps], params:LangChainChatParameter) -> dict:
    # langchan_chatを実行
    result = LangChainUtil.langchain_chat(openai_props, vector_db_items, params)
    return result

# vector db関連
def delete_collection(openai_props: OpenAIProps, vector_db_items: list[VectorDBProps]):
    # vector_db_itemsからVectorDBPropsを取得
    # LangChainVectorDBを生成
    for vector_db_props in vector_db_items:
        vector_db = LangChainVectorDB.get_vector_db(openai_props, vector_db_props)
        # delete_collectionを実行
        vector_db.delete_collection()

def delete_index(params: ContentUpdateOrDeleteRequestParams):
    vector_db_props = params.vector_db_props_list[0]
    vector_db = LangChainVectorDB.get_vector_db(params.openai_props, vector_db_props)
    vector_db.delete_document(params.id)

def update_content_index(params: ContentUpdateOrDeleteRequestParams):
    # LangChainVectorDBを生成
    vector_db_props = params.vector_db_props_list[0]
    vector_db = LangChainVectorDB.get_vector_db(params.openai_props, vector_db_props)
    vector_db.update_document(params)

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
