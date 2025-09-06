using System.Collections.ObjectModel;
using System.Windows;
using LibMain.Model.Chat;
using LibMain.Model.VectorDB;
using LibMain.PythonIF.Request;
using LibMain.Utils.Common;
using LibUIMain.Resource;
using LibUIMain.Utils;
using LibUIMain.ViewModel.Common;

namespace LibUIMain.ViewModel.VectorDB {
    public class VectorSearchWindowViewModel : CommonViewModelBase {

        public static readonly int MultiVectorRetrieverTabIndex = 0; // MultiVectorRetrieverのタブインデックス
        public static readonly int VectorRetrieverTabIndex = 1; // VectorRetrieverのタブインデックス

        public VectorSearchWindowViewModel(VectorSearchItem vectorSearchItem) {
            VectorSearchItem = vectorSearchItem;
        }

        // VectorSearchItem
        private LibMain.Model.VectorDB.VectorSearchItem? _VectorSearchItem;
        public LibMain.Model.VectorDB.VectorSearchItem? VectorSearchItem {
            get => _VectorSearchItem;
            set {
                UpdateVectorSearchItemAsync(value);
                UpdateMultiVectorRetrieverVisibilityAsync();
            }
        }
        private void UpdateVectorSearchItemAsync(VectorSearchItem? value) {
            var item = VectorDBItem.GetItemByName(value?.VectorDBItemName);
            if (item == null) {
                // VectorDBItemがnullの場合はエラーを表示
                LogWrapper.Error("VectorDBItem is null.");
                return;
            }
            _VectorSearchItem = value;

            // IsUseMultiVectorRetrieverがfalseの場合は、SelectedTabIndexを1にする
            if (item.IsUseMultiVectorRetriever) {
                SelectedTabIndex = MultiVectorRetrieverTabIndex;
            } else {
                SelectedTabIndex = VectorRetrieverTabIndex;
            }

            // StatusTextを更新
            UpdateStatusText();

            OnPropertyChanged(nameof(VectorSearchItem));
            OnPropertyChanged(nameof(MultiVectorRetrieverVisibility));
            OnPropertyChanged(nameof(VectorSearchResults));
        }

        public ObservableCollection<VectorEmbeddingItem> MultiVectorSearchResults { get; set; } = [];

        // SubDocsのVectorSearchResults
        public ObservableCollection<VectorEmbeddingItem> VectorSearchResults { get; set; } = [];

        // ベクトルDBアイテムを選択したときのアクション
        public Action<List<LibMain.Model.VectorDB.VectorSearchItem>> SelectVectorDBItemAction { get; set; } = (items) => { };

        // SelectedIndex
        private int _selectedTabIndex = MultiVectorRetrieverTabIndex;
        public int SelectedTabIndex {
            get => _selectedTabIndex;
            set {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
                OnPropertyChanged(nameof(PreviewJson));
            }
        }
        // MultiVectorRetrieverの場合のVisibility 
        public Visibility MultiVectorRetrieverVisibility { get; private set; } = Visibility.Collapsed;

        private void UpdateMultiVectorRetrieverVisibilityAsync() {
            // VectorDBItemを取得
            var item = VectorDBItem.GetItemByName(VectorSearchItem?.VectorDBItemName);
            if (item == null) {
                MultiVectorRetrieverVisibility = Visibility.Collapsed;
            } else {
                MultiVectorRetrieverVisibility = item.IsUseMultiVectorRetriever ? Visibility.Visible : Visibility.Collapsed;
            }
            OnPropertyChanged(nameof(MultiVectorRetrieverVisibility));
        }



        // クリアボタンのコマンド
        public SimpleDelegateCommand<object> ClearCommand => new((parameter) => {
            // VectorSearchResultsをクリア
            MultiVectorSearchResults.Clear();
            OnPropertyChanged(nameof(MultiVectorSearchResults));
        });

        // SendCommand
        public SimpleDelegateCommand<object> SendCommand => new(async (parameter) => {
            // VectorDBItemがnullの場合は何もしない
            if (VectorSearchItem == null) {
                LogWrapper.Error("VectorDBItem is null.");
                return;
            }

            CommonViewModelProperties.UpdateIndeterminate(true);
            var vectorDBItem = VectorDBItem.GetItemByName(VectorSearchItem.VectorDBItemName);
            if (vectorDBItem == null) {
                LogWrapper.Error("VectorDBItem is null.");
                return;
            }

            List<VectorEmbeddingItem> vectorSearchResults = [];
            // ベクトル検索を実行
            try {
                var searchResults = await VectorSearchItem.VectorSearchAsync();
                vectorSearchResults.AddRange(searchResults);
            } finally {
                CommonViewModelProperties.UpdateIndeterminate(false);
            }
            MainUITask.Run(() => {
                // VectorSearchResultsを更新
                MultiVectorSearchResults.Clear();
                VectorSearchResults.Clear();

                if (vectorDBItem.IsUseMultiVectorRetriever) {
                    foreach (VectorEmbeddingItem vectorSearchResult in vectorSearchResults) {
                        MultiVectorSearchResults.Add(vectorSearchResult);
                        // sub_docsを追加
                        foreach (VectorEmbeddingItem subDoc in vectorSearchResult.SubDocs) {
                            VectorSearchResults.Add(subDoc);
                        }
                    }
                } else {
                    // VectorSearchResultsを更新
                    VectorSearchResults.Clear();
                    foreach (VectorEmbeddingItem vectorSearchResult in vectorSearchResults) {
                        VectorSearchResults.Add(vectorSearchResult);
                    }
                }

                OnPropertyChanged(nameof(MultiVectorSearchResults));
                OnPropertyChanged(nameof(VectorSearchResults));

            });
        });

        // PreviewJson
        public string PreviewJson {
            get {
                if (VectorSearchItem == null) {
                    return "";
                }
                // ChatRequestContextを作成
                ChatSettings chatSettings = new() {
                    VectorSearchRequests = [new VectorSearchRequest(VectorSearchItem)],
                };
                ChatRequestContext chatRequestContext = new(chatSettings);

                RequestContainer requestContainer = new() {
                    RequestContextInstance = chatRequestContext,
                };

                string json = requestContainer.ToJson();
                // ChatRequestContextをJson文字列化したものと、検索文字列を結合
                return json;
            }
        }

        // ベクトルDB検索画面の表示
        public SimpleDelegateCommand<object> SelectVectorDBItemCommand => new((parameter) => {
            // ベクトルDB検索画面を表示
            List<LibMain.Model.VectorDB.VectorSearchItem> items = [];
            SelectVectorDBItemAction(items);
            // itemsが1つ以上ある場合は、VectorDBItemを設定
            if (items.Count > 0) {
                VectorSearchItem = items[0];
                // MultiVectorRetrieverの場合のVisibilityを更新
                OnPropertyChanged(nameof(MultiVectorRetrieverVisibility));

                // StatusTextを更新
                UpdateStatusText();
            }
        });

        // UpdateStatusText
        private void UpdateStatusText() {
            // StatusTextを更新
            if (VectorSearchItem == null) {
                return;
            }
            StatusText.Instance.ReadyText = $"{CommonStringResources.Instance.VectorDB}:[{VectorSearchItem.DisplayText}]";
            StatusText.Instance.Text = $"{CommonStringResources.Instance.VectorDB}:[{VectorSearchItem.DisplayText}]";
        }

        // Closed時の処理
        public SimpleDelegateCommand<object> ClosedCommand => new((parameter) => {
            // StatusTextを初期化
            StatusText.Instance.Init();
        });
    }
}
