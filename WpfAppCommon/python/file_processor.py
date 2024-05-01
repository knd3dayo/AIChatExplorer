import sys
sys.path.append('python')
from file_loader import FileLoader
from langchain_vector_db import LangChainVectorDB
from langchain_openai_client import LangChainOpenAIClient
from langchain.docstore.document import Document
from env_to_props import get_props


def update_index(props, mode, workdir, relative_path, url):
        
    if mode == "update":
        # ドキュメントを取得
        loader = FileLoader(workdir, relative_path, url)
        documents = loader.get_document_list()

        client = LangChainOpenAIClient(props)
        vector_db = LangChainVectorDB(client, props.get("VectorDBURL"))
        if len(documents) == 0:
            print("No documents to update.")
            return
        
        # DBを更新
        token_count = vector_db.update_documents(documents, props)
        for _id, doc in vector_db.db.docstore._dict.items():
           print(f"{_id} {doc.metadata} {doc.page_content}")
        print(vector_db.db.index.ntotal)
 
        return token_count
    
    elif mode == "delete":
        client = LangChainOpenAIClient(props)
        vector_db = LangChainVectorDB(client, props.get("VectorDBURL"))
        # 第2引数、第3引数からドキュメントを作成
        document: Document = Document(page_content="", metadata={"source_url": url, "source_path": relative_path})
        vector_db.delete_doucments_by_sources([document])
        for _id, doc in vector_db.db.docstore._dict.items():
           print(f"{_id} {doc.metadata} {doc.page_content}")
        print(vector_db.db.index.ntotal)

        return 0
    
    else:
        print("第1引数はupdate、deleteを指定してください。", file=sys.stderr)
        return 0
    
if __name__ == "__main__":
    props = get_props()

    # 第1引数は処理タイプ(update,delete)
    # 第2引数は作業ディレクトリ
    # 第3引数は作業ディレクトリからの相対パス
    # 第4引数はソースURL
    
    arg1 = sys.argv[1]
    arg2 = sys.argv[2]
    arg3 = sys.argv[3]
    # optional
    if len(sys.argv) > 4:
        arg4 = sys.argv[4]
    else:
        arg4 = ""
        
    update_index(props, arg1, arg2, arg3, arg4)
    