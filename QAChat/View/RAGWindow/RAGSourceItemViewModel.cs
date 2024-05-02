using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;

namespace QAChat.View.RAGWindow {
    public class RAGSourceItemViewModel : MyWindowViewModel {

        private RAGSourceItem item;
        public RAGSourceItemViewModel(RAGSourceItem item) {
            this.item = item;
        }
        public RAGSourceItem Item {
            get {
                return item;
            }
        }
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
                return [.. VectorDBItem.GetItems()];
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
