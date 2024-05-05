import faiss
import os

index_file_name="index.faiss"
index = None

# faissのindexをロードする関数
def load_faiss_index():
    global index
    # indexファイルが存在しない場合はindexを作成する
    if not os.path.exists(index_file_name):
        index = faiss.IndexFlatL2(1536)
    else:
        index = faiss.read_index(index_file_name)
            
    return index

# faissのindexを保存する関数
def save_faiss_index():
    faiss.write_index(index, index_file_name)
    