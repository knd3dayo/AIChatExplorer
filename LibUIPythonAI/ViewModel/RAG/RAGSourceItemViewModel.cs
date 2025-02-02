using System.Collections.ObjectModel;
using PythonAILib.Model.VectorDB;

namespace LibUIPythonAI.ViewModel.RAG {
    public class RAGSourceItemViewModel(RAGSourceItem item) : ChatViewModelBase {
        public RAGSourceItem Item { get; set; } = item;
        // SourceURL
        public string SourceURL {
            get => Item.SourceURL;
            set {
                Item.SourceURL = value;
                OnPropertyChanged(nameof(SourceURL));
            }
        }
        // WorkingDirectory
        public string WorkingDirectory {
            get => Item.WorkingDirectory;
            set {
                Item.WorkingDirectory = value;
                // フォルダが存在する場合はソースURLを取得してSourceURLを更新
                SourceURL = Item.GetRemoteURL();
                OnPropertyChanged(nameof(WorkingDirectory));
            }
        }

        // LastIndexCommitHash
        public string LastIndexCommitHash {
            get => Item.LastIndexCommitHash;
            set {
                Item.LastIndexCommitHash = value;
                OnPropertyChanged(nameof(LastIndexCommitHash));
            }
        }

        // VectorSearchProperty
        public VectorDBItem? VectorDBItem {
            get => Item.VectorDBItem;
            set {
                Item.VectorDBItem = value;
                OnPropertyChanged(nameof(VectorDBItem));
            }
        }
        // ComboBoxの選択肢
        public ObservableCollection<VectorDBItem> VectorDBItems {
            get {
                return [.. VectorDBItem.GetExternalVectorDBItems()];
            }
        }
        public VectorDBItem? SelectedVectorDBItem {
            get {
                return Item.VectorDBItem;
            }
            set {
                Item.VectorDBItem = value;
                OnPropertyChanged(nameof(SelectedVectorDBItem));
            }
        }

        // 最後にインデックス化したコミットの情報
        public string LastIndexedCommitInfo {
            get {
                return Item.LastIndexedCommitInfoDisplayString;
            }
        }

        // save
        public void Save() {
            Item.Save();
        }
        // delete
        public void Delete() {
            Item.Delete();
        }
        // checkWorkingDirectory
        public bool CheckWorkingDirectory() {
            return Item.CheckWorkingDirectory();
        }
    }

}
