import os, json
import ai_app_wrapper

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
    opts, args = getopt.getopt(sys.argv[1:], "p:")
    for opt, arg in opts:
        if opt == "-p":
            props_file = arg

    # プロパティファイル(JSON)を読み込む
    if props_file:
        print(f"props_file:{props_file}")
        with open(props_file, "r", encoding="utf-8") as f:
            props_dict = json.load(f)
            open_ai_props_dict = props_dict.get("open_ai_props", {})
            vector_db_props_dict = props_dict.get("vector_db_props", [])
            request = props_dict.get("chat_request", {})

    props_json = json.dumps(open_ai_props_dict, ensure_ascii=False, indent=4)
    vector_db_items_json = json.dumps(vector_db_props_dict, ensure_ascii=False, indent=4)
    request_json = json.dumps(request, ensure_ascii=False, indent=4)

    result:dict = ai_app_wrapper.run_langchain_chat(props_json, vector_db_items_json, request_json)

    print (f"result:{result}")


