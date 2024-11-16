
import sys

import uuid, json, os
from typing import Tuple, List, Any
import copy

from langchain_doc_store import SQLDocStore
from langchain_core.documents import Document
from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain.retrievers.multi_vector import MultiVectorRetriever

from ai_app_file_util import FileUtil
from ai_app_vector_db_util import VectorDBProps, ContentUpdateOrDeleteRequestParams, ImageUpdateOrDeleteRequestParams, FileUpdateOrDeleteRequestParams
from ai_app_langchain_util import LangChainOpenAIClient
from ai_app_openai_util import OpenAIProps
from langchain_core.runnables import chain
from langchain_core.callbacks import (
    CallbackManagerForRetrieverRun,
)
from collections import defaultdict
from typing import Optional

class CustomMultiVectorRetriever(MultiVectorRetriever):
    def _get_relevant_documents(self, query: str, *, run_manager: CallbackManagerForRetrieverRun) -> list[Document]:
        """Get documents relevant to a query.
        Args:
            query: String to find relevant documents for
            run_manager: The callbacks handler to use
        Returns:
            List of relevant documents
        """
        results = self.vectorstore.similarity_search_with_score(query, **self.search_kwargs)

        # Map doc_ids to list of sub-documents, adding scores to metadata
        id_to_doc = defaultdict(list)
        for doc, score in results:
            doc_id = doc.metadata.get("doc_id")
            if doc_id:
                doc.metadata["score"] = score
                id_to_doc[doc_id].append(doc)

        # Fetch documents corresponding to doc_ids, retaining sub_docs in metadata
        docs = []
        for _id, sub_docs in id_to_doc.items():
            docstore_docs = self.docstore.mget([_id])
            if docstore_docs:
                docstore_doc: Optional[Document]= docstore_docs[0]
                if docstore_doc is not None:
                    docstore_doc.metadata["sub_docs"] = sub_docs
                    docs.append(docstore_doc)

        return docs

