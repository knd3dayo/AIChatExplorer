# 5秒ごとにプロセスを監視し、異常があれば指定したURLを呼び出す
# このアプリケーションは、メインアプリのプロセス終了時にFlaskサーバーを停止するためのもの

import sys, os
import requests # type: ignore

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)

def stop_process(url : str):
    try:
        os.environ["NO_PROXY"] = "localhost"
        #エラーを無視してリクエストを送信 timeout=5
        requests.post(url, timeout=60)
    except Exception as e:
        import traceback
        logger.error(traceback.format_exc())
        logger.error("Failed to send a request to the specified URL")

# メイン
if __name__ == "__main__":
    # 引数の取得
    args = sys.argv
    if len(args) != 2:
        print("Usage: python ai_app_process_checker.py [pid_file]")
        sys.exit(1)
    url = args[1]
    # プロセスを監視
    stop_process(url)

