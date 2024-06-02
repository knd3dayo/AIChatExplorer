import sys, json, os
sys.path.append('python')
from file_loader import FileLoader
from langchain.docstore.document import Document
from langchain_client import LangChainOpenAIClient
import langchain_util
from openai_props import env_to_props
from openai_props import OpenAIProps, VectorDBProps


def update_index(props: OpenAIProps, vector_db_props: VectorDBProps, mode, workdir, relative_path, url):

    vector_db_type_string = vector_db_props.VectorDBTypeString
    vector_db_url = vector_db_props.VectorDBURL

    if mode == "update":
        # ファイルの存在チェック
        file_path = os.path.join(workdir, relative_path)
        if not os.path.exists(file_path):
            print("ファイルが存在しません。", file=sys.stderr)
            return 0
        
        # ドキュメントを取得
        loader = FileLoader(workdir, relative_path, url)
        documents = loader.get_document_list()

        client = LangChainOpenAIClient(props)
        vector_db = langchain_util.get_vector_db(client, vector_db_type_string, vector_db_url)
        if len(documents) == 0:
            print("No documents to update.")
            return 0
        
        #  sourceを指定してドキュメントを削除
        delete_token_count = vector_db.delete([relative_path])
        # DBを更新
        add_token_count = vector_db.add_documents(documents)
 
        return delete_token_count + add_token_count
    
    elif mode == "delete":
        client = LangChainOpenAIClient(props)
        vector_db = langchain_util.get_vector_db(client, vector_db_type_string, vector_db_url)
        
        if not vector_db:
            return 0

        #  sourceを指定してドキュメントを削除
        vector_db.delete([relative_path])

        return 0
    
    else:
        print("第1引数はupdate、deleteを指定してください。", file=sys.stderr)
        return 0
    
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
    