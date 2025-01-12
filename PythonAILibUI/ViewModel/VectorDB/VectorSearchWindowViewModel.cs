using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
using QAChat.Model;
using QAChat.ViewModel.Common;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.VectorDB {
    public class VectorSearchWindowViewModel : QAChatViewModelBase {

        public VectorSearchWindowViewModel() {
            InputText = string.Empty;

            StatusBar = new MyStatusBarViewModel {
                StatusText = new StatusText()
            };
            // StatusTextをLogWrapperに登録
            LogWrapper.TemporaryStatusText.Add(StatusBar.StatusText);

        }
        // VectorSearchProperty
        private VectorSearchProperty? _vectorSearchProperty;
        public VectorSearchProperty? VectorSearchProperty {
            get => _vectorSearchProperty;
            set {
                _vectorSearchProperty = value;
                // マルチベクターリトリーバーの場合はSelectedIndex=1
                var item = VectorSearchProperty?.GetVectorDBItem();
                if (item != null && item.IsUseMultiVectorRetriever) {
                    SelectedIndex = 1;
                } else {
                    SelectedIndex = 0;
                }

                OnPropertyChanged(nameof(VectorSearchProperty));
                OnPropertyChanged(nameof(MultiVectorRetrieverVisibility));
                OnPropertyChanged(nameof(NormalVectorRetrieverVisibility));
                OnPropertyChanged(nameof(SubDocsVectorSearchResults));
            }
        }

        // InputText
        public string InputText { get; set; }
        public ObservableCollection<VectorDBEntry> VectorSearchResults { get; set; } = [];

        // SubDocsのVectorSearchResults
        public ObservableCollection<VectorDBEntry> SubDocsVectorSearchResults { get; set; } = [];

        // ベクトルDBアイテムを選択したときのアクション
        public Action<List<VectorSearchProperty>> SelectVectorDBItemAction { get; set; } = (items) => { };

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
                var item = VectorSearchProperty?.GetVectorDBItem();
                if (item == null) {
                    return Visibility.Collapsed;
                }
                if (item.IsUseMultiVectorRetriever) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
        // 通常のVectorRetrieverの場合のVisibility
        public Visibility NormalVectorRetrieverVisibility {
            get {
                var item = VectorSearchProperty?.GetVectorDBItem();
                if (item == null) {
                    return Visibility.Visible;
                }
                if (item.IsUseMultiVectorRetriever) {
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
            if (VectorSearchProperty == null) {
                LogWrapper.Error("VectorDBItem is null.");
                return;
            }
            VectorSearchResults.Clear();
            IsIndeterminate = true;
            await Task.Run(() => {
                List<VectorDBEntry> vectorSearchResults = [];
                // ベクトル検索を実行
                ContentItem contentItem = new() {
                    Content = InputText
                };
                try {
                    vectorSearchResults.AddRange(contentItem.VectorSearch([VectorSearchProperty]));
                } finally {
                    IsIndeterminate = false;
                }
                MainUITask.Run(() => {
                    // VectorSearchResultsを更新
                    VectorSearchResults.Clear();
                    foreach (VectorDBEntry vectorSearchResult in vectorSearchResults) {
                        VectorSearchResults.Add(vectorSearchResult);
                        // sub_docsを追加
                        foreach (VectorDBEntry subDoc in vectorSearchResult.SubDocs) {
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
            List<VectorSearchProperty> items = [];
            SelectVectorDBItemAction(items);
            // itemsが1つ以上ある場合は、VectorDBItemを設定
            if (items.Count > 0) {
                VectorSearchProperty = items[0];
                // MultiVectorRetrieverの場合のVisibilityを更新
                OnPropertyChanged(nameof(MultiVectorRetrieverVisibility));

                // StatusTextを更新
                var item = VectorSearchProperty?.GetVectorDBItem();
                if (item != null) {
                    if (item.CollectionName != null) {
                        StatusBar.StatusText.ReadyText = $"{StringResources.VectorDB}:[{item.Name}]:[{item.CollectionName}]";
                        StatusBar.StatusText.Text = $"{StringResources.VectorDB}:[{item.Name}]:[{item.CollectionName}]";
                    } else {
                        StatusBar.StatusText.ReadyText = $"{StringResources.VectorDB}:[{item.Name}]";
                        StatusBar.StatusText.Text = $"{StringResources.VectorDB}:[{item.Name}]";
                    }
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
