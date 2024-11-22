
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from collections.abc import Generator
import queue
from typing import Any

from ai_app_autogen.ai_app_autogen_tools import AutoGenTools
from ai_app_autogen.ai_app_autogen_client import AutoGenProps

class AutoGenAgents:
    # 途中のメッセージを格納するキュー
    message_queue = queue.Queue()

    def __init__(self, autgenProps: AutoGenProps, auto_execute_code: bool = False, vector_db_props_list = []):
        
        self.autogen_pros = autgenProps
        self.work_dir_path = autgenProps.work_dir_path
        # コード実行者が自動的にコードを実行するようにするかどうか
        # self.auto_execute_code = auto_execute_code
        self.auto_execute_code = True
        
        self.print_messages_function = AutoGenAgents.create_print_messages_function()

        self.autogen_tools = AutoGenTools(autgenProps.OpenAIProps, vector_db_props_list)

        self.agents : dict[str, tuple[ConversableAgent, str]] = {}
        self.agents["user_proxy"] = self.__create_user_proxy()
        self.agents["code_writer"] = self.__create_code_writer()
        self.agents["file_writer"] = self.__create_file_writer()
        self.agents["code_executor"] = self.__create_code_executor()
        self.agents["vector_searcher"] = self.__create_vector_searcher()
        self.agents["file_extractor"] = self.__create_file_extractor()
        self.agents["web_searcher"] = self.__create_web_searcher()
        self.agents["azure_document_searcher"] = self.__create_azure_document_searcher()
        self.agents["file_checker"] = self.__create_file_checker()

        # 終了フラグ
        self.finished = False
        self.temp_dir = None


    def add_agents(self, agents: dict[str, tuple[ConversableAgent, str]]):
        self.agents.update(agents)

    # エージェントの終了処理
    def finish(self):
        self.finished = True
        self.message_queue.put(None)
        if self.temp_dir:
            self.temp_dir.cleanup()

    # キューからメッセージを取得 yiled で返す
    def get_messages(self) -> Generator[Any, None, None]:
        while True:
            if self.finished:
                break
            message = self.message_queue.get()
            yield message, False
        
        return None, True

    # キューから取り出したメッセージを表示
    def print_messages(self):
        for message, is_last_message in self.get_messages():
            if message is None:
                break
            print(message)

    @classmethod
    def create_print_messages_function(cls):
        def print_messages(recipient, messages, sender, config): 
            if "callback" in config and  config["callback"] is not None:
                callback = config["callback"]
                callback(sender, recipient, messages[-1])

            # Print the messages in the group chat.
            # roleがuserまたはassistantの場合はrole, name, contentを表示
            message = messages[-1]
            header = f"role:[{message['role']}] name:[{message['name']}]\n------------------------------------------\n"
            content = f"{message['content']}\n"
            if message["role"] in ["user", "assistant"]:
                response = f"Messages sent to: {recipient.name} | num messages: {len(messages)}\n{header}{content}"
            else:
                response = f"Messages sent to: {recipient.name} | num messages: {len(messages)}\n{header}" 
            # queueにresponseを追加
            cls.message_queue.put(response)

            return False, None  # required to ensure the agent communication flow continues
        
        return print_messages

    def __create_user_proxy(self):
        # グループチャットの議題提供者
        description = "ユーザーの依頼を達成するためのタスク一覧を考えて各エージェントにタスクを割り当てます。"
        user_proxy = autogen.UserProxyAgent(
            system_message="""
                ユーザーの依頼を達成するため各エージェントと協力してタスクを実行します。
                ユーザーからファイルの保存場所の指定があれば、指定された場所にファイルを保存します。
                  指定がない場合は、{self.autogen_pros.work_dir_path}に保存します。
                  ファイル名は英語でお願いします。
                ユーザーの依頼を達成したら、グループチャットの管理者に[会議を終了]と返信します。
                """,
            name="user_proxy",
            human_input_mode="NEVER",
            code_execution_config=False,
            is_termination_msg=lambda msg: "会議を終了" in msg["content"].lower(),
            description=description,
            llm_config=self.autogen_pros.create_llm_config()
        )

        # user_proxyのregister_from_execution()を設定
        for func, description  in self.autogen_tools.tools.values():
            print(f"register_for_execution: {func.__name__}")
            user_proxy.register_for_execution()(func)

        return user_proxy, description

    def __create_code_writer(self):
        # コードの作成者と実行者は分離する。以下はコード推論 Agent。LLM を持つ。
        description = "ユーザーの指示に従い、Pythonスクリプトを作成します。"
        code_writer_agent = ConversableAgent(
            "code-writer",
            system_message=f"""
                あなたはスクリプト開発者です。
                あなたがコードを書くと自動的に外部のアプリケーション上で実行されます。
                ユーザーの指示に従ってあなたはコードを書きます。
                コードの実行結果は、あなたがコードを投稿した後に自動的に表示されます。
                ただし、次の条件を厳密に遵守する必要があります。
                ルール:
                * コードブロック内でのみコードを提案します。
                * スクリプトの実行結果がエラーである場合、エラー文を元に対策を考え、修正したコードを再度作成します。
                * スクリプトを実行した結果、情報を十分に得られなかった場合、現状得られた情報から修正したコードを再度作成します。
                * あなたはユーザーの指示を最終目標とし、これを満たす迄何度もコードを作成・修正します。
                """
        ,
            llm_config=self.autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        return code_writer_agent, description


    def __create_file_writer(self):
        # コードの作成者と実行者は分離する。以下はコード推論 Agent。LLM を持つ。
        description = "ユーザーの指示に従い、Pythonでデータをファイルに保存します。"
        file_writer = ConversableAgent(
            "file-writer",
            system_message=f"""
                ユーザーの指示に従い、Pythonでデータをファイルに保存します。
                デフォルトの保存場所は{self.autogen_pros.work_dir_path}です。
                ユーザーからファイルの保存場所の指定があれば、指定された場所にファイルを保存します。
                """
        ,
            llm_config=self.autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )
        save_tools, description = self.autogen_tools.tools["save_text_file"]
        # register_for_llm
        file_writer.register_for_llm(description=description)(save_tools)

        return file_writer, description

    def __create_code_executor(self):
        # コードの作成者と実行者は分離する。以下はコード実行者 Agent。LLM は持たない。
        description = "コード作成者から提供されたコードを実行します。"
        code_execution_agent = ConversableAgent(
            "code-execution",
            system_message=f"""
                あなたはコード実行者です。
                コード作成者から提供されたコードを実行します。
                コードの実行結果を表示します。
                """,
            llm_config=False,
            code_execution_config={"executor": self.autogen_pros.create_code_executor()},   
            description=description,
            human_input_mode="NEVER",
        )

        # コード実行者が自動的にコードを実行するようにするかどうか
        # self.auto_execute_code == True の場合はhuman_input_mode="NEVER"とする
        if self.auto_execute_code:
            code_execution_agent.human_input_mode = "NEVER"
        else:
            code_execution_agent.human_input_mode = "ALWAYS" 

        return code_execution_agent, description

    # ベクトル検索者を有効にする
    def __create_vector_searcher(self):
        # ベクトル検索者
        description = "ユーザーの指示に従い、ベクトルデータベースから情報を検索します。"
        vector_searcher = ConversableAgent(
            "vector-searcher",
            system_message="""
                あなたはベクトル検索者です。ユーザーの指示に従い、ベクトルデータベースから情報を検索します。
                提供された関数を用いて、検索した結果を表示します。
                """,
            llm_config=self.autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # 利用可能な関数の情報をエージェントに登録する
        vector_search, description = self.autogen_tools.tools["vector_search"]
        vector_searcher.register_for_llm(description=description)(vector_search)
        
        return vector_searcher, description

    # ファイル抽出者を有効にする
    def __create_file_extractor(self):
        # ファイル抽出者
        description = "ユーザーの指示に従い、ファイルから情報を抽出します。"
        file_extractor = ConversableAgent(
            "file-extractor",
            system_message="""
                あなたはファイル抽出者です。ユーザーの指示に従い、ファイルから情報を抽出します。
                提供された関数を用いて、抽出した結果を表示します。
                """,
            llm_config=self.autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # 利用可能な関数の情報をエージェントに登録する
        extract_text_from_file, description = self.autogen_tools.tools["extract_text_from_file"]
        file_extractor.register_for_llm (description=description)(extract_text_from_file)

        list_files_in_directory,  description = self.autogen_tools.tools["list_files_in_directory"]
        file_extractor.register_for_llm (description=description)(list_files_in_directory)

        return file_extractor, description

    def __create_web_searcher(self):
        
        # Web検索者
        description = "指定されたURLから情報を取得します。"
        web_searcher = ConversableAgent(
            "web-searcher",
            system_message="""
                ユーザーの指示に従い、指定されたURLから情報を取得します。
                - 提供された関数を用いて、指定されたURLのテキストとリンクを取得します。
                - ユーザーからURLが提供されなかった場合は、WikiPedia(日本語版)から検索対象文字列に関連するページを検索します。
                - リンク先に必要なドキュメントがない場合はさらにリンクされた情報を検索します。
                - 必要なドキュメントがあった場合はドキュメントのテキストをユーザーに提供します
                """,
            llm_config=self.autogen_pros.create_llm_config(),
            code_execution_config={"executor": self.autogen_pros.create_code_executor()},   
            description=description,
            human_input_mode="NEVER",
        )

        # 利用可能な関数の情報をエージェントに登録する

        # Wikipedia(日本語版)から検索対象文字列に関連するページを検索します。
        search_wikipedia_ja, description = self.autogen_tools.tools["search_wikipedia_ja"]
        web_searcher.register_for_llm(description=description)(search_wikipedia_ja)

        # 指定されたURLのテキストとリンクを取得します。
        extract_webpage, description = self.autogen_tools.tools["extract_webpage"]
        web_searcher.register_for_llm(description=description)(extract_webpage)

        return web_searcher, description

    def __create_azure_document_searcher(self):
        
        # Web検索者
        description = "Azure関連のドキュメントを検索します。"
        azure_document_searcher = ConversableAgent(
            "azure-document-searcher",
            system_message="""
                あなたはAzure関連のドキュメント検索者です。ユーザーの指示に従いWeb上のAzure関連のドキュメントを検索します。
                - 提供された関数を用いて、https://learn.microsoft.com/ja-jp/azure/?product=popularからリンクされた情報を検索します。
                - リンク先に必要なドキュメントがない場合はさらにリンクされた情報を検索します。
                - 必要なドキュメントがあった場合はドキュメントのテキストをユーザーに提供します。
                """,
            llm_config=self.autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # 指定されたURLのテキストとリンクを取得します。
        extract_webpage, description = self.autogen_tools.tools["extract_webpage"]
        azure_document_searcher.register_for_llm(description=description)(extract_webpage)

        return azure_document_searcher, description

    def __create_file_checker(self):
        
        # ファイルチェッカー
        description = "指定されたファイルが存在するか確認します。"
        file_checker = ConversableAgent(
            "file_checker",
            system_message="""
                ファイルチェッカーです。指定されたファイルが存在するか確認します。
                """,
            llm_config=self.autogen_pros.create_llm_config(),
            code_execution_config=False,
            description=description,
            human_input_mode="NEVER",
        )

        # 指定されたURLのテキストとリンクを取得します。
        func, description = self.autogen_tools.tools["check_file"]
        file_checker.register_for_llm(description=description)(func)

        return file_checker, description

from openpyxl import load_workbook
from autogen import ConversableAgent, UserProxyAgent, GroupChatManager, GroupChat, GroupChatManager
from ai_app_autogen.ai_app_autogen_client import AutoGenProps

class AutoGenAgentGenerator:
    def __init__(self, autogen_pros: AutoGenProps, autogen_tools: dict):
        self.autogen_pros = autogen_pros
        self.autogen_tools = autogen_tools

    def create_agent(self, params: dict) -> ConversableAgent:
        
        # human_input_modeが指定されていない場合はデフォルト値を設定
        human_input_mode = params.get('human_input_mode', None)
        params['human_input_mode'] = human_input_mode if human_input_mode else "NEVER"
        # is_termination_msgが指定されていない場合はNone, それ以外の場合はtermination_msgを設定
        termination_msg = params.get('is_termination_msg', None)
        if termination_msg:
            is_termination_msg = lambda msg: termination_msg in msg["content"].lower()
            params['is_termination_msg'] = is_termination_msg

        # code_executionを大文字にしたものがTRUEの場合はauto_gen_pros.create_code_executor()を呼び出し
        # それ以外の場合はFalseを設定
        code_execution = params.get('code_execution_config', None)
        params['code_execution_config'] = self.autogen_pros.create_code_executor() if code_execution.upper() == "TRUE" else False
        # llm_configを設定 llmをお大文字にしたものがTRUEの場合はauto_gen_pros.create_llm_config()を呼び出し
        # それ以外の場合はNoneを設定
        llm = params.get('llm_config', None)
        params['llm_config'] = self.autogen_pros.create_llm_config() if llm.upper() == "TRUE" else None

        type_value = params.get('type', None)
        agent = None
        if type_value == 'userproxy':
            # userproxyの場合
            agent: UserProxyAgent = UserProxyAgent( **params)
        elif type_value == 'assistant':
            # assistantの場合
            agent: ConversableAgent = ConversableAgent( **params)
        else:
            raise ValueError(f"Unknown agent type: {type_value}")

        # agentにツールを設定
        # tools :str からtools_listに変換
        tool_name_text = params.get('tools', None)
        if tool_name_text:
            tool_names = tool_name_text.split(',')
            for tool_name in tool_names:
                # self.autogen_toolsからtool_nameを指定して関数と説明を取得
                function_obj, description = self.autogen_tools.get(tool_name, (None, None))
                if function_obj is None:
                    raise ValueError(f"Unknown tool: {tool_name}")
                # agentにツールを設定
                agent.register_for_llm(name=tool_name, description=description)(function_obj)
        
        return agent

    def create_agents_dict(self, data_list: list[dict]) -> dict:
        agent_dict = {}
        for data in data_list:
            name = data['name']
            description = data['description']
            # エージェントを作成
            agent_obj = self.create_agent(**data)
            # dictに格納
            agent_dict[name] = (agent_obj, description)

        # 結果を表示（必要に応じて）
        for name, (agent, desc) in agent_dict.items():
            print(f'Name: {name}, Description: {desc}, Agent: {agent}')
        
        return agent_dict

    def create_agents_dict_from_sheet(self, excel_file):
        # Excelファイルを開く
        wb = load_workbook(excel_file)
        sheet = wb['agents']

        # ヘッダー行から列名を取得
        headers = [cell.value for cell in sheet[1]]
        name_index = headers.index('name')
        description_index = headers.index('description')
        system_message_index = headers.index('system_message')
        type_index = headers.index('type')
        tools_index = headers.index('tools')
        human_input_mode_index = headers.index('human_input_mode')
        is_termination_msg_index = headers.index('is_termination_msg')
        code_execution_config_index = headers.index('code_execution_config')
        llm_config_index = headers.index('llm_config')

        agent_data_list = []
        # 2行目から最終行まで処理を行う
        for row in sheet.iter_rows(min_row=2, values_only=True):
            # 各列の値を取得
            name = row[name_index]
            description = row[description_index]
            system_message = row[system_message_index]
            type_value = row[type_index]
            tools = row[tools_index]
            human_input_mode = row[human_input_mode_index]
            is_termination_msg = row[is_termination_msg_index]
            code_execution_config = row[code_execution_config_index]
            llm_config = row[llm_config_index]

            agent_data_list.append({
                'name': name, 'description': description, 'system_message': system_message, 
                'type': type_value, 'tools': tools, 'human_input_mode': human_input_mode,
                'is_termination_msg': is_termination_msg, 'code_execution_config': code_execution_config,
                'llm_config': llm_config
            })
            return self.create_agents_dict(agent_data_list)

