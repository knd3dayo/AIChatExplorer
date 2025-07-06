using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.Chat;
using LibPythonAI.PythonIF.Request;
using LibUIPythonAI.Utils;
using System.Windows.Controls;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.VectorDB;

namespace LibUIPythonAI.ViewModel.Chat {
    public class ChatRequestContextViewModel : ObservableObject {

        public ChatRequestContextViewModel() {
            // コンストラクタ
        }

        public ChatSettings ChatSettings { get; set; } = new ();

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
                return (int)ChatSettings.RAGMode;
            }
            set {
                ChatSettings.RAGMode = (RAGModeEnum)value;
                OnPropertyChanged(nameof(RAGModeValue));
            }
        }

        // VectorDBSearchResultMax
        public int VectorDBSearchResultMax { get; set; } = 10;

        private ObservableCollection<LibPythonAI.Model.VectorDB.VectorSearchItem> _vectorSearchProperties = [];
        public ObservableCollection<LibPythonAI.Model.VectorDB.VectorSearchItem> VectorSearchProperties {
            get {
                return _vectorSearchProperties;
            }
            set {
                _vectorSearchProperties = value;
                OnPropertyChanged(nameof(VectorSearchProperties));
            }
        }

        // _vectorSearchPropertiesをChatRequestContext.VectorSearchRequestsに適用
        public ChatRequestContext  GetChatRequestContext() {
            ChatSettings.VectorSearchRequests.Clear();
            foreach (var item in VectorSearchProperties) {
                ChatSettings.VectorSearchRequests.Add(new VectorSearchRequest(item) {
                    TopK = VectorDBSearchResultMax
                });
            }
            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new(ChatSettings);
            return new ChatRequestContext(ChatSettings);
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

        private LibPythonAI.Model.VectorDB.VectorSearchItem? _selectedVectorSearchItem = null;
        public LibPythonAI.Model.VectorDB.VectorSearchItem? SelectedVectorSearchItem {
            get {
                return _selectedVectorSearchItem;
            }
            set {
                _selectedVectorSearchItem = value;
                OnPropertyChanged(nameof(SelectedVectorSearchItem));
            }
        }

        // ベクトルDBをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorSearchItem != null) {
                // VectorDBItemsから削除
                VectorSearchProperties.Remove(SelectedVectorSearchItem);
            }
            OnPropertyChanged(nameof(VectorSearchProperties));
        });

        // ベクトルDBを追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select,
                FolderViewModelManagerBase.FolderViewModels, (vectorDBItemBase) => {
                    VectorSearchProperties.Add(vectorDBItemBase);
                });

            OnPropertyChanged(nameof(VectorSearchProperties));
        });

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



        public Visibility VectorDBItemVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(ChatSettings.RAGMode != RAGModeEnum.None);

        public Visibility SplitMOdeVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(ChatSettings.SplitMode != SplitModeEnum.None);

    }
}
