using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Common;
using PythonAILib.Model.VectorDB;
using QAChat.Model;
using QAChat.View.RAG;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.RAG {
    /// <summary>
    /// RAGのドキュメントソースとなるGitリポジトリ、作業ディレクトリを管理するためのウィンドウのViewModel
    /// </summary>
    public class ListRAGSourceWindowViewModel : QAChatViewModelBase {

        public ListRAGSourceWindowViewModel() {
            // RagSourceItemのリストを初期化
            RagSourceItems.Clear();
            var collection = PythonAILibManager.Instance.DataFactory.GetRAGSourceCollection<RAGSourceItem>();
            foreach (var item in collection.FindAll()) {
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
            SelectedRagSourceItem = new RAGSourceItemViewModel(new RAGSourceItem());

            // RAG Sourceの編集Windowを開く
            EditRAGSourceWindow.OpenEditRAGSourceWindow(SelectedRagSourceItem, (afterUpdate) => {
                // リストを更新
                RagSourceItems.Clear();
                var collection = PythonAILibManager.Instance.DataFactory.GetRAGSourceCollection<RAGSourceItem>();
                foreach (var item in collection.FindAll()) {
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
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
                var collection = PythonAILibManager.Instance.DataFactory.GetRAGSourceCollection<RAGSourceItem>();
                foreach (var item in collection.FindAll()) {
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
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
                var collection = PythonAILibManager.Instance.DataFactory.GetRAGSourceCollection<RAGSourceItem>();
                foreach (var item in collection.FindAll()) {
                    RagSourceItems.Add(new RAGSourceItemViewModel(item));
                }
            }
        });
    }
}
