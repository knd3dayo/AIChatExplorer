from typing import Any, Annotated, Union, Callable
from autogen_core.code_executor import ImportFromModule
from autogen_core.tools import FunctionTool
from autogen_agentchat.messages import BaseChatMessage
import asyncio

def search_wikipedia_ja(query: Annotated[str, "String to search for"], lang: Annotated[str, "Language of Wikipedia"], num_results: Annotated[int, "Maximum number of results to display"]) -> list[str]:
    """
    This function searches Wikipedia with the specified keywords and returns related articles.
    """
    import wikipedia # type: ignore[import]
    import ai_chat_lib.log_modules.log_settings as log_settings
    logger = log_settings.getLogger(__name__)

    # Use the Japanese version of Wikipedia
    wikipedia.set_lang(lang)
    
    # Retrieve search results
    search_results = wikipedia.search(query, results=num_results)
    
    result_texts = []
    # Display the top results
    for i, title in enumerate(search_results):
    
        logger.debug(f"Result {i + 1}: {title}")
        try:
            # Retrieve the content of the page
            page = wikipedia.page(title)
            logger.debug(page.content[:500])  # Display the first 500 characters
            text = f"Title:\n{title}\n\nContent:\n{page.content}\n"
            result_texts.append(text)
        except wikipedia.exceptions.DisambiguationError as e:
            logger.debug(f"Disambiguation: {e.options}")
        except wikipedia.exceptions.PageError:
            logger.debug("Page not found.")
        logger.debug("\n" + "-"*50 + "\n")
    return result_texts

def list_files_in_directory(directory_path: Annotated[str, "Directory path"]) -> list[str]:
    """
    This function returns a list of files in the specified directory.
    """
    import os
    files = os.listdir(directory_path)
    return files

async def extract_file(file_path: Annotated[str, "File path"]) -> str:
    """
    This function extracts text from the specified file.
    """
    from ai_chat_lib.file_modules.file_util import FileUtil
    # Extract text from a temporary file
    text = await FileUtil.extract_text_from_file_async(file_path)
    return text

def check_file(file_path: Annotated[str, "File path"]) -> bool:
    """
    This function checks if the specified file exists.
    """
    # Check if the file exists
    import os
    check_file = os.path.exists(file_path)
    return check_file

# Edge用のWebドライバーを毎回ダウンロードしなくてもよいようにグローバル変数化
edge_driver = None # type: ignore
def extract_webpage(url: Annotated[str, "URL of the web page to extract text and links from"]) -> Annotated[tuple[str, list[tuple[str, str]]], "Page text, list of links (href attribute and link text from <a> tags)"]:
    """
    This function extracts text and links from the specified URL of a web page.
    """
    from selenium import webdriver
    from selenium.webdriver.edge.service import Service
    from selenium.webdriver.edge.options import Options
    from webdriver_manager.microsoft import EdgeChromiumDriverManager

    # ヘッドレスモードのオプションを設定
    edge_options = Options()
    edge_options.add_argument("--headless")
    edge_options.add_argument("--disable-gpu")
    edge_options.add_argument("--no-sandbox")
    edge_options.add_argument("--disable-dev-shm-usage")

    global edge_driver
    if not edge_driver:
        # Edgeドライバをセットアップ
        edge_driver = Service(EdgeChromiumDriverManager().install())

    driver = webdriver.Edge(service=edge_driver, options=edge_options)
    
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
    urls: list[tuple[str, str]] = [(a.get("href"), a.get_text()) for a in soup.find_all("a")] # type: ignore
    driver.close()
    return text, urls

def search_duckduckgo(query: Annotated[str, "String to search for"], num_results: Annotated[int, "Maximum number of results to display"], site: Annotated[str, "Site to search within. Leave blank if no site is specified"] = "") -> Annotated[list[tuple[str, str, str]], "(Title, URL, Body) list"]:
    """
    This function searches DuckDuckGo with the specified keywords and returns related articles.
    ユーザーから特定のサイト内での検索を行うように指示を受けた場合、siteパラメータを使用して検索を行います。
    """
    from duckduckgo_search import DDGS
    ddgs = DDGS()

    import ai_chat_lib.log_modules.log_settings as log_settings
    logger = log_settings.getLogger(__name__)

    try:
        # Retrieve DuckDuckGo search results
        if site:
            query = f"{query} site:{site}"

        logger.debug(f"Search query: {query}")

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
            logger.debug(f'Title: {title}, URL: {href}, Body: {body[:100]}')
            results.append((title, href, body))

        return results
    except Exception as e:
        logger.error(e)
        import traceback
        logger.error(traceback.format_exc())
        return []

