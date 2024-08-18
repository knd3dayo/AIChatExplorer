import sys
sys.path.append("python")

import openpyxl


def export_to_excel(filePath, data):
    # Workbookオブジェクトを生成
    wb = openpyxl.Workbook()
    # アクティブなシートを取得
    ws = wb.active
    # シート名を設定
    ws.title = "Sheet1"
    # データを書き込む
    for row in data:
        ws.append(row)
    # ファイルを保存
    wb.save(filePath)
    
def import_from_excel(filePath):
    # Workbookオブジェクトを生成
    wb = openpyxl.load_workbook(filePath)
    # アクティブなシートを取得
    ws = wb.active
    # データを取得
    data = []
    for row in ws.iter_rows(values_only=True):
        data.append(row)
    
    return data
