import os
import sys
import getopt
import json
import logging 

from ai_chat_lib.chat_modules import ChatUtil

logging.basicConfig(level=logging.ERROR)
os.environ["PYTHONUTF8"] = "1"


async def main():
    
    # リクエストJSONファイルの指定
    request_json_file = None

    opts, args = getopt.getopt(sys.argv[1:], "p:d:")
    for opt, arg in opts:
        if opt == "-p":
            request_json_file = arg
        elif opt == "-d":
            os.environ["APP_DATA_PATH"] = arg
        
    if not request_json_file:
        raise ValueError("request_json_file is not set.")

    if not os.environ.get("APP_DATA_PATH", None):
        raise ValueError("APP_DATA_PATH is not set.")

    with open(request_json_file, "r", encoding="utf-8") as f:
        request_json = f.read()
        request_dict = json.loads(request_json)

    # run_openai_chat_async_apiを実行
    response_dict = await ChatUtil.run_openai_chat_async_api(request_dict)
    
    # outputの取得
    output = response_dict.get("output")
    # outputの表示
    if output:
        print(output)
    else:
        raise ValueError("No output found in the response.")

if __name__ == '__main__':
    import asyncio
    asyncio.run(main())