
import sys
from langchain.docstore.document import Document

sys.path.append("python")
from langchain_client import LangChainOpenAIClient
from langchain_doc_store import SQLDocStore
from openai_props import VectorDBProps

from langchain_text_splitters import RecursiveCharacterTextSplitter

class LangChainVectorDB:

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_props = vector_db_props
        if vector_db_props.IsUseMultiVectorRetriever:
            print("DocStoreURL:", vector_db_props.DocStoreURL)
            self.doc_store = SQLDocStore(vector_db_props.DocStoreURL)
        else:
            print("DocStoreURL is None")

        self.load()

    def load(self):
        pass

    
    # vector idのリストとmetadataのリストを返す
    def _get_metadata_by_source(self, sources:list={}) -> tuple[list, list]:
        pass

    def _save(self, documents:list=[]):
        pass

    def _delete(self, doc_ids:list=[]):
        pass

    def vector_search(self, query, k=10 , score_threshold=0.0):
        answers = self.db.similarity_search_with_relevance_scores(
            query, k=k, score_threshold=score_threshold)

        return answers

    def add_multivector_document(self, source_document: Document):
        # MultiVectorRetrieverのテスト
        text_splitter = RecursiveCharacterTextSplitter(chunk_size=200)
        # doc_idを取得
        doc_id = source_document.metadata.get("doc_id", None)
        # doc_idがNoneの場合はエラー
        if doc_id is None:
            raise ValueError("doc_id is None")

        # source_documentを分割
        tmp_sub_docs = text_splitter.split_documents([source_document])

        for tmp_sub_doc in tmp_sub_docs:
            tmp_sub_doc.metadata["doc_id"] = doc_id
            tmp_sub_doc.metadata["source"] = source_document.metadata.get("source", None)
            tmp_sub_doc.metadata["source_url"] = source_document.metadata.get("source_url", None)
                    
        # ベクトルDB固有の保存メソッドを呼び出し                
        self._save(tmp_sub_docs)
            
        # DocStoreに保存
        param = []
        param.append((doc_id, source_document))
        self.doc_store.mset(param)

    def delete_multivector_document(self, source_document: Document ) -> int:
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata = self._get_metadata_by_source([source_document])
        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0
        # documentのmetadataのdoc_idを取得
        doc_ids = [data.get("doc_id", None) for data in metadata]
        # doc_idsが空ではない場合
        if len(doc_ids) > 0:
            # DocStoreから削除
            self.doc_store.mdelete(doc_ids)

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)
        
    
    def add_document(self, document: Document):
        # ベクトルDB固有の保存メソッドを呼び出し                
        self._save([document])

    def delete_document(self, source: str):
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata = self._get_metadata_by_source([source])
        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)
    


        
    
