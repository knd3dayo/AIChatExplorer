import sys, json, os
sys.path.append('python')
from langchain.docstore.document import Document
from langchain_client import LangChainOpenAIClient
import langchain_util
import uuid
from langchain_file_loader import FileLoader
import file_extractor

from openai_props import OpenAIProps, VectorDBProps

class LangChainObjectProcessor:
    def __init__(self, openai_props: OpenAIProps, vector_db_props: VectorDBProps):
        
        langchain_client = LangChainOpenAIClient(openai_props)
        self.vector_db_props = vector_db_props
        self.vector_db = langchain_util.get_vector_db(langchain_client, vector_db_props)

    def delete_content_index(self, source: str):
        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            # DBからsourceを指定して既存ドキュメントを削除
            self.vector_db.delete_multivector_document(source)
        else:
            # DBからsourceを指定して既存ドキュメントを削除
            self.vector_db.delete_document(source)
    
    def update_content_index(self, text: str, source: str, source_url: str):
        # MultiVectorRetrieverの場合
        # 既に存在するドキュメントを削除
        self.delete_content_index(source)
        # ドキュメントを取得
        documents = self._get_document_list(text, source, source_url)

        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            for document in documents:
                self.vector_db.add_multivector_document(document)
        else:
            for document in documents:
                self.vector_db.add_document(document)

    def delete_image_index(self, source: str):
        # ★TODO *_content_indexと同じ処理になっているので、共通化する
        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            # DBからsourceを指定して既存ドキュメントを削除
            self.vector_db.delete_multivector_document(source)
        else:
            # DBからsourceを指定して既存ドキュメントを削除
            self.vector_db.delete_document(source)
            
    def update_image_index(self, image_url: str, source: str):
        # ★TODO *_content_indexと同じ処理になっているので、共通化する
        # 既に存在するドキュメントを削除
        self.delete_content_index(source)
        # ドキュメントを取得
        documents = self._get_document_list(image_url, source)
        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            for document in documents:
                self.vector_db.add_multivector_document(document)
        else:
            for document in documents:
                self.vector_db.add_document(document)

    def delete_file_index(self, source: str):
        # ★TODO *_content_indexと同じ処理になっているので、共通化する
        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            # DBからsourceを指定して既存ドキュメントを削除
            self.vector_db.delete_multivector_document(source)
        else:
            # DBからsourceを指定して既存ドキュメントを削除
            self.vector_db.delete_document(source)

    def update_file_index(self, document_root: str, relative_path: str, source_url: str):
        # ★TODO *_content_indexと同じ処理になっているので、共通化する
        # 既に存在するドキュメントを削除
        self.delete_file_index(relative_path)

        # ファイルの存在チェック
        file_path = os.path.join(document_root, relative_path)
        if not os.path.exists(file_path):
            print("ファイルが存在しません。", file=sys.stderr)
            return

        # ドキュメントを取得
        documents = self._load_file(document_root, relative_path, source_url)

        # MultiVectorRetrieverの場合
        if self.vector_db_props.IsUseMultiVectorRetriever:
            for document in documents:
                self.vector_db.add_multivector_document(document)
        else:
            for document in documents:
                self.vector_db.add_document(document)

    def _load_file(self, document_root: str, relative_path: str, source_url: str ):

        # 絶対パスを取得
        absolute_file_path = os.path.join (document_root, relative_path)
        
        # ファイルサイズが0の場合は空のリストを返す
        if os.path.getsize(absolute_file_path) == 0:
            return []
        # テキスト抽出
        text = file_extractor.extract_text(absolute_file_path)

        # テキストを分割してDocumentのリストを返す
        return self._get_document_list(text, relative_path, source_url)

    def _get_document_list(self, text: str, source: str, source_url: str):
        # 入力テキストを分割してDocumentのリストを返す
        text_list = self._split_text(text)
        document_list = []
        for text in text_list:
            # doc_idを生成
            doc_id = str(uuid.uuid4())
            document = Document(page_content=text, metadata={"source_url": source_url, "source": source, "doc_id": doc_id})
            document_list.append(document)
        
        return document_list

    def _split_text(self, text: str, chunk_size: int=500):
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
    text = request["Content"]
    source = request["Id"]

    return openai_props, vector_db_props, text, source        

def process_image_update_or_datele_request_params(props_json: str, request_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    vector_db_props = openai_props.VectorDBItems[0]
    
    # request_jsonをdictに変換
    request = json.loads(request_json)
    image_url = request["image_url"]
    source = request["Id"]

    return openai_props, vector_db_props, image_url, source

def process_file_update_or_datele_request_params(props_json: str, request_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    vector_db_props = openai_props.VectorDBItems[0]
    
    # request_jsonをdictに変換
    request = json.loads(request_json)
    document_root = request["WorkDirectory"]
    relative_path = request["RelativePath"]
    source_url = request["RepositoryURL"]

    return openai_props, vector_db_props, document_root, relative_path, source_url