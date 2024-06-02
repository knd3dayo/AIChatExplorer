import sys, json, os
sys.path.append('python')
from file_loader import FileLoader
from langchain.docstore.document import Document
from langchain_client import LangChainOpenAIClient
import langchain_util
from openai_props import OpenAIProps, VectorDBProps

def update_index(props: OpenAIProps, vector_db_props: VectorDBProps, mode, text, object_id_string):

    vector_db_type_string = vector_db_props.VectorDBTypeString
    vector_db_url = vector_db_props.VectorDBURL
    client = LangChainOpenAIClient(props)
    vector_db = langchain_util.get_vector_db(client, vector_db_type_string, vector_db_url)

    # ドキュメントを取得
    documents = get_document_list(text, object_id_string)
    # DBを更新
    add_token_count = vector_db.add_documents(documents)


def get_document_list(text, object_id_string):
    text_list = split_text(text)
    return [ Document(page_content=text, metadata={"source_url": "", "source": object_id_string}) for text in text_list]    


def split_text(text, chunk_size=500):
    text_list = []
    # テキストをchunk_sizeで分割
    for i in range(0, len(text), chunk_size):
        text_list.append(text[i:i + chunk_size])
    return text_list
