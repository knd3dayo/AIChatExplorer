import os, json
from typing import Any
import ai_chat_lib.api_modules.ai_app_wrapper as ai_app_wrapper
import asyncio
import sys
import getopt
import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

async def main():
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

    if props_file:
        logger.debug(f"props_file:{props_file}")
        with open(props_file, "r", encoding="utf-8") as f:
            props_dict = json.load(f)

            request_dict = props_dict.get("chat_request", None)
            if not request_dict:
                raise ValueError("request is not found in props.")
            
            # autogen_props 
            autogen_props = ai_app_wrapper.get_autogen_objects(props_dict)


            # メッセージを取得
            # requestの[messages][0][content]の最後の要素を入力テキストとする
            messages = request_dict.get("messages", [])
            if not messages:
                raise ValueError("messages is not found in request.")

            last_content = messages[-1].get("content",[])[-1]
            input_text = last_content.get("text", "")

    # メッセージを表示
    print(f"Input message: {input_text}")

    # AutogenGroupChatを実行
    message_count = 0
    # run_group_chatを実行
    async for message in autogen_props.run_autogen_chat(input_text):
        if not message:
            break
        message_count += 1
        print(f"[step {message_count}]:{message}")
        # print(f"source:{message.source}\nmessage:{message.content}")

if __name__ == '__main__':
    asyncio.run(main())
