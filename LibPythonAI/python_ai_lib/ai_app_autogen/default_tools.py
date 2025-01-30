import json

from typing import Annotated




def search_wikipedia_ja(query: Annotated[str, "String to search for"], lang: Annotated[str, "Language of Wikipedia"], num_results: Annotated[int, "Maximum number of results to display"]) -> list[str]:
    """
    This function searches Wikipedia with the specified keywords and returns related articles.
    """
    import wikipedia

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

def list_files_in_directory(directory_path: Annotated[str, "Directory path"]) -> list[str]:
    """
    This function returns a list of files in the specified directory.
    """
    import os
    files = os.listdir(directory_path)
    return files

def extract_file(file_path: Annotated[str, "File path"]) -> str:
    """
    This function extracts text from the specified file.
    """
    from ai_app_file.ai_app_file_util import FileUtil
    # Extract text from a temporary file
    text = FileUtil.extract_text_from_file(file_path)
    return text

def check_file(file_path: Annotated[str, "File path"]) -> bool:
    """
    This function checks if the specified file exists.
    """
    # Check if the file exists
    import os
    check_file = os.path.exists(file_path)
    return check_file

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

def search_duckduckgo(query: Annotated[str, "String to search for"], num_results: Annotated[int, "Maximum number of results to display"], site: Annotated[str, "Site to search within. Leave blank if no site is specified"] = "") -> Annotated[list[tuple[str, str, str]], "(Title, URL, Body) list"]:
    """
    This function searches DuckDuckGo with the specified keywords and returns related articles.
    ユーザーから特定のサイト内での検索を行うように指示を受けた場合、siteパラメータを使用して検索を行います。
    """
    from duckduckgo_search import DDGS
    ddgs = DDGS()
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

def save_text_file(name: Annotated[str, "File name"], dirname: Annotated[str, "Directory name"], text: Annotated[str, "Text data to save"]) -> Annotated[str, "Saveed file path. if failed, return empty string."]:
    """
    This function saves text data as a file.
    """
    
    # Save in the specified directory
    try:
        import os
        if not os.path.exists(dirname):
            os.makedirs(dirname)
        path = os.path.join(dirname, name)
        with open(path, "w", encoding="utf-8") as f:
            f.write(text)
        # Check if the file exists
        if os.path.exists(path):
            return path
        else:
            return None
    except Exception as e:
        print(e)
        return None

def get_current_time() -> str:
    """
    This function returns the current time in the format yyyy/mm/dd (Day) hh:mm:ss.
    """
    from datetime import datetime
    now = datetime.now()
    return now.strftime("%Y/%m/%d (%a) %H:%M:%S")

# ツール一覧を取得する関数
def list_tools() -> Annotated[list[dict[str, str]], "List of registered tools, each containing 'name' and 'description'"]:
    """
    This function retrieves a list of registered tools from the database.
    """
    global autogen_props
    autogen_db_path = autogen_props.autogen_db_path
    tools_table_name = "tools"
    # sqlite3のDBを開く
    import sqlite3
    conn = sqlite3.connect(autogen_db_path)
    cursor = conn.cursor()
    cursor.execute(f"SELECT * FROM {tools_table_name}")
    # ツール一覧を取得
    tools = cursor.fetchall()
    tool_list = []
    for tool in tools:
        tool_list.append({"name": tool[0], "description": tool[2]})
    conn.close()
    return tool_list


