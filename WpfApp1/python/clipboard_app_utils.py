import traceback
import os, json
from PIL import Image
import pyocr
import io

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

# openai関連
def openai_json_chat(input_json, props={}):
    return clipboard_app_openai.openai_chat(input_json, props)
def openai_chat(input_text, props={}):
    return clipboard_app_openai.openai_chat(input_text, props)
def openai_embedding(input_text, props={}):
    return clipboard_app_openai.openai_embedding(input_text, props)

# faiss関連
def save_faiss_index():
    return clipboard_app_faiss.save_faiss_index()
def load_faiss_index():
    return clipboard_app_faiss.load_faiss_index()

# pyocr関連
def extract_text_from_image(byte_data):
    # OCRエンジンを取得
    pyocr.tesseract.TESSERACT_CMD = r'C:\\Program Files\\Tesseract-OCR\\tesseract.exe'
    engines = pyocr.get_available_tools()
    
    tool = engines[0]

    # langs = tool.get_available_languages()
    # print("Available languages: %s" % ", ".join(langs))


    txt = tool.image_to_string(
        Image.open(io.BytesIO(byte_data)),
        lang="jpn",
        # builder=pyocr.builders.TextBuilder(tesseract_layout=6)
        )
    return txt


