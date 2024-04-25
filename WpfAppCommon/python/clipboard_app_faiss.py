import faiss
import os

index_file_name="index.faiss"
index = None
def load_faiss_index():
    global index
    # indexファイルが存在しない場合はindexを作成する
    if os.path.exists(index_file_name) == False:
        index = faiss.IndexFlatL2(1536)
    else:
        index = faiss.read_index(index_file_name)
        
    return index

def save_faiss_index():
    faiss.write_index(index, index_file_name)
    