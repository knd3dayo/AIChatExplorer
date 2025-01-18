using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
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
                if (value == null) {
                    return;
                }
                var item = value.GetVectorDBItem();
                if (item == null) {
                    return;
                }
                _vectorSearchProperty = value;

                // VectorDBSearchResultMax
                VectorDBSearchResultMax = item.DefaultSearchResultLimit;
                // StatusTextを更新
                UpdateStatusText();

                OnPropertyChanged(nameof(VectorSearchProperty));
                OnPropertyChanged(nameof(MultiVectorRetrieverVisibility));
                OnPropertyChanged(nameof(SubDocsVectorSearchResults));

            }
        }

        // InputText
        public string InputText { get; set; }

        // VectorDBSearchResultMax
        public int VectorDBSearchResultMax { get; set; }

        public ObservableCollection<VectorDBEntry> VectorSearchResults { get; set; } = [];

        // SubDocsのVectorSearchResults
        public ObservableCollection<VectorDBEntry> SubDocsVectorSearchResults { get; set; } = [];

        // ベクトルDBアイテムを選択したときのアクション
        public Action<List<VectorSearchProperty>> SelectVectorDBItemAction { get; set; } = (items) => { };

        public MyStatusBarViewModel StatusBar { get; set; }

        // SelectedIndex
        private int _selectedTabIndex = 0;
        public int SelectedTabIndex {
            get => _selectedTabIndex;
            set {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
                OnPropertyChanged(nameof(PreviewJson));
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
                // VectorDBSearchResultMaxをVectorSearchPropertyに設定
                VectorSearchProperty.TopK = VectorDBSearchResultMax;

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

        // PreviewJson
        public string PreviewJson {
            get {
                if (VectorSearchProperty == null) {
                    return "";
                }
                // VectorDBSearchResultMaxをVectorSearchPropertyに設定
                VectorSearchProperty.TopK = VectorDBSearchResultMax;

                PythonAILibManager libManager = PythonAILibManager.Instance;
                OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
                // ChatRequestContextを作成
                ChatRequestContext chatRequestContext = new() {
                    VectorSearchProperties = [VectorSearchProperty],
                    OpenAIProperties = openAIProperties
                };

                string json = chatRequestContext.ToJson();
                // ChatRequestContextをJson文字列化したものと、検索文字列を結合
                return $"# ChatRequestContext:\n{json}\n\n# Search Text:\n{InputText}";
            }
        }

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
                UpdateStatusText();
            }
        });

        // UpdateStatusText
        private void UpdateStatusText () {
            // StatusTextを更新
            if (VectorSearchProperty == null) {
                return;
            }
            StatusBar.StatusText.ReadyText = $"{StringResources.VectorDB}:[{VectorSearchProperty.DisplayText}]";
            StatusBar.StatusText.Text = $"{StringResources.VectorDB}:[{VectorSearchProperty.DisplayText}]";
        }

        // Closed時の処理
        public SimpleDelegateCommand<object> ClosedCommand => new((parameter) => {
            // LogWrapperのTemporaryStatusTextから削除
            LogWrapper.TemporaryStatusText.Remove(StatusBar.StatusText);

        });


    }
}
