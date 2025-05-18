import os
import sys
import getopt
import json
import logging 

from ai_chat_lib.chat_modules import ChatUtil

logger = logging.getLogger(__name__)
logger.setLevel(logging.ERROR)
os.environ["PYTHONUTF8"] = "1"

def check_app_data_path():
    """
    環境変数APP_DATA_PATHが設定されているか確認する関数
    :return: APP_DATA_PATHの値
    """
    if not os.environ.get("APP_DATA_PATH", None):
        # 環境変数APP_DATA_PATHが指定されていない場合はエラー. APP_DATA_PATHの説明を出力するとともに終了する
        logger.error("APP_DATA_PATH is not set.")
        logger.error("APP_DATA_PATH is the path to the root directory where the application data is stored.")
        raise ValueError("APP_DATA_PATH is not set.")

def create_request_from_json_file(request_json_file: str) -> dict:
    """
    JSONファイルからリクエストを作成する関数
    :param request_json_file: JSONファイルのパス
    :return: リクエスト辞書
    """
    with open(request_json_file, "r", encoding="utf-8") as f:
        request_json = f.read()
        request_dict = json.loads(request_json)
    return request_dict

async def main():
    
    # リクエストJSONファイルの指定
    request_json_file = None

    opts, args = getopt.getopt(sys.argv[1:], "p:d:")
    for opt, arg in opts:
        if opt == "-p":
            request_json_file = arg
        elif opt == "-d":
            os.environ["APP_DATA_PATH"] = arg

    # 環境変数APP_DATA_PATHの確認
    check_app_data_path() 

    if request_json_file:
        # request_json_fileが指定されている場合はそのファイルを読み込む
        request_dict = create_request_from_json_file(request_json_file)
    else:
        # TODO request_json_fileが指定されていない場合は環境変数から情報を取得するモード
        raise ValueError("request_json_file is not set.")

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