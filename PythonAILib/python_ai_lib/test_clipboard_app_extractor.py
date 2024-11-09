from ai_app_file_util import FileUtil
# main
if __name__ == "__main__":
    import sys

    # ファイル名を指定してテキストを抽出する
    filename = sys.argv[1]
    print(FileUtil.extract_text_from_file(filename))
    
            