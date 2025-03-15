import os, json
import logging 
import clipboard_app.ai_app_wrapper as ai_app_wrapper
logging.basicConfig(level=logging.ERROR)

# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

import os
import sys
import getopt

if __name__ == '__main__':

    # getoptsでオプション引数の解析
    # -p オプションでプロパティファイル(JSON)を指定する
    # -r オプションがある場合はVectorDBの検索リクエストを実行する。
    props_file = None
    vector_search_request = False
    opts, args = getopt.getopt(sys.argv[1:], "p:d")
    for opt, arg in opts:
        if opt == "-p":
            props_file = arg
        elif opt == "-d":
            from promptflow.tracing import start_trace # type: ignore
            # instrument OpenAI
            start_trace()
    if not props_file:
        raise ValueError("props_file is not set.")
    
    print(f"props_file:{props_file}")
    with open(props_file, "r", encoding="utf-8") as f:
        request_json = f.read()

    result_string = ai_app_wrapper.openai_chat(request_json)
    result = json.loads(result_string)
    for key, value in result.items():
        print(f"{key}:{value}")


