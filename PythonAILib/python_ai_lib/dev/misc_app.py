import sys,io,os
sys.path.append("python")
from PIL import Image
import pyocr #type: ignore
from unittest import result
import sqlite3
from io import StringIO

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

# pyocr関連
def extract_text_from_image_impl(byte_data, tessercat_exe_path):
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


nlp = None

dbname = 'clipboard_pyhton.db'


def create_masked_data_table():

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    cursor.execute("CREATE TABLE IF NOT EXISTS MASKED_TEXT (id INTEGER PRIMARY KEY AUTOINCREMENT, BEFORE TEXT, AFTER, TEXT)")
    # ENT_LABELとLAST_NUMの値を保存するテーブルの作成
    cursor.execute("CREATE TABLE IF NOT EXISTS TEXT_INT (id INTEGER PRIMARY KEY AUTOINCREMENT, ENT_LABEL TEXT, LAST_NUM INTEGER)")
    conn.commit()

    # データベースへのコネクションを閉じる。(必須)
    conn.close()

def select_masked_data_table(rowText: str):

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    # BEFOREがrowTextのデータを取得する
    cursor.execute("SELECT AFTER FROM MASKED_TEXT WHERE BEFORE = ?", (rowText,))
    
    rows = cursor.fetchall()
    conn.close()
    if len(rows) == 0:
        return None
    else:
        return rows[0][0]

def insert_masked_data_table(beforeText: str, afterText: str):

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    cursor.execute("INSERT INTO MASKED_TEXT (BEFORE, AFTER) VALUES (?, ?)", (beforeText, afterText))
    
    conn.commit()
    conn.close()

# テーブルTEXT_INTから指定されたENT＿LABELのINTを取得する
def select_text_int_table(ent_label: str):

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    # ENT_LABELがent_labelのデータを取得する
    cursor.execute("SELECT LAST_NUM FROM TEXT_INT WHERE ENT_LABEL = ?", (ent_label,))
    rows = cursor.fetchall()
    conn.close()
    if len(rows) == 0:
        return 0
    else:
        # ENT_LABELを返す
        return rows[0][0]
    
# テーブルTEXT_INTに指定されたENT＿LABELのlast_numを＋１する
def update_text_int_table(ent_label: str):

    conn = sqlite3.connect(dbname)

    cursor = conn.cursor()
    # ENT_LABELがent_labelのデータを取得する
    last_num = select_text_int_table(ent_label)
    if last_num == 0:
        cursor.execute("INSERT INTO TEXT_INT (ENT_LABEL, LAST_NUM) VALUES (?, 1)", (ent_label,))
    else:
        cursor.execute("UPDATE TEXT_INT SET LAST_NUM = ? WHERE ENT_LABEL = ?", (last_num + 1, ent_label))
    
    conn.commit()
    conn.close()




def mask_data(textList: list, props = {}):
    global nlp
    if (nlp is None):
        import spacy #type: ignore
        model_name = props.get("SpacyModel", None)
        if model_name is None:
            raise Exception("SpacyModel is not set.")
            return {}
        else:
            nlp = spacy.load(model_name)
    
    # TEXTのbeforeのbefore,afterを格納するdictとentityラベル毎のdictを格納するdictを作成
    # result_dict[TEXT] = result_text_list[] # BEFOREとAFTERを格納したdictのリスト
    # result_dict{<EntityLabel>} = {<EntityLabel>, result_entity_dict{}} # EntityLabelとBEFOREとAFTERを格納するdict
            
    result_dict = {}
    # キーがTEXTのdictを作成
    result_dict["TEXT"] = {"BEFORE": textList, "AFTER": []}

    # tableが存在しない場合、作成する
    create_masked_data_table()
    
    # textの中からPERSON, ORGを抽出し、それぞれを[MASKED {label} {連番}]に置き換える
    for beforeText in textList:
        afterText = beforeText
        doc = nlp(beforeText)

        for ent in doc.ents:
            if ent.label_ in ['ORG', 'PERSON', 'PRODUCT', 'WORK_OF_ART', 'EVENT']:
                # masked_data_tableにBEFOREがent.textのデータがあるか確認する
                masked_data_string = select_masked_data_table(ent.text)
                if masked_data_string is None:
                    # text_intテーブルのPERSONのlast_numを取得する
                    num = select_text_int_table(ent.label_)
                    masked_text = f"[MASKED {ent.label_} {num + 1}]"
                    afterText = afterText.replace(ent.text, masked_text)

                    # masked_data_tableにBEFOREがent.textのデータを追加する
                    insert_masked_data_table(ent.text, masked_text)
                    # text_intテーブルのPERSONのlast_numを+1する
                    update_text_int_table(ent.label_)
                    # 結果を格納
                    result_dict.get(ent.label_,{})[ent.text] = masked_text
                else:
                    masked_text = afterText.replace(ent.text, masked_data_string)
                    # 結果を格納
                    result_dict.get(ent.label_,{})[ent.text] = masked_text
                    
        result_dict["TEXT"]["AFTER"].append(afterText)

    # それぞれのdictをresut_dictに格納する

    return result_dict

def extract_entity(text, props = {}):
    # entityを格納するset
    result_set = set()

    global nlp
    if (nlp is None):
        import spacy #type: ignore
        model_name = props.get("SpacyModel", None)
        if model_name is None:
            return result_set
        else:
            nlp = spacy.load(model_name)
    
    doc = nlp(text)

    for ent in doc.ents:
        # ent.label_が
        # ['ORG', 'CARDINAL', 'DATE', 'GPE', 'PERSON', 'MONEY', 'PRODUCT', 'TIME', 'PERCENT', 'WORK_OF_ART', 'QUANTITY', 'NORP', 'LOC', 'EVENT', 'ORDINAL', 'FAC', 'LAW', 'LANGUAGE']
        # のうち
        # ['ORG', 'DATE', 'GPE', 'PERSON', 'PRODUCT', 'TIME', 'WORK_OF_ART', 'NORP', 'LOC', 'EVENT', 'FAC', 'LAW', 'LANGUAGE']
        # のent.textをresult_setに格納する
        if ent.label_ in ['ORG', 'DATE', 'GPE', 'PERSON', 'PRODUCT', 'TIME', 'WORK_OF_ART', 'NORP', 'LOC', 'EVENT', 'FAC', 'LAW', 'LANGUAGE']:
            result_set.add(ent.text)

    return result_set

# 外部公開用の関数
# spacy関連
def mask_data(textList: list, props = {}):
    return mask_data(textList, props)

def extract_entity(text, props = {}):
    return extract_entity(text, props)

# run_script関数
def run_script(script, input_str):
    exec(script, globals())
    result = execute(input_str)
    return result

# pyocr関連
def extract_text_from_image(byte_data,tessercat_exe_path) -> dict:
    # strout,stderrorをStringIOでキャプチャする
    buffer = StringIO()
    sys.stdout = buffer
    sys.stderr = buffer
    
    result: str =  extract_text_from_image_impl(byte_data, tessercat_exe_path)
    # strout,stderrorを元に戻す
    sys.stdout = sys.__stdout__
    sys.stderr = sys.__stderr__
    
    result_dict = {"text": result, "log": buffer.getvalue()}
    return result_dict

