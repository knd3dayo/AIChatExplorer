import traceback
import os, json
from PIL import Image
from io import StringIO
import sys


sys.path.append("python")
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

import clipboard_app_sqlite, clipboard_app_openai, clipboard_app_spacy, clipboard_app_pyocr

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"


def extract_text(filename):
    import clipboard_app_extractor
    return clipboard_app_extractor.extract_text(filename)

# spacy関連
def mask_data(textList: list, props = {}):
    return clipboard_app_spacy.mask_data(textList, props)
def extract_entity(text, props = {}):
    return clipboard_app_spacy.extract_entity(text, props)

# openai関連
def openai_chat(props: dict, input_json: str, json_mode:bool = False):
    return clipboard_app_openai.openai_chat(props, input_json, json_mode)

def openai_embedding(props: dict, input_text: str):
    return clipboard_app_openai.openai_embedding(props, input_text)


def langchain_chat( props: dict, vector_db_items_json: str, prompt: str, chat_history_json: str = None):
    # strout,stderrorをStringIOでキャプチャする
    buffer = StringIO()
    sys.stdout = buffer
    sys.stderr = buffer
    
    import langchain_util
    result = langchain_util.langchain_chat(props, vector_db_items_json, prompt, chat_history_json)
    # dict["log"]にログを追加
    result["log"] = buffer.getvalue()
    # strout,stderrorを元に戻す
    sys.stdout = sys.__stdout__
    sys.stderr = sys.__stderr__
    
    return result

def list_openai_models():
    return clipboard_app_openai.list_openai_models()

def openai_chat_with_vision(props: dict, prompt: str, image_file_name_list:list):
    return clipboard_app_openai.openai_chat_with_vision(props, prompt, image_file_name_list)

# vector db関連
def update_index(props, mode, workdir, relative_path, url):
    # strout,stderrorをStringIOでキャプチャする
    buffer = StringIO()
    sys.stdout = buffer
    sys.stderr = buffer

    import file_processor
    result = file_processor.update_index(props, mode, workdir, relative_path,  url)

    return result

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
