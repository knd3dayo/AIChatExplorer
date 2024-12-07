using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
using QAChat.Model;
using QAChat.ViewModel.Common;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.VectorDB
{
    public class VectorSearchWindowViewModel : QAChatViewModelBase {


        public VectorSearchWindowViewModel() {
            InputText = string.Empty;

            StatusBar = new MyStatusBarViewModel {
                StatusText = new StatusText()
            };
            // StatusTextをLogWrapperに登録
            LogWrapper.TemporaryStatusText.Add(StatusBar.StatusText);

        }
        // VectorDBItem
        private VectorDBItem? _vectorDBItem;
        public VectorDBItem? VectorDBItem {
            get => _vectorDBItem;
            set {
                _vectorDBItem = value;
                // マルチベクターリトリーバーの場合はSelectedIndex=1
                if (VectorDBItem != null && VectorDBItem.IsUseMultiVectorRetriever) {
                    SelectedIndex = 1;
                } else {
                    SelectedIndex = 0;
                }

                OnPropertyChanged(nameof(VectorDBItem));
                OnPropertyChanged(nameof(MultiVectorRetrieverVisibility));
                OnPropertyChanged(nameof(NormalVectorRetrieverVisibility));
                OnPropertyChanged(nameof(SubDocsVectorSearchResults));
            }
        }

        // InputText
        public string InputText { get; set; }
        public ObservableCollection<VectorSearchResult> VectorSearchResults { get; set; } = [];

        // SubDocsのVectorSearchResults
        public ObservableCollection<VectorSearchResult> SubDocsVectorSearchResults { get; set; } = [];

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
        public MyStatusBarViewModel StatusBar { get; set; }

        // SelectedIndex
        private int _selectedIndex = 0;
        public int SelectedIndex {
            get => _selectedIndex;
            set {
                _selectedIndex = value;
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        // MultiVectorRetrieverの場合のVisibility 
        public Visibility MultiVectorRetrieverVisibility {
            get {
                if (VectorDBItem == null) {
                    return Visibility.Collapsed;
                }
                if (VectorDBItem.IsUseMultiVectorRetriever) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        // 通常のVectorRetrieverの場合のVisibility
        public Visibility NormalVectorRetrieverVisibility {
            get {
                if (VectorDBItem == null) {
                    return Visibility.Visible;
                }
                if (VectorDBItem.IsUseMultiVectorRetriever) {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
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
                ContentItem contentItem = new() {
                    Content = InputText
                };
                try {
                    vectorSearchResults.AddRange(contentItem.VectorSearch([VectorDBItem]));
                } finally {
                    IsIndeterminate = false;
                }
                MainUITask.Run(() => {
                    // VectorSearchResultsを更新
                    VectorSearchResults.Clear();
                    foreach (VectorSearchResult vectorSearchResult in vectorSearchResults) {
                        VectorSearchResults.Add(vectorSearchResult);
                        // sub_docsを追加
                        foreach (VectorSearchResult subDoc in vectorSearchResult.SubDocs) {
                            SubDocsVectorSearchResults.Add(subDoc);
                        }
                    }
                    OnPropertyChanged(nameof(VectorSearchResults));
                    OnPropertyChanged(nameof(SubDocsVectorSearchResults));

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
                // MultiVectorRetrieverの場合のVisibilityを更新
                OnPropertyChanged(nameof(MultiVectorRetrieverVisibility));

                // StatusTextを更新
                if (VectorDBItem.CollectionName != null) {
                    StatusBar.StatusText.ReadyText = $"{StringResources.VectorDB}:[{VectorDBItem.Name}]:[{VectorDBItem.CollectionName}]";
                    StatusBar.StatusText.Text = $"{StringResources.VectorDB}:[{VectorDBItem.Name}]:[{VectorDBItem.CollectionName}]";
                } else{
                    StatusBar.StatusText.ReadyText = $"{StringResources.VectorDB}:[{VectorDBItem.Name}]";
                    StatusBar.StatusText.Text = $"{StringResources.VectorDB}:[{VectorDBItem.Name}]";
                }
            }
        });

        // Closed時の処理
        public SimpleDelegateCommand<object> ClosedCommand => new((parameter) => {
            // LogWrapperのTemporaryStatusTextから削除
            LogWrapper.TemporaryStatusText.Remove(StatusBar.StatusText);

        });


    }
}
