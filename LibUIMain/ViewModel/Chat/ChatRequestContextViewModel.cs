using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibMain.Model.Chat;
using LibMain.PythonIF.Request;
using LibUIMain.Utils;

namespace LibUIMain.ViewModel.Chat {
    public class ChatRequestContextViewModel : ObservableObject {

        public ChatRequestContextViewModel() {
            // コンストラクタ
        }

        public ChatSettings ChatSettings { get; set; } = new();

        public VectorSearchSettings VectorSearchSettings { get; set; } = new();

        // SendRelatedItemsOnlyFirstRequest
        public int SendRelatedItemsOnlyFirstRequest {
            get {
                return ChatSettings.SendRelatedItemsOnlyFirstRequest ? 0 : 1;
            }
            set {
                ChatSettings.SendRelatedItemsOnlyFirstRequest = value == 0;
                OnPropertyChanged(nameof(SendRelatedItemsOnlyFirstRequest));
            }
        }

        // Temperature
        public double Temperature {
            get {
                return ChatSettings.Temperature;
            }
            set {
                ChatSettings.Temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }


        public int SplitMode {
            get {
                return (int)ChatSettings.SplitMode;
            }
            set {
                ChatSettings.SplitMode = (SplitModeEnum)value;
                OnPropertyChanged(nameof(SplitMode));
            }
        }

        public int SplitTokenCount {
            get {
                return ChatSettings.SplitTokenCount;
            }
            set {
                ChatSettings.SplitTokenCount = value;
                OnPropertyChanged(nameof(SplitTokenCount));
            }
        }

        // MaxImagesPerRequest
        public int MaxImagesPerRequest {
            get {
                return ChatSettings.MaxImagesPerRequest;
            }
            set {
                ChatSettings.MaxImagesPerRequest = value;
                OnPropertyChanged(nameof(MaxImagesPerRequest));
            }
        }

        public string PromptTemplateText {
            get {
                return ChatSettings.PromptTemplateText;
            }
            set {
                ChatSettings.PromptTemplateText = value;
                OnPropertyChanged(nameof(PromptTemplateText));
            }
        }

        public int RAGModeValue {
            get {
                return (int)VectorSearchSettings.RAGMode;
            }
            set {
                VectorSearchSettings.RAGMode = (RAGModeEnum)value;
                OnPropertyChanged(nameof(RAGModeValue));
            }
        }

        // VectorDBSearchResultMax
        public int VectorDBSearchResultMax { get; set; } = 10;

        private LibMain.Model.VectorDB.VectorSearchItem? _vectorSearchProperty;
        public LibMain.Model.VectorDB.VectorSearchItem? VectorSearchProperty {
            get {
                return _vectorSearchProperty;
            }
            set {
                _vectorSearchProperty = value;
                OnPropertyChanged(nameof(VectorSearchProperty));
            }
        }

        // _vectorSearchPropertiesをChatRequestContext.VectorSearchRequestsに適用
        public ChatRequestContext GetChatRequestContext() {

            // VectorSearchSettingsに設定
            if (VectorSearchProperty != null) {
                VectorSearchSettings.VectorSearchRequest = new VectorSearchRequest(VectorSearchProperty) {
                    TopK = VectorDBSearchResultMax
                };
            }
            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new(ChatSettings, VectorSearchSettings);
            return chatRequestContext;
        }

        // Splitモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> SplitModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            SplitMode = comboBox.SelectedIndex;
            // SplitMOdeVisibility
            OnPropertyChanged(nameof(SplitMOdeVisibility));
        });

        public static ChatMessage? SelectedItem { get; set; }

        private LibMain.Model.VectorDB.VectorSearchItem? _selectedVectorSearchItem = null;
        public LibMain.Model.VectorDB.VectorSearchItem? SelectedVectorSearchItem {
            get {
                return _selectedVectorSearchItem;
            }
            set {
                _selectedVectorSearchItem = value;
                OnPropertyChanged(nameof(SelectedVectorSearchItem));
            }
        }

        // RAGモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> RAGModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            RAGModeValue = comboBox.SelectedIndex;
            // VectorDBItemVisibility
            OnPropertyChanged(nameof(VectorDBItemVisibility));
        });

        // SendRelatedItemsOnlyFirstRequestModeが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> SendRelatedItemsOnlyFirstRequestModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            SendRelatedItemsOnlyFirstRequest = comboBox.SelectedIndex;
            OnPropertyChanged(nameof(SendRelatedItemsOnlyFirstRequest));
        });

        public Visibility VectorDBItemVisibility => LibUIMain.Utils.Tools.BoolToVisibility(VectorSearchSettings.RAGMode != RAGModeEnum.None);

        public Visibility SplitMOdeVisibility => LibUIMain.Utils.Tools.BoolToVisibility(ChatSettings.SplitMode != SplitModeEnum.None);

    }
}
