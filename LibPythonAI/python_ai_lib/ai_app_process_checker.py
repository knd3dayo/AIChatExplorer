# 5秒ごとにプロセスを監視し、異常があれば指定したURLを呼び出す
# このアプリケーションは、メインアプリのプロセス終了時にFlaskサーバーを停止するためのもの

import sys, os
import time
import requests
import psutil

# プロセスを監視する
def check_process(pid : int, url : str):
    while True:
        # pidのプロセスが存在しない場合
        if not psutil.pid_exists(pid):
            # 指定したURLにリクエストを送信
            try:
                #エラーを無視してリクエストを送信
                requests.post(url)
            except:
                pass

            # prompt flowサービスを停止
            try:
                # エラーを無視してコマンドを実行
                # pf service stopコマンドを実行
                os.system("pf service stop")
            except: 
                pass       

            break

        time.sleep(5)

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

