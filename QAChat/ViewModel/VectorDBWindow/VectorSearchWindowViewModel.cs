using System.Collections.ObjectModel;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
using QAChat.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.VectorDBWindow {
    public class VectorSearchWindowViewModel : QAChatViewModelBase {


        public VectorSearchWindowViewModel() {
            InputText = string.Empty;
        }
        // VectorDBItem
        public VectorDBItem? VectorDBItem { get; set; }
        // InputText
        public string InputText { get; set; }
        public ObservableCollection<VectorSearchResult> VectorSearchResults { get; set; } = [];

        // ベクトルDBアイテムを選択したときのアクション
        public Action<List<VectorDBItem>> SelectVectorDBItemAction { get; set; } = (items) => { };

        // ProgressIndicatorの表示
        private bool _isIndeterminate = false;
        public bool IsIndeterminate {
            get => _isIndeterminate;
            set {
                _isIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }

        // クリアボタンのコマンド
        public SimpleDelegateCommand<object> ClearCommand => new((parameter) => {
            // VectorSearchResultsをクリア
            VectorSearchResults.Clear();
            OnPropertyChanged(nameof(VectorSearchResults));
        });

        // SendCommand
        public SimpleDelegateCommand<object> SendCommand => new(async (parameter) => {
            // VectorDBItemがnullの場合は何もしない
            if (VectorDBItem == null) {
                LogWrapper.Error("VectorDBItem is null.");
                return;
            }
            VectorSearchResults.Clear();
            IsIndeterminate = true;
            await Task.Run(() => {
                List<VectorSearchResult> vectorSearchResults = [];
                // ベクトル検索を実行
                ContentItem contentItem = new();
                contentItem.Content = InputText;
                try {
                    vectorSearchResults.AddRange(contentItem.VectorSearchCommandExecute(VectorDBItem, false));
                } finally {
                    IsIndeterminate = false;
                }
                MainUITask.Run(() => {
                    // VectorSearchResultsを更新
                    VectorSearchResults.Clear();
                    foreach (VectorSearchResult vectorSearchResult in vectorSearchResults) {
                        VectorSearchResults.Add(vectorSearchResult);
                    }
                    OnPropertyChanged(nameof(VectorSearchResults));
                });
            });
        });

        // ベクトルDB検索画面の表示
        public SimpleDelegateCommand<object> SelectVectorDBItemCommand => new((parameter) => {
            // ベクトルDB検索画面を表示
            List<VectorDBItem> items = [];
            SelectVectorDBItemAction(items);
            // itemsが1つ以上ある場合は、VectorDBItemを設定
            if (items.Count > 0) {
                VectorDBItem = items[0];
            }
        });
    }
}
