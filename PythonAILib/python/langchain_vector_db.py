
import os, json, sys
import uuid
from langchain.docstore.document import Document
from langchain_community.callbacks import get_openai_callback

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
    def _get_metadata_by_source(self, sources:list={}) -> (list, list):
        pass

    def _save(self, documents:list=[]):
        pass

    def _delete(self, doc_ids:list=[]):
        pass

    def vector_search(self, query, k=10 , score_threshold=0.0):
        answers = self.db.similarity_search_with_relevance_scores(
            query, k=k, score_threshold=score_threshold)

        return answers

    def add_documents(self, documents: list):
        # MultiVectorRetrieverのテスト
        if self.vector_db_props.DocStoreURL:
            text_splitter = RecursiveCharacterTextSplitter(chunk_size=200)
            sub_docs = []
            doc_ids = []
            for doc in documents:
                # doc_idを生成
                doc_id = str(uuid.uuid4())
                # doc_idsにdoc_idを追加
                doc_ids.append(doc_id)

                # docを分割
                tmp_sub_docs = text_splitter.split_documents([doc])
                # sub_docsのmetadataにdoc_idを追加
                print(f"add_documents doc_id:{doc_id}")

                for tmp_sub_doc in tmp_sub_docs:
                    tmp_sub_doc.metadata["doc_id"] = doc_id
                    print(f"sub_document:{tmp_sub_doc}")
                    
                # sub_docsを追加
                sub_docs.extend(tmp_sub_docs)    
        
            # ベクトルDB固有の保存メソッドを呼び出し                
            self._save(sub_docs)
            
            # DocStoreに保存
            self.doc_store.mset(list(zip(doc_ids, documents)))
            
        else:
            # ベクトルDB固有の保存メソッドを呼び出し                
            self._save(documents)

        return len(documents)
        
    def delete_doucments(self, sources :list =[] ):
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata = self._get_metadata_by_source(sources)
        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0

        # DocStoreURLが指定されている場合はDocStoreから削除
        if self.vector_db_props.DocStoreURL:
            # documentのmetadataのdoc_idを取得
            doc_ids = [data.get("doc_id", None) for data in metadata]
            # doc_idsが空ではない場合
            if len(doc_ids) > 0:
                # DocStoreから削除
                self.doc_store.mdelete(doc_ids)
            
        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)
    
        return len(sources)
    


        
    
