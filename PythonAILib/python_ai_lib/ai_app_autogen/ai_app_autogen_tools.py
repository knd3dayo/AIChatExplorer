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
        self.tool: Callable = AutoGenToolWrapper.__create_tool(name, source_path, autogen_props, vector_db_props_list)

    @classmethod
    def __create_tool(cls, name: str, source_path: str, autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]) -> Callable:
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
        # nameにマッチする関数を取得
        for tool in tools:
            if tool.__name__ == name:
                return tool
        # nameにマッチする関数がない場合は例外を発生させる
        raise ValueError(f"function not found: {name}")

    @classmethod
    def create_definition_list(cls, tool_wrapper_list: list["AutoGenToolWrapper"]) -> list[dict]:
        dict_list = []
        for tool_wrapper in tool_wrapper_list:
            data = {
                "name": tool_wrapper.name,
                "description": tool_wrapper.description,
                "source_path": tool_wrapper.tool.__file__
            }
            dict_list.append(data)

    @classmethod
    def create_wrapper(cls, data: dict, autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]) -> "AutoGenToolWrapper":
        name = data['name']
        description = data['description']
        source_path = data['source_path']

        return AutoGenToolWrapper(name, description, source_path, autogen_props, vector_db_props_list)

    @classmethod
    def create_wrapper_list(cls, data_list: list[dict], autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]) -> list["AutoGenToolWrapper"]:
        wrapper_list = []
        for data in data_list:
            wrapper = AutoGenToolWrapper.create_wrapper(data, autogen_props, vector_db_props_list)
            wrapper_list.append(wrapper)
        return wrapper_list

    @classmethod
    def create_wrapper_all_from_source(cls, source_path: str, autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]) -> list["AutoGenToolWrapper"]:
        with open(source_path, "r", encoding="utf-8") as f:
            content = f.read()
        
        locals_copy = {}
        exec(content, globals(), locals_copy)

        create_tools = locals_copy.get("create_tools")
        tools = create_tools(autogen_props, vector_db_props_list)
        wrapper_list = []
        for tool in tools:
            name = tool.__name__
            description = tool.__doc__
            wrapper = AutoGenToolWrapper(name, description, source_path, autogen_props, vector_db_props_list)
            wrapper_list.append(wrapper)
        return wrapper_list

    @classmethod
    def create_vector_search_tools(cls, openai_props: OpenAIProps, vector_db_props_list: list[VectorDBProps]) -> list["AutoGenToolWrapper"]:
        def vector_search(query: Annotated[str, "String to search for"]) -> list[str]:
            """
            This function performs a vector search on the specified text and returns the related documents.
            """
            params: VectorSearchParameter = VectorSearchParameter(openai_props, vector_db_props_list, query)
            result = LangChainVectorDB.vector_search(params)
            # Retrieve documents from result
            documents = result.get("documents", [])
            # Extract content of each document from documents
            result = [doc.get("content", "") for doc in documents]
            return result
        
        tools = []
        for vector_db_props in vector_db_props_list:
            tool = AutoGenToolWrapper("vector_search_" + vector_db_props.id, vector_db_props.VectorDBDescription, __file__, vector_search)
            tools.append(tool)

        return tools