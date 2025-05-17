import os
import sys
import getopt
import httpx  # type: ignore
import logging 

logging.basicConfig(level=logging.ERROR)
os.environ["PYTHONUTF8"] = "1"


async def main():
    # リクエストJSONファイルの指定
    request_json_file = None
    # APIのURL
    api_url = None
    opts, args = getopt.getopt(sys.argv[1:], "p:s:")
    for opt, arg in opts:
        if opt == "-p":
            request_json_file = arg
        elif opt == "-s":
            api_base = arg
        
    if not request_json_file:
        raise ValueError("request_json_file is not set.")
    if not api_base:
        raise ValueError("api_base is not set.")

    with open(request_json_file, "r", encoding="utf-8") as f:
        request_json = f.read()

    # APIエンドポイント api_base + /    openai_chat
    api_endpoint = api_base + "/openai_chat"

    # APIリクエスト 現時点では認証はなし
    headers = {
        "Content-Type": "application/json",
        "Accept": "application/json",
    }
    # APIリクエスト送信
    async with httpx.AsyncClient() as client:
        response = await client.post(api_endpoint, headers=headers, data=request_json)

    # レスポンスの取得
    if response.status_code != 200:
        raise ValueError(f"API request failed with status code {response.status_code}")
    
    # レスポンスのJSONをdictionaryに変換
    response_dict = response.json()  # response.json() は非同期ではない
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