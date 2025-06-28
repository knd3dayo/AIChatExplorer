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

        private ChatRequestContext ChatRequestContext { get; set; } = new ChatRequestContext();

        public int SplitMode {
            get {
                return (int)ChatRequestContext.SplitMode;
            }
            set {
                ChatRequestContext.SplitMode = (SplitModeEnum)value;
                OnPropertyChanged(nameof(SplitMode));
            }
        }

        public int SplitTokenCount {
            get {
                return ChatRequestContext.SplitTokenCount;
            }
            set {
                ChatRequestContext.SplitTokenCount = value;
                OnPropertyChanged(nameof(SplitTokenCount));
            }
        }
        public string PromptTemplateText {
            get {
                return ChatRequestContext.PromptTemplateText;
            }
            set {
                ChatRequestContext.PromptTemplateText = value;
                OnPropertyChanged(nameof(PromptTemplateText));
            }
        }

        public int RAGModeValue {
            get {
                return (int)ChatRequestContext.RAGMode;
            }
            set {
                ChatRequestContext.RAGMode = (RAGModeEnum)value;
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
            ChatRequestContext.VectorSearchRequests.Clear();
            foreach (var item in VectorSearchProperties) {
                ChatRequestContext.VectorSearchRequests.Add(new VectorSearchRequest(item) {
                    TopK = VectorDBSearchResultMax
                });
            }
            return ChatRequestContext;
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




        public Visibility VectorDBItemVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(ChatRequestContext.RAGMode != RAGModeEnum.None);

        public Visibility SplitMOdeVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(ChatRequestContext.SplitMode != SplitModeEnum.None);

    }
}
