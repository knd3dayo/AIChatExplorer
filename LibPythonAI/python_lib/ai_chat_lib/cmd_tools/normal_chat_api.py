import os
import sys
import getopt
import httpx  # type: ignore

from ai_chat_lib.cmd_tools.client_util import *

import ai_chat_lib.log_settings as log_settings
logger = log_settings.getLogger(__name__)

os.environ["PYTHONUTF8"] = "1"

def process_arguments(sys_args: list[str]) -> tuple:
    """
    コマンドライン引数を処理する関数
    :param sys_args: コマンドライン引数
    :return: リクエストJSONファイル, APIのURL, インタラクティブモードのフラグ, メッセージ
    """
    # リクエストJSONファイルの指定
    request_json_file = None
    # インタラクティブモードの指定
    interactive_mode = False
    # メッセージの指定
    message = None
    # APIのURL
    api_base = None

    opts, args = getopt.getopt(sys_args[1:], "f:s:m:i")
    for opt, arg in opts:
        if opt == "-f":
            # リクエストJSONファイルの指定
            request_json_file = arg
        elif opt == "-s":
            # APIのURLの指定
            api_base = arg
        elif opt == "-i":
            # インタラクティブモードの指定
            interactive_mode = True
        elif opt == "-m":
            # メッセージの指定
            message = arg

    return request_json_file, api_base, interactive_mode, message

async def call_api(request_dict: dict, api_endpoint: str):
    """
    APIを呼び出す関数
    :param request_dict: リクエスト辞書
    :param api_endpoint: APIのURL
    :return: なし
    """
    response_dict =  await send_request(request_dict, api_endpoint)
        # outputの取得
    output = response_dict.get("output")
    # outputの表示
    if output:
        print(output)
    else:
        raise ValueError("No output found in the response.")

async def call_api_interactve(request_dict: dict, api_endpoint: str):
    """
    インタラクティブモードでAPIを呼び出す関数
    :param request_dict: リクエスト辞書
    :param api_endpoint: APIのURL
    :return: なし
    """
    # インタラクティブモードでチャットを実行
    while True:
        input_message = input("User: ")
        # ユーザーメッセージを追加
        update_normal_chat_messages("user", input_message, request_dict)
        response_dict = await send_request(request_dict, api_endpoint)
        # レスポンスを取得
        output = response_dict.get("output")
        if output:
            print(f"Assistant:\n{output}")
            # レスポンスを追加
            update_normal_chat_messages("assistant", output, request_dict)
        else:
            print("No output found in the response.")

async def main():

    # コマンドライン引数の処理
    request_json_file, api_base, interactive_mode, message = process_arguments(sys.argv)
        
    # リクエストの準備
    request_dict = prepare_normal_chat_request(request_json_file, interactive_mode, message)

    # APIエンドポイント api_base + /    openai_chat
    api_endpoint = api_base + "/openai_chat"

    # インタラクティブモードの場合
    if interactive_mode:
        await call_api_interactve(request_dict, api_endpoint)
    else:
        await call_api(request_dict, api_endpoint)

if __name__ == '__main__':
    import asyncio
    asyncio.run(main())