import os
import sys
import getopt

from ai_chat_lib.cmd_tools.client_util import *

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

os.environ["PYTHONUTF8"] = "1"
def usage():
    """
    コマンドライン引数の使い方を表示する関数
    :return: None
    """
    print("Usage: python normal_chat_api.py  -s <api_base> [-f <request_json_file>] [-k <search_result_count>] [-p <vector_db_folder>] [-m <message>]")
    print("Options:")
    print("  -f <request_json_file>   : リクエストJSONファイル")
    print("  -s <api_base>            : APIのURL")
    print("  -p <vector_db_folder>    : ベクトルDBの検索対象フォルダ")
    print("  -k <search_result_count> : 検索結果の数")
    print("  -m <message>             : メッセージ")
    print("  -h                       : ヘルプ")

def __process_arguments(sys_args: list[str]) -> tuple:
    """
    コマンドライン引数を処理する関数
    :param sys_args: コマンドライン引数
    :return: リクエストJSONファイル, APIのURL,  メッセージ, 検索結果数, ベクトルDBの検索対象フォルダ
    """
    # リクエストJSONファイルの指定
    request_json_file = None
    # メッセージの指定
    message = None
    # APIのURL
    api_base = None
    # 検索結果の数の指定
    search_result_count = 10
    # ベクトルDBの検索対象フォルダ
    vector_db_folder = None

    opts, args = getopt.getopt(sys_args[1:], "f:s:m:k:p:h")
    for opt, arg in opts:
        if opt == "-f":
            # リクエストJSONファイルの指定
            request_json_file = arg
        elif opt == "-s":
            # APIのURLの指定
            api_base = arg
        elif opt == "-m":
            # メッセージの指定
            message = arg
        elif opt == "-k":
            # 検索結果の数の指定
            search_result_count = int(arg)
            if search_result_count <= 0:
                raise ValueError("Search result count must be greater than 0.")
        elif opt == "-p":
            # ベクトルDBの検索対象フォルダの指定
            vector_db_folder = arg
        elif opt == "-h":
            # ヘルプの表示
            usage()
            sys.exit(0)

    return request_json_file, api_base, message, search_result_count, vector_db_folder

async def call_api(request_dict: dict, api_endpoint: str):
    """
    APIを呼び出す関数
    :param request_dict: リクエスト辞書
    :param api_endpoint: APIのURL
    :return: なし
    """
    response_dict =  await send_request(request_dict, api_endpoint)
        # outputの取得
    logger.debug(f"Response data: {response_dict}")
    documents: list[dict] = response_dict.get("documents",[])
    for document in documents:
        print("--------------------------------------------------")
        # doc_id
        print(f"Document ID: {document['doc_id']}")
        # score
        print(f"Document Score: {document['score']}")
        # forder_path
        print(f"Document Folder Path: {document['folder_path']}")
        print(f"Document Source Path: {document.get('source_path', 'N/A')}")
        print(f"Document Content: {document['content']}")

async def main():

    # コマンドライン引数の処理
    request_json_file, api_base,  message, search_result_count, vector_db_folder = __process_arguments(sys.argv)
        
    # リクエストの準備
    request_dict = prepare_vector_search_request(request_json_file, message, search_result_count, vector_db_folder)

    # APIエンドポイント
    api_endpoint = api_base + "/vector_search"

    await call_api(request_dict, api_endpoint)

if __name__ == '__main__':
    import asyncio
    asyncio.run(main())