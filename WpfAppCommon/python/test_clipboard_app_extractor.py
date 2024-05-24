from clipboard_app_extractor import extract_text
# main
if __name__ == "__main__":
    import sys

    # ファイル名を指定してテキストを抽出する
    filename = sys.argv[1]
    print(extract_text(filename))
    
            