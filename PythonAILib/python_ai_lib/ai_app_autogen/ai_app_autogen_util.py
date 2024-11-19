import os, json
from typing import Any
from collections.abc import Generator

from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps
from ai_app_autogen.ai_app_autogen_client import  AutoGenProps
from ai_app_autogen.ai_app_autogen_agent import AutoGenAgents
import tempfile

class AutoGenUtil:
    def __init__(self, openAIProps: OpenAIProps, work_dir_path: str = None, vector_db_props_list: list[VectorDBProps] =[]):
        if openAIProps is None:
            raise ValueError("openAIProps is None")
        
        # 基本設定
        self.work_dir_path = work_dir_path
        self.OpenAIProps = openAIProps
        self.vector_db_props_list = vector_db_props_list
        self.last_message = None
        self.temp_dir = None

    def run_group_chat(self, input_text: str) -> Generator[Any, None, None]:

        # self.work_dir_pathがNoneの場合はtempfileを使用
        if not self.work_dir_path:
            self.temp_dir = tempfile.TemporaryDirectory()
            self.work_dir_path = self.temp_dir.name

        autogenProps: AutoGenProps = AutoGenProps(self.OpenAIProps, self.work_dir_path )
        client = AutoGenAgents(autogenProps, self.vector_db_props_list)
        # threadを使ってgroup_chatを実行
        import threading
        thread = threading.Thread(target=self.create_group_chat_thread, args=(client, input_text))
        thread.start()

        return client.get_messages()
    
    def create_group_chat_thread(self, client: AutoGenAgents, input_text: str):

        # client.enable_code_writer()
        # client.enable_code_executor()
        client.enable_web_searcher()
        client.enable_azure_document_searcher()
        client.enable_vector_searcher()
        client.enable_file_extractor()
        # client.enable_code_writer()
        # client.enable_code_executor()
        client.enable_autogen_tool_writer()

        if not self.work_dir_path:
            # Create a temporary directory to store the code files.
            temp_dir = tempfile.TemporaryDirectory()
            self.work_dir = temp_dir.name

        try:
            group_chat = client.execute_group_chat(input_text, 10)
            '''
            # Print the messages in the group chat.
            for message in group_chat.messages:
                # roleがuserまたはassistantの場合はrole, name, contentを表示
                if message["role"] in ["user", "assistant"]:
                    print(f"role:[{message['role']}] name:[{message['name']}]")
                    print ("------------------------------------------")
                    print(f"content:{message['content']}\n")
            '''


        except Exception as e:
            print(f"Error: {e}")
        finally:
            self.finish(client)

    def finish(self, client: AutoGenAgents):
        client.finish()
        if self.temp_dir:
            self.temp_dir.cleanup()