import os, json
import logging 
import ai_chat_lib.api_modules.ai_app_wrapper as ai_app_wrapper
logging.basicConfig(level=logging.ERROR)

# AutoGenのCodeExecutor実行時にUncicodeEncodeErrorが発生するため、Pythonのデフォルトの文字コードをUTF-8に設定
os.environ["PYTHONUTF8"] = "1"

import os
import sys
import getopt

async def main():
    # プロパティファイルの指定
    props_file = "ai_chat_explorer/debug_tool/props/openai_chat_props.json"
    # VectorDBの検索リクエストを実行する場合は、以下のように指定する。
    # props_file = "ai_chat_explorer/debug_tool/props/openai_chat_props_vector_db.json"

    # getoptsでオプション引数の解析
    # -p オプションでプロパティファイル(JSON)を指定する
    # -r オプションがある場合はVectorDBの検索リクエストを実行する。
    vector_search_request = False
    opts, args = getopt.getopt(sys.argv[1:], "p:")
    for opt, arg in opts:
        if opt == "-p":
            props_file = arg

    if not props_file:
        raise ValueError("props_file is not set.")
    
    print(f"props_file:{props_file}")
    with open(props_file, "r", encoding="utf-8") as f:
        request_json = f.read()

    result_string = await ai_app_wrapper.openai_chat_async(request_json)
    result = json.loads(result_string)
    for key, value in result.items():
        print(f"{key}:{value}")

if __name__ == '__main__':
    # asyncio.run(main())
    import asyncio
    asyncio.run(main())

