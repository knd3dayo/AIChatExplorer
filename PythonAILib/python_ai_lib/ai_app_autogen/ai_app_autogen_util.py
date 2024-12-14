from autogen import ConversableAgent
from typing import Any

from ai_app_autogen.ai_app_autogen_props import AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgentGenerator, AutoGenAgentWrapper
from ai_app_autogen.ai_app_autogen_tools import AutoGenToolWrapper
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps


class AutoGenChatUtil:

    def __init__(self, autogen_props: AutoGenProps , vector_db_items: list[VectorDBProps] = []):
        self.autogen_props = autogen_props
        # group_chat_dict
        group_chat_dict = autogen_props.chat_dict
        
        # name
        self.name = group_chat_dict.get("name", None)
        if self.name is None:
            raise ValueError("name is None")

        # description
        self.description = group_chat_dict.get("description", None)
        if self.description is None:
            raise ValueError("description is None")

        # init_agent_name
        self.init_agent_name = group_chat_dict.get("init_agent_name", None)
        if self.init_agent_name is None:
            raise ValueError("init_agent_name is None")

        # agent_names
        self.agent_names =  group_chat_dict.get("agent_names", [])

        agent_names_string = ",".join(self.agent_names)
        print (f"agent_names: {agent_names_string}")


        # autogen_propsからtoolsを取得
        self.tool_wrappers: list[AutoGenToolWrapper] = AutoGenToolWrapper.create_wrapper_list(autogen_props.tools_list, autogen_props, vector_db_items)
        # tool_wrappersにデフォルトのツールを追加
        import ai_app_autogen.default_tools
        default_tool_wrappers = AutoGenToolWrapper.create_wrapper_all_from_source(
            ai_app_autogen.default_tools.__file__, autogen_props, vector_db_items
        )
        
        # VectorDBPropsがある場合はAutoGenToolWrapperのcreate_vector_search_toolsを呼び出す
        if len(vector_db_items) > 0:
            default_tool_wrappers += AutoGenToolWrapper.create_vector_search_tools(self.autogen_props.openai_props, vector_db_items)

        # 既に追加されている場合は追加しない
        for default_tool_wrapper in default_tool_wrappers:
            if default_tool_wrapper not in self.tool_wrappers:
                self.tool_wrappers.append(default_tool_wrapper)

        # autogen_propsからagentsを取得
        self.agent_wrappers: list[AutoGenAgentWrapper] = AutoGenAgentWrapper.create_wrapper_list(
            autogen_props.agents_list
        )
        # agent_wrappersにデフォルトのエージェントを追加
        default_agent_wrappers = AutoGenAgentGenerator.create_default_agents(self.autogen_props, self.tool_wrappers)

        # 既に追加されている場合は追加しない
        for default_agent_wrapper in default_agent_wrappers:
            if default_agent_wrapper not in self.agent_wrappers:
                self.agent_wrappers.append(default_agent_wrapper)

        # VectorDBPropsがある場合はAutoGenAgentGeneratorのcreate_vector_search_agentsを呼び出す
        self.vector_search_agents = []
        if len(vector_db_items) > 0:
            self.vector_search_agents = AutoGenAgentGenerator.create_vector_search_agents(vector_db_items)

        # エージェントのリストを生成
        self.agents: list[ConversableAgent] = [agent.create_agent(self.autogen_props, self.tool_wrappers) for agent in self.agent_wrappers if agent.name in self.agent_names]

        # ベクトルDB検索用のagentを追加
        for vector_search_agent in self.vector_search_agents:
            self.agents.append(vector_search_agent.create_agent(self.autogen_props, self.tool_wrappers))
        
        # init_agent_nameに一致するエージェントを取得
        init_agent_wrappers = [agent for agent in self.agent_wrappers if agent.name == self.init_agent_name]

        if len(init_agent_wrappers) == 0:
            raise ValueError(f"init_agent not found: {self.init_agent_name}")

        self.init_agent: ConversableAgent = init_agent_wrappers[0].create_agent(self.autogen_props, self.tool_wrappers)

        # agentのリストを表示
        for agent in self.agents:
            print(f"agent:{agent.name} is ready.")

