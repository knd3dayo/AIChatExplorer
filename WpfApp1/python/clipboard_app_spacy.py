import sys
sys.path.append("python")
import clipboard_app_sqlite

nlp = None

def extract_text(filename):
    from unstructured.partition.auto import partition

    # filenameのファイルからテキストを抽出する
    elements = partition(filename=filename)
    return "\n".join([element.text for element in elements])

def mask_data(textList: list, props = {}):
    raise Exception("Not implemented")
    global nlp
    if (nlp is None):
        import spacy
        model_name = props.get("SpacyModel", None)
        if model_name is None:
            return {}
        else:
            nlp = spacy.load(model_name)
    
    # text、PERSON、ORG、GPEを格納するdictを作成
    result_dict = {}
    
    # PERSONの変換前後のdict
    result_persion_dict = {}
    # ORGの変換前後のdict
    result_org_dict = {}
    # ent.label_がPERSONのent.textと 連番を保持する
    person_dict = {}
    # ent.label_がORGのent.textと 連番を保持する
    org_dict = {}

    result_text_list = []
    # tableが存在しない場合、作成する
    clipboard_app_sqlite.create_masked_data_table()
    
    # textの中からPERSON, ORGを抽出し、それぞれを[MASKED {label} {連番}]に置き換える
    for beforeText in textList:
        afterText = beforeText
        doc = nlp(beforeText)

        for ent in doc.ents:
            if ent.label_ in ["PERSON", "ORG"]:
                # PERSONの場合
                if ent.label_ == "PERSON":
                    target_dict = result_persion_dict
                # ORGの場合
                elif ent.label_ == "ORG":
                    target_dict = result_org_dict    
                # masked_data_tableにBEFOREがent.textのデータがあるか確認する
                masked_data_id = clipboard_app_sqlite.select_masked_data_table(ent.text)
                if masked_data_id is None:
                    # text_intテーブルのPERSONのlast_numを取得する
                    num = clipboard_app_sqlite.select_text_int_table(ent.label_)
                    temp_text = f"[MASKED {ent.label_} {num + 1}]"
                    afterText = afterText.replace(ent.text, temp_text)
                    # masked_data_tableにBEFOREがent.textのデータを追加する
                    clipboard_app_sqlite.insert_masked_data_table(ent.text, temp_text)
                    # text_intテーブルのPERSONのlast_numを+1する
                    clipboard_app_sqlite.update_text_int_table(ent.label_)
                    
                    # 結果を格納
                    target_dict[ent.text] = temp_text
                else:
                    afterText = afterText.replace(ent.text, masked_data_id)
                    # 結果を格納
                    target_dict[ent.text] = masked_data_id
                    
        result_text_list.append({"BEFORE": beforeText, "AFTER": afterText})

    # それぞれのdictをresut_dictに格納する
    result_dict["PERSON"] = result_persion_dict
    result_dict["ORG"] = result_org_dict
    result_dict["TEXT"] = result_text_list

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
