from typing import Annotated, Callable
from ai_chat_explorer.db_modules import VectorDBItem

from ai_chat_explorer.langchain_modules.langchain_vector_db import LangChainVectorDB
from ai_chat_explorer.openai_modules.openai_util import OpenAIProps


def create_vector_search_tool(openai_props: OpenAIProps, vector_db_props_list: list[VectorDBItem]) -> Callable:

    def vector_search(query: Annotated[str, "String to search for"]) -> list[str]:
        """
        This function performs a vector search on the specified text and returns the related documents.
        """
        # vector_db_props_listの各要素にinput_textを設定
        for vector_db_props in vector_db_props_list:
            vector_db_props.input_text = query
        search_results = LangChainVectorDB.vector_search(openai_props, vector_db_props_list)
        # Retrieve documents from result
        documents = search_results.get("documents", [])
        # Extract content of each document from documents
        result = [doc.get("content", "") for doc in documents]
        return result

    return vector_search