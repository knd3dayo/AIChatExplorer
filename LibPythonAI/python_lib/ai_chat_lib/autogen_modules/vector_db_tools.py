from typing import Annotated, Callable
from ai_chat_lib.db_modules import VectorSearchRequest

from ai_chat_lib.langchain_modules.langchain_util import LangChainUtil
from ai_chat_lib.openai_modules.openai_util import OpenAIProps


def create_vector_search_tool(openai_props: OpenAIProps, vector_search_requests: list[VectorSearchRequest]) -> Callable:

    def vector_search(query: Annotated[str, "String to search for"]) -> list[str]:
        """
        This function performs a vector search on the specified text and returns the related documents.
        """

        search_results = LangChainUtil.vector_search(openai_props, vector_search_requests)
        # Retrieve documents from result
        documents = search_results.get("documents", [])
        # Extract content of each document from documents
        result = [doc.get("content", "") for doc in documents]
        return result

    return vector_search