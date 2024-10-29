from file_extractor import extract_text_from_file
# main
if __name__ == "__main__":
    import sys

    # ファイル名を指定してテキストを抽出する
    filename = sys.argv[1]
    print(extract_text_from_file(filename))
    
            