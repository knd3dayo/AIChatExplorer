# 5秒ごとにプロセスを監視し、異常があれば指定したURLを呼び出す
# このアプリケーションは、メインアプリのプロセス終了時にFlaskサーバーを停止するためのもの

import sys, os
import time
import requests # type: ignore
import psutil # type: ignore

import ai_chat_lib.log_modules.log_settings as log_settings
logger = log_settings.getLogger(__name__)


# プロセスを監視する
def check_process(pid : int, url : str):
    
    while True:
        time.sleep(5)
        # pidのプロセスが存在しない場合
        if not psutil.pid_exists(pid):
            # 指定したURLにリクエストを送信
            try:
                os.environ["NO_PROXY"] = "localhost"
                #エラーを無視してリクエストを送信 timeout=5
                requests.post(url, timeout=5)
            except:
                pass

            break

# メイン
if __name__ == "__main__":
    # 引数の取得
    args = sys.argv
    if len(args) != 3:
        print("Usage: python ai_app_process_checker.py [pid] [url]")
        sys.exit(1)
    pid = int(args[1])
    url = args[2]

    # プロセスを監視
    check_process(pid, url)

