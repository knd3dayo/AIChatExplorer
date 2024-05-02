import sys
sys.path.append("python")

import openai_util


def openai_chat(props: dict, input_json: str, json_mode:bool = False):
    return openai_util.openai_chat(props, input_json, json_mode)

def openai_embedding(props: dict, input_text: str):
    return openai_util.openai_embedding(props, input_text)

import retrieval_qa_util

def langchain_chat( props: dict, vector_db_items_json: str, prompt: str, chat_history_json: str = None):
    return retrieval_qa_util.langchain_chat(props, vector_db_items_json, prompt, chat_history_json)
