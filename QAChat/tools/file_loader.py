from unstructured.partition.auto import partition
from langchain.docstore.document import Document

class FileLoader:
    def __init__(self, file_path: str):
        self.file_path = file_path
        self.text_list = self.__load_file()
        document_overview = self.text_list[0]
        if len(self.text_list) == 1:
            self.text_with_overview_list = [ document_overview ]
        else:
            self.text_with_overview_list = [ document_overview + '\n' + text for text in self.text_list[1:]]
    
    def __load_file(self, chunk_size=1000):
        elements = partition(filename=self.file_path)
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
        return [Document(page_content=text, metadata={"source": self.file_path}) for text in self.text_with_overview_list]

