using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Common;
using LibUIPythonAI.View.RAG;
using LibUIPythonAI.Utils;
using LibPythonAI.Utils.Common;
using LibPythonAI.Data;
using LibUIPythonAI.Resource;
using LibPythonAI.Model.VectorDB;

namespace LibUIPythonAI.ViewModel.RAG {
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class ListRAGSourceWindowViewModel : CommonViewModelBase {

        public ListRAGSourceWindowViewModel() {
            // RagSourceItemのリストを初期化
            RagSourceItems.Clear();
            using PythonAILibDBContext db = new();
            foreach (var itemEntity in db.RAGSourceItems) {
                RAGSourceItem item = new(itemEntity);
                RagSourceItems.Add(new RAGSourceItemViewModel(item));
            }
            OnPropertyChanged(nameof(RagSourceItems));
        }

        // RagSourceItemのリスト
        public ObservableCollection<RAGSourceItemViewModel> RagSourceItems { get; set; } = [];

        // 選択中のRagSourceItem
        private RAGSourceItemViewModel? selectedRagSourceItem;
        public RAGSourceItemViewModel? SelectedRagSourceItem {
            get {
                return selectedRagSourceItem;
            }
            set {
                selectedRagSourceItem = value;
                OnPropertyChanged(nameof(SelectedRagSourceItem));
            }
        }

        // RAG Sourceの追加
        public SimpleDelegateCommand<object> AddRagSourceCommand => new((parameter) => {
            // SelectRAGSourceItemを設定
            SelectedRagSourceItem = new RAGSourceItemViewModel(new RAGSourceItem(new RAGSourceItemEntity()));

            // RAG Sourceの編集Windowを開く
            EditRAGSourceWindow.OpenEditRAGSourceWindow(SelectedRagSourceItem, (afterUpdate) => {
                // リストを更新
                RagSourceItems.Clear();
                using PythonAILibDBContext db = new();
                var items = db.RAGSourceItems;
                foreach (var itemEntity in items) {
                    RAGSourceItem item = new(itemEntity);
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
                }
                OnPropertyChanged(nameof(RagSourceItems));
            });

        });
        // RAG Sourceの編集
        public SimpleDelegateCommand<object> EditRagSourceCommand => new((parameter) => {
            if (SelectedRagSourceItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.SelectRAGSourceToEdit);
                return;
            }
            // RAG Sourceの編集Windowを開く
            EditRAGSourceWindow.OpenEditRAGSourceWindow(SelectedRagSourceItem, (afterUpdate) => {

                // リストを更新
                RagSourceItems.Clear();
                using PythonAILibDBContext db = new();
                var items = db.RAGSourceItems;
                foreach (var itemEntity in items) {
                    RAGSourceItem item = new(itemEntity);
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
                }
                OnPropertyChanged(nameof(RagSourceItems));
            });

        });
        // DeleteRAGSourceCommand
        public SimpleDelegateCommand<object> DeleteRAGSourceCommand => new((parameter) => {
            if (SelectedRagSourceItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.SelectRAGSourceToDelete);
                return;
            }
            // 確認ダイアログを表示
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteSelectedRAGSource, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {

                // 削除
                SelectedRagSourceItem.Item.Delete();
                // リストを更新
                RagSourceItems.Clear();
                using PythonAILibDBContext db = new();
                var items = db.RAGSourceItems;
                foreach (var itemEntity in items) {
                    RAGSourceItem item = new(itemEntity);
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
                }
            }
        });
    }
}
