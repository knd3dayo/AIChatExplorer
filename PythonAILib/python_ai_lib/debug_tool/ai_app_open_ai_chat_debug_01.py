import os, json
import ai_app_wrapper
import logging 
import ai_app_wrapper
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
            from promptflow.tracing import start_trace
            # instrument OpenAI
            start_trace()

    print(f"props_file:{props_file}")
    with open(props_file, "r", encoding="utf-8") as f:
        props_dict = json.load(f)
        context_dict = props_dict.get("context", None)
        if not context_dict:
            raise ValueError("context is not found in props.")
        request_dict = props_dict.get("request", None)
        if not request_dict:
            raise ValueError("request is not found in props.")

    context_json = json.dumps(context_dict, ensure_ascii=False, indent=4)
    request_json = json.dumps(request_dict, ensure_ascii=False, indent=4)

    result_string = ai_app_wrapper.run_openai_chat(context_json, request_json)
    result = json.loads(result_string)
    for key, value in result.items():
        print(f"{key}:{value}")


