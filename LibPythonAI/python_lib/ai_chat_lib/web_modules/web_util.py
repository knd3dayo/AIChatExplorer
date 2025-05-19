from typing import Any, Union
from typing import Annotated
import json

from selenium import webdriver
from selenium.webdriver.edge.service import Service
from selenium.webdriver.edge.options import Options
from webdriver_manager.microsoft import EdgeChromiumDriverManager
from ai_chat_lib.file_modules.file_util import FileUtil

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)


class WebUtil:
    web_request_name = "web_request"
    @classmethod
    def get_web_request_objects(cls, request_dict: dict) -> dict:
        '''
        {"context": {"web_request": {}}}の形式で渡される
        '''
        # contextを取得
        request:dict = request_dict.get(cls.web_request_name, None)
        if not request:
            raise ValueError("request is not set.")
        return request
    
    @classmethod
    def extract_webpage_api(cls,request_json: str):
        # request_jsonからrequestを作成
        request_dict: dict = json.loads(request_json)
        # web_requestを取得
        request = WebUtil.get_web_request_objects(request_dict)

        url = request.get("url", None)
        text, urls = WebUtil.extract_webpage(url)
        result: dict[str, Any] = {}
        result["output"] = text
        result["urls"] = urls
        return result

    @classmethod
    def extract_webpage(cls, url: Annotated[str, "URL of the web page to extract text and links from"]) -> Annotated[tuple[str, list[tuple[str, str]]], "Page text, list of links (href attribute and link text from <a> tags)"]:
        """
        This function extracts text and links from the specified URL of a web page.
        """
        # Edgeドライバをセットアップ
        web_driver = cls._create_web_driver()

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

    edge_driver_path: Union[str, None] = None

    @classmethod
    def _create_web_driver(cls) -> webdriver.Edge:
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
        if cls.edge_driver_path is None:
            cls.edge_driver_path = EdgeChromiumDriverManager().install()
            logger.debug(f"EdgeDriverのインストール場所: {cls.edge_driver_path}")
        # Edgeドライバをセットアップ
        return webdriver.Edge(service=Service(cls.edge_driver_path), options=edge_options)

