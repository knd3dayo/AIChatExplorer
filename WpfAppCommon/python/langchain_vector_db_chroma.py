
import os, json, sys
from langchain_community.vectorstores import Chroma
from langchain_core.vectorstores import VectorStore
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback
import chromadb

sys.path.append("python")
from langchain_client import LangChainOpenAIClient
from langchain_vector_db import LangChainVectorDB

class LangChainVectorDBChroma(LangChainVectorDB):

    def __init__(self, langchain_openai_client: LangChainOpenAIClient,
                 vector_db_url, collection : str = None):
        super().__init__(langchain_openai_client, vector_db_url, collection)


    def load(self, _vector_db_url:str=None):
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not _vector_db_url or not os.path.exists(_vector_db_url):
            # ディレクトリを作成
            os.makedirs(_vector_db_url)
        client = chromadb.PersistentClient(path=_vector_db_url)
        self.db = Chroma(
            embedding_function = self.langchain_openai_client.get_embedding_client(), 
            client=client
            )

    def save(self, _vector_db_url, documents:list=None):
        if not _vector_db_url:
            return
        self.db.add_documents(documents=documents, embedding=self.langchain_openai_client.get_embedding_client())

        
    def delete(self, sources:list=None):
        # 既存のDBから指定されたsourceを持つドキュメントを削除
        for source in sources:
            docs = self.db.get(where={"source": source})
            ids = docs.get("ids", [])
            if len(ids) > 0:
                self.db._collection.delete(ids=ids)
        return 0    
 
if __name__ == "__main__":
    from langchain_client import LangChainOpenAIClient
    from langchain.docstore.document import Document
    from langchain_community.callbacks import get_openai_callback
    from langchain_community.vectorstores import FAISS
    from langchain_core.vectorstores import VectorStore
    import os
    # clipboard_app_props
    import openai_props
    props = openai_props.get_props()

    langchain_openai_client = LangChainOpenAIClient(props)
    vector_db_url = "vector_db"
    langchain_vector_db = LangChainVectorDBChroma(langchain_openai_client, vector_db_url)

    documents = [
        Document(
            page_content="ぽんちょろりん汁",
            metadata={"source": "test1"}
        ),
        Document(
            page_content="ぽこぽこ鉄",
            metadata={"source": "test2"}
        ),
    ]

    langchain_vector_db.add_documents(documents)

    print("Done")
    
    doc_and_score_list = langchain_vector_db.db.similarity_search_with_relevance_scores("ぽんちょろりん汁", k=10, score_threshold=0.0)
    
    print(doc_and_score_list)