def save_text_file(path: Annotated[str, "File path"], text: Annotated[str, "Text data to save"]) -> Annotated[str, "Saveed file path. if failed, return empty string."]:
    """
    This function saves text data as a file.
    """
    
    # Save in the specified directory
    import os
    if  os.path.exists(os.path.dirname(path)) == False:
        os.makedirs(os.path.dirname(path))
    with open(path, "w", encoding="utf-8") as f:
        f.write(text)
    # Check if the file exists
    if os.path.exists(path):
        return path
    else:
        return ""

def get_current_time() -> str:
    """
    This function returns the current time in the format yyyy/mm/dd (Day) hh:mm:ss.
    """
    from datetime import datetime
    now = datetime.now()
    return now.strftime("%Y/%m/%d (%a) %H:%M:%S")

def arxiv_search(query: str, max_results: int = 2) -> list:  # type: ignore[type-arg]
    """
    Search Arxiv for papers and return the results including abstracts.
    """
    import arxiv # type: ignore[import]

    client = arxiv.Client()
    search = arxiv.Search(query=query, max_results=max_results, sort_by=arxiv.SortCriterion.Relevance)

    results = []
    for paper in client.results(search):
        results.append(
            {
                "title": paper.title,
                "authors": [author.name for author in paper.authors],
                "published": paper.published.strftime("%Y-%m-%d"),
                "abstract": paper.summary,
                "pdf_url": paper.pdf_url,
            }
        )

    # # Write results to a file
    # with open('arxiv_search_results.json', 'w') as f:
    #     json.dump(results, f, indent=2)

    return results

def vector_search(query: Annotated[str, "String to search for"]) -> list[str]:
    """
    This function performs a vector search on the specified text and returns the related documents.
    if vector_db_items is empty, return None
    """
    global autogen_props
    from ai_chat_lib.langchain_modules.langchain_util import LangChainUtil
    from ai_chat_lib.autogen_modules import AutoGenProps
    props : AutoGenProps = autogen_props # type: ignore
    if not props.vector_search_requests:
        return []

    # vector_db_prop_listの各要素にinput_textを設定
    for vector_search_request in props.vector_search_requests:
        vector_search_request.query = query

    search_results = LangChainUtil.vector_search(props.openai_props, props.vector_search_requests)
    # Retrieve documents from result
    documents = search_results.get("documents", [])
    # Extract content of each document from documents
    result = [doc.get("content", "") for doc in documents]
    return result

def global_vector_search(query: Annotated[str, "String to search for"]) -> list[str]:
    """
    メインDBに登録されている全てのデータについて、ベクトル検索を行い、関連するドキュメントを返します。
    """
    global autogen_props
    from ai_chat_lib.db_modules import MainDB
    from ai_chat_lib.langchain_modules.langchain_util import LangChainUtil
    from ai_chat_lib.autogen_modules import AutoGenProps

    props : AutoGenProps = autogen_props # type: ignore
    main_db = main_db = MainDB(props.autogen_db_path) 
    main_vector_db_item = main_db.get_vector_db_by_id(props.main_vector_db_id)
    vector_db_item_list = [] if main_vector_db_item is None else [main_vector_db_item]
    # vector_db_prop_listの各要素にinput_textを設定
    for request in props.vector_search_requests:
        request.query = query

    search_results = LangChainUtil.vector_search(props.openai_props, props.vector_search_requests)
    # Retrieve documents from result
    documents = search_results.get("documents", [])
    # Extract content of each document from documents
    result = [doc.get("content", "") for doc in documents]
    return result

def past_chat_history_vector_search(query: Annotated[str, "String to search for"]) -> list[str]:
    """
    過去のチャット履歴に関連するドキュメントを検索します。
    """
    global autogen_props
    from ai_chat_lib.db_modules import MainDB
    from ai_chat_lib.langchain_modules.langchain_util import LangChainUtil
    from ai_chat_lib.autogen_modules import AutoGenProps

    props : AutoGenProps = autogen_props # type: ignore
    main_db = main_db = MainDB(props.autogen_db_path) 
    main_vector_db_item = main_db.get_vector_db_by_id(props.main_vector_db_id)
    if main_vector_db_item is None:
        raise ValueError("main_vector_db_id is not set.")
    if props.chat_history_folder_id is None:
        raise ValueError("chat_history_folder_id is not set.")

    main_vector_db_item.FolderId = props.chat_history_folder_id

    vector_db_item_list = [] if main_vector_db_item is None else [main_vector_db_item]
    # vector_db_prop_listの各要素にinput_textを設定
    for request in props.vector_search_requests:
        request.query = query
    search_results = LangChainUtil.vector_search(props.openai_props, props.vector_search_requests)
    # Retrieve documents from result
    documents = search_results.get("documents", [])
    # Extract content of each document from documents
    result = [doc.get("content", "") for doc in documents]
    return result


# execute_agent
# エージェントを実行する関数
async def execute_agent(
        agent_name: Annotated[str, "Agent name"], input_text: Annotated[str, "Input text"],
        ) -> Annotated[str, "Output text"]:
    """
    This function executes the specified agent with the input text and returns the output text.
    First argument: agent name, second argument: input text.
    - Agent name: Specify the name of the agent as the Python function name.
    - Input text: The text data to be processed by the agent.
    """
    import ai_chat_lib.log_modules.log_settings as log_settings
    logger = log_settings.getLogger(__name__)
    
    global autogen_props 
    from ai_chat_lib.db_modules import MainDB, AutogenTools
    from ai_chat_lib.autogen_modules import AutoGenProps
    props : AutoGenProps = autogen_props # type: ignore
    autogen_db_path = props.autogen_db_path
    main_db = MainDB(autogen_db_path)
    agent = main_db.get_autogen_agent(agent_name)
    if agent is None:
        return "The specified agent does not exist."
    
    output_text = ""
    # run_agent関数を使用して、エージェントを実行
    import uuid
    trace_id = str(uuid.uuid4())
    async for message in props.run_agent(agent_name, input_text):
        if isinstance(message, BaseChatMessage):
            message_str = f"{message.source}(in agent selector:{trace_id}): {message.content}" # type: ignore
            logger.debug(message_str)
            output_text += message_str + "\n"   
    return output_text

# エージェント一覧を取得する関数
def list_agents() -> Annotated[list[dict[str, str]], "List of registered agents, each containing 'name' and 'description'"]:
    """
    This function retrieves a list of registered agents from the database.
    """
    global autogen_props
    from ai_chat_lib.db_modules import MainDB, AutogenTools
    from ai_chat_lib.autogen_modules import AutoGenProps
    props : AutoGenProps = autogen_props # type: ignore
    autogen_db_path = props.autogen_db_path

    main_db = MainDB(autogen_db_path)
    agents = main_db.get_autogen_agent_list()
    agent_list = []
    for agent in agents:
        agent_list.append({"name": agent.name, "description": agent.description})
    return agent_list

import ast

def extract_imports(code) -> list[Union[str, ImportFromModule]]:
    """
    Pythonコードからimport文を抽出する
    引数:
    - code: Pythonコード（文字列）

    戻り値: import文のリスト
    """
    
    tree = ast.parse(code)
    imports: list[Union[str, ImportFromModule]] = []
    for node in tree.body:
        if isinstance(node, ast.Import):
            for alias in node.names:
                imports.append(alias.name)
        elif isinstance(node, ast.ImportFrom):
            module = node.module if node.module else ""
            for alias in node.names:
                imports.append(ImportFromModule(module=module, imports=[alias.name]))
    return imports


def move_imports_to_function(code, function_name):
    """
    グローバルなimport文を特定の関数内に移動する
    引数:
    - code: Pythonコード（文字列）
    - function_name: 移動先の関数名

    戻り値: 更新されたコード（文字列）
    """
    tree = ast.parse(code)

    # import文を収集
    imports = []
    body_without_imports = []

    for node in tree.body:
        if isinstance(node, ast.Import) or isinstance(node, ast.ImportFrom):
            imports.append(node)
        else:
            body_without_imports.append(node)

    # 指定した関数を探す
    function_found = False
    for node in body_without_imports:
        if isinstance(node, ast.FunctionDef) and node.name == function_name:
            # 関数の先頭にimport文を追加
            node.body = imports + node.body
            function_found = True

    if not function_found:
        raise ValueError(f"関数 '{function_name}' が見つかりませんでした。")

    # 残りのコード（importを削除済み）の組み直し
    tree.body = body_without_imports

    # 再構築されたコードを文字列として返す
    updated_code = ast.unparse(tree)
    return updated_code


def list_tool_agents() -> Annotated[list[dict[str, str]], "List of registered tools, each containing 'name' and 'description'"]:
    """
    This function retrieves a list of registered tool agents.
    """
    import ai_chat_lib.log_modules.log_settings as log_settings
    logger = log_settings.getLogger(__name__)

    logger.debug('start list_tool_agents')
    global autogen_props
    from ai_chat_lib.db_modules import MainDB, AutogenTools
    from ai_chat_lib.autogen_modules import AutoGenProps
    props : AutoGenProps = autogen_props # type: ignore
    autogen_db_path = props.autogen_db_path
    main_db = MainDB(autogen_db_path)
    tools = main_db.get_autogen_tool_list()

    tool_descption_list = []
    for agent in tools:
        tool_descption_list.append({"name": agent.name, "description": agent.description, "path": agent.path})
        logger.debug(f"tool name:{agent.name}, description:{agent.description}")

    return tool_descption_list

# FunctionToolを実行するエージェントをwork_agentsに追加
def register_tool_agent(name: Annotated[str, "Function name"], doc: Annotated[str, "Function documentation"], 
                         code: Annotated[str, "Python Code"]) -> Annotated[str, "Message indicating that the tool agent has been registered"]:
    """
    This function creates a FunctionTool object with the specified function name, documentation, and Python code.
    引数で与えられたPythonコードからexec関数を使用して関数を作成し、FunctionToolオブジェクトを作成します。
    作成したFunctionToolを実行するためのエージェントを作成し、tool_agentsに追加します。
    """
    import ai_chat_lib.log_modules.log_settings as log_settings
    logger = log_settings.getLogger(__name__)

    global autogen_props

    import os
    from ai_chat_lib.db_modules import MainDB, AutogenTools
    from ai_chat_lib.autogen_modules import AutoGenProps
    props : AutoGenProps = autogen_props # type: ignore

    # toolsディレクトリがない場合は作成
    python_file_path = os.path.join(props.tool_dir_path, f"{name}.py")

    # toolsディレクトリに{name}.pyとして保存
    with open(python_file_path, "w", encoding="utf-8") as f:
        f.write(code)

    # main_dbに登録
    main_db = MainDB(props.autogen_db_path)
    main_db.update_autogen_tool(AutogenTools({"name": name, "description": doc, "path": python_file_path}))
    
    message = f"Registered tool agent: {name}"
    logger.debug(message)
    return message

# エージェントを実行する関数
async def execute_tool_agent(
        agent_name: Annotated[str, "Agent name"], initial_message: Annotated[str, "Input text"],
        ) -> Annotated[str, "Output text"]:
    """
    This function executes the specified agent with the input text and returns the output text.
    First argument: agent name, second argument: input text.
    - Agent name: Specify the name of the agent as the Python function name.
    - Input text: The text data to be processed by the agent.
    """
    import ai_chat_lib.log_modules.log_settings as log_settings
    logger = log_settings.getLogger(__name__)
    global autogen_props

    import os
    from ai_chat_lib.db_modules import MainDB, AutogenTools
    from ai_chat_lib.autogen_modules import AutoGenProps
    props : AutoGenProps = autogen_props # type: ignore

    try:
        # agent_nameに対応するエージェントを取得
        logger.debug('start execute_tool_agent')
        tool_list = [ tool for tool in list_tool_agents() if tool["name"] == agent_name]
        tool = tool_list[0] if len(tool_list) > 0 else None

        if tool is None:
            return f"The specified agent does not exist. Agent name: {agent_name}"
        name = tool["name"]
        path = tool["path"]
        # ファイルから関数を読み込む
        func = props.load_function(path, name)
        output_text = ""
        # エージェントを作成
        agent = props.create_agent(
            name=name,
            description=f"{name}ツール実行エージェント",
            system_message=f"""
            あなたは{name}を実行するエージェントです。{name}の説明：{func.__doc__}
            """,
            tools=[
                FunctionTool(func, func.__doc__, name=name) # type: ignore
            ]
        )

        import uuid
        # 識別子を生成
        trace_id = str(uuid.uuid4())
        # run_agent関数を使用して、エージェントを実行
        async for message in agent.run_stream(task=initial_message):
            if isinstance(message, BaseChatMessage):
                message_str = f"{message.source}(in tool agent selector:{trace_id}): {message.content}" # type: ignore
                print(message_str)
                output_text += message_str + "\n"

        return output_text
    except Exception as e:
        import traceback
        logger.error(f"Error in execute_tool_agent: {e}")
        logger.error(traceback.format_exc())
        return f"Error: {e}"

    
