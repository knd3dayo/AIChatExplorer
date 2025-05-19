
import uuid
from typing import Tuple, List, Any, Union, Optional
from collections import defaultdict
import asyncio

from langchain_core.documents import Document
from langchain_core.vectorstores import VectorStore

from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain.retrievers.multi_vector import MultiVectorRetriever
from langchain_core.runnables import chain
from langchain_core.callbacks import (
    CallbackManagerForRetrieverRun,
)
from openai import RateLimitError

from ai_chat_lib.langchain_modules.langchain_client import LangChainOpenAIClient
from ai_chat_lib.langchain_modules.langchain_doc_store import SQLDocStore

from ai_chat_lib.db_modules import EmbeddingData

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

class CustomMultiVectorRetriever(MultiVectorRetriever):
    def _get_relevant_documents(self, query: str, *, run_manager: CallbackManagerForRetrieverRun) -> list[Document]:
        """Get documents relevant to a query.
        Args:
            query: String to find relevant documents for
            run_manager: The callbacks handler to use
        Returns:
            List of relevant documents
        """
        results = self.vectorstore.similarity_search_with_relevance_scores(query, **self.search_kwargs)

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
    
    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_url :str, collection_name :str = "", multi_vector_doc_store_url: str = "", chunk_size: int = 1024):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_url = vector_db_url
        self.collection_name = collection_name
        self.multi_vector_doc_store_url = multi_vector_doc_store_url
        self.chunk_size = chunk_size

        # 子クラスで実装
        self.db: Union[VectorStore, None] = None
        self.doc_store: Union[SQLDocStore, None] = None

    def _load(self) -> VectorStore:
        # 未実装例外をスロー
        raise NotImplementedError("Not implemented")

    # document_idのリストとmetadataのリストを返す
    def _get_document_ids_by_tag(self, name: str = "", value: str = "") -> Tuple[List, List]:
        # 未実装例外をスロー
        raise NotImplementedError("Not implemented")

    async def _save(self, documents:list=[]):
        if self.db is None:
            raise ValueError("db is None")
        # リトライ処理
        await self.add_doucment_with_retry(self.db, documents)    

    def _delete(self, doc_ids:list=[]):
        if len(doc_ids) == 0:
            return
        if self.db is None:
            raise ValueError("db is None")

        self.db.delete(ids=doc_ids)

        return len(doc_ids)    

    def _delete_collection(self):
        self.db.delete_collection()
        # self.db.persist()

    async def __add_document(self, document: Document):
        # ベクトルDB固有の保存メソッドを呼び出し                
        await self._save([document] )

    async def __add_multivector_document(self, source_document: Document):

        # doc_idを取得
        doc_id = source_document.metadata.get("doc_id", None)
        # doc_idがNoneの場合はエラー
        if doc_id is None:
            raise ValueError("doc_id is None")

        text = source_document.page_content
        # チャンクサイズ 
        chunk_size_list = []
        # textの長さが256以上の場合は256に分割
        # if len(text) > 256:
        #     chunk_size_list.append(256)
        # textの長さが512以上の場合は512に分割
        # if len(text) > 512:
        #     chunk_size_list.append(512)
        # textの長さが1024以上の場合は1024に分割
        if len(text) > 1024:
            chunk_size_list.append(1024)
        
        # テキストをchunk_size_listの値で分割
        sub_docs = []
        if len(chunk_size_list) == 0:
            sub_docs.append(source_document)
        else:
            for chunk_size in chunk_size_list:
                text_splitter = RecursiveCharacterTextSplitter(chunk_size=chunk_size)
                splited_docs = text_splitter.split_documents([source_document])
                if len(splited_docs) == 0:
                    raise ValueError("splited_docs is empty")
                for sub_doc in splited_docs:
                    sub_doc.metadata["doc_id"] = doc_id
                    sub_docs.append(sub_doc)

        # Retoriverを作成
        retriever = self.create_retriever()
        # ドキュメントを追加
        await self.add_doucment_with_retry(retriever.vectorstore, sub_docs)    

        # 元のドキュメントをDocStoreに保存
        param = []
        param.append((doc_id, source_document))
        retriever.docstore.mset(param)

    def __delete_folder(self, folder_id: str):
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata = self._get_document_ids_by_tag("folder_id", folder_id)
        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)

    def __delete_multivector_folder(self, folder_id: str ) :
        
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata_list = self._get_document_ids_by_tag("older_id", folder_id)

        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0
        # documentのmetadataのdoc_idを取得
        doc_ids = [data.get("doc_id", None) for data in metadata_list]
        # doc_idsが空ではない場合
        if len(doc_ids) > 0:
            # DocStoreから削除
            if self.doc_store is not None:
                self.doc_store.mdelete(doc_ids)

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)


    def __delete_document(self, source_id: str):
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata = self._get_document_ids_by_tag("source_id", source_id)
        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)

    def __delete_multivector_document(self, source_id: str ) :
        
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata_list = self._get_document_ids_by_tag("source_id", source_id)

        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0
        # documentのmetadataのdoc_idを取得
        doc_ids = [data.get("doc_id", None) for data in metadata_list]
        # doc_idsが空ではない場合
        if len(doc_ids) > 0:
            # DocStoreから削除
            if self.doc_store is not None:
                self.doc_store.mdelete(doc_ids)

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)

    def __create_decorated_retriever(self, vectorstore: VectorStore, **kwargs: Any):
        # ベクトル検索の結果にスコアを追加する
        @chain
        def retriever(query: str) -> list[Document]:
            result = []
            params = kwargs.copy()
            params["query"] = query
            search_results = vectorstore.similarity_search_with_relevance_scores(**params)
            if not search_results:
                return []

            docs, scores = zip(*search_results)
            for doc, score in zip(docs, scores):
                doc.metadata["score"] = score
                result.append(doc)
            return result   

        return retriever

    async def add_document_list(self, content_text: str, description_text: str, folder_id: str, source_id: str, source_path: str,  chunk_size: int) -> list[Document]:
        
        document_list = []

        # テキストをサニタイズ
        content_text = self._sanitize_text(content_text)
        # テキストをchunk_sizeで分割
        text_list = self._split_text(content_text, chunk_size=chunk_size)
        for text in text_list:
            doc_id = str(uuid.uuid4())
            logger.info(f"folder_id:{folder_id}")
            metadata = LangChainVectorDB.create_metadata(doc_id, source_id, folder_id, "", source_path, description_text)
            logger.debug("metadata:", metadata)
            document = Document(page_content=text, metadata=metadata)
            document_list.append(document)

        # MultiVectorRetrieverの場合はadd_multivector_documentを呼び出す
        if self.multi_vector_doc_store_url:
            for document in document_list:
                await self.__add_multivector_document(document)
            return document_list
        else:
            for document in document_list:
                await self.__add_document(document)
            return document_list


    # テキストをサニタイズする
    def _sanitize_text(self, text: str) -> str:
        # textが空の場合は空の文字列を返す
        if not text or len(text) == 0:
            return ""
        import re
        # 1. 複数の改行を1つの改行に変換
        text = re.sub(r'\n+', '\n', text)
        # 2. 複数のスペースを1つのスペースに変換
        text = re.sub(r' +', ' ', text)

        return text

    def _split_text(self, text: str, chunk_size: int):
        text_list : list[str] = []
        # textが空の場合は空のリストを返す
        if not text or len(text) == 0:
            return text_list
        
        # テキストをchunk_sizeで分割
        for i in range(0, len(text), chunk_size):
            text_list.append(text[i:i + chunk_size])
        return text_list

    ########################################
    # パブリック
    ########################################
    def create_retriever(self, search_kwargs: dict[str, Any] = {}) -> Any:
        # ベクトルDB検索用のRetrieverオブジェクトの作成と設定

        if not search_kwargs:
            # デフォルトの検索パラメータを設定
            logger.info("search_kwargs is empty. Set default search_kwargs")
            search_kwargs = {"k": 10}

        # IsUseMultiVectorRetriever=Trueの場合はMultiVectorRetrieverを生成
        if self.db is None:
            raise ValueError("db is None")
        if self.multi_vector_doc_store_url:
            logger.info("Creating MultiVectorRetriever")
            if self.doc_store is None:
                raise ValueError("doc_store is None")
            
            retriever = CustomMultiVectorRetriever(
                vectorstore=self.db,
                docstore=self.doc_store,
                id_key="doc_id",
                search_kwargs=search_kwargs
            )

        else:
            logger.debug("Creating a regular Retriever")
            retriever = self.__create_decorated_retriever(self.db, **search_kwargs)
         
        return retriever


    def delete_collection(self):
        # ベクトルDB固有の削除メソッドを呼び出してコレクションを削除
        self._delete_collection()

    def delete_folder(self, folder_id: str):
        # MultiVectorRetrieverの場合
        if self.multi_vector_doc_store_url:
            # DBからfolder_idを指定して既存フォルダを削除
            self.__delete_multivector_folder(folder_id)
        else:
            # DBからfolder_idを指定して既存フォルダを削除
            self.__delete_folder(folder_id)
            
    def delete_document(self, source_id: str):
        # MultiVectorRetrieverの場合
        if self.multi_vector_doc_store_url:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_multivector_document(source_id)
        else:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_document(source_id)
    
    async def update_document(self, params: EmbeddingData):
        
        # 既に存在するドキュメントを削除
        self.delete_document(params.source_id)
        # ドキュメントを格納する。
        await self.add_document_list(params.content, params.description, params.FolderId, params.source_id, params.source_path, chunk_size=self.chunk_size)

    # RateLimitErrorが発生した場合は、指数バックオフを行う
    async def add_doucment_with_retry(self, vector_db: VectorStore, documents: list[Document], max_retries: int = 5, delay: float = 1.0):
        for attempt in range(max_retries):
            try:
                await vector_db.aadd_documents(documents=documents)
                return
            except RateLimitError as e:
                if attempt < max_retries - 1:
                    logger.warning(f"RateLimitError: {e}. Retrying in {delay} seconds...")
                    await asyncio.sleep(delay)
                    delay *= 2
                else:
                    logger.error(f"Max retries reached. Failed to add documents: {e}")
                    break
            except Exception as e:
                logger.error(f"Error adding documents: {e}")
                break

    @classmethod
    def create_metadata(cls, 
                        doc_id: str, source_id: str, folder_id: str, 
                        folder_path: str, source_path: str,
                        description: str, score = 0.0
                        ) -> dict[str, Any]:
        metadata = {
            "folder_id": folder_id, "folder_path": folder_path, "source_path": source_path,   
            "description": description, 
            "doc_id": doc_id, "source_id": source_id, "source_type": 0, "score": score
        }
        return metadata

    @classmethod
    def create_metadata_from_document(cls, document: Document):
        metadata = document.metadata
        source_id = metadata.get("source_id", "")
        folder_id = metadata.get("folder_id", "")
        doc_id = metadata.get("doc_id", "")
        source_path = metadata.get("source_path", "")
        description = metadata.get("description", "")
        score = metadata.get("score", 0)
        folder_path = metadata.get("folder_path", "")
        return cls.create_metadata(doc_id, source_id, folder_id, folder_path ,source_path, description, score)

    
