import os, json
from typing import Any
from ai_app_openai.ai_app_openai_util import OpenAIProps
from ai_app_vector_db.ai_app_vector_db_util import VectorDBProps
from ai_app_autogen.ai_app_autogen_util import AutoGenUtil
from ai_app_autogen.ai_app_autogen_groupchat import AutoGenGroupChat
from ai_app_autogen.ai_app_autogen_client import AutoGenProps


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
    
    # プロパティファイル(JSON)を読み込む
    open_ai_props_dict = {}
    vector_db_props_dict = []
    request = {}

    if props_file:
        print(f"props_file:{props_file}")
        with open(props_file, "r", encoding="utf-8") as f:
            props_dict = json.load(f)
            # AutoGenPropsを生成
            autogen_props = AutoGenProps(props_dict)

    input_text = ""
    # messageが指定されている場合は, messageを入力テキストとする
    if message:
        input_text = message
    else:
        # requestの[messages][0][content]の最後の要素を入力テキストとする
        messages = request.get("messages", [])
        if messages:
            last_content = messages[0].get("content",[])[-1]
            input_text = last_content.get("text", "")

    # メッセージが指定されていない場合は入力メッセージがない旨を表示して終了
    if not input_text:
        print("Input message is not specified.")
        sys.exit(1)

    # メッセージを表示
    print(f"Input message: {input_text}")

    # AutogenGroupChatを生成
    default_group_chat = AutoGenGroupChat.create_default_group_chat(open_ai_props, vector_db_props_list)
    autogen_util = AutoGenUtil(open_ai_props, work_dir, vector_db_props_list)
    result = autogen_util.run_default_group_chat(input_text)
    for message, _ in result:
        print(f"message:{message}")

