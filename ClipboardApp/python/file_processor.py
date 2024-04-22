import sys
sys.path.append('..')
from file_loader import FileLoader
from common.langchain_vector_db import LangChainVectorDB
from common.langchain_openai_client import LangChainOpenAIClient
from common.env_to_props import get_props
if __name__ == "__main__":
    # 第1引数は処理タイプ(update,delete)
    # 第2引数はファイルパス
    arg1 = sys.argv[1]
    arg2 = sys.argv[2]
    # optional
    if len(sys.argv) > 3:
        arg3 = sys.argv[3]
    else:
        arg3 = ""

    if arg1 == "update":
        loader = FileLoader(arg2)
        props = get_props()
        client = LangChainOpenAIClient(props)
        vector_db = LangChainVectorDB(client, props.get("VectorDBURL"))
        documents = loader.get_document_list()
        if len(documents) == 0:
            print("No documents to update.")
            exit()
        
        # 既存のDBからソースファイルが一致するドキュメントを削除
        vector_db.delete_doucments_by_sources([arg2])
        vector_db.add_documents(documents)
        for _id, doc in vector_db.db.docstore._dict.items():
           print(f"{_id} {doc.metadata} {doc.page_content}")
        print(vector_db.db.index.ntotal)
        vector_db.save()

    elif arg1 == "delete":
        props = get_props()
        client = LangChainOpenAIClient(props)
        vector_db = LangChainVectorDB(client, props.get("VectorDBURL"))
        vector_db.delete_doucments_by_sources([arg2])
        for _id, doc in vector_db.db.docstore._dict.items():
           print(f"{_id} {doc.metadata} {doc.page_content}")
        print(vector_db.db.index.ntotal)
        vector_db.save()

    elif arg1 == "summary":
        # 未実装
        # result = maker.create_document_summary_from_file(arg2)
        # print(f"[title]\n{result['title']}\n[description]\n{result['description']}")
        pass
    
    else:
        print("第1引数はupdate、delete、summaryを指定してください。", file=sys.stderr)
        exit()
    
    