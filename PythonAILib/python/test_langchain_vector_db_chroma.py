from langchain_client import LangChainOpenAIClient
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback
import os
from langchain_vector_db_chroma import LangChainVectorDBChroma
if __name__ == "__main__":
    # clipboard_app_props
    import openai_props
    props = openai_props.env_to_props()
    vector_db_props = openai_props.get_vector_db_settings()

    langchain_openai_client = LangChainOpenAIClient(props)
    langchain_vector_db = LangChainVectorDBChroma(langchain_openai_client, vector_db_props)

    langchain_vector_db.update_content_index("ぽんちょろりん汁", "test1", "")
    langchain_vector_db.update_content_index("ぽこぽこ鉄", "test2", "")

    print("Done")
    
    doc_and_score_list = langchain_vector_db.db.similarity_search_with_relevance_scores("ぽんちょろりん汁", k=10, score_threshold=0.0)
    
    print(doc_and_score_list)
