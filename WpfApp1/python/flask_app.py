from flask import Flask

app = Flask(__name__)

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

openai_util = OpenAIUtil(
    openai_api_key="{{OPENAI_API_KEY}}", 
    azure_openai={{AZURE_OPENAI}}, 
    azure_openai_endpoint={{AZURE_OPENAI_ENDPOINT}}
)
chat_model_name = "{{CHAT_MODEL_NAME}}"

@app.route('/')
def hello_world():
    return 'Hello, World!'

def openai_chat(input_json, chat_model_name):
    # OpenAIのchatを実行する
    json_obj = json.loads(input_json)
    client = openai_util.client
    response = client.chat.completions.create(
        model=chat_model_name,
        messages=json_obj
    )
    return response.choices[0].message.content



if __name__ == '__main__':
    app.run(debug=True , port={{PORT}})
