import os, json
from typing import Any
import ai_app_wrapper
import ai_app

import logging 
logging.basicConfig(level=logging.ERROR)


import sys
import getopt

if __name__ == '__main__':
    # AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
    os.environ["PYTHONUTF8"] = "1"

    # getoptsでオプション引数の解析
    # -p オプションでOpenAIプロパティファイル(JSON)を指定する
    # -v オプションでVectorDBプロパティファイル(JSON)を指定する
    # -o オプションで出力ファイルを指定する
    # -m オプションでメッセージを指定する
    message = None
    output_file = None
    props_file = None
    work_dir = None

    opts, args = getopt.getopt(sys.argv[1:], "m:o:p:d")
    for opt, arg in opts:
        if opt == "-m":
            message = arg
        elif opt == "-o":
            output_file = arg
        elif opt == "-p":
            props_file = arg
        elif opt == "-d":
            from promptflow.tracing import start_trace
            # instrument OpenAI
            start_trace()

    if props_file:
        print(f"props_file:{props_file}")
        with open(props_file, "r", encoding="utf-8") as f:
            props_dict = json.load(f)
            context_dict = props_dict.get("context", None)
            if not context_dict:
                raise ValueError("context is not found in props.")
            context_json = json.dumps(context_dict, ensure_ascii=False)

            request_dict = props_dict.get("request", None)
            if not request_dict:
                raise ValueError("request is not found in props.")
            
            # openai_props
            openai_props, _ = ai_app_wrapper.get_openai_objects(context_json)
            # vector_db_items
            vector_db_items = ai_app_wrapper.get_vector_db_objects(context_json)
            # autogen_props
            autogen_props = ai_app_wrapper.get_autogen_objects(openai_props,  context_json)

            # メッセージを取得
            # requestの[messages][0][content]の最後の要素を入力テキストとする
            messages = request_dict.get("messages", [])
            if not messages:
                raise ValueError("messages is not found in request.")

            last_content = messages[0].get("content",[])[-1]
            input_text = last_content.get("text", "")

    # メッセージを表示
    print(f"Input message: {input_text}")

    # AutogenGroupChatを実行
    for message, _ in ai_app.run_autogen_normal_chat(autogen_props, vector_db_items, input_text):
        print(f"message:{message}")

