
from autogen import ConversableAgent, UserProxyAgent
import autogen 
from typing import Any, Callable

from sample_autogen_tools import AutoGenToolWrapper
from sample_autogen_props import AutoGenProps


class AutoGenAgentWrapper:

    def __init__(self, name: str, description: str, system_message: str, 
                 type_value: str, human_input_mode: str, termination_msg: str, 
                 code_execution: bool, llm_execution: bool, 
                 tool_names_for_execution: list[str], tool_names_for_llm: list[str]):
        self.name = name
        self.description = description
        self.system_message = system_message
        self.type_value = type_value
        self.human_input_mode = human_input_mode
        self.termination_msg = termination_msg
        self.code_execution = code_execution
        self.llm_execution = llm_execution
        self.tool_names_for_execution = tool_names_for_execution
        self.tool_names_for_llm = tool_names_for_llm

    def create_agent(self, autogen_props: AutoGenProps, tool_wrappers: list[AutoGenToolWrapper]):
        params = {}
        params['name'] = self.name
        params['description'] = self.description
        params['system_message'] = self.system_message

        params['human_input_mode'] = self.human_input_mode if self.human_input_mode else "NEVER"
        # is_termination_msgが指定されていない場合はNone, それ以外の場合はtermination_msgを設定
        if self.termination_msg:
            params["is_termination_msg"] = lambda msg: self.termination_msg in msg["content"].lower()

        # code_executionを大文字にしたものがTRUEの場合はauto_gen_pros.create_code_executor()を呼び出し
        # それ以外の場合はFalseを設定
        params['code_execution_config'] = autogen_props.create_code_executor_config() if self.code_execution else False
        # llm_configを設定 llmをお大文字にしたものがTRUEの場合はauto_gen_pros.create_llm_config()を呼び出し
        # それ以外の場合はNoneを設定
        params['llm_config'] = autogen_props.create_llm_config() if self.llm_execution else None

        agent = None
        if self.type_value == 'userproxy':
            # userproxyの場合
            agent: UserProxyAgent = UserProxyAgent( **params)
        elif self.type_value == 'assistant':
            # assistantの場合
            agent: ConversableAgent = ConversableAgent( **params)
        else:
            raise ValueError(f"Unknown agent type: {self.type_value}")

        for tool_name in self.tool_names_for_execution:
            tool_wrapper = next((tool for tool in tool_wrappers if tool.name == tool_name), None)
            if tool_wrapper:
                agent.register_for_execution()(tool_wrapper.tool)
        
        for tool_name in self.tool_names_for_llm:
            tool_wrapper = next((tool for tool in tool_wrappers if tool.name == tool_name), None)
            if tool_wrapper:
                agent.register_for_llm(description=tool_wrapper.description)(tool_wrapper.tool)

        return agent
        

    @classmethod
    def create_wrapper(cls, data: dict, tool_wrappers: list[AutoGenToolWrapper]) -> "AutoGenAgentWrapper":
        return cls(
            name=data["name"],
            description=data["description"],
            system_message=data["system_message"],
            type=data["type"],
            human_input_mode=data["human_input_mode"],
            is_termination_msg=data["is_termination_msg"],
            code_execution_config=data["code_execution_config"],
            llm_config=data["llm_config"],
            tools=["tools"]
        )
    @classmethod
    def create_wrapper_list(cls, data: list[dict], tool_wrappers: list[AutoGenToolWrapper]) -> list["AutoGenAgentWrapper"]:
        return [cls.create_wrapper(d, tool_wrappers) for d in data]

    @classmethod
    def create_dict(cls, agent: "AutoGenAgentWrapper") -> dict:
        return {
            "name": agent.name,
            "description": agent.description,
            "system_message": agent.system_message,
            "type_value": agent.type_value,
            "human_input_mode": agent.human_input_mode,
            "termination_msg": agent.termination_msg,
            "code_execution": agent.code_execution,
            "llm_execution": agent.llm_execution,
            "tool_names_for_execution": agent.tool_names_for_execution,
            "tool_names_for_llm": agent.tool_names_for_llm
        }
    
    @classmethod
    def create_dict_list(cls, agents: list["AutoGenAgentWrapper"]) -> list[dict]:
        return [cls.create_dict(agent) for agent in agents]


