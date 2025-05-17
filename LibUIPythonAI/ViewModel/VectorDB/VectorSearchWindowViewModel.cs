using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using PythonAILib.Common;
using WpfAppCommon.Model;

namespace LibUIPythonAI.ViewModel.VectorDB {
    public class VectorSearchWindowViewModel : CommonViewModelBase {

        public VectorSearchWindowViewModel() {
            InputText = string.Empty;
        }

        // VectorSearchProperty
        private LibPythonAI.Model.VectorDB.VectorSearchProperty? _vectorSearchProperty;
        public LibPythonAI.Model.VectorDB.VectorSearchProperty? VectorSearchProperty {
            get => _vectorSearchProperty;
            set {
                if (value == null) {
                    return;
                }
                var item = VectorDBItem.GetItemByName(value.VectorDBItemName);
                if (item == null) {
                    return;
                }
                _vectorSearchProperty = value;

                // VectorDBSearchResultMax
                VectorDBSearchResultMax = item.DefaultSearchResultLimit;

                // IsUseMultiVectorRetrieverがfalseの場合は、SelectedTabIndexを1にする
                if (!item.IsUseMultiVectorRetriever) {
                    SelectedTabIndex = 1;
                } else {
                    SelectedTabIndex = 0;
                }

                // StatusTextを更新
                UpdateStatusText();

                OnPropertyChanged(nameof(VectorSearchProperty));
                OnPropertyChanged(nameof(MultiVectorRetrieverVisibility));
                OnPropertyChanged(nameof(VectorSearchResults));

            }
        }

        // InputText
        public string InputText { get; set; }

        // VectorDBSearchResultMax
        public int VectorDBSearchResultMax { get; set; }

        public ObservableCollection<VectorEmbedding> MultiVectorSearchResults { get; set; } = [];

        // SubDocsのVectorSearchResults
        public ObservableCollection<VectorEmbedding> VectorSearchResults { get; set; } = [];

        // ベクトルDBアイテムを選択したときのアクション
        public Action<List<LibPythonAI.Model.VectorDB.VectorSearchProperty>> SelectVectorDBItemAction { get; set; } = (items) => { };

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

                var item = VectorDBItem.GetItemByName(VectorSearchProperty?.VectorDBItemName); 
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
            MultiVectorSearchResults.Clear();
            OnPropertyChanged(nameof(MultiVectorSearchResults));
        });

        // SendCommand
        public SimpleDelegateCommand<object> SendCommand => new( (parameter) => {
            // VectorDBItemがnullの場合は何もしない
            if (VectorSearchProperty == null) {
                LogWrapper.Error("VectorDBItem is null.");
                return;
            }
            var vectorDBItem = VectorDBItem.GetItemByName(VectorSearchProperty.VectorDBItemName); 
            if (vectorDBItem == null) {
                LogWrapper.Error("VectorDBItem is null.");
                return;
            }

            CommonViewModelProperties.UpdateIndeterminate(true);
            Task.Run(async () => {
                List<VectorEmbedding> vectorSearchResults = [];
                // ベクトル検索を実行
                // VectorDBSearchResultMaxをVectorSearchPropertyに設定
                VectorSearchProperty.TopK = VectorDBSearchResultMax;

                try {
                    var searchResults = await VectorSearchProperty.VectorSearch(InputText);
                    vectorSearchResults.AddRange(searchResults);
                } finally {
                    CommonViewModelProperties.UpdateIndeterminate(false);
                }
                MainUITask.Run(() => {
                    // VectorSearchResultsを更新
                    MultiVectorSearchResults.Clear();
                    VectorSearchResults.Clear();

                    if (vectorDBItem.IsUseMultiVectorRetriever) {
                        foreach (VectorEmbedding vectorSearchResult in vectorSearchResults) {
                            MultiVectorSearchResults.Add(vectorSearchResult);
                            // sub_docsを追加
                            foreach (VectorEmbedding subDoc in vectorSearchResult.SubDocs) {
                                VectorSearchResults.Add(subDoc);
                            }
                        }
                    } else {
                        // VectorSearchResultsを更新
                        VectorSearchResults.Clear();
                        foreach (VectorEmbedding vectorSearchResult in vectorSearchResults) {
                            VectorSearchResults.Add(vectorSearchResult);
                        }
                    }

                    OnPropertyChanged(nameof(MultiVectorSearchResults));
                    OnPropertyChanged(nameof(VectorSearchResults));

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
                    OpenAIProperties = openAIProperties,
                    VectorSearchRequests = [VectorSearchProperty],
                    UseVectorDB = true,
                };
                RequestContainer requestContainer = new() {
                    RequestContextInstance = chatRequestContext,
                };

                string json = requestContainer.ToJson();
                // ChatRequestContextをJson文字列化したものと、検索文字列を結合
                return $"# ChatRequestContext:\n{json}\n\n# Search Text:\n{InputText}";
            }
        }

        // ベクトルDB検索画面の表示
        public SimpleDelegateCommand<object> SelectVectorDBItemCommand => new((parameter) => {
            // ベクトルDB検索画面を表示
            List<LibPythonAI.Model.VectorDB.VectorSearchProperty> items = [];
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
        private void UpdateStatusText() {
            // StatusTextを更新
            if (VectorSearchProperty == null) {
                return;
            }
            StatusText.Instance.ReadyText = $"{CommonStringResources.Instance.VectorDB}:[{VectorSearchProperty.DisplayText}]";
            StatusText.Instance.Text = $"{CommonStringResources.Instance.VectorDB}:[{VectorSearchProperty.DisplayText}]";
        }

        // Closed時の処理
        public SimpleDelegateCommand<object> ClosedCommand => new((parameter) => {
            // StatusTextを初期化
            StatusText.Instance.Init();
        });
    }
}
