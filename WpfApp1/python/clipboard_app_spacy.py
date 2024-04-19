import sys
from unittest import result
sys.path.append("python")
import clipboard_app_sqlite

nlp = None

def extract_text(filename):
    from unstructured.partition.auto import partition

    # filenameのファイルからテキストを抽出する
    elements = partition(filename=filename)
    return "\n".join([element.text for element in elements])

def mask_data(textList: list, props = {}):
    global nlp
    if (nlp is None):
        import spacy
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
    clipboard_app_sqlite.create_masked_data_table()
    
    # textの中からPERSON, ORGを抽出し、それぞれを[MASKED {label} {連番}]に置き換える
    for beforeText in textList:
        afterText = beforeText
        doc = nlp(beforeText)

        for ent in doc.ents:
            if ent.label_ in ['ORG', 'PERSON', 'PRODUCT', 'WORK_OF_ART', 'EVENT']:
                # masked_data_tableにBEFOREがent.textのデータがあるか確認する
                masked_data_string = clipboard_app_sqlite.select_masked_data_table(ent.text)
                if masked_data_string is None:
                    # text_intテーブルのPERSONのlast_numを取得する
                    num = clipboard_app_sqlite.select_text_int_table(ent.label_)
                    masked_text = f"[MASKED {ent.label_} {num + 1}]"
                    afterText = afterText.replace(ent.text, masked_text)

                    # masked_data_tableにBEFOREがent.textのデータを追加する
                    clipboard_app_sqlite.insert_masked_data_table(ent.text, masked_text)
                    # text_intテーブルのPERSONのlast_numを+1する
                    clipboard_app_sqlite.update_text_int_table(ent.label_)
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
        import spacy
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
