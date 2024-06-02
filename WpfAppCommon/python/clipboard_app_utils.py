import os, json
from PIL import Image
from io import StringIO
import sys
sys.path.append("python")

from openai_props import OpenAIProps, VectorDBProps
from openai_client import OpenAIClient
import langchain_util

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

import clipboard_app_sqlite, openai_client, clipboard_app_spacy, clipboard_app_pyocr

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
        return result, buffer.getvalue()
    return wrapper

def extract_text(filename):
    import clipboard_app_extractor
    return clipboard_app_extractor.extract_text(filename)

# spacy関連
def mask_data(textList: list, props = {}):
    return clipboard_app_spacy.mask_data(textList, props)
def extract_entity(text, props = {}):
    return clipboard_app_spacy.extract_entity(text, props)

########################
# openai関連
########################
def openai_chat(props_json: str, input_json: str, json_mode:bool = False) -> str:
    # OpenAIチャットを実行する関数を定義
    def func() -> str:
        # OpenAIPorpsを生成
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)
        # OpenAIClientを生成
        openai_client = OpenAIClient(openai_props)
        content = openai_client.openai_chat(input_json, json_mode)
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

def openai_chat_with_vision(props_json: str, prompt: str, image_file_name_list:list):
    # OpenAIチャットを実行する関数を定義
    def func() -> str:
        # OpenAIPorpsを生成
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)
        # OpenAIClientを生成
        openai_client = OpenAIClient(openai_props)
        content = openai_client.openai_chat_with_vision(prompt, image_file_name_list)
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
def langchain_chat( props_json: str, vector_db_items_json: str, prompt: str, chat_history_json: str = None):
    # OpenAIチャットを実行する関数を定義
    def func() -> dict:
        # OpenAIPorpsを生成
        props = json.loads(props_json)
        openai_props = OpenAIProps(props)

        # vector_db_items_jsonをVectorDBPropsに変換
        vector_db_items_list = json.loads(vector_db_items_json)
        vector_db_props = [VectorDBProps(vector_db_item) for vector_db_item in vector_db_items_list]    

        result = langchain_util.langchain_chat(openai_props, vector_db_props, prompt, chat_history_json)
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
def update_index(props, mode, workdir, relative_path, url):
    # strout,stderrorをStringIOでキャプチャする
    buffer = StringIO()
    sys.stdout = buffer
    sys.stderr = buffer
    openai_props = OpenAIProps(props)
    vector_db_props = VectorDBProps(props)
    
    import file_processor
    result = file_processor.update_index(openai_props, vector_db_props, mode, workdir, relative_path,  url)

    return result

def update_index_with_clipboard_item(props, mode, text, object_id_string):
    # strout,stderrorをStringIOでキャプチャする
    buffer = StringIO()
    sys.stdout = buffer
    sys.stderr = buffer
    openai_props = OpenAIProps(props)
    vector_db_props = VectorDBProps(props)
    
    import clipboard_item_processor
    result = clipboard_item_processor.update_index(openai_props, vector_db_props, mode, text, object_id_string)


# pyocr関連
def extract_text_from_image(byte_data,tessercat_exe_path) -> dict:
    # strout,stderrorをStringIOでキャプチャする
    buffer = StringIO()
    sys.stdout = buffer
    sys.stderr = buffer
    
    result: str =  clipboard_app_pyocr.extract_text_from_image(byte_data, tessercat_exe_path)
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
