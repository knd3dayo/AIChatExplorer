from tkinter import SE
from langchain.docstore.document import Document
import os
import tempfile
import file_extractor


class FileLoader:
    def __init__(self, workdir_path:str, relative_file_path: str, repository_url: str):
        self.workdir_path = workdir_path
        self.relative_file_path = relative_file_path
        self.repository_url = repository_url
        self.file_path = os.path.join(workdir_path, relative_file_path)
        
        # ファイルの読み込み
        self.text_list = self.load_file()
        if len(self.text_list) == 0:
            self.text_with_overview_list = []
        # text_listが1つの場合はそのままtext_with_overview_listに代入
        elif len(self.text_list) == 1:
            self.text_with_overview_list = self.text_list
        else:
            document_overview = self.text_list[0]
            self.text_with_overview_list = [ document_overview + '\n' + text for text in self.text_list[1:]]
    
    def load_file(self, chunk_size=500):
        text_list = []
        # 絶対パスを取得
        absolute_file_path = os.path.join (self.workdir_path, self.relative_file_path)
        
        # ファイルサイズが0の場合は空のリストを返す
        if os.path.getsize(absolute_file_path) == 0:
            return text_list
        # テキスト抽出
        text = file_extractor.extract_text(absolute_file_path)
        
        # テキストをchunk_sizeで分割
        for i in range(0, len(text), chunk_size):
            text_list.append(text[i:i + chunk_size])

        return text_list
    
    def get_document_list(self):
        return [ Document(page_content=text, metadata={"source_url": self.repository_url, "source": self.relative_file_path}) for text in self.text_with_overview_list]


if __name__ == "__main__":
    import sys
    workdir_path = "."
    repository_url = "https://example.com"
    relative_file_path = sys.argv[1]
    loader = FileLoader(workdir_path, relative_file_path, repository_url)
    documents = loader.get_document_list()
    for doc in documents:
        print(doc)
        print("\n-------\n")