class LangChainVectorDB:

    @staticmethod
    def get_vector_db(openai_props: OpenAIProps, vector_db_props: VectorDBProps):

        langchain_openai_client = LangChainOpenAIClient(openai_props)
        # ベクトルDBのタイプがChromaの場合
        if vector_db_props.VectorDBTypeString == "Chroma":
            from langchain_vector_db_chroma import LangChainVectorDBChroma
            return LangChainVectorDBChroma(langchain_openai_client, vector_db_props)
        # ベクトルDBのタイプがPostgresの場合
        elif vector_db_props.VectorDBTypeString == "PGVector":
            from langchain_vector_db_pgvector import LangChainVectorDBPGVector
            return LangChainVectorDBPGVector(langchain_openai_client, vector_db_props)
        else:
            # それ以外の場合は例外
            raise ValueError("VectorDBType is invalid")

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_props: VectorDBProps):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_props = vector_db_props
        if vector_db_props.IsUseMultiVectorRetriever:
            print("DocStoreURL:", vector_db_props.DocStoreURL)
            self.doc_store = SQLDocStore(vector_db_props.DocStoreURL)
        else:
            print("DocStoreURL is None")

        self._load()

    def _load(self):
        pass

    # document_idのリストとmetadataのリストを返す
    def _get_document_ids_by_tag(self, name: str = "", value: str = "") -> Tuple[List, List]:
        # 未実装例外をスロー
        raise NotImplementedError("Not implemented")

    def _save(self, documents:list=[]):
        # 未実装例外をスロー
        raise NotImplementedError("Not implemented")

    def _delete(self, doc_ids:list=[]):
        # 未実装例外をスロー
        raise NotImplementedError("Not implemented")

    def _delete_collection(self):
        # 未実装例外をスロー
        raise NotImplementedError("Not implemented")
    
    def __add_document(self, document: Document):
        # ベクトルDB固有の保存メソッドを呼び出し                
        self._save([document])

    def __add_multivector_document(self, source_document: Document):

        # doc_idを取得
        doc_id = source_document.metadata.get("doc_id", None)
        # doc_idがNoneの場合はエラー
        if doc_id is None:
            raise ValueError("doc_id is None")

        text = source_document.page_content
        # チャンクサイズ 
        chunk_size_list = []
        # textの長さが256以上の場合は256に分割
        if len(text) > 256:
            chunk_size_list.append(256)
        # textの長さが512以上の場合は512に分割
        if len(text) > 512:
            chunk_size_list.append(512)
        # textの長さが1024以上の場合は1024に分割
        if len(text) > 1024:
            chunk_size_list.append(1024)
        
        # テキストをchunk_size_listの値で分割
        for chunk_size in chunk_size_list:
            sub_docs = []
            text_splitter = RecursiveCharacterTextSplitter(chunk_size=chunk_size)
            sub_docs = text_splitter.split_documents([source_document])
            for sub_doc in sub_docs:
                sub_doc.metadata["doc_id"] = doc_id
        
        # Retoriverを作成
        retriever = self.create_retriever()
        # ドキュメントを追加
        retriever.vectorstore.add_documents(sub_docs)

        # 元のドキュメントをDocStoreに保存
        param = []
        param.append((doc_id, source_document))
        retriever.doc_store.mset(param)

    def __delete_document(self, source: str):
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata = self._get_document_ids_by_tag("source", source)
        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)

    def __delete_multivector_document(self, source: str ) :
        
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata_list = self._get_document_ids_by_tag("source", source)

        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0
        # documentのmetadataのdoc_idを取得
        doc_ids = [data.get("doc_id", None) for data in metadata_list]
        # doc_idsが空ではない場合
        if len(doc_ids) > 0:
            # DocStoreから削除
            self.doc_store.mdelete(doc_ids)

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)
        

    def _add_file(self, document_root: str, relative_path: str, source_url: str ,description:str="", reliability=0):

        # チャンクサイズの取得
        chunk_size = self.vector_db_props.ChunkSize
        # 絶対パスを取得
        absolute_file_path = os.path.join (document_root, relative_path)
        
        # ファイルサイズが0の場合は空のリストを返す
        if os.path.getsize(absolute_file_path) == 0:
            return []
        # テキスト抽出
        text = FileUtil.extract_text_from_file(absolute_file_path)

        # テキストを分割してDocumentのリストを返す
        return self._add_document_list(text, description, relative_path, source_url, chunk_size, reliability=reliability)

    def __create_metadata(self, doc_id, source: str, source_url: str, description: str, 
                          content_type: str, image_url: str, reliability: int):
        metadata = {"source": source, "source_url": source_url, "description": description,
                      "content_type": content_type, "image_url": image_url, "reliability": reliability,
                      "folder_id": self.vector_db_props.CollectionName,
                      "doc_id": doc_id
                }
        return metadata
    
    def _add_document_list(self, content_text: str, description_text: str, source: str, source_url: str, content_type:str="text" , image_url="", reliability=0):
        
        # MultiVectorRetrieverの場合はchunk_size=MultiVectorRetrieverのChunkSize
        if self.vector_db_props.IsUseMultiVectorRetriever:
            chunk_size = self.vector_db_props.MultiVectorDocChunkSize
        else:
            chunk_size = self.vector_db_props.ChunkSize
    
        document_list = []
        print("content_type:", content_type)

        if not content_type:
            content_type = "text"
        # content_typeが"text"または"image"以外の場合は例外をスロー
        if content_type not in ["text", "image"]:
            raise ValueError("content_type must be 'text' or 'image'")

        # テキストをchunk_sizeで分割
        text_list = self._split_text(content_text, chunk_size=chunk_size)
        for text in text_list:
            doc_id = str(uuid.uuid4())
            metadata = self.__create_metadata(doc_id, source, source_url, description_text, content_type, image_url, reliability)
            print("metadata:", metadata)
            document = Document(page_content=text, metadata=metadata)
            document_list.append(document)

        # MultiVectorRetrieverの場合はadd_multivector_documentを呼び出す
        if self.vector_db_props.IsUseMultiVectorRetriever:
            for document in document_list:
                self.__add_multivector_document(document)
            return document_list
        else:
            for document in document_list:
                self.__add_document(document)


    def _split_text(self, text: str, chunk_size: int):
        text_list = []
        # テキストをchunk_sizeで分割
        for i in range(0, len(text), chunk_size):
            text_list.append(text[i:i + chunk_size])
        return text_list

    def __create_decorated_retriever(self, vectorstore, **kwargs: Any):
        # ベクトル検索の結果にスコアを追加する
        @chain
        def retriever(query: str) -> list[Document]:
            result = []
            docs, scores = zip(*vectorstore.similarity_search_with_score(query, kwargs))
            for doc, score in zip(docs, scores):
                doc.metadata["score"] = score
                result.append(doc)
            return result

        return retriever

    def create_retriever(self, search_kwargs: dict[str, Any] = {}) -> Any:
        # ベクトルDB検索用のRetrieverオブジェクトの作成と設定

        vector_db_props = self.vector_db_props
        if not search_kwargs:
            search_kwargs = {"k": 10}

        # IsUseMultiVectorRetriever=Trueの場合はMultiVectorRetrieverを生成
        if vector_db_props.IsUseMultiVectorRetriever:
            print("Creating MultiVectorRetriever")
            
            langChainVectorDB = LangChainVectorDB.get_vector_db(self.langchain_openai_client.props, vector_db_props)
            retriever = CustomMultiVectorRetriever(
                vectorstore=langChainVectorDB.db,
                docstore=langChainVectorDB.doc_store,
                id_key="doc_id",
                search_kwargs=search_kwargs
            )

        else:
            print("Creating a regular Retriever")
            langChainVectorDB = LangChainVectorDB.get_vector_db(self.langchain_openai_client.props, vector_db_props)
            retriever = self.__create_decorated_retriever(langChainVectorDB.db, search_kwargs=search_kwargs)
         
        return retriever


    def delete_collection(self):
        # ベクトルDB固有の削除メソッドを呼び出してコレクションを削除
        self._delete_collection()

    def delete_content_index(self, source: str):
        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_multivector_document(source)
        else:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_document(source)
    
    def update_content_index(self, params: ContentUpdateOrDeleteRequestParams):
        
        # 既に存在するドキュメントを削除
        self.delete_content_index(params.source)
        # ドキュメントを格納する。
        self._add_document_list(params.text, params.description, params.source, params.source_url, reliability=params.reliability)

    def update_image_index(self, params: ImageUpdateOrDeleteRequestParams):
        # 既に存在するドキュメントを削除
        self.delete_content_index(params.source)
        # ドキュメントを取得
        self._add_document_list(params.text, params.description, params.source_url, params.source, 
                                content_type="image", image_url=params.image_url , reliability=params.reliability)
        
    def update_file_index(self, params: FileUpdateOrDeleteRequestParams):

        # ★TODO *_content_indexと同じ処理になっているので、共通化する
        # 既に存在するドキュメントを削除
        self.delete_content_index(params)

        # ファイルの存在チェック
        file_path = os.path.join(params.document_root, params.relative_path)
        if not os.path.exists(file_path):
            print("ファイルが存在しません。", file=sys.stderr)
            return

        # ドキュメントを格納
        self._add_file(params.document_root, params.relative_path, params.source_url, description=params.description, reliability=params.reliability)

    