from autogen import ConversableAgent, UserProxyAgent
from sample_autogen_props import AutoGenProps

class AutoGenAgentGenerator:

    @classmethod
    def create_default_agents(cls, autogen_props: AutoGenProps, tool_wrappers: list[AutoGenToolWrapper]) -> list[AutoGenAgentWrapper]:

        agent_wrappers = []
        agent_wrappers.append(cls.__create_user_proxy_wrapper(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_code_executor(autogen_props, tool_wrappers, False))
        agent_wrappers.append(cls.__create_code_writer(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_file_operator(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_vector_searcher(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_web_searcher(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_current_time(autogen_props, tool_wrappers))
        agent_wrappers.append(cls.__create_planner(autogen_props, tool_wrappers))
        # agent_wrappers.append(cls.__create_critic(autogen_props, tool_wrappers))

        return agent_wrappers

    @classmethod
    def __create_user_proxy_wrapper(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper: 
        # グループチャットのタスクアサイナー
        name="user_proxy"
        description = "ユーザーの要求を達成するためのタスクリストを作成し、各エージェントにタスクを割り当てます。"
        system_message="""
            各エージェントと協力してユーザーの要求を達成するためのタスクを実行します。
            - まず、ユーザーの要求を達成するための計画とタスクリストを作成します。
            - 各エージェントにタスクを割り当ててタスクを実行します。
            - 各エージェントによるタスクの実行で計画が達成された場合、[End Meeting]と返信します。
            - 追加の質問がない場合、[End Meeting]と返信します。
            """
        description=description
        
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="userproxy",
            human_input_mode="NEVER",
            termination_msg="end meeting",
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[tool.name for tool in autogen_tools],
            tool_names_for_llm=[]
        )
        return agent_wrapper

    @classmethod
    def __create_planner(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # グループチャットのタスクアサイナー
        description = "プランナー。計画を提案します。管理者および批評家からのフィードバックに基づいて計画を修正し、管理者の承認を得るまで繰り返します。"
        system_message="""プランナー。計画を提案します。管理者および批評家からのフィードバックに基づいて計画を修正し、
        管理者の承認を得るまで繰り返します。
        """
        name="planner"
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[],
            tool_names_for_llm=[]
        )
        return agent_wrapper

    @classmethod
    def __create_critic(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # グループチャットのタスクアサイナー
        description = "批評家。他のエージェントによる計画、主張、コードをダブルチェックし、フィードバックを提供します。計画に検証可能な情報（例: ソースURL）が含まれているか確認します。"
        system_message="""批評家。他のエージェントによる計画、主張、コードをダブルチェックし、フィードバックを提供します。
        計画に検証可能な情報（例: ソースURL）が含まれているか確認します。
        """
        name="critic"
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[],
            tool_names_for_llm=[]
        )
        return agent_wrapper

    @classmethod
    def __create_code_writer(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # コード作成者と実行者を分離。以下はLLMを使用したコード推論エージェントです。
        description = "ユーザーの指示に従ってPythonスクリプトを作成します。"
        name = "code_writer"
        system_message=f"""
            あなたはスクリプト開発者です。
            コードを書くと、それは外部アプリケーションで自動的に実行されます。
            あなたはユーザーの指示に従ってコードを書きます。
            コードの実行結果はコードを投稿した後に自動的に表示されます。
            ただし、次の条件を厳守しなければなりません:
            規則:
            * コードはコードブロック内にのみ提案してください。
            * スクリプトの実行結果がエラーの場合は、エラーメッセージに基づいて対策を考え、再度修正されたコードを作成してください。
            * スクリプトの実行から得られた情報が不十分な場合は、現在得られている情報に基づいて再度修正されたコードを作成してください。
            * あなたの最終的な目標はユーザーの指示を達成することであり、この目標を満たすために必要な回数だけコードを作成および修正します。
            """
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=True,
            llm_execution=False,
            tool_names_for_execution=[],
            tool_names_for_llm=[]
        )
        return agent_wrapper

    @classmethod
    def __create_code_executor(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper], auto_execute_code: bool = False) -> AutoGenAgentWrapper:
        # コード作成者と実行者を分離。以下はLLMを使用しないコード実行エージェントです。
        description = "コード作成者が提供するコードを実行します。"
        name = "code_executor"
        system_message=f"""
            あなたはコード実行者です。
            コード作成者が提供するコードを実行します。
            コードの実行結果を表示します。
            """
        if auto_execute_code:
            human_input_mode = "NEVER"
        else:
            human_input_mode = "ALWAYS"

        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode= human_input_mode,
            termination_msg=None,
            code_execution=True,
            llm_execution=False,
            tool_names_for_execution=[],
            tool_names_for_llm=[]
        )
        return agent_wrapper

    # ベクターサーチャーを有効にする
    @classmethod
    def __create_vector_searcher(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # ベクターサーチャー
        description = "ユーザーの指示に従って、ベクターデータベースから情報を検索します。"
        name = "vector_searcher"
        system_message="""
            あなたはベクターサーチャーです。ユーザーの指示に従って、ベクターデータベースから情報を検索します。
            提供された関数を使用して検索結果を表示してください。
            """
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[],
            tool_names_for_llm=["vector_search"]
        )
        return agent_wrapper

    @classmethod
    def __create_web_searcher(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # ウェブサーチャー
        description = "ウェブ上のドキュメントを検索します。"
        name = "web_searcher"
        system_message="""
            あなたはウェブサーチャーです。ユーザーの指示に従って、ウェブ上のドキュメントを検索します。
            - 情報を検索するために提供された search_duckduckgo 関数を使用してください。
            - 必要なドキュメントがリンク先にない場合は、さらにリンクされた情報を検索してください。
            - 必要なドキュメントが見つかった場合は、extract_webpage でドキュメントを取得し、ユーザーにテキストを提供してください。
            """
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[],
            tool_names_for_llm=["search_duckduckgo", "extract_webpage"]
        )

        # トークン消費量が多くなるのでコメントアウト
        # 日本語版ウィキペディアから関連ページを検索します。
        # search_wikipedia_ja, description = autogen_tools.tools["search_wikipedia_ja"]
        # web_searcher.register_for_llm(description=description)(search_wikipedia_ja)

        return agent_wrapper


    # ファイル抽出機能を有効にする
    @classmethod
    def __create_file_operator(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # ファイルオペレーター
        description = "ファイルオペレーター。例: テキストファイルの書き込み。ユーザーの指示に従ってファイルから情報を抽出します。"
        name = "file_operator"
        system_message=f"""
            あなたはファイルオペレーターです。
            - ユーザーの指示に従ってファイルから情報を抽出します。
            提供された関数を使用して抽出結果を表示してください。
            - ユーザーの指示に従ってPythonでファイルにデータを保存します。
            デフォルトの保存場所は {autogen_pros.work_dir_path} です。
            ユーザーが保存場所を指定した場合は、指定された場所にファイルを保存してください。
            - ディレクトリのファイルをリストします
            - ファイルチェッカー：指定されたファイルが存在するかどうかを確認します。
            """
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[],
            tool_names_for_llm=["extract_text_from_file", "list_files_in_directory", "save_text_file", "check_file"]
        )

        return agent_wrapper

    # 現在の時刻を取得するエージェントを作成
    @classmethod
    def __create_current_time(cls, autogen_pros: AutoGenProps, autogen_tools: list[AutoGenToolWrapper]) -> AutoGenAgentWrapper:
        # 現在の時刻を取得するエージェント
        description = "現在の時刻を取得します。"
        name = "current_time"
        system_message="""
            現在の時刻を取得します。
            """
        agent_wrapper = AutoGenAgentWrapper(
            name=name,
            description=description,
            system_message=system_message,
            type_value="assistant",
            human_input_mode="NEVER",
            termination_msg=None,
            code_execution=False,
            llm_execution=True,
            tool_names_for_execution=[],
            tool_names_for_llm=["get_current_time"]
        )

        return agent_wrapper
