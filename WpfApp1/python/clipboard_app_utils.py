


def extract_text(filename):
    from unstructured.partition.auto import partition

    # filenameのファイルからテキストを抽出する
    elements = partition(filename=filename)
    return "\n".join([element.text for element in elements])


def mask_data(nlp, text ):
    doc = nlp(text)
    # ent.label_がPERSONのent.textと 連番を保持する
    person_dict = {}
    # ent.label_がORGのent.textと 連番を保持する
    org_dict = {}
    # ent.label_がGPEのent.textと 連番を保持する
    gpe_dict = {}

    for ent in doc.ents:
        if ent.label_ == "PERSON":
            num = person_dict.get(ent.text, None)
            if num is None:
                num = len(person_dict) + 1
                person_dict[ent.text] = num

            text = text.replace(ent.text, f"[MASKED {ent.label_} {num}]")
       
        elif ent.label_ == "ORG":
            num = org_dict.get(ent.text, None)
            if num is None:
                num = len(org_dict) + 1
                org_dict[ent.text] = num

            text = text.replace(ent.text, f"[MASKED {ent.label_} {num}]")

        elif ent.label_ == "GPE":
            num = gpe_dict.get(ent.text, None)
            if num is None:
                num = len(gpe_dict) + 1
                gpe_dict[ent.text] = num
        
            text = text.replace(ent.text, f"[MASKED {ent.label_} {num}]")

    return text

def openai_chat2():
    pass

def openai_chat(input_json, azure_openai, openai_api_key, chat_model_name, azure_openai_endpoint):
    # OpenAIのchatを実行する
    openai_util = OpenAIUtil(azure_openai, openai_api_key, azure_openai_endpoint)
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
    def __init__(self, azure_openai, openai_api_key, azure_openai_endpoint=None):
        self.azure_openai = azure_openai
        self.openai_api_key = openai_api_key
        self.azure_openai_endpoint = azure_openai_endpoint

        self.client = self._create_openai_object()

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

