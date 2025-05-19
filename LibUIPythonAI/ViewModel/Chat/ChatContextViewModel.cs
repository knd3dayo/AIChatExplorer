using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.VectorDB;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.Chat;

namespace LibUIPythonAI.ViewModel.Chat {
    public class ChatContextViewModel : ObservableObject {

        private QAChatStartupProps QAChatStartupPropsInstance { get; set; }

        public ChatContextViewModel(QAChatStartupProps qaChatStartupProps) {
            QAChatStartupPropsInstance = qaChatStartupProps;
            // VectorSearchRequests = [.. qaChatStartupProps.ContentItem.GetFolder().GetVectorSearchProperties()];
            // AutoGenPropertiesを設定
            _autoGenProperties = new();
            _autoGenProperties.AutoGenDBPath = PythonAILibManager.Instance.ConfigParams.GetMainDBPath();
            _autoGenProperties.WorkDir = PythonAILibManager.Instance.ConfigParams.GetAutoGenWorkDir();
            _autoGenProperties.VenvPath = PythonAILibManager.Instance.ConfigParams.GetPathToVirtualEnv();

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
        private SplitOnTokenLimitExceedModeEnum _splitMode = SplitOnTokenLimitExceedModeEnum.None;
        public int SplitMode {
            get {
                return (int)_splitMode;
            }
            set {
                _splitMode = (SplitOnTokenLimitExceedModeEnum)value;
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

        private AutoGenProperties _autoGenProperties;
        public AutoGenProperties AutoGenProperties {
            get {
                return _autoGenProperties;
            }
            set {
                _autoGenProperties = value;
                OnPropertyChanged(nameof(AutoGenProperties));
            }
        }


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

        // UseFolderVectorSearchItem
        // フォルダのベクトルDBを使用するか否か
        public bool UseFolderVectorSearchItem {
            get {
                return QAChatStartupPropsInstance.ContentItem.UseFolderVectorSearchItem;
            }
            set {
                QAChatStartupPropsInstance.ContentItem.UseFolderVectorSearchItem = value;

                InitVectorDBProperties();
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

        private void InitVectorDBProperties() {
            VectorSearchProperties.Clear();
            if (UseVectorDB) {
                ObservableCollection<LibPythonAI.Model.VectorDB.VectorSearchItem> items = [];
                // QAChatStartupPropsInstance.ContentItem.UseFolderVectorSearchItem == Trueの場合
                if (UseFolderVectorSearchItem) {
                    // フォルダのベクトルDBを取得
                    items = QAChatStartupPropsInstance.ContentItem.GetFolder().GetVectorSearchProperties();
                    foreach (var item in items) {
                        VectorSearchProperties.Add(item);
                    }
                } else {
                    // ContentItemのベクトルDBを取得
                    items = QAChatStartupPropsInstance.ContentItem.VectorDBProperties;
                    foreach (var item in items) {
                        VectorSearchProperties.Add(item);
                    }
                }
            }
        }
        // UseVectorDB
        private bool _UseVectorDB = false;
        public bool UseVectorDB {
            get {
                return _UseVectorDB;
            }
            set {
                _UseVectorDB = value;
                InitVectorDBProperties();

                OnPropertyChanged(nameof(UseVectorDB));
                OnPropertyChanged(nameof(VectorDBItemVisibility));

            }
        }
        //
        public Visibility VectorDBItemVisibility => Tools.BoolToVisibility(UseVectorDB);

        public Visibility UseFolderVectorSearchItemVisibility => Tools.BoolToVisibility(UseFolderVectorSearchItem);

        public Visibility UseItemVectorSearchItemVisibility => Tools.BoolToVisibility(UseFolderVectorSearchItem == false);


        public Visibility SplitMOdeVisibility => Tools.BoolToVisibility(_splitMode != SplitOnTokenLimitExceedModeEnum.None);


        // Splitモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> SplitModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            SplitMode = comboBox.SelectedIndex;
            // SplitMOdeVisibility
            OnPropertyChanged(nameof(SplitMOdeVisibility));

        });

        // ベクトルDBをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorSearchItem == null) {
                return;
            }
            // VectorDBItemsから削除
            VectorSearchProperties.Remove(SelectedVectorSearchItem);
            // UseFolderVectorSearchItemがFalseの場合、ContentItemからも削除
            if (UseFolderVectorSearchItem == false) {
                QAChatStartupPropsInstance.ContentItem.VectorDBProperties.Remove(SelectedVectorSearchItem);
            }
            OnPropertyChanged(nameof(VectorSearchProperties));
        });

        // ベクトルDBを追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select,
                RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    VectorSearchProperties.Add(vectorDBItemBase);
                    // UseFolderVectorSearchItemがFalseの場合、ContentItemに追加
                    if (UseFolderVectorSearchItem == false) {
                        QAChatStartupPropsInstance.ContentItem.VectorDBProperties.Add(vectorDBItemBase);
                    }
                });

            OnPropertyChanged(nameof(VectorSearchProperties));
        });

        public ChatRequestContext CreateChatRequestContext(string PromptText, string sessionToken) {
            // ベクトルDB検索結果最大値をVectorSearchItemに設定
            foreach (var item in VectorSearchProperties) {
                item.TopK = VectorDBSearchResultMax;
            }
            int splitTokenCount = int.Parse(SplitTokenCount);
            ChatRequestContext chatRequestContext = ChatRequestContext.CreateDefaultChatRequestContext(
                _chatMode, _splitMode, splitTokenCount, UseVectorDB, [.. VectorSearchProperties], AutoGenProperties, PromptText
                );
            return chatRequestContext;
        }


    }
}
