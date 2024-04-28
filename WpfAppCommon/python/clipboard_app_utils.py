import traceback
import os, json
from PIL import Image
import pyocr
import io

import sys
sys.path.append("python")
import clipboard_app_sqlite, clipboard_app_openai, clipboard_app_faiss, clipboard_app_spacy, clipboard_app_pyocr

# Proxy環境下でのSSLエラー対策。HTTPS_PROXYが設定されていない場合はNO_PROXYを設定する
if "HTTPS_PROXY" not in os.environ:
    os.environ["NO_PROXY"] = "*"

# spacy関連
def extract_text(filename):
    return clipboard_app_spacy.extract_text(filename)
def mask_data(textList: list, props = {}):
    return clipboard_app_spacy.mask_data(textList, props)
def extract_entity(text, props = {}):
    return clipboard_app_spacy.extract_entity(text, props)

# openai関連
def openai_json_chat(input_json, props={}):
    return clipboard_app_openai.openai_chat(input_json, props)
def openai_chat(input_text, props={}):
    return clipboard_app_openai.openai_chat(input_text, props)
def openai_embedding(input_text, props={}):
    return clipboard_app_openai.openai_embedding(input_text, props)

def list_openai_models():
    return clipboard_app_openai.list_openai_models()

# faiss関連
def save_faiss_index():
    return clipboard_app_faiss.save_faiss_index()
def load_faiss_index():
    return clipboard_app_faiss.load_faiss_index()

# pyocr関連
def extract_text_from_image(byte_data,tessercat_exe_path):
    return clipboard_app_pyocr.extract_text_from_image(byte_data, tessercat_exe_path)


# run_script関数
def run_script(script, input_str):
    exec(script, globals())
    result = execute(input_str)
    return result
    
# テスト用
def hello_world():
    return "Hello World"
