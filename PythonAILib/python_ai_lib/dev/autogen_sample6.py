import os
import tempfile
import sys

from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor # type: ignore

sys.path.append(os.path.join(os.path.dirname(__file__), '..'))
from ai_app_autogen_util import AutoGenProps, AutoGenGroupChatTest1
from ai_app_openai_util import OpenAIProps, OpenAIClient 
from ai_app_vector_db_props import VectorDBProps


if __name__ == '__main__':
    # Create a temporary directory to store the code files.
    temp_dir = tempfile.TemporaryDirectory()

    # Create a local command line code executor.
    executor = LocalCommandLineCodeExecutor(
        timeout=10,  # Timeout for each code execution in seconds.
        work_dir=temp_dir.name,  # Use the temporary directory to store the code files.
    )

    openAIProps: OpenAIProps = OpenAIProps.env_to_props()
    autogenProps: AutoGenProps = AutoGenProps(openAIProps)


    client = AutoGenGroupChatTest1(autogenProps.llm_config, executor, temp_dir.name)
    client.enable_code_writer_and_executor()
    client.enable_wikipedia_searcher()
    client.enable_vector_searcher(openAIProps, [])

    user_message  = f"""
        ぽんちょろりん汁はどのようなものですか？
        """
    group_chat = client.execute_group_chat(user_message, 3)
    print(group_chat.messages)

