using System.Collections.ObjectModel;
using PythonAILib.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Model.QAChat;

namespace QAChat.ViewModel {
    public class RAGSourceItemViewModel : MyWindowViewModel {

        public RAGSourceItemViewModel(RAGSourceItemBase item) {
            this.Item = item;
        }
        public RAGSourceItemBase Item { get; set; }
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
                SourceURL = Item.SeekSourceURL(value);
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

        // VectorDBItem
        public VectorDBItemBase? VectorDBItem {
            get => Item.VectorDBItem;
            set {
                Item.VectorDBItem = value;
                OnPropertyChanged(nameof(VectorDBItem));
            }
        }
        // ComboBoxの選択肢
        public ObservableCollection<VectorDBItemBase> VectorDBItems {
            get {
                return [.. ClipboardAppVectorDBItem.GetItems(false)];
            }
        }
        public VectorDBItemBase? SelectedVectorDBItem {
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
                if (Item == null) {
                    return "";
                }
                if (string.IsNullOrEmpty(Item.LastIndexCommitHash)) {
                    return "";
                }
                CommitInfo commitInfo = Item.GetCommit(Item.LastIndexCommitHash);
                string dateString = commitInfo.Date.ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
                string result = $"{dateString} {commitInfo.Hash} {commitInfo.Message}";
                return result;

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
