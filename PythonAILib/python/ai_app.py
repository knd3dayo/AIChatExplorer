import os, json
from PIL import Image
from io import StringIO
import sys
sys.path.append("python")

from openai_props import OpenAIProps, VectorDBProps
from openai_client import OpenAIClient
import langchain_util
import langchain_object_processor

# sys.stdout、sys.stderrが存在しない場合にエラーになるのを回避するために、ダミーのsys.stdout、sys.stderrを設定する
# see: https://github.com/huggingface/transformers/issues/24047
if sys.stdout is None:
    sys.stdout = open(os.devnull, "w", encoding="utf-8")
if sys.stderr is None:
    sys.stderr = open(os.devnull, "w", encoding="utf-8")

# FaissのIndex更新後にretrieveを行うと
# OMP: Error #15: Initializing libomp140.x86_64.dll, but found libiomp5md.dll already initialized.
# が出力されることへの対応。
# see: https://stackoverflow.com/questions/64209238/error-15-initializing-libiomp5md-dll-but-found-libiomp5md-dll-already-initial
os.environ["KMP_DUPLICATE_LIB_OK"]="TRUE"

import openai_client, ai_app_spacy, ai_app_pyocr

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"

# stdout,stderrを文字列として取得するためラッパー関数を定義
def capture_stdout_stderr(func):
    def wrapper(*args, **kwargs):
        # strout,stderrorをStringIOでキャプチャする
        buffer = StringIO()
        sys.stdout = buffer
        sys.stderr = buffer
        result = func(*args, **kwargs)
        # strout,stderrorを元に戻す
        sys.stdout = sys.__stdout__
        sys.stderr = sys.__stderr__
        
        # bufferをリストに変換して返す
        log = []
        for line in buffer.getvalue().splitlines():
            log.append(line)

        return result, log
    return wrapper

def extract_text(filename):
    import file_extractor
    return file_extractor.extract_text(filename)

# spacy関連
def mask_data(textList: list, props = {}):
    return ai_app_spacy.mask_data(textList, props)

def extract_entity(text, props = {}):
    return ai_app_spacy.extract_entity(text, props)

########################
# openai関連
########################
def run_openai_chat(props_json: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> str:
        # OpenAIPorpsを生成
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)
        # OpenAIClientを生成
        openai_client = OpenAIClient(openai_props)
        # request_jsonをdictに変換
        request = json.loads(request_json)
        content = openai_client.run_openai_chat(request)
        return content

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    content, log = wrapper()

    # 結果格納用のdictを生成
    result = {}
    result["content"] = content
    # dict["log"]にログを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json


def openai_embedding(props_json: str, input_text: str):
    # OpenAIPorpsを生成
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    # OpenAIClientを生成
    openai_client = OpenAIClient(openai_props)
    
    return openai_client.openai_embedding(input_text)

def list_openai_models(props_json: str):
    props = json.loads(props_json)
    openai_props = OpenAIProps(props)
    client = OpenAIClient(openai_props)
    return client.list_openai_models()

########################
# langchain関連
########################

def run_vector_search(props_json: str, prompt: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        result = langchain_util.run_vector_search(props_json, prompt)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

def run_langchain_chat( props_json: str, prompt: str, request_json: str):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:

        result = langchain_util.run_langchain_chat(props_json, prompt, request_json)
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

# vector db関連
def update_file_index(props_json, request_json):
    return __update_or_delete_file_index(props_json, request_json, "update")

def delete_file_index(props_json, request_json):
    return __update_or_delete_file_index(props_json, request_json, "delete")


def __update_or_delete_file_index(props_json, request_json, mode):

    # update_indexを実行する関数を定義
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, mode, workdir, relative_path, urlを取得
        openai_props, vector_db_props, document_root, relative_path, url, description = langchain_object_processor.process_file_update_or_datele_request_params(props_json, request_json)
        # LangChainFileProcessorオブジェクトを生成
        processor = langchain_object_processor.LangChainObjectProcessor(openai_props, vector_db_props)

        # modeに応じて処理を分岐
        if mode == "delete":
            # delete_file_indexを実行
            processor.delete_file_index(document_root, relative_path, url)
        if mode == "update":
            # update_file_indexを実行
            processor.update_file_index(document_root, relative_path, url, description=description)

        # 結果用のdictを生成
        result = {}
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log

    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

# ベクトルDBのコンテンツインデックスを削除する
def delete_content_index(props_json, request_json):
    return __update_or_delete_content_index(props_json, request_json, "delete")

# ベクトルDBのコンテンツインデックスを更新する
def update_content_index(props_json, request_json):
    return __update_or_delete_content_index(props_json, request_json, "update")

def __update_or_delete_content_index(props_json, request_json, mode):
    # update_indexを実行する関数を定義
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, sourceを取得
        openai_props, vector_db_props, text, source, source_url, description  = langchain_object_processor.process_content_update_or_datele_request_params(props_json, request_json)
        # LangChainObjectProcessorオブジェクトを生成
        processor = langchain_object_processor.LangChainObjectProcessor(openai_props, vector_db_props)
        
        if mode == "delete":
            # delete_content_indexを実行
            processor.delete_content_index(source)
        if mode == "update":
            # update_content_indexを実行
            processor.update_content_index(text, source, source_url, description=description)
            
        # 結果用のdictを生成
        result = {}
        return result

    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

# ベクトルDBの画像インデックスを削除する
def delete_image_index(props_json, request_json):
    return __update_or_delete_image_index(props_json, request_json, "delete")

# ベクトルDBの画像インデックスを更新する
def update_image_index(props_json, request_json):
    return __update_or_delete_image_index(props_json, request_json, "update")

def __update_or_delete_image_index(props_json, request_json, mode):
    # update_indexを実行する関数を定義
    def func () -> dict:
        # props_json, request_jsonからOpenAIProps, VectorDBProps, text, image_url, sourceを取得
        openai_props, vector_db_props, text, source, image_url, description = langchain_object_processor.process_image_update_or_datele_request_params(props_json, request_json)
        # LangChainObjectProcessorオブジェクトを生成
        processor = langchain_object_processor.LangChainObjectProcessor(openai_props, vector_db_props)
        
        # 初期化
        update_count = 0
        delete_count = 0
        
        if mode == "delete":
            # delete_image_indexを実行
             processor.delete_image_index(source)
        if mode == "update":
            # update_image_indexを実行
            processor.update_image_index(text, image_url, source, description=description)
            
        # 結果用のdictを生成
        result = {}
        return result
    
    # strout,stderrをキャプチャするラッパー関数を生成
    wrapper = capture_stdout_stderr(func)
    # ラッパー関数を実行
    result, log = wrapper()
    
    # resultにlogを追加
    result["log"] = log
    
    # resultをJSONに変換して返す
    result_json = json.dumps(result, ensure_ascii=False, indent=4)
    return result_json

# pyocr関連
def extract_text_from_image(byte_data,tessercat_exe_path) -> dict:
    # strout,stderrorをStringIOでキャプチャする
    buffer = StringIO()
    sys.stdout = buffer
    sys.stderr = buffer
    
    result: str =  ai_app_pyocr.extract_text_from_image(byte_data, tessercat_exe_path)
    # strout,stderrorを元に戻す
    sys.stdout = sys.__stdout__
    sys.stderr = sys.__stderr__
    
    result_dict = {"text": result, "log": buffer.getvalue()}
    return result_dict

# run_script関数
def run_script(script, input_str):
    exec(script, globals())
    result = execute(input_str)
    return result
    
# テスト用
def hello_world():
    return "Hello World"
