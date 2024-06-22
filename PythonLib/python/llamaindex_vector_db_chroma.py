
import os, json, sys
from llama_index.core import VectorStoreIndex, SimpleDirectoryReader
from llama_index.vector_stores.chroma import ChromaVectorStore
from llama_index.core import Document
import chromadb

sys.path.append("python")
from llamaindex_client import LlamaIndexClient
from llamaindex_vector_db import LlamaIndexVectorDB

class LlamaIndexVectorDBChroma(LlamaIndexVectorDB):

    def __init__(self, llamaindex_openai_client: LlamaIndexClient, vector_db_url: str, collection : str = "default"):
        super().__init__(llamaindex_openai_client, vector_db_url, collection)


    def load(self):
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not self.vector_db_url or not os.path.exists(self.vector_db_url):
            # ディレクトリを作成
            os.makedirs(self.vector_db_url)
        client = chromadb.PersistentClient(path=self.vector_db_url)

        self.vector_store  = ChromaVectorStore(chroma_collection=self.collection)
        self.index = VectorStoreIndex.from_vector_store(
            self.vector_store ,
            embed_model=self.llamaindex_openai_client.get_embedding_client(),
        )

    def save(self, documents:list=[]):
        if self.vector_store is None:
            self.load()

        for document in documents:
            # indexを更新
            self.index.update_ref_doc(document, update_kwargs={"delete_kwargs": {"delete_from_docstore": True}})
        
    def delete(self, doc_ids:list=[]):
        for doc_id in doc_ids:
            # indexからmetadataがsourceのものを削除
            self.index.delete_ref_doc(doc_id, delete_from_docstore=True)

        return len(doc_ids)    
 
if __name__ == "__main__":

    # clipboard_app_props
    import openai_props
    props = openai_props.env_to_props()

    llhamaindex_client = LlamaIndexClient(props)
    vector_db_url = "vector_db"
    llamaindex_vector_db = LlamaIndexVectorDBChroma(llhamaindex_client, vector_db_url, collection="default")

    documents = [
        Document(
            text="ぽんちょろりん汁",
            metadata={"source": "test1"}
        ),
        Document(
            page_content="ぽこぽこ鉄",
            metadata={"source": "test2"}
        ),
    ]

    llamaindex_vector_db.save(documents)

    print("Done")
    query_engine = llamaindex_vector_db.index.as_query_engine()
    response = query_engine.query("ぽんちょろりん汁", k=10)
    
    print(response)
