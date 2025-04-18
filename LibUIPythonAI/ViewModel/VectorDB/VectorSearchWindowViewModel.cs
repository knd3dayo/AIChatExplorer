using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;
using WpfAppCommon.Model;

namespace LibUIPythonAI.ViewModel.VectorDB {
    public class VectorSearchWindowViewModel : ChatViewModelBase {

        public VectorSearchWindowViewModel() {
            InputText = string.Empty;
        }

        // VectorSearchProperty
        private VectorDBProperty? _vectorSearchProperty;
        public VectorDBProperty? VectorSearchProperty {
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
                OnPropertyChanged(nameof(VectorSearchResults));

            }
        }

        // InputText
        public string InputText { get; set; }

        // VectorDBSearchResultMax
        public int VectorDBSearchResultMax { get; set; }

        public ObservableCollection<VectorDBEmbedding> MultiVectorSearchResults { get; set; } = [];

        // SubDocsのVectorSearchResults
        public ObservableCollection<VectorDBEmbedding> VectorSearchResults { get; set; } = [];

        // ベクトルDBアイテムを選択したときのアクション
        public Action<List<VectorDBProperty>> SelectVectorDBItemAction { get; set; } = (items) => { };

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
            MultiVectorSearchResults.Clear();
            OnPropertyChanged(nameof(MultiVectorSearchResults));
        });

        // SendCommand
        public SimpleDelegateCommand<object> SendCommand => new(async (parameter) => {
            // VectorDBItemがnullの場合は何もしない
            if (VectorSearchProperty == null) {
                LogWrapper.Error("VectorDBItem is null.");
                return;
            }
            var vectorDBItem = VectorSearchProperty.GetVectorDBItem();
            if (vectorDBItem == null) {
                LogWrapper.Error("VectorDBItem is null.");
                return;
            }

            UpdateIndeterminate(true);
            await Task.Run(() => {
                List<VectorDBEmbedding> vectorSearchResults = [];
                // ベクトル検索を実行
                // VectorDBSearchResultMaxをVectorSearchPropertyに設定
                VectorSearchProperty.TopK = VectorDBSearchResultMax;

                try {
                    vectorSearchResults.AddRange(VectorSearchProperty.VectorSearch(InputText));
                } finally {
                    UpdateIndeterminate(false);
                }
                MainUITask.Run(() => {
                    // VectorSearchResultsを更新
                    MultiVectorSearchResults.Clear();
                    VectorSearchResults.Clear();

                    if (vectorDBItem.IsUseMultiVectorRetriever) {
                        foreach (VectorDBEmbedding vectorSearchResult in vectorSearchResults) {
                            MultiVectorSearchResults.Add(vectorSearchResult);
                            // sub_docsを追加
                            foreach (VectorDBEmbedding subDoc in vectorSearchResult.SubDocs) {
                                VectorSearchResults.Add(subDoc);
                            }
                        }
                    } else {
                        // VectorSearchResultsを更新
                        VectorSearchResults.Clear();
                        foreach (VectorDBEmbedding vectorSearchResult in vectorSearchResults) {
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
                    VectorDBProperties = [VectorSearchProperty],
                    OpenAIProperties = openAIProperties,
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
            List<VectorDBProperty> items = [];
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
            StatusText.Instance.ReadyText = $"{StringResources.VectorDB}:[{VectorSearchProperty.DisplayText}]";
            StatusText.Instance.Text = $"{StringResources.VectorDB}:[{VectorSearchProperty.DisplayText}]";
        }

        // Closed時の処理
        public SimpleDelegateCommand<object> ClosedCommand => new((parameter) => {
            // StatusTextを初期化
            StatusText.Instance.Init();
        });
    }
}
