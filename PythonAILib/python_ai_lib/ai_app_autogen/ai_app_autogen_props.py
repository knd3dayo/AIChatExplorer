from autogen.coding import LocalCommandLineCodeExecutor
import venv
from ai_app_openai.ai_app_openai_util import OpenAIProps
# sqlite3
import sqlite3
from autogen import ConversableAgent, GroupChat

class AutoGenProps:

    llm_configs_table_name = "llm_configs"
    agents_table_name = "agents"
    tools_table_name = "tools"
    group_chats_table_name = "group_chats"

    def __init__(self, openai_props: OpenAIProps,  props_dict: dict):
        # OpenAIProps
        self.openai_props = openai_props

        # autogen_db_path
        autogen_db_path = props_dict.get("autogen_db_path", None)
        if autogen_db_path is None:
            raise ValueError("autogen_db_path is None")
        self.autogen_db_path = autogen_db_path

        # work_dir
        work_dir = props_dict.get("work_dir", None)
        if work_dir is None:
            raise ValueError("work_dir is None")
        self.work_dir_path = work_dir

        # venv_path
        self.venv_path = props_dict.get("venv_path", None)

        # group_chat
        self.chat_dict = props_dict.get("group_chat", None)

        if self.chat_dict is None:
            raise ValueError("chat_dict is None")

    # 指定したnameのGroupChatをDBから取得して、GroupChatを返す
    def create_group_chat(self, name: str):
        # sqlite3のDBを開く
        conn = sqlite3.connect(self.autogen_db_path)
        cursor = conn.cursor()
        cursor.execute(f"SELECT * FROM {self.group_chats_table_name} WHERE name = ?", (name,))
        row = cursor.fetchone()
        # データが存在しない場合は例外を発生させる
        if row is None:
            raise ValueError(f"GroupChat:{name} is not found.")
        # DBから取得したデータをchat_dictに変換する
        # なお、group_chatsのテーブル定義は以下の通り
        # "CREATE TABLE IF NOT EXISTS group_chats (name TEXT, description TEXT, init_agent_name TEXT, agent_names TEXT, max_round INTEGER)"
        chat_dict = {}
        chat_dict["name"] = row[0]
        chat_dict["description"] = row[1]
        chat_dict["init_agent_name"] = row[2]
        chat_dict["agent_names"] = row[3]
        chat_dict["max_round"] = row[4]
        conn.close()

        # agent_namesを取得
        agent_names = chat_dict["agent_names"].split(",")
        agents = []
        for agent_name in agent_names:
            agent = self.create_agent(agent_name)
            agents.append(agent)
        
        # グループチャットを作成
        groupchat = GroupChat(
            admin_name="chat_admin_agent",
            agents=agents,
            messages=[],
            max_round=chat_dict["max_round"],
        )
        

    # 指定したnameのAgentをDBから取得して、Agentを返す
    def create_agent(self, name: str):
        # sqlite3のDBを開く
        conn = sqlite3.connect(self.autogen_db_path)
        cursor = conn.cursor()
        cursor.execute(f"SELECT * FROM {self.agents_table_name} WHERE name = ?", (name,))
        row = cursor.fetchone()
        # データが存在しない場合は例外を発生させる
        if row is None:
            raise ValueError(f"Agent:{name} is not found.")
        # DBから取得したデータをagent_dictに変換する
        # なお、agentsのテーブル定義は以下の通り
        # "CREATE TABLE IF NOT EXISTS agents (name TEXT PRIMARY KEY, description TEXT, system_message TEXT, human_input_mode TEXT, termination_msg TEXT, code_execution BOOLEAN, llm_config_name TEXT, tool_names TEXT)"
        agent_dict = {}
        agent_dict["name"] = row[0]
        agent_dict["description"] = row[1]
        agent_dict["system_message"] = row[2]
        agent_dict["human_input_mode"] = row[3]
        agent_dict["termination_msg"] = row[4]
        agent_dict["code_execution"] = row[5]
        agent_dict["llm_config_name"] = row[6]
        agent_dict["tool_names"] = row[7]
        conn.close()
        # ConversableAgent object用の引数辞書を作成
        params = {}
        params["name"] = agent_dict["name"]
        params["description"] = agent_dict["description"]
        params["system_message"] = agent_dict["system_message"]
        params["human_input_mode"] = agent_dict["human_input_mode"]
        params["is_termination_msg"] = lambda msg: agent_dict["termination_msg"] in msg["content"].lower()
        # code_executionがTrueの場合は、code_executor_dictを作成
        if agent_dict["code_execution"]:
            code_executor_dict = self.create_code_executor_dict()
            params["code_executor_dict"] = code_executor_dict
        else:
            params["code_executor_dict"] = False
        # llm_config_nameが指定されている場合は、llm_config_dictを作成
        if agent_dict["llm_config_name"]:
            llm_config_dict = self.create_llm_config_dict(agent_dict["llm_config_name"])
            params["llm_config"] = llm_config_dict
        else:
            params["llm_config"] = False

        # ConversableAgent objectを作成
        agent = ConversableAgent(**params)

        # tool_namesが指定されている場合は、tool_dictを作成
        if agent_dict["tool_names"]:
            tool_names = agent_dict["tool_names"].split(",")
            tool_dict_list = []
            for tool_name in tool_names:
                tool_dict = self.create_tool(tool_name)
                # agentのregister_for_llmを実行
                agent.register_for_llm(name = tool_dict["name"], description = tool_dict["description"])(tool_dict["func"]) 


    def create_tool(self, name: str):
        # sqlite3のDBを開く
        conn = sqlite3.connect(self.autogen_db_path)
        cursor = conn.cursor()
        cursor.execute(f"SELECT * FROM {self.tools_table_name} WHERE name = ?", (name,))
        row = cursor.fetchone()
        # データが存在しない場合は例外を発生させる
        if row is None:
            raise ValueError(f"Tool:{name} is not found.")
        # DBから取得したデータをtool_dictに変換する
        # なお、toolsのテーブル定義は以下の通り
        # "CREATE TABLE IF NOT EXISTS tools (name TEXT, path TEXT, description TEXT)"
        tool_dict = {}
        tool_dict["name"] = row[0]
        tool_dict["path"] = row[1]
        tool_dict["description"] = row[2]
        conn.close()
        
        # source_pathからファイルを読み込む
        with open(tool_dict["path"], "r", encoding="utf-8") as f:
            content = f.read()
        locals_copy = {}
        # contentから関数オブジェクトを作成する。
        exec(content, globals(), locals_copy)

        # nameの関数を取得
        tool_dict["func"] = locals_copy[name]

        return tool_dict
    
    # 指定したnameのLLMConfigをDBから取得して、llm_configを返す    
    def create_llm_config_dict(self, name: str):
        # sqlite3のDBを開く
        conn = sqlite3.connect(self.autogen_db_path)
        cursor = conn.cursor()
        cursor.execute(f"SELECT * FROM {self.llm_configs_table_name} WHERE name = ?", (name,))
        row = cursor.fetchone()
        # データが存在しない場合は例外を発生させる
        if row is None:
            raise ValueError(f"LLMConfig:{name} is not found.")
        # DBから取得したデータをllm_configに変換する
        # なお、llm_configsのテーブル定義は以下の通り
        # CREATE TABLE IF NOT EXISTS llm_configs (name TEXT, api_type TEXT, api_version TEXT, model TEXT, api_key TEXT, base_url TEXT)
        llm_config_entry: dict = {}
        llm_config_entry["api_type"] = row[1]
        llm_config_entry["api_version"] = row[2]
        llm_config_entry["model"] = row[3]
        llm_config_entry["api_key"] = row[4]
        llm_config_entry["base_url"] = row[5]
        conn.close() 

        # config_listに追加
        config_list = []
        config_list.append(llm_config_entry)
        llm_config = {}
        llm_config["config_list"] = config_list
        llm_config["cache_seed"] = None

        return llm_config


    # TODO Agent毎に設定できるようにする
    def create_code_executor_dict(self):
        params = {}
        params["timeout"] = 120
        params["work_dir"] = self.work_dir_path
        if self.venv_path:
            env_builder = venv.EnvBuilder(with_pip=True)
            virtual_env_context = env_builder.ensure_directories(self.venv_path)
            params["virtual_env_context"] = virtual_env_context
            
        # Create a local command line code executor.
        print(f"work_dir_path:{self.work_dir_path}")
        executor = LocalCommandLineCodeExecutor(
            **params
        )
        return {"executor": executor}
