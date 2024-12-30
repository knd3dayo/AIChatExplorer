from flask import Flask, request, jsonify # type: ignore
import os
import importlib
import json
import spacy # type: ignore

import ai_app

app = Flask(__name__)


openai_util = ai_app.OpenAIUtil(
    openai_api_key=os.getenv("OPENAI_API_KEY"),
    azure_openai=os.getenv("AZURE_OPENAI"), 
    azure_openai_endpoint=os.getenv("AZURE_OPENAI_ENDPOINT")
)
chat_model_name = os.getenv("CHAT_MODEL_NAME")
spacy_model_name = os.getenv("SPACY_MODEL_NAME")
    
# textの中から個人情報をマスクする
nlp = spacy.load(spacy_model_name)

@app.route('/', methods = ['GET', 'POST'])
def hello_world():
    return 'Hello, World!'

@app.route('/api/clipboard_item', methods = ['POST'])
def clipboard():
    # クリップボードの内容を取得する
    post_data = request.get_json()
    print(post_data)
    

    # actionを取得
    action = post_data.get("action","")
    # actionによって処理を分岐
    if action == "openai_chat":
        # OpenAIのchatを実行
        return openai_chat(post_data["item"], chat_model_name)
    if action == "mask_data":
        # テキストから個人情報をマスク
        item = post_data.get("item", None)
        if item is None:
            response = jsonify({"error": "item is required"})
            return response
        # マスクしたテキストを返す
        content = item.get("content", None)
        if content is None:
            response = jsonify({"error": "content is required"})
            return response
        masked_content = ai_app.mask_data(nlp, content)
        item["content"] = masked_content
        # マスクしたテキストを返す
        return jsonify({"item": item});

    # その他の場合はerrorを設定して返す。
    response = jsonify({"error": "action not found"})
    return response
    

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
    app.run(debug=True , port=os.getenv("PORT"))
