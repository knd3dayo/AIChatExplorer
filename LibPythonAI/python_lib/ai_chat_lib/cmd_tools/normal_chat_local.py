import os
import sys
import getopt
import json
from ai_chat_lib.cmd_tools.client_util import *
from ai_chat_lib.chat_modules import ChatUtil

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

os.environ["PYTHONUTF8"] = "1"
def usage():
    """
    コマンドライン引数の使い方を表示する関数
    :return: None
    """
    print("Usage: python normal_chat_local.py -f <request_json_file> [-d <app_data_path>] [-i] [-m <message>]")
    print("Options:")
    print("  -f <request_json_file> : リクエストJSONファイル")
    print("  -d <app_data_path>     : アプリケーションデータのパス")
    print("  -i                     : インタラクティブモード")
    print("  -m <message>           : メッセージ")
    print("  init                   : アプリケーションの初期化")
    print("  -h                     : ヘルプ")

def __process_arguments(sys_args: list[str]) -> tuple:
    """
    コマンドライン引数を処理する関数
    :param sys_args: コマンドライン引数
    :return: リクエストJSONファイル, インタラクティブモードのフラグ, メッセージ, 初期化フラグ
    """
    # リクエストJSONファイルの指定
    request_json_file = None
    # インタラクティブモードの指定
    interactive_mode = False
    # メッセージの指定
    message = None

    opts, args = getopt.getopt(sys_args[1:], "f:d:m:ih")
    for opt, arg in opts:
        if opt == "-f":
            # リクエストJSONファイルの指定
            request_json_file = arg
        elif opt == "-d":
            os.environ["APP_DATA_PATH"] = arg
        elif opt == "-i":
            # インタラクティブモードの指定
            interactive_mode = True
        elif opt == "-m":
            # メッセージの指定
            message = arg
        elif opt == "-h":
            # ヘルプの表示
            usage()
            sys.exit(0)

    # 非オプション引数の処理
    if args:
        # args[0] がinitの場合はアプリケーションの初期化のみを行い終了する。
        if args[0] == "init":
            return None, False, None, True
        else:
            print(f"Unknown argument: {args[0]}")
            usage()
            sys.exit(1)
        
    return request_json_file, interactive_mode, message, False


def init_app() -> None:
    """
    アプリケーション初期化時に呼び出される関数
    :return: None
    """
    # MainDBの初期化
    from ai_chat_lib.db_modules import MainDB
    MainDB.init()
    print("MainDB initialized.")

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

async def run_chat_async(request_dict: dict ):
    """
    非同期でチャットを実行する関数
    :param request_dict: リクエスト辞書
    :return: レスポンス辞書
    """
    # run_openai_chat_async_apiを実行
    response_dict = await ChatUtil.run_openai_chat_async_api(request_dict)
    output:str = response_dict.get("output", "")
    if output:
        print(output)
    else:
        print("No output found in the response.")

async def run_chat_interactive_async(request_dict: dict) -> None:
    """
    インタラクティブモードでチャットを実行する関数
    :param request_dict: リクエスト辞書
    :return: None
    """
    # インタラクティブモードでチャットを実行
    while True:
        input_message = input("User: ")
        # ユーザーメッセージを追加
        add_normal_chat_message("user", input_message, request_dict)
        response_dict = await ChatUtil.run_openai_chat_async_api(request_dict)
        # レスポンスを取得
        output = response_dict.get("output")
        if output:
            print(f"Assistant:\n{output}")
            # レスポンスを追加
            add_normal_chat_message("assistant", output, request_dict)
        else:
            print("No output found in the response.")

async def main():
    
    # コマンドライン引数の処理
    request_json_file, interactive_mode, message, init_flag = __process_arguments(sys.argv)
    if init_flag:
        # アプリケーションの初期化
        init_app()
        return

    # 環境変数APP_DATA_PATHの確認
    check_app_data_path() 

    # リクエストの準備
    request_dict = prepare_normal_chat_request(request_json_file, interactive_mode, message)

    # run_openai_chat_async_apiを実行
    if interactive_mode:
        # インタラクティブモードでチャットを実行
        await run_chat_interactive_async(request_dict)
    else:
        # 非同期でチャットを実行
        await run_chat_async(request_dict)

if __name__ == '__main__':
    import asyncio
    asyncio.run(main())