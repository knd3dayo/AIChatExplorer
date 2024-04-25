
import os, json, sys
from langchain_community.vectorstores import FAISS
from langchain.docstore.document import Document

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
        if not os.path.exists(self.vector_db_url) or len(os.listdir(self.vector_db_url)) == 0:    
            # faissのインデックスを読み込む
            docs = [
                Document(
                    page_content="",
                    metadata={"source": ""}
                )
            ]
            self.db = FAISS.from_documents(docs, self.langchain_openai_client.embeddings)
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

    def add_documents(self, documents=[]):
        # 新しいDBを作成
        new_db = FAISS.from_documents(documents, self.langchain_openai_client.embeddings)
        # 既存のDBにマージ
        self.db.merge_from(new_db)
        self.__save()

    def delete_doucments_by_sources(self, sources: list):
        doc_ids = []
        # 既存のDBから指定されたsourceを持つドキュメントを削除
        for _id, doc in self.db.docstore._dict.items():
            if doc.metadata["source"] in sources:
                doc_ids.append(_id)
        if len(doc_ids) > 0:
            self.db.delete(doc_ids)
            self.__save()
    
    
