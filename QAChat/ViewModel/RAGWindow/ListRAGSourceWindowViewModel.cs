using System.Collections.ObjectModel;
using System.Windows;
using QAChat.View.RAGWindow;
using WpfAppCommon.Utils;
using QAChat.Model;

namespace QAChat.ViewModel.RAGWindow {
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class ListRAGSourceWindowViewModel : QAChatViewModelBase {

        public ListRAGSourceWindowViewModel() {
            // RagSourceItemのリストを初期化
            RagSourceItems.Clear();
            var items = PythonAILibManager.Instance?.DataFactory.GetRAGSourceItems();
            if (items != null) {
                foreach (var item in items) {
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
                }
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
            var item = PythonAILibManager.Instance?.DataFactory.CreateRAGSourceItem();
            if (item == null) {
                return;
            }
            SelectedRagSourceItem = new RAGSourceItemViewModel(item);

            // RAG Sourceの編集Windowを開く
            EditRAGSourceWindow.OpenEditRAGSourceWindow(SelectedRagSourceItem, (afterUpdate) => {
                // リストを更新
                RagSourceItems.Clear();
                var items = PythonAILibManager.Instance?.DataFactory.GetRAGSourceItems();
                if (items != null) {
                    foreach (var item in items) {
                        RagSourceItems.Add(new RAGSourceItemViewModel(item));
                    }
                }
                OnPropertyChanged(nameof(RagSourceItems));
            });

        });
        // RAG Sourceの編集
        public SimpleDelegateCommand<object> EditRagSourceCommand => new((parameter) => {
            if (SelectedRagSourceItem == null) {
                LogWrapper.Error(StringResources.SelectRAGSourceToEdit);
                return;
            }
            // RAG Sourceの編集Windowを開く
            EditRAGSourceWindow.OpenEditRAGSourceWindow(SelectedRagSourceItem, (afterUpdate) => {

                // リストを更新
                RagSourceItems.Clear();
                var items = PythonAILibManager.Instance?.DataFactory.GetRAGSourceItems();
                if (items != null) {
                    foreach (var item in items) {
                        RagSourceItems.Add(new RAGSourceItemViewModel(item));
                    }
                }
                OnPropertyChanged(nameof(RagSourceItems));
            });

        });
        // DeleteRAGSourceCommand
        public SimpleDelegateCommand<object> DeleteRAGSourceCommand => new((parameter) => {
            if (SelectedRagSourceItem == null) {
                LogWrapper.Error(StringResources.SelectRAGSourceToDelete);
                return;
            }
            // 確認ダイアログを表示
            MessageBoxResult result = MessageBox.Show(StringResources.ConfirmDeleteSelectedRAGSource, StringResources.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {

                // 削除
                SelectedRagSourceItem.Item.Delete();
                // リストを更新
                RagSourceItems.Clear();
                var items = PythonAILibManager.Instance?.DataFactory.GetRAGSourceItems();
                if (items != null) {
                    foreach (var item in items) {
                        RagSourceItems.Add(new RAGSourceItemViewModel(item));
                    }
                }
            }
        });
    }
}
