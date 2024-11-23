import queue
from typing import Any
from collections.abc import Generator
from autogen import ConversableAgent

from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps
from ai_app_autogen.ai_app_autogen_client import  AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgents
from ai_app_autogen.ai_app_autogen_groupchat import AutoGenGroupChat
class AutoGenUtil:
    def __init__(self, openAIProps: OpenAIProps, work_dir_path: str = None, vector_db_props_list: list[VectorDBProps] =[], agent_names: list[str] = []):
        if openAIProps is None:
            raise ValueError("openAIProps is None")
        
        # 基本設定
        self.work_dir_path = work_dir_path
        self.OpenAIProps = openAIProps
        self.vector_db_props_list = vector_db_props_list
        self.last_message = None
        autogenProps: AutoGenProps = AutoGenProps(self.OpenAIProps, self.work_dir_path )
        self.client = AutoGenAgents(autogenProps, self.vector_db_props_list)

        if not agent_names:
            agent_names = [
                "user_proxy",
                "web_searcher", "azure_document_searcher", "vector_searcher", 
                "file_extractor", "code_writer", 
                "code_executor", 
                "file_writer", 
                #"file_checker"
                ]

        self.agents: list[tuple[ConversableAgent, str]] = self.prepare_agents(agent_names)
     

    def prepare_agents(self, agent_names: list[str]) -> list[tuple[ConversableAgent, str]]:
        agents: list[tuple[ConversableAgent, str]] = []
        for agent_name in agent_names:
            agent = self.client.agents[agent_name]
            if agent is not None:
                print (f"Agent {agent_name} is ready.")
                agents.append(agent)
        return agents

    def run_default_group_chat(self, init_message: str, max_round: int = 10) -> queue.Queue:
        autogen_agents = AutoGenAgents(self.client.autogen_props, self.vector_db_props_list)
        autogen_group_chat = AutoGenGroupChat(autogen_agents) 

        return autogen_group_chat.run_group_chat(init_message, max_round)

    