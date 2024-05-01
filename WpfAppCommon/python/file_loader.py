from tkinter import SE
from unstructured.partition.auto import partition
from langchain.docstore.document import Document
import os
import tempfile

class FileLoader:
    def __init__(self, workdir_path:str, relative_file_path: str, repository_url: str):
        self.workdir_path = workdir_path
        self.relative_file_path = relative_file_path
        self.repository_url = repository_url
        self.file_path = os.path.join(workdir_path, relative_file_path)
        
        self.text_list = self.__load_file()
        if len(self.text_list) == 0:
            self.text_with_overview_list = []
        else:
            document_overview = self.text_list[0]
            if len(self.text_list) == 1:
                self.text_with_overview_list = [ document_overview ]
            else:
                self.text_with_overview_list = [ document_overview + '\n' + text for text in self.text_list[1:]]
    
    def __load_file(self, chunk_size=1000):
        text_list = []
        # 絶対パスを取得
        absolute_file_path = os.path.join (self.workdir_path, self.relative_file_path)
        
        # ファイルサイズが0の場合は空のリストを返す
        if os.path.getsize(absolute_file_path) == 0:
            return text_list
        # python-magicが2バイトファイル名を扱うとエラーになる場合があるため、ファイルを一時ファイルにコピーして処理する
        # 一時ファイルの拡張子は元のファイルの拡張子と同じにする
        with tempfile.NamedTemporaryFile(delete=False, suffix=os.path.splitext(self.relative_file_path)[1]) as temp_file:
            with open(absolute_file_path, 'rb') as f:
                temp_file.write(f.read())
            absolute_file_path = temp_file.name


        elements = partition(filename=absolute_file_path)
        text = ""
        for el in elements:
            text += str(el)
            # print(text)
            if len(text) > chunk_size:
                text_list.append(text)
                text = ""
        if len(text) > 0:
            text_list.append(text)
        # 一時ファイルを削除
        os.remove(absolute_file_path)

        return text_list
    
    def get_document_list(self):
        return [ Document(page_content=text, metadata={"source_url": self.repository_url, "source_path": self.relative_file_path}) for text in self.text_with_overview_list]

