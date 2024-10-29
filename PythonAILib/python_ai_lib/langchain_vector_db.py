
import sys
sys.path.append("python")

import uuid, json, os
from typing import Tuple, List

from langchain_client import LangChainOpenAIClient
from langchain_doc_store import SQLDocStore
from langchain_core.documents import Document
from langchain_text_splitters import RecursiveCharacterTextSplitter

import file_extractor
from openai_props import OpenAIProps, VectorDBProps


class LangChainVectorDB:

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

    def __add_multivector_document(self, source_document: Document):
        # チャンクサイズ
        chunk_size = self.vector_db_props.ChunkSize
        # MultiVectorRetriever 
        text_splitter = RecursiveCharacterTextSplitter(chunk_size=chunk_size)
        # doc_idを取得
        doc_id = source_document.metadata.get("doc_id", None)
        # doc_idがNoneの場合はエラー
        if doc_id is None:
            raise ValueError("doc_id is None")

        tmp_sub_docs = [source_document]
        # content_type = "text"の場合はsource_documentを分割
        if source_document.metadata.get("content_type", "text") == "text":
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
        
    
    def __add_document(self, document: Document):
        # ベクトルDB固有の保存メソッドを呼び出し                
        self._save([document])

    def __delete_document(self, source: str):
        # ベクトルDB固有のvector id取得メソッドを呼び出し。
        vector_ids, metadata = self._get_document_ids_by_tag("source", source)
        # vector_idsが空の場合は何もしない
        if len(vector_ids) == 0:
            return 0

        # ベクトルDB固有の削除メソッドを呼び出し
        self._delete(vector_ids)


    def vector_search(self, query, k=10 , score_threshold=0.0):
        answers = self.db.similarity_search_with_relevance_scores(
            query, k=k, score_threshold=score_threshold)

        return answers

    def delete_content_index(self, source: str):
        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_multivector_document(source)
        else:
            # DBからsourceを指定して既存ドキュメントを削除
            self.__delete_document(source)
    
    def update_content_index(self, text: str, source: str, source_url: str, description=""):
        
        # 既に存在するドキュメントを削除
        self.delete_content_index(source)
        # ドキュメントを格納する。
        self._add_document_list(text, description, source, source_url)


    def delete_image_index(self, source: str):
        # ★TODO *_content_indexと同じ処理になっているので、共通化する
        self.delete_content_index(source)
            
    def update_image_index(self, text: str, source: str, source_url: str, description:str="", image_url:str = ""):
        # 既に存在するドキュメントを削除
        self.delete_image_index(source)
        # ドキュメントを取得
        self._add_document_list(text, description, source_url, source, content_type="image", image_url=image_url)
        
    def delete_file_index(self, source: str):
        self.delete_content_index(source)

    def update_file_index(self, document_root: str, relative_path: str, source_url: str, description:str=""):

        # ★TODO *_content_indexと同じ処理になっているので、共通化する
        # 既に存在するドキュメントを削除
        self.delete_file_index(relative_path)

        # ファイルの存在チェック
        file_path = os.path.join(document_root, relative_path)
        if not os.path.exists(file_path):
            print("ファイルが存在しません。", file=sys.stderr)
            return

        # ドキュメントを格納
        self._load_file(document_root, relative_path, source_url, description=description)

    def _load_file(self, document_root: str, relative_path: str, source_url: str ,description:str=""):

        # チャンクサイズの取得
        chunk_size = self.vector_db_props.ChunkSize
        # 絶対パスを取得
        absolute_file_path = os.path.join (document_root, relative_path)
        
        # ファイルサイズが0の場合は空のリストを返す
        if os.path.getsize(absolute_file_path) == 0:
            return []
        # テキスト抽出
        text = file_extractor.extract_text_from_file(absolute_file_path)

        # テキストを分割してDocumentのリストを返す
        return self._add_document_list(text, description, relative_path, source_url, chunk_size)


    def _add_document_list(self, content_text: str, description_text: str, source: str, source_url: str, content_type:str="text" , image_url="" ):
        
        # MultiVectorRetrieverの場合はchunk_size=MultiVectorRetrieverのChunkSize
        if self.vector_db_props.IsUseMultiVectorRetriever:
            chunk_size = self.vector_db_props.MultiVectorDocChunkSize
        else:
            chunk_size = self.vector_db_props.ChunkSize
    
        document_list = []
           
        if content_type == "text":
            # テキストの場合は入力テキストを分割してDocumentのリストを返す
            text_list = self._split_text(content_text, chunk_size=chunk_size)
            for text in text_list:
                # text毎にdoc_idを生成
                doc_id = str(uuid.uuid4())
                document = Document(page_content=text, metadata={"source_url": source_url, "source": source, "doc_id": doc_id, "description": description_text, "content_type": content_type})
                document_list.append(document)
    
        elif content_type == "image":
            # 画像の場合はそのままDocumentのリストを返す
            # doc_idを生成
            doc_id = str(uuid.uuid4())
            document = Document(page_content=content_text, metadata={"source_url": source_url, "source": source, "doc_id": doc_id, "description": description_text, "content_type": content_type, "image_url": image_url})
            document_list.append(document)

        else:
            # 例外処理
            raise ValueError("content_type is invalid")

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


def process_content_update_or_datele_request_params(props_json: str, request_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    vector_db_props = openai_props.VectorDBItems[0]
    
    # request_jsonをdictに変換
    request = json.loads(request_json)
    text = request["content"]
    source = request["id"]
    source_url = ""
    
    description = request.get("description", "")

    return openai_props, vector_db_props, text, source, source_url, description        

def process_image_update_or_datele_request_params(props_json: str, request_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    vector_db_props = openai_props.VectorDBItems[0]
    
    # request_jsonをdictに変換
    request = json.loads(request_json)
    text = request["content"]
    source = request["id"]
    source_url = ""
    image_url = request["image_url"]

    description = request.get("description", "")

    return openai_props, vector_db_props, text, source, source_url, description, image_url

def process_file_update_or_datele_request_params(props_json: str, request_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    vector_db_props = openai_props.VectorDBItems[0]
    
    # request_jsonをdictに変換
    request = json.loads(request_json)
    document_root = request["WorkDirectory"]
    relative_path = request["RelativePath"]
    source_url = request["RepositoryURL"]

    description = request.get("description", "")

    return openai_props, vector_db_props, document_root, relative_path, source_url, description

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
        
    
