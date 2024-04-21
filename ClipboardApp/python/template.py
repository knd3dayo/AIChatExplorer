## -------------------------------------------------------------------
## ここから改変不可
## -------------------------------------------------------------------
'''
コピペアプリのClipboardItemオブジェクトの内容に従ったJSON文字列を引数にとり任意の処理を行うためのテンプレート
 引数はstr型のJSON文字列で下記のフォーマット
{
     "id": "ClipboardItemのID",
     "CollectionName": "コレクション名",
     "CreatedAt": "生成日時",
     "UpdatedAt": "更新日時",
     "Content": "クリップボードの内容",
     "ContentType": "クリップボードの内容の種類",
     "Tags": ["タグ1", "タグ2", ...],
     "Description": "説明",
     "SourceApplicationName": "貼り付け元のアプリケーション名",
     "SourceApplicationTitle": "貼り付け元のアプリケーションのタイトル",
     "SourceApplicationID": "貼り付け元のアプリケーションのID",
     "SourceApplicationPath": "貼り付け元のアプリケーションのパス"
}


戻り値はstr型 or None
戻り値がstr型の場合は、その文字列がクリップボードにコピーされる
戻り値がNoneの場合は、クリップボードの内容は変更されない


'''
import json
import datetime

def parse_clipboard_item_str(json_string) -> dict:
    result = {}
    # JSON文字列をパース
    data = json.loads(json_string)

    # CreatedAt
    result['CreatedAt'] = parse_datestr(data.get('CreatedAt',datetime.datetime.now().isoformat()))
    # UpdatedAt
    result['UpdatedAt']  = parse_datestr(data.get('UpdatedAt',datetime.datetime.now().isoformat()))
    return data

def parse_datestr(datestr: str) -> datetime.datetime:
        return datetime.datetime.fromisoformat(datestr)

def json_default(value):
    if isinstance(value, datetime.datetime):
        return value.isoformat()
    else:
        return value

def execute(json_str: str) -> str:

    clipboard_item_dict = parse_clipboard_item_str(json_str)
    result:dict = process_clipboard_item(clipboard_item_dict)
    result_str = json.dumps(result, ensure_ascii=False , default=json_default)
    return result_str

## -------------------------------------------------------------------
## ここまで改変不可
## -------------------------------------------------------------------

def process_clipboard_item(item:dict) -> dict:
    result = item
    # ここに処理を記述する

    return result