# ツールを登録する関数
def register_tool(
        tool_name: Annotated[str, "Tool name"], tool_description: Annotated[str, "Tool description"], 
        source_path: Annotated[str, "Absolute File Path of the file where the Python function defining the tool is located."], 
        ) -> tuple[Annotated[bool, "Registration result"], Annotated[str, "result message"]]:
    """
    This function saves the specified tools to a sqlite3 database.
    First argument: tool name, second argument: tool description, third argument: tool path.
    - Tool name: Specify the name of the tool as the Python function name.
    - Tool description: Provide a description of what the tool does.
    - Tool path: The absolute path to the file where the tool is saved.First argument: tool name, second argument: tool description, third argument: tool path.
    If the tool with the same name already exists, return tuple(True, "The tool with the same name already exists.").
    If the registration is successful, return tuple(True, "The tool has been successfully registered.").
    If the registration fails, return tuple(False, "Failed to register the tool.").
    """

    global autogen_props
    autogen_db_path = autogen_props.autogen_db_path
    tools_table_name = "tools"
    # sqlite3のDBを開く
    import sqlite3
    conn = sqlite3.connect(autogen_db_path)
    cursor = conn.cursor()
    cursor.execute(f"SELECT * FROM {tools_table_name} WHERE name = ?", (tool_name,))
    # 既に同じ名前のツールが登録されているか確認
    if cursor.fetchone() is not None:
        cursor.close()
        conn.close()
        return True, "The tool with the same name already exists."

    # ツールを登録
    cursor = conn.cursor()
    cursor.execute(f"INSERT INTO {tools_table_name} (name, path, description) VALUES (?, ?, ?)", (tool_name, source_path, tool_description))
    conn.commit()
    cursor.close()

    # ツール登録結果を確認
    cursor = conn.cursor()
    cursor.execute(f"SELECT * FROM {tools_table_name} WHERE name = ?", (tool_name,))
    tool = cursor.fetchone()
    cursor.close()
    conn.close()
    if tool is None:
        return False, "Failed to register the tool."
    return True, "The tool has been successfully registered."
    
# エージェント一覧を取得する関数
def list_agents() -> Annotated[list[dict[str, str]], "List of registered agents, each containing 'name' and 'description'"]:
    """
    This function retrieves a list of registered agents from the database.
    """
    global autogen_props
    autogen_db_path = autogen_props.autogen_db_path
    agents_table_name = "agents"
    # sqlite3のDBを開く
    import sqlite3
    conn = sqlite3.connect(autogen_db_path)
    cursor = conn.cursor()
    cursor.execute(f"SELECT * FROM {agents_table_name}")
    # エージェント一覧を取得
    agents = cursor.fetchall()
    agent_list = []
    for agent in agents:
        agent_list.append({"name": agent[0], "description": agent[1]})
    conn.close()
    return agent_list

# register_agent
# sqlite3のagentsテーブルにエージェントを登録する関数
# なお、agentsのテーブル定義は以下の通り
# "CREATE TABLE IF NOT EXISTS agents (name TEXT PRIMARY KEY, description TEXT, system_message TEXT, code_execution BOOLEAN, llm_config_name TEXT, tool_names TEXT)"
def register_agent(
        agent_name: Annotated[str, "Agent name"], description: Annotated[str, "Agent description"],
        system_message: Annotated[str, "System message"], code_execution: Annotated[bool, "Code execution flag"],
        llm_config_name: Annotated[str, "LLM config name"], tool_names: Annotated[list[str], "List of tool names to be executed"],
        ) -> tuple[Annotated[bool, "Registration result"], Annotated[str, "result message"]]:
    """
    This function saves the specified agents to a sqlite3 database.
    First argument: agent name, second argument: agent description, third argument: system message, fourth argument: code execution flag, fifth argument: LLM config name, sixth argument: list of tool names
    - Agent name: Specify the name of the agent as the Python function name.
    - Agent description: Provide a description of what the agent does.
    - System message: Provide a system message that describes the agent's behavior.
    - Code execution flag: Specify whether the agent executes code.
    - LLM config name: Specify the name of the LLM configuration to use.
    - List of tool names: Specify the names of the tools to be executed by the agent.
    If the agent with the same name already exists, return tuple(True, "The agent with the same name already exists.").
    If the registration is successful, return tuple(True, "The agent has been successfully registered.").
    If the registration fails, return tuple(False, "Failed to register the agent.").
    """

    global autogen_props
    autogen_db_path = autogen_props.autogen_db_path
    agents_table_name = "agents"
    # sqlite3のDBを開く
    import sqlite3
    conn = sqlite3.connect(autogen_db_path)
    cursor = conn.cursor()
    cursor.execute(f"SELECT * FROM {agents_table_name} WHERE name = ?", (agent_name,))
    # 既に同じ名前のエージェントが登録されているか確認
    if cursor.fetchone() is not None:
        cursor.close()
        conn.close()
        return True, "The agent with the same name already exists."
    
    # エージェントを登録
    cursor = conn.cursor()
    cursor.execute(f"INSERT INTO {agents_table_name} (name, description, system_message, code_execution, llm_config_name, tool_names) VALUES (?, ?, ?, ?, ?, ?)", (agent_name, description, system_message, code_execution, llm_config_name, ",".join(tool_names)))
    conn.commit()
    cursor.close()

    # エージェント登録結果を確認
    cursor = conn.cursor()
    cursor.execute(f"SELECT * FROM {agents_table_name} WHERE name = ?", (agent_name,))
    agent = cursor.fetchone()
    cursor.close()
    conn.close()
    if agent is None:
        return False, "Failed to register the agent."
    return True, "The agent has been successfully registered."

# register_agent
# register_toolで登録したツールを使用するエージェントを登録する関数
def register_tool_agent(
        tool_name: Annotated[str, "Tool name"], tool_description: Annotated[str, "Tool description"], 
        source_path: Annotated[str, "Absolute File Path of the file where the Python function defining the tool is located."], 
        ) -> tuple[Annotated[bool, "Registration result"], Annotated[str, "result message"]]:
    """
    This function saves the agent that use the specified tool to a sqlite3 database.
    First argument: tool name.
    - Tool name: Specify the name of the tool as the Python function name.
    Second argument: tool description.
    - Tool description: Provide a description of what the tool does.
    Third argument: tool path.
    - Tool path: The absolute path to the file where the tool is saved.
    The agent name is [tool_name]_agent, and the agent description is the same as the tool description.
    If the agent with the same name already exists, return tuple(True, "The agent with the same name already exists.").
    If the agent registration is successful, return tuple(True, "The agent has been successfully registered.").
    If the agent registration fails, return tuple(False, "Failed to register the agent.").
    """
    agent_name = f"{tool_name}_agent"

    code_execution = False
    llm_config_name = "default"

    # list_agents関数を使用して、登録済みのエージェントを取得
    # この関数はexecで実行されるため、ファイル内の関数を直接呼び出すことはできません
    # execを用いて別関数をロードします.
    global autogen_props
    default_tools_path = autogen_props.default_tools_path
    locals_dict = locals()
    with open(default_tools_path, "r", encoding="utf-8") as f:
        exec(f.read(), globals(), locals_dict)
    
    agents = locals_dict["list_agents"]()
    agent_names = [agent["name"] for agent in agents if agent["name"] == agent_name]
    # 登録済みのエージェント名に指定されたエージェント名が含まれているか確認. 
    if agent_name in agent_names:
        return True, "The agent with the same name already exists."
    
    # list_tools関数を使用して、登録済みのツールを取得
    tools = locals_dict["list_tools"]()
    registered_tools = [tool for tool in tools if tool["name"] == tool_name]
    # 登録済みのツール名に指定されたツール名が含まれているか確認
    if tool_name not in registered_tools:
        # ツールが登録されていない場合は登録する
        register_tool = locals_dict["register_tool"]
        result, message = register_tool(tool_name, tool_description, source_path)
        if not result:
            return False, message
        
    tool_description = registered_tools[0]["description"]
    agent_description = f"""This agent executes the fllowing tool:
    Tool name: {tool_name}
    Tool description: {tool_description}
    """
    system_message = agent_description

    register_agent = locals_dict["register_agent"]
    # register_agent関数を使用して、エージェントを登録. 登録に失敗した場合はFalseを返す
    result, message =  register_agent(agent_name, agent_description, system_message, code_execution, llm_config_name, [tool_name])
    if not result:
        return False, message
    
    return True, "The agent has been successfully registered."
    
