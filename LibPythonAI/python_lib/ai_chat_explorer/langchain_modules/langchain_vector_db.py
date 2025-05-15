
import uuid
from typing import Tuple, List, Any, Union, Optional
from collections import defaultdict
import copy
import logging
import asyncio

from langchain_core.documents import Document
from langchain_core.vectorstores import VectorStore

from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain.retrievers.multi_vector import MultiVectorRetriever
from langchain_core.runnables import chain
from langchain_core.callbacks import (
    CallbackManagerForRetrieverRun,
)

from ai_chat_explorer.langchain_modules.langchain_client import LangChainOpenAIClient
from ai_chat_explorer.langchain_modules.langchain_doc_store import SQLDocStore

from ai_chat_explorer.openai_modules.openai_util import OpenAIProps

from ai_chat_explorer.db_modules import EmbeddingData, VectorDBItem

logger = logging.getLogger(__name__)

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

    @staticmethod
    def get_vector_db(openai_props: OpenAIProps, vector_db_props: VectorDBItem):

        langchain_openai_client = LangChainOpenAIClient(openai_props)
        # ベクトルDBのタイプがChromaの場合
        if vector_db_props.VectorDBTypeString == "Chroma":
            from ai_chat_explorer.langchain_modules.langchain_vector_db_chroma import LangChainVectorDBChroma
            return LangChainVectorDBChroma(langchain_openai_client, vector_db_props)
        # ベクトルDBのタイプがPostgresの場合
        elif vector_db_props.VectorDBTypeString == "PGVector":
            from ai_chat_explorer.langchain_modules.langchain_vector_db_pgvector import LangChainVectorDBPGVector
            return LangChainVectorDBPGVector(langchain_openai_client, vector_db_props)
        else:
            # それ以外の場合は例外
            raise ValueError("VectorDBType is invalid")
    
    @staticmethod
    def get_vector_db_with_default_collection(openai_props: OpenAIProps, vector_db_props: VectorDBItem):
        # vector_db_propsのコピーを作成
        new_vector_db_props = copy.deepcopy(vector_db_props)
        # デフォルトのコレクション名を設定
        new_vector_db_props.CollectionName = VectorDBItem.DEFAULT_COLLECTION_NAME

        return LangChainVectorDB.get_vector_db(openai_props, new_vector_db_props)

    def __init__(self, langchain_openai_client: LangChainOpenAIClient, vector_db_props: VectorDBItem):
        self.langchain_openai_client = langchain_openai_client
        self.vector_db_props = vector_db_props
        if vector_db_props.IsUseMultiVectorRetriever:
            print("doc_store_url:", vector_db_props.DocStoreURL)
            self.doc_store = SQLDocStore(vector_db_props.DocStoreURL)
        else:
            print("doc_store_url is None")

        # 子クラスで実装
        self.db: Union[VectorStore, None] = None

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
        
        await self.db.aadd_documents(documents=documents, embedding=self.langchain_openai_client.get_embedding_client())

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

    def __add_document(self, document: Document):
        # ベクトルDB固有の保存メソッドを呼び出し                
        asyncio.run( self._save([document] ))

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
        asyncio.run(retriever.vectorstore.aadd_documents(sub_docs))

        # 元のドキュメントをDocStoreに保存
        param = []
        param.append((doc_id, source_document))
        retriever.docstore.mset(param)

    def __delete_folder(self, folder_id: str):
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata = self._get_document_ids_by_tag("FolderId", folder_id)
        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)

    def __delete_multivector_folder(self, folder_id: str ) :
        
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata_list = self._get_document_ids_by_tag("FolderId", folder_id)

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
            self.doc_store.mdelete(doc_ids)

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)

    def __create_decorated_retriever(self, vectorstore, **kwargs: Any):
        # ベクトル検索の結果にスコアを追加する
        @chain
        def retriever(query: str) -> list[Document]:
            result = []
            params = {}
            if kwargs:
                params = kwargs
            params["query"] = query
            docs, scores = zip(*vectorstore.similarity_search_with_relevance_scores(**params))
            for doc, score in zip(docs, scores):
                doc.metadata["score"] = score
                result.append(doc)
            return result

        return retriever

    def add_document_list(self, content_text: str, description_text: str, folder_id: str, source_id: str, source_path: str, source_url: str, image_url=""):
        
        chunk_size = self.vector_db_props.ChunkSize
    
        document_list = []

        # テキストをサニタイズ
        content_text = self._sanitize_text(content_text)
        # テキストをchunk_sizeで分割
        text_list = self._split_text(content_text, chunk_size=chunk_size)
        for text in text_list:
            doc_id = str(uuid.uuid4())
            logger.info(f"folder_id:{folder_id}")
            metadata = LangChainVectorDB.create_metadata(doc_id, source_id, folder_id, source_path, source_url, description_text, image_url)
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

        vector_db_props = self.vector_db_props
        if not search_kwargs:
            # デフォルトの検索パラメータを設定
            print("search_kwargs is empty. Set default search_kwargs")
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
            retriever = self.__create_decorated_retriever(langChainVectorDB.db, **search_kwargs)
         
        return retriever


    def delete_collection(self):
        # ベクトルDB固有の削除メソッドを呼び出してコレクションを削除
        self._delete_collection()

    def delete_folder(self, folder_id: str):
        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            # DBからfolder_idを指定して既存フォルダを削除
            self.__delete_multivector_folder(folder_id)
        else:
            # DBからfolder_idを指定して既存フォルダを削除
            self.__delete_folder(folder_id)
            
    def delete_document(self, source_id: str):
        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_multivector_document(source_id)
        else:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_document(source_id)
    
    def update_document(self, params: EmbeddingData):
        
        # 既に存在するドキュメントを削除
        self.delete_document(params.source_id)
        # ドキュメントを格納する。
        self.add_document_list(params.content, params.description, params.FolderId, params.source_id, params.source_path, params.git_relative_path)

    # ベクトル検索を行う
    @classmethod
    def vector_search(cls, openai_props: OpenAIProps, vector_db_props: list[VectorDBItem]) -> dict[str, Any]:    

        if not openai_props:
            raise ValueError("openai_props is None")
        client = LangChainOpenAIClient(openai_props)

        # documentsの要素からcontent, source, source_urlを取得
        result = []
        # vector_db_propsの要素毎にRetrieverを作成して、検索を行う
        for vector_db_item in vector_db_props:

            # デバッグ出力
            logger.info(f'検索文字列: {vector_db_item.input_text}')
            logger.info('ベクトルDBの設定')
            logger.info(f'name:{vector_db_item.Name} vector_db_description:{vector_db_item.Description} VectorDBTypeString:{vector_db_item.VectorDBTypeString} VectorDBURL:{vector_db_item.VectorDBURL} CollectionName:{vector_db_item.CollectionName}')
            logger.info(f'SearchKwargs:{vector_db_item.SearchKwargs}')
            logger.info(f'IsUseMultiVectorRetriever:{vector_db_item.IsUseMultiVectorRetriever}')
            retriever = LangChainVectorDB(client, vector_db_item).create_retriever(vector_db_item.SearchKwargs)
            documents: list[Document] = retriever.invoke(vector_db_item.input_text)

            logger.debug(f"documents:\n{documents}")
            for doc in documents:
                content = doc.page_content
                doc_dict = cls.create_metadata_from_document(doc)
                doc_dict["content"] = content

                sub_docs: list[Document]= doc.metadata.get("sub_docs", [])
                # sub_docsの要素からcontent, source, source_url,scoreを取得してdictのリストに追加
                sub_docs_result = []
                for sub_doc in sub_docs:
                    content = sub_doc.page_content
                    sub_doc_dict = cls.create_metadata_from_document(sub_doc)
                    sub_doc_dict["content"] = content
                    sub_docs_result.append(sub_doc_dict)

                doc_dict["sub_docs"] = sub_docs_result
                result.append(doc_dict)
            
        return {"documents": result}

    @classmethod
    def create_metadata(cls, doc_id, source_id, folder_id, source_path: str, source_url: str, description: str, image_url: str, score = 0.0):
        metadata = {
            "folder_id": folder_id, "source_path": source_path, "git_repository_url": source_url, 
            "description": description, "image_url": image_url, "git_relative_path": "",
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
        source_url = metadata.get("git_repository_url", "")
        description = metadata.get("description", "")
        image_url = metadata.get("image_url", "")
        score = metadata.get("score", 0)
        return cls.create_metadata(doc_id, source_id, folder_id, source_path, source_url, description, image_url, score)

    
