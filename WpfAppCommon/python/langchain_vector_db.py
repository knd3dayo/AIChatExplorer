
import os, json, sys
from langchain_community.vectorstores import FAISS
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback

sys.path.append("python")
from langchain_openai_client import LangChainOpenAIClient

class LangChainVectorDB:

    def __init__(self, langchain_openai_client: LangChainOpenAIClient,
                 vector_db_url, collection : str = None):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_url = vector_db_url
        self.collection = collection

        self.__load_faiss_index()

    def __load_faiss_index(self):
        # ベクトルDB用のディレクトリが存在しない、または空の場合
        if not os.path.exists(self.vector_db_url):
            # ディレクトリを作成
            os.makedirs(self.vector_db_url)
        if len(os.listdir(self.vector_db_url)) == 0:    
            # faissのインデックスを読み込む
            docs = [
                Document(
                    page_content="",
                    metadata={"source": ""}
                )
            ]
            self.db = FAISS.from_documents(docs, self.langchain_openai_client.embeddings)
            # 保存
            self.__save()
            
        else:
            self.db = FAISS.load_local(
                self.vector_db_url, self.langchain_openai_client.embeddings,
                allow_dangerous_deserialization=True
                )  

    def __save(self):
        # 既存のDBを保存
        self.db.save_local(self.vector_db_url)

    def vector_search(self, query, k=10 , score_threshold=0.0):
        answers = self.db.similarity_search_with_relevance_scores(
            query, k=k, score_threshold=score_threshold)

        return answers

    def add_documents(self, documents: list):
        token_count = 0
        with get_openai_callback() as cb:
            # 新しいDBを作成
            new_db = FAISS.from_documents(documents, self.langchain_openai_client.embeddings)
            total_tokens = cb.total_tokens
        # 既存のDBにマージ
        self.db.merge_from(new_db)
        self.__save()
        
        return total_tokens

    def delete_doucments_by_sources(self, sources :list ):
        doc_ids = []
        # 既存のDBから指定されたsourceを持つドキュメントを削除
        
        for _id, doc in self.db.docstore._dict.items():
            if not doc.metadata.get("source_url", None):
                if doc.metadata.get("source", None) in [source.metadata.get("source",None) for source in sources]:
                    doc_ids.append(_id)
            else:
                source_url_check =  doc.metadata.get("source_url", None)in [source.metadata.get("source_url", None) for source in sources]
                source_path_check = doc.metadata.get("source", None) in [source.metadata.get("source", None) for source in sources]
                if source_url_check and source_path_check:
                    doc_ids.append(_id)

        if len(doc_ids) > 0:
            self.db.delete(doc_ids)
            self.__save()
    
    def update_documents(self, documents: list, props: dict):
        token_count = 0
        client = LangChainOpenAIClient(props)
        vector_db = LangChainVectorDB(client, props.get("VectorDBURL"))
        if len(documents) == 0:
            print("No documents to update.")
            return token_count
        
        # 既存のDBからソースファイルが一致するドキュメントを削除
        vector_db.delete_doucments_by_sources(documents)
        return vector_db.add_documents(documents)
        
    
