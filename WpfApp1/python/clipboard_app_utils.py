

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
    # textの中からPERSON, ORGを抽出し、それぞれを[MASKED {label} {連番}]に置き換える
    for beforeText in textList:
        afterText = beforeText
        doc = nlp(beforeText)

        for ent in doc.ents:
            if ent.label_ == "PERSON":
                num = person_dict.get(ent.text, None)
                if num is None:
                    num = len(person_dict) + 1
                    person_dict[ent.text] = num

                afterText = afterText.replace(ent.text, f"[MASKED {ent.label_} {num}]")
                # 結果を格納
                result_persion_dict[ent.text] = f"[MASKED {ent.label_} {num}]"

            elif ent.label_ == "ORG":
                num = org_dict.get(ent.text, None)
                if num is None:
                    num = len(org_dict) + 1
                    org_dict[ent.text] = num

                afterText = afterText.replace(ent.text, f"[MASKED {ent.label_} {num}]")
                # 結果を格納
                result_org_dict[ent.text] = f"[MASKED {ent.label_} {num}]"

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


def openai_json_chat(input_json, props={}):
    # propsからopenaiの設定を取得する
    openai_api_key = props.get("OpenAIKey", None)
    azure_openai_endpoint = props.get("AzureOpenAIEndpoint", None)
    chat_model_name = props.get("OpenAICompletionModel", None)
    azure_openai_string = props.get("AzureOpenAI", False)
    # azure_openaiがTrueの場合、AzureOpenAIを使用する.azure_openai_stringを大文字にしてTRUEの場合はTrueに変換する
    azure_openai = azure_openai_string.upper() == "TRUE"
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil(azure_openai, openai_api_key, azure_openai_endpoint)
    json_obj = json.loads(input_json)
    client = openai_util.client
    response = client.chat.completions.create(
        model=chat_model_name,
        messages=json_obj,
        response_format={"type": "json_object"}    
        )
    return response.choices[0].message.content
    

def openai_chat(input_json, props ={}):
    # propsからopenaiの設定を取得する
    openai_api_key = props.get("OpenAIKey", None)
    azure_openai_endpoint = props.get("AzureOpenAIEndpoint", None)
    chat_model_name = props.get("OpenAICompletionModel", None)
    azure_openai_string = props.get("AzureOpenAI", False)
    # azure_openaiがTrueの場合、AzureOpenAIを使用する.azure_openai_stringを大文字にしてTRUEの場合はTrueに変換する
    azure_openai = azure_openai_string.upper() == "TRUE"
    # base_urlを取得する
    base_url = props.get("OpenAIBaseURL", None)
    
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil(azure_openai, openai_api_key, azure_openai_endpoint, base_url=base_url)
    json_obj = json.loads(input_json)
    client = openai_util.client
    response = client.chat.completions.create(
        model=chat_model_name,
        messages=json_obj
    )
    return response.choices[0].message.content

import importlib
import json
class OpenAIUtil:
    def __init__(self, azure_openai, openai_api_key, azure_openai_endpoint=None, base_url=None):
        self.azure_openai = azure_openai
        self.openai_api_key = openai_api_key
        self.azure_openai_endpoint = azure_openai_endpoint

        self.client = self._create_openai_object()
        if base_url is not None:
            self.client.base_url = base_url

    def _create_openai_object(self):
        # OpenAIオブジェクトを作成
        openai = importlib.import_module("openai")
        if self.azure_openai:
            client = openai.AzureOpenAI(
                api_key=self.openai_api_key,
                api_version="2023-12-01-preview",
                azure_endpoint = self.azure_openai_endpoint
            )
        else:
            client = openai.OpenAI(
                api_key=self.openai_api_key,
            )
        return client

