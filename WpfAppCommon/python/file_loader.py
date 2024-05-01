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
        # ��΃p�X���擾
        absolute_file_path = os.path.join (self.workdir_path, self.relative_file_path)
        
        # �t�@�C���T�C�Y��0�̏ꍇ�͋�̃��X�g��Ԃ�
        if os.path.getsize(absolute_file_path) == 0:
            return text_list
        # python-magic��2�o�C�g�t�@�C�����������ƃG���[�ɂȂ�ꍇ�����邽�߁A�t�@�C�����ꎞ�t�@�C���ɃR�s�[���ď�������
        # �ꎞ�t�@�C���̊g���q�͌��̃t�@�C���̊g���q�Ɠ����ɂ���
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
        # �ꎞ�t�@�C�����폜
        os.remove(absolute_file_path)

        return text_list
    
    def get_document_list(self):
        return [ Document(page_content=text, metadata={"source_url": self.repository_url, "source_path": self.relative_file_path}) for text in self.text_with_overview_list]

