import queue
from typing import Any
from collections.abc import Generator
from autogen import ConversableAgent

from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps
from ai_app_autogen.ai_app_autogen_client import  AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgents

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
            agent_names = ["web_searcher", "azure_document_searcher", "vector_searcher", "file_extractor", "code_writer", "code_executor", "autogen_tool_writer"]

        self.agents = self.prepare_agents(agent_names)

        self.user_agent = self.client.create_user_proxy()
        self.agents.append(self.user_agent)

    def prepare_agents(self, agent_names: list[str]) -> list[ConversableAgent]:
        agents: list[ConversableAgent] = []
        for agent_name in agent_names:
            agent = self.client.get_agent(agent_name)
            if agent is not None:
                print (f"Agent {agent_name} is ready.")
                agents.append(agent)
        return agents

    def run_group_chat(self, input_text: str) -> Generator[Any, None, None]:

        # threadを使ってgroup_chatを実行
        import threading
        thread = threading.Thread(target=self.create_group_chat_thread, args=(self.client, input_text))
        thread.start()

        return self.client.get_messages()
    
    def create_group_chat_thread(self, client: AutoGenAgents, input_text: str):
        try:
            client.run_group_chat(input_text, 10, self.user_agent, self.agents)
        except Exception as e:
            print(f"Error: {e}")
        finally:
            self.finish(client)

    def finish(self, client: AutoGenAgents):
        client.finish()
