from typing import Annotated, Callable
from clipboard_app.db_modules import VectorDBItem, VectorSearchParameter

from clipboard_app.langchain_modules.langchain_vector_db import LangChainVectorDB
from clipboard_app.openai_modules.openai_util import OpenAIProps


def create_vector_search_tool(openai_props: OpenAIProps, vector_db_props_list: list[VectorDBItem]) -> Callable:

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

    return vector_search