import sys, json, os
sys.path.append('python')
from langchain.docstore.document import Document
from langchain_client import LangChainOpenAIClient
import langchain_util

from langchain_vector_db import LangChainVectorDB
from openai_props import OpenAIProps, VectorDBProps
from langchain_file_loader import FileLoader

def update_index(props: OpenAIProps, vector_db_props: VectorDBProps, mode, text, object_id_string):

    # 結果格納用のdict
    result = {}
    # 初期化
    result["delete_count"] = 0
    result["update_count"] = 0

    vector_db_type_string = vector_db_props.VectorDBTypeString
    vector_db_url = vector_db_props.VectorDBURL
    vector_db_collection = vector_db_props.CollectionName
    vector_db_doc_store_url = vector_db_props.DocStoreURL
    
    client = LangChainOpenAIClient(props)
    vector_db: LangChainVectorDB= langchain_util.get_vector_db(client, vector_db_type_string, vector_db_url, collection=vector_db_collection, doc_store_url=vector_db_doc_store_url)

    # DBからsourceを指定して既存ドキュメントを削除
    delete_count = vector_db.delete_doucments([object_id_string])
    result["delete_count"] = delete_count

    # mode == "delete"の場合、削除のみ行う
    if mode == "delete":
        return result

    # ドキュメントを取得
    documents = get_document_list(text, object_id_string)
    
    # DBにdockumentsを更新
    vector_db.add_documents(documents)
    result["update_count"] = len(documents)
    
    return result


def get_document_list(text, object_id_string):
    text_list = split_text(text)
    return [ Document(page_content=text, metadata={"source_url": "", "source": object_id_string}) for text in text_list]    

def split_text(text, chunk_size=500):
    text_list = []
    # テキストをchunk_sizeで分割
    for i in range(0, len(text), chunk_size):
        text_list.append(text[i:i + chunk_size])
    return text_list
