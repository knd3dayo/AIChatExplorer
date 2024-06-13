
import os, json, sys, uuid
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
                 vector_db_url, collection : str = None, doc_store_url: str = None):
        super().__init__(langchain_openai_client, vector_db_url, collection, doc_store_url)


    def load(self):
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not self.vector_db_url or not os.path.exists(self.vector_db_url):
            # ディレクトリを作成
            os.makedirs(self.vector_db_url)
        # params
        params = {}
        params["client"] = chromadb.PersistentClient(path=self.vector_db_url)
        params["embedding_function"] = self.langchain_openai_client.get_embedding_client()
        # collectionが指定されている場合
        if self.collection:
            params["collection_name"] = self.collection
        
        self.db = Chroma(
            **params
            )

    def _save(self, documents:list=None):
        if not self.vector_db_url:
            return
        
        self.db.add_documents(documents=documents, embedding=self.langchain_openai_client.get_embedding_client())

        
    def _delete(self, sources:list=None):
        # 既存のDBから指定されたsourceを持つドキュメントを削除
        for source in sources:
            docs = self.db.get(where={"source": source})

            print("docs:", docs)
            
            # docsのmetadataのdoc_idを取得
            metadatas = docs.get("metadatas", [])
            ids = [metadata.get("doc_id", None) for metadata in metadatas]
            self._delete_docstore_data(doc_ids=ids)

            ids = docs.get("ids", [])
            if len(ids) > 0:
                self.db._collection.delete(ids=ids)

        return len(ids)    
 
if __name__ == "__main__":
    from langchain_client import LangChainOpenAIClient
    from langchain.docstore.document import Document
    from langchain_community.callbacks import get_openai_callback
    import os
    # clipboard_app_props
    import openai_props
    props = openai_props.env_to_props()

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
