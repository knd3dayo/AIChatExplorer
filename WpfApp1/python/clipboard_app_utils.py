import traceback
import os, json
import sys
sys.path.append("python")
import clipboard_app_sqlite, clipboard_app_openai, clipboard_app_faiss, clipboard_app_spacy


os.environ["NO_PROXY"] = "*"

clipboard_app_faiss.load_faiss_index()

# spacy関連
def extract_text(filename):
    return clipboard_app_spacy.extract_text(filename)
def mask_data(textList: list, props = {}):
    return clipboard_app_spacy.mask_data(textList, props)
def extract_entity(text, props = {}):
    return clipboard_app_spacy.extract_entity(text, props)

def openai_json_chat(input_json, props={}):
    return clipboard_app_openai.openai_chat(input_json, props)
def openai_chat(input_text, props={}):
    return clipboard_app_openai.openai_chat(input_text, props)
def openai_embedding(input_text, props={}):
    return clipboard_app_openai.openai_embedding(input_text, props)
def save_faiss_index():
    return clipboard_app_faiss.save_faiss_index()
def load_faiss_index():
    return clipboard_app_faiss.load_faiss_index()