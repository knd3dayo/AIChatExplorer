using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Common;
using LibPythonAI.Model.AutoGen;
using LibPythonAI.Model.Chat;
using LibPythonAI.PythonIF.Request;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Chat;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.VectorDB;

namespace LibUIAutoGenChat.ViewModel.Chat {
    public class AutoGenChatContextViewModel : ObservableObject {

        private QAChatStartupProps QAChatStartupPropsInstance { get; set; }

        public AutoGenChatContextViewModel(QAChatStartupProps qaChatStartupProps) {
            QAChatStartupPropsInstance = qaChatStartupProps;
            // VectorSearchRequests = [.. qaChatStartupProps.ContentItem.GetFolder().GetVectorSearchProperties()];
            // AutoGenPropertiesを設定
            _autoGenProperties = new();
            _autoGenProperties.AutoGenDBPath = PythonAILibManager.Instance.ConfigParams.GetMainDBPath();
            _autoGenProperties.WorkDir = PythonAILibManager.Instance.ConfigParams.GetAutoGenWorkDir();
            _autoGenProperties.VenvPath = PythonAILibManager.Instance.ConfigParams.GetPathToVirtualEnv();

            // AutoGenGroupChatを設定
            Task.Run(async () => {
                var autogenGroupChatList = await AutoGenGroupChat.GetAutoGenChatListAsync();
                MainUITask.Run(() => {
                    AutoGenGroupChatList = [.. autogenGroupChatList];
                    if (AutoGenGroupChatList.Count > 0) {
                        SelectedAutoGenGroupChat = AutoGenGroupChatList[0];
                    }
                });
            });
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

        private OpenAIExecutionModeEnum _chatMode = OpenAIExecutionModeEnum.AutoGenGroupChat;
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
        // RAGModeValue
        private RAGModeEnum _ragMode = RAGModeEnum.None;
        public int RAGMode {
            get {
                return (int)_ragMode;
            }
            set {
                _ragMode = (RAGModeEnum)value;

                OnPropertyChanged(nameof(RAGMode));
                InitVectorDBProperties();

            }
        }


        public Visibility VectorDBItemVisibility => Tools.BoolToVisibility(_ragMode != RAGModeEnum.None);

        public Visibility SplitMOdeVisibility => Tools.BoolToVisibility(_splitMode != SplitModeEnum.None);

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
        public Visibility UseFolderVectorSearchItemVisibility => Tools.BoolToVisibility(UseFolderVectorSearchItem);

        public Visibility UseItemVectorSearchItemVisibility => Tools.BoolToVisibility(UseFolderVectorSearchItem == false);



        private async Task InitVectorDBProperties() {
            VectorSearchProperties.Clear();
            if (_ragMode != RAGModeEnum.None) {
                ObservableCollection<LibPythonAI.Model.VectorDB.VectorSearchItem> items = [];
                // QAChatStartupPropsInstance.ContentItem.UseFolderVectorSearchItem == Trueの場合
                if (UseFolderVectorSearchItem) {
                    // フォルダのベクトルDBを取得
                    items = await QAChatStartupPropsInstance.ContentItem.GetFolder().GetVectorSearchProperties();
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


        #region AutoGen Group Chat
        // AutoGen関連のVisibility
        public Visibility AutoGenGroupChatVisibility => Tools.BoolToVisibility(_chatMode == OpenAIExecutionModeEnum.AutoGenGroupChat);

        // AutoGenGroupChatList
        private ObservableCollection<AutoGenGroupChat> _AutoGenGroupChatList = [];
        public ObservableCollection<AutoGenGroupChat> AutoGenGroupChatList {
            get {
                return _AutoGenGroupChatList;
            }
            set {
                _AutoGenGroupChatList = value;
                OnPropertyChanged(nameof(AutoGenGroupChatList));
            }
        }
        // SelectedAutoGenGroupChat
        private AutoGenGroupChat? _SelectedAutoGenGroupChat = null;
        public AutoGenGroupChat? SelectedAutoGenGroupChat {
            get {
                return _SelectedAutoGenGroupChat;
            }
            set {
                _SelectedAutoGenGroupChat = value;
                if (_SelectedAutoGenGroupChat != null) {
                    AutoGenProperties.ChatType = AutoGenProperties.CHAT_TYPE_GROUP;
                    AutoGenProperties.ChatName = _SelectedAutoGenGroupChat.Name;
                }
                OnPropertyChanged(nameof(SelectedAutoGenGroupChat));
            }
        }
        // AutoGenGroupChatSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenGroupChatSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is ComboBox comboBox) {
                // 選択されたComboBoxItemのIndexを取得
                int index = comboBox.SelectedIndex;
                SelectedAutoGenGroupChat = AutoGenGroupChatList[index];
            }
        });

        // terminate_msg
        public string TerminateMsg {
            get {
                return AutoGenProperties.TerminateMsg;
            }
            set {
                AutoGenProperties.TerminateMsg = value;
                OnPropertyChanged(nameof(TerminateMsg));
            }
        }
        // max_msg
        public int MaxMsg {
            get {
                return AutoGenProperties.MaxMsg;
            }
            set {
                AutoGenProperties.MaxMsg = value;
                OnPropertyChanged(nameof(MaxMsg));
            }
        }
        // timeout
        public int Timeout {
            get {
                return AutoGenProperties.Timeout;
            }
            set {
                AutoGenProperties.Timeout = value;
                OnPropertyChanged(nameof(Timeout));
            }
        }

        #endregion




        // Splitモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> SplitModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            SplitMode = comboBox.SelectedIndex;
            // SplitMOdeVisibility
            OnPropertyChanged(nameof(SplitMOdeVisibility));

        });
        // RAGModeが変更されたときの処理

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

        public ChatRequestContext CreateChatRequestContext(string promptText, string sessionToken) {
            // ベクトルDB検索結果最大値をVectorSearchItemに設定
            foreach (var item in VectorSearchProperties) {
                item.TopK = VectorDBSearchResultMax;
            }
            // AutoGenPropertiesにSessionTokenを設定
            AutoGenProperties.SessionToken = sessionToken;

            int splitTokenCount = int.Parse(SplitTokenCount);
            ChatRequestContext chatRequestContext = ChatRequestContext.CreateDefaultChatRequestContext(
                _chatMode, _splitMode, splitTokenCount, _ragMode, [.. VectorSearchProperties], AutoGenProperties, promptText
                );
            return chatRequestContext;
        }


    }
}
