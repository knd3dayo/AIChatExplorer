import sys
sys.path.append("python")
from PIL import Image
import pyocr
import io

# pyocr関連
def extract_text_from_image(byte_data, tessercat_exe_path):
    # OCRエンジンを取得
    # pyocr.tesseract.TESSERACT_CMD = r'C:\\Program Files\\Tesseract-OCR\\tesseract.exe'
    pyocr.tesseract.TESSERACT_CMD = tessercat_exe_path
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

