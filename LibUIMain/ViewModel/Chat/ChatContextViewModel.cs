using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibMain.Model.Chat;
using LibMain.PythonIF.Request;
using LibUIMain.Utils;
using LibUIMain.View.VectorDB;
using LibUIMain.ViewModel.Folder;
using LibUIMain.ViewModel.VectorDB;

namespace LibUIMain.ViewModel.Chat {
    public class ChatContextViewModel : ObservableObject {

        private QAChatStartupPropsBase QAChatStartupPropsInstance { get; set; }

        public ChatContextViewModel(QAChatStartupPropsBase qaChatStartupProps) {
            QAChatStartupPropsInstance = qaChatStartupProps;

        }

        // Temperature
        private double _temperature = 0.5;
        public double Temperature {
            get {
                return _temperature;
            }
            set {
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }

        private OpenAIExecutionModeEnum _chatMode = OpenAIExecutionModeEnum.Normal;
        public int ChatMode {
            get {
                return (int)_chatMode;
            }
            set {
                _chatMode = (OpenAIExecutionModeEnum)value;
                OnPropertyChanged(nameof(ChatMode));
            }
        }
        private SplitModeEnum _splitMode = SplitModeEnum.None;
        public int SplitMode {
            get {
                return (int)_splitMode;
            }
            set {
                _splitMode = (SplitModeEnum)value;
                OnPropertyChanged(nameof(SplitMode));
            }
        }

        // SplitTokenCount
        private int _SplitTokenCount = 8000;
        public string SplitTokenCount {
            get {
                return _SplitTokenCount.ToString();
            }
            set {
                try {
                    int count = int.Parse(value);
                    _SplitTokenCount = count;
                } catch (Exception) {
                    return;
                }
                OnPropertyChanged(nameof(SplitTokenCount));
            }
        }

        // VectorDBSearchResultMax
        public int VectorDBSearchResultMax { get; set; } = 10;

        // ScoreThreshold
        public float ScoreThreshold { get; set; } = 0.5f;

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

        // UseFolderVectorSearchItem
        // フォルダのベクトルDBを使用するか否か
        public bool UseFolderVectorSearchItem {
            get {
                return QAChatStartupPropsInstance.GetContentItem().UseFolderVectorSearchItem;
            }
            set {
                QAChatStartupPropsInstance.GetContentItem().UseFolderVectorSearchItem = value;

                InitVectorDBProperties().Wait();
                OnPropertyChanged(nameof(UseFolderVectorSearchItem));
                OnPropertyChanged(nameof(UseItemVectorSearchItem));
                OnPropertyChanged(nameof(UseFolderVectorSearchItemVisibility));
                OnPropertyChanged(nameof(UseItemVectorSearchItemVisibility));
            }
        }
        // アイテムのベクトルDBを使用するか否か
        public bool UseItemVectorSearchItem {
            get {
                return !UseFolderVectorSearchItem;
            }
        }

        private async Task InitVectorDBProperties() {
            // フォルダのベクトルDBを取得
            var item = QAChatStartupPropsInstance.GetContentItem();
            var folder = await item.GetFolderAsync();
            VectorSearchProperty = await folder.GetMainVectorSearchItem();
        }

        // RAGModeValue
        private RAGModeEnum _ragMode = RAGModeEnum.None;
        public int RAGMode {
            get {
                return (int)_ragMode;
            }
            set {
                _ragMode = (RAGModeEnum)value;

                InitVectorDBProperties().Wait();
                OnPropertyChanged(nameof(RAGMode));
                OnPropertyChanged(nameof(VectorDBItemVisibility));

            }
        }
        //
        public Visibility VectorDBItemVisibility => Tools.BoolToVisibility(_ragMode != RAGModeEnum.None);

        public Visibility UseFolderVectorSearchItemVisibility => Tools.BoolToVisibility(UseFolderVectorSearchItem);

        public Visibility UseItemVectorSearchItemVisibility => Tools.BoolToVisibility(UseFolderVectorSearchItem == false);


        public Visibility SplitMOdeVisibility => Tools.BoolToVisibility(_splitMode != SplitModeEnum.None);


        // Splitモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> SplitModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            SplitMode = comboBox.SelectedIndex;
            // SplitMOdeVisibility
            OnPropertyChanged(nameof(SplitMOdeVisibility));

        });
        // RAGモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> RAGModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            RAGMode = comboBox.SelectedIndex;
            // VectorDBItemVisibility
            OnPropertyChanged(nameof(VectorDBItemVisibility));
        });

        public ChatRequestContext CreateChatRequestContext(string PromptText, string sessionToken) {
            int splitTokenCount = int.Parse(SplitTokenCount);
            ChatRequestContext chatRequestContext = ChatRequestContext.CreateDefaultChatRequestContext(
                _chatMode, _splitMode, splitTokenCount,
                (RAGModeEnum)RAGMode, VectorSearchProperty , PromptText);
            return chatRequestContext;
        }


    }
}
