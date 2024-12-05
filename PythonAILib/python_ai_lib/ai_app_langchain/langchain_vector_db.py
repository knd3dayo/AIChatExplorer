
import sys

import uuid, json, os
from typing import Tuple, List, Any
import copy

from langchain_core.documents import Document
from langchain_text_splitters import RecursiveCharacterTextSplitter
from langchain.retrievers.multi_vector import MultiVectorRetriever
from langchain_core.runnables import chain
from langchain_core.callbacks import (
    CallbackManagerForRetrieverRun,
)
from collections import defaultdict
from typing import Optional

from ai_app_file.ai_app_file_util import FileUtil

from ai_app_langchain.ai_app_langchain_client import LangChainOpenAIClient
from ai_app_langchain.langchain_doc_store import SQLDocStore

from ai_app_openai.ai_app_openai_util import OpenAIProps

from ai_app_vector_db.ai_app_vector_db_props import ContentUpdateOrDeleteRequestParams, FileUpdateOrDeleteRequestParams
from ai_app_vector_db.ai_app_vector_db_props import VectorSearchParameter, VectorDBProps


# コレクションの指定がない場合はデフォルトのコレクション名を使用
DEFAULT_COLLECTION_NAME = "ai_app_default_collection"

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
            from ai_app_langchain.langchain_vector_db_chroma import LangChainVectorDBChroma
            return LangChainVectorDBChroma(langchain_openai_client, vector_db_props)
        # ベクトルDBのタイプがPostgresの場合
        elif vector_db_props.VectorDBTypeString == "PGVector":
            from ai_app_langchain.langchain_vector_db_pgvector import LangChainVectorDBPGVector
            return LangChainVectorDBPGVector(langchain_openai_client, vector_db_props)
        else:
            # それ以外の場合は例外
            raise ValueError("VectorDBType is invalid")
    
    @staticmethod
    def get_vector_db_with_default_collection(openai_props: OpenAIProps, vector_db_props: VectorDBProps):
        # vector_db_propsのコピーを作成
        new_vector_db_props = copy.deepcopy(vector_db_props)
        # デフォルトのコレクション名を設定
        new_vector_db_props.CollectionName = DEFAULT_COLLECTION_NAME
        return LangChainVectorDB.get_vector_db(openai_props, new_vector_db_props)

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
        self.db.delete_collection()
        # self.db.persist()

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
        retriever.vectorstore.add_documents(sub_docs)

        # 元のドキュメントをDocStoreに保存
        param = []
        param.append((doc_id, source_document))
        retriever.docstore.mset(param)


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
        
    def __create_metadata(self, doc_id, source: str, source_url: str, description: str, 
                          content_type: str, image_url: str, reliability: int):
        metadata = {"source": source, "source_url": source_url, "description": description,
                      "content_type": content_type, "image_url": image_url, "reliability": reliability,
                      "folder_id": self.vector_db_props.CollectionName,
                      "doc_id": doc_id
                }
        return metadata

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

    def _add_document_list(self, content_text: str, description_text: str, source: str, source_url: str, content_type:str="text" , image_url="", reliability=0):
        
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

    ########################################
    # パブリック
    ########################################
    def create_retriever(self, search_kwargs: dict[str, Any] = {}) -> Any:
        # ベクトルDB検索用のRetrieverオブジェクトの作成と設定

        vector_db_props = self.vector_db_props
        if not search_kwargs:
            if vector_db_props.CollectionName:
                search_kwargs = {"k": 10, 'filter': {'folder_id': vector_db_props.CollectionName},}
            else:
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

    def delete_document(self, source: str):
        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_multivector_document(source)
        else:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_document(source)
    
    def update_document(self, params: ContentUpdateOrDeleteRequestParams):
        
        # 既に存在するドキュメントを削除
        self.delete_document(params.source)
        # ドキュメントを格納する。
        self._add_document_list(params.text, params.description, params.source, params.source_url, reliability=params.reliability)

    def update_file_index(self, params: FileUpdateOrDeleteRequestParams):

        # ★TODO *_content_indexと同じ処理になっているので、共通化する
        # 既に存在するドキュメントを削除
        self.delete_document(params)

        # ファイルの存在チェック
        file_path = os.path.join(params.document_root, params.relative_path)
        if not os.path.exists(file_path):
            print("ファイルが存在しません。", file=sys.stderr)
            return

        # ドキュメントを格納
        self._add_file(params.document_root, params.relative_path, params.source_url, description=params.description, reliability=params.reliability)

    # ベクトル検索を行う
    @staticmethod
    def vector_search( params: VectorSearchParameter):    

        client = LangChainOpenAIClient(params.openai_props)

        # documentsの要素からcontent, source, source_urlを取得
        result = []
        # vector_db_propsの要素毎にRetrieverを作成して、検索を行う
        for vector_db_item in params.vector_db_props:

            # デバッグ出力
            print(f'検索文字列: {params.query}')
            print(f'検索条件: {params.search_kwarg}')
            print('ベクトルDBの設定')
            print(f'Name:{vector_db_item.Name} VectorDBDescription:{vector_db_item.VectorDBDescription} VectorDBTypeString:{vector_db_item.VectorDBTypeString} VectorDBURL:{vector_db_item.VectorDBURL} CollectionName:{vector_db_item.CollectionName}')
            retriever = LangChainVectorDB(client, vector_db_item).create_retriever(params.search_kwarg)
            documents: list[Document] = retriever.invoke(params.query)

            print(f"documents:\n{documents}")
            for doc in documents:
                content = doc.page_content
                doc_id = doc.metadata.get("doc_id", "")
                folder_id = doc.metadata.get("folder_id", "")
                source = doc.metadata.get("source", "")
                source_url = doc.metadata.get("source_url", "")
                score = doc.metadata.get("score", 0.0)
                # description, reliabilityを取得
                description = doc.metadata.get("description", "")
                reliability = doc.metadata.get("reliability", 0)

                sub_docs = doc.metadata.get("sub_docs", [])
                # sub_docsの要素からcontent, source, source_url,scoreを取得してdictのリストに追加
                sub_docs_result = []
                for sub_doc in sub_docs:
                    sub_content = sub_doc.page_content
                    sub_source = sub_doc.metadata.get("source", "")
                    sub_source_url = sub_doc.metadata.get("source_url", "")
                    sub_score = sub_doc.metadata.get("score", 0.0)
                    sub_doc_id = sub_doc.metadata.get("doc_id", "")
                    sub_folder_id = sub_doc.metadata.get("folder_id", "")
                    
                    sub_docs_result.append({
                        "doc_id": sub_doc_id, "folder_id": sub_folder_id,
                        "content": sub_content, "source": sub_source, "source_url": sub_source_url, "score": sub_score})

                result.append(
                    {"doc_id": doc_id, "folder_id": folder_id,
                    "content": content, "source": source, "source_url": source_url, "score": score, 
                    "description": description, "reliability": reliability, "sub_docs": sub_docs_result})
            
        return {"documents": result}

    
