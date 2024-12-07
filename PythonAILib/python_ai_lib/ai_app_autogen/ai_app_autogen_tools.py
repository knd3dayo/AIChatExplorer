from typing import Annotated, Callable

from ai_app_openai.ai_app_openai_util import OpenAIProps 
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps, VectorSearchParameter
from ai_app_langchain.langchain_vector_db import LangChainVectorDB
from ai_app_autogen.ai_app_autogen_props import AutoGenProps

class AutoGenToolWrapper:
    def __init__(self, name: str, description: str, source_path: str,
                 autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]):
        self.name = name
        self.description = description
        self.source_path = source_path

        # print(f"name: {name}, description: {description}, source_path:, {source_path}")

        # toolがない場合は関数を作成
        self.tools: list[callable] = AutoGenToolWrapper.__create_tools(source_path, autogen_props, vector_db_props_list)

    @classmethod
    def __create_tools(cls, source_path: str, autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]) -> Callable:
        if source_path is None:
            raise ValueError("source_path is None")

        # source_pathからファイルを読み込む
        with open(source_path, "r", encoding="utf-8") as f:
            content = f.read()
        
        locals_copy = {}
        # contentから関数オブジェクトを作成する。
        exec(content, globals(), locals_copy)

        # create_tools関数を取得
        create_tools = locals_copy.get("create_tools")
        tools = create_tools(autogen_props, vector_db_props_list)
        return tools

    @classmethod
    def create_definition(cls, tool_wrapper: "AutoGenToolWrapper") -> list[dict]:
        dict_list = []
        for tool in tool_wrapper.tools:
            data = {
                "name": tool.__name__,
                "description": tool.__doc__,
                "source_path": tool.__file__
            }
            dict_list.append(data)

    @classmethod
    def create_wrapper(cls, data: dict, autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]) -> "AutoGenToolWrapper":
        name = data['name']
        description = data['description']
        source_path = data['source_path']

        return AutoGenToolWrapper(name, description, source_path, autogen_props, vector_db_props_list)


