import sys, json, os
sys.path.append('python')
from langchain.docstore.document import Document
import langchain_util

from openai_props import env_to_props
from openai_props import OpenAIProps, VectorDBProps
from langchain_file_loader import FileLoader
from langchain_client import LangChainOpenAIClient


def update_index(props: OpenAIProps, vector_db_props: VectorDBProps, mode, workdir, relative_path, url) -> dict:

    # 結果格納用のdict
    result = {}
    # 初期化
    result["delete_count"] = 0
    result["update_count"] = 0

    vector_db_type_string = vector_db_props.VectorDBTypeString
    vector_db_url = vector_db_props.VectorDBURL
    vector_db_collection = vector_db_props.VectorDBCollectionName

    if mode == "update":
        # ファイルの存在チェック
        file_path = os.path.join(workdir, relative_path)
        if not os.path.exists(file_path):
            print("ファイルが存在しません。", file=sys.stderr)
            return result
        
        # ドキュメントを取得
        loader = FileLoader(workdir, relative_path, url)
        documents = loader.get_document_list()

        client = LangChainOpenAIClient(props)
        vector_db = langchain_util.get_vector_db(client, vector_db_type_string, vector_db_url, collection=vector_db_collection)
        if len(documents) == 0:
            print("No documents to update.")
            return result
        
        result["update_count"] = len(documents)
        
        #  sourceを指定してドキュメントを削除
        delete_token_count = vector_db.delete([relative_path])
        # DBを更新
        add_token_count = vector_db.add_documents(documents)
 
        return result
    
    elif mode == "delete":
        client = LangChainOpenAIClient(props)
        vector_db = langchain_util.get_vector_db(client, vector_db_type_string, vector_db_url, collection=vector_db_collection)
        
        if not vector_db:
            return  result
        
        #  sourceを指定してドキュメントを削除
        delete_count = vector_db.delete([relative_path])
        result["delete_count"] = delete_count

        return result
    
    else:
        print("第1引数はupdate、deleteを指定してください。", file=sys.stderr)
        return result
    
if __name__ == "__main__":
    props = env_to_props()
    open_ai_props = OpenAIProps(props)
    vector_db_props = VectorDBProps(props)
    
    # 第1引数は処理タイプ(update,delete)
    # 第2引数は作業ディレクトリ
    # 第3引数は作業ディレクトリからの相対パス
    
    arg1 = sys.argv[1]
    arg2 = sys.argv[2]
    arg3 = sys.argv[3]
    # optional
    if len(sys.argv) > 4:
        arg4 = sys.argv[4]
    else:
        arg4 = ""
        
    update_index(open_ai_props, vector_db_props, arg1, arg2, arg3, arg4)
    