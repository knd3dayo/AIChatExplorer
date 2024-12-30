import os
from typing import Annotated
from dotenv import load_dotenv
import autogen  # type: ignore
import tempfile

from autogen import ConversableAgent
from autogen.coding import LocalCommandLineCodeExecutor # type: ignore

# Create a temporary directory to store the code files.
temp_dir = tempfile.TemporaryDirectory()

# Create a local command line code executor.
executor = LocalCommandLineCodeExecutor(
    timeout=10,  # Timeout for each code execution in seconds.
    work_dir=temp_dir.name,  # Use the temporary directory to store the code files.
)
load_dotenv()

llm_config = {
    "config_list": [{
        "model": "gpt-4o-mini", "api_key": os.environ["OPENAI_API_KEY"], }],
}

cathy = ConversableAgent(
    "matsumoto",
    system_message="あなたの名前は松本です。お笑いコンビの一人です",
    llm_config={"config_list": [{"model": "gpt-4", "temperature": 0.9, "api_key": os.environ.get("OPENAI_API_KEY")}]},
    human_input_mode="NEVER",  # Never ask for human input.
)

joe = ConversableAgent(
    "hamada",
    system_message="あなたの名前は浜田です。お笑いコンビの一人です",
    llm_config={"config_list": [{"model": "gpt-4", "temperature": 0.7, "api_key": os.environ.get("OPENAI_API_KEY")}]},
    human_input_mode="NEVER",  # Never ask for human input.
)

result = joe.initiate_chat(cathy, message="浜田、なんかおもろい話ないんか？", max_turns=2)
