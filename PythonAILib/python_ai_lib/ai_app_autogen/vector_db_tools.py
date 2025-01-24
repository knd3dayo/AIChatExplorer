from typing import Annotated


from ai_app_autogen.ai_app_autogen_props import AutoGenProps
from ai_app_vector_db.ai_app_vector_db_props import VectorDBProps, VectorSearchParameter
from ai_app_langchain.langchain_vector_db import LangChainVectorDB

def create_tools(autogen_props: AutoGenProps, vector_db_props_list: list[VectorDBProps]) -> list[callable]:

    def vector_search(query: Annotated[str, "String to search for"]) -> list[str]:
        """
        This function performs a vector search on the specified text and returns the related documents.
        """
        params: VectorSearchParameter = VectorSearchParameter(autogen_props.openai_props, vector_db_props_list, query)
        result = LangChainVectorDB.vector_search(params)
        # Retrieve documents from result
        documents = result.get("documents", [])
        # Extract content of each document from documents
        result = [doc.get("content", "") for doc in documents]
        return result

    return [vector_search]