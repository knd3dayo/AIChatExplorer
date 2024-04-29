from unstructured.partition.auto import partition
from langchain.docstore.document import Document
import os

class FileLoader:
    def __init__(self, workdir_path:str, relative_file_path: str, repository_url: str):
        self.workdir_path = workdir_path
        self.relative_file_path = relative_file_path
        self.repository_url = repository_url
        self.file_path = os.path.join(workdir_path, relative_file_path)
        
        self.text_list = self.__load_file()
        document_overview = self.text_list[0]
        if len(self.text_list) == 1:
            self.text_with_overview_list = [ document_overview ]
        else:
            self.text_with_overview_list = [ document_overview + '\n' + text for text in self.text_list[1:]]
    
    def __load_file(self, chunk_size=1000):
        # â‘ÎƒpƒX‚ðŽæ“¾
        absolute_file_path = os.path.join (self.workdir_path, self.relative_file_path)
        elements = partition(filename=absolute_file_path)
        text_list = []
        text = ""
        for el in elements:
            text += str(el)
            # print(text)
            if len(text) > chunk_size:
                text_list.append(text)
                text = ""
        if len(text) > 0:
            text_list.append(text)

        return text_list
    
    def get_document_list(self):
        return [ Document(page_content=text, metadata={"source_url": self.repository_url, "source_path": self.relative_file_path}) for text in self.text_with_overview_list]

