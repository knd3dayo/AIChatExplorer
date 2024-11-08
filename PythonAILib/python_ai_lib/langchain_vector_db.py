
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

class VectorSearchParameter:
    def __init__(self, openai_props: OpenAIProps = None, vector_db_props: list[VectorDBProps] = None, query: str = "", search_kwarg: dict = {}):

        # OpenAIPorpsを生成
        self.openai_props = openai_props

        # VectorDBPropsのリストを取得
        self.vector_db_props = vector_db_props

        #  openai_props, vector_db_items, query, search_kwargを設定する
        self.query = query
        self.search_kwarg = search_kwarg

    @classmethod
    def from_json(cls, openai_props_json: str = "{}", vector_db_items_json: str = "{}", request_json: str = "{}"):
        params:VectorSearchParameter = VectorSearchParameter()
        # OpenAIPorpsを生成
        props = json.loads(openai_props_json)
        params.openai_props = OpenAIProps(props)

        # VectorDBPropsのリストを取得
        vector_db_items = json.loads(vector_db_items_json)
        params.vector_db_props = [VectorDBProps(item) for item in vector_db_items]

        #  openai_props, vector_db_items, query, search_kwargを設定する
        request: dict = json.loads(request_json)
        params.query = request.get("query", "")
        params.search_kwarg = request.get("search_kwarg", {})

        return params

class ContentUpdateOrDeleteRequestParams:
    def __init__(self, openai_props: OpenAIProps = None, vector_db_props_list: list[VectorDBProps] =[], 
                 text: str = "", source: str = "", source_url: str = "", description: str = "", reliability: int = 0, mode: str = ""):
        self.openai_props = openai_props

        self.vector_db_props_list = vector_db_props_list

        self.text = text
        self.source = source
        self.source_url = source_url
        self.description = description
        self.reliability = reliability
        self.mode = mode

    @classmethod
    def from_json(cls, props_json: str = "{}", vector_db_items_json: str = "{}", request_json: str = "{}"):
        params:ContentUpdateOrDeleteRequestParams = ContentUpdateOrDeleteRequestParams()
        props = json.loads(props_json)
        params.openai_props = OpenAIProps(props)

        vector_db_items = json.loads(vector_db_items_json)
        params.vector_db_props_list = []
        for vector_db_item in vector_db_items:
            params.vector_db_props_list.append(VectorDBProps(vector_db_item))
        
        # request_jsonをdictに変換
        request: dict = json.loads(request_json)
        params.text = request["content"]
        params.source = request["id"]
        params.source_url = ""
        params.description = request.get("description", "")
        params.reliability = request.get("reliability", 0)
        params.mode = request.get("mode", "")

        return params
        
class ImageUpdateOrDeleteRequestParams:
    def __init__(self, openai_props: OpenAIProps = None, vector_db_props_list: list[VectorDBProps] = [], 
                    text: str = "", source: str = "", source_url: str = "", image_url: str = "", description: str = "", reliability: int = 0, mode: str = ""):

        self.openai_props = openai_props

        self.vector_db_props_list = vector_db_props_list
        
        # request_jsonをdictに変換
        self.text = text
        self.source = source
        self.source_url = source_url
        self.image_url = image_url
        self.description = description
        self.reliability = reliability
        self.mode = mode

    @classmethod
    def from_json(cls, props_json: str = "{}", vector_db_items_json: str = "{}", request_json: str = "{}"):
        params:ImageUpdateOrDeleteRequestParams = ImageUpdateOrDeleteRequestParams()
        props = json.loads(props_json)
        params.openai_props = OpenAIProps(props)

        vector_db_items = json.loads(vector_db_items_json)
        params.vector_db_props_list = []
        for vector_db_item in vector_db_items:
            params.vector_db_props_list.append(VectorDBProps(vector_db_item))
        
        # request_jsonをdictに変換
        request: dict= json.loads(request_json)
        params.text = request["content"]
        params.source = request["id"]
        params.source_url = ""
        params.image_url = request["image_url"]
        params.description = request.get("description", "")
        params.reliability = request.get("reliability", 0)
        params.mode = request.get("mode", "")

        return params

class FileUpdateOrDeleteRequestParams:
    def __init__(self, openai_props: OpenAIProps = None, vector_db_props_list: list[VectorDBProps] = [], 
                    document_root: str = "", relative_path: str = "", source_url: str = "", description: str = "", reliability: int = 0, mode: str = ""):
        
        self.openai_props = openai_props

        self.vector_db_props_list = vector_db_props_list
        
        # request_jsonをdictに変換
        self.document_root = document_root
        self.relative_path = relative_path
        self.source_url = source_url
        self.description = description
        self.reliability = reliability
        self.mode = mode
    
    @classmethod
    def from_json(cls, props_json: str = "{}", vector_db_items_json: str = "{}", request_json: str = "{}"):
        params: FileUpdateOrDeleteRequestParams = FileUpdateOrDeleteRequestParams()

        props = json.loads(props_json)
        params.openai_props = OpenAIProps(props)

        vector_db_items = json.loads(vector_db_items_json)
        params.vector_db_props_list = []
        for vector_db_item in vector_db_items:
            props.vector_db_props_list.append(VectorDBProps(vector_db_item))
        
        # request_jsonをdictに変換
        request: dict = json.loads(request_json)
        params.document_root = request["WorkDirectory"]
        params.relative_path = request["RelativePath"]
        params.source_url = request["RepositoryURL"]
        params.description = request.get("description", "")
        params.reliability = request.get("reliability", 0)
        params.mode = request.get("mode", "")

        return params

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

    def _delete_collection(self):
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
        self.delete_file_index(params)

        # ファイルの存在チェック
        file_path = os.path.join(params.document_root, params.relative_path)
        if not os.path.exists(file_path):
            print("ファイルが存在しません。", file=sys.stderr)
            return

        # ドキュメントを格納
        self._add_file(params.document_root, params.relative_path, params.source_url, description=params.description, reliability=params.reliability)

    def _add_file(self, document_root: str, relative_path: str, source_url: str ,description:str="", reliability=0):

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
        return self._add_document_list(text, description, relative_path, source_url, chunk_size, reliability=reliability)


    def _add_document_list(self, content_text: str, description_text: str, source: str, source_url: str, content_type:str="text" , image_url="", reliability=0):
        
        # MultiVectorRetrieverの場合はchunk_size=MultiVectorRetrieverのChunkSize
        if self.vector_db_props.IsUseMultiVectorRetriever:
            chunk_size = self.vector_db_props.MultiVectorDocChunkSize
        else:
            chunk_size = self.vector_db_props.ChunkSize
    
        document_list = []
        print("content_type:", content_type)

        if not content_type or content_type == "text":
            # テキストの場合は入力テキストを分割してDocumentのリストを返す
            text_list = self._split_text(content_text, chunk_size=chunk_size)
            for text in text_list:
                # text毎にdoc_idを生成
                doc_id = str(uuid.uuid4())
                document = Document(
                    page_content=text, metadata={"source_url": source_url, "source": source, 
                                                 "doc_id": doc_id, "description": description_text, "content_type": content_type, 
                                                 "image_url": image_url, "reliability": reliability})
                document_list.append(document)
    
        elif content_type == "image":
            # 画像の場合はそのままDocumentのリストを返す
            # doc_idを生成
            doc_id = str(uuid.uuid4())
            document = Document(
                page_content=content_text, metadata={"source_url": source_url, "source": source, 
                                                     "doc_id": doc_id, "description": description_text, "content_type": content_type, 
                                                     "image_url": image_url, "reliability": reliability})
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
        
    
