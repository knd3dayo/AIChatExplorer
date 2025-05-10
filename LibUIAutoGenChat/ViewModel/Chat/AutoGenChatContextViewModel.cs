using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.AutoGen;
using LibPythonAI.PythonIF.Request;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Chat;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.VectorDB;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.Chat;

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
            SelectedAutoGenGroupChat = AutoGenGroupChat.GetAutoGenChatList().FirstOrDefault();
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

        private ObservableCollection<LibPythonAI.Model.VectorDB.VectorSearchProperty> _vectorSearchProperties = [];
        public ObservableCollection<LibPythonAI.Model.VectorDB.VectorSearchProperty> VectorSearchProperties {
            get {
                return _vectorSearchProperties;
            }
            set {
                _vectorSearchProperties = value;
                OnPropertyChanged(nameof(VectorSearchProperties));
            }
        }

        private LibPythonAI.Model.VectorDB.VectorSearchProperty? _selectedVectorSearchProperty = null;
        public LibPythonAI.Model.VectorDB.VectorSearchProperty? SelectedVectorSearchProperty {
            get {
                return _selectedVectorSearchProperty;
            }
            set {
                _selectedVectorSearchProperty = value;
                OnPropertyChanged(nameof(SelectedVectorSearchProperty));
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
                // _UserVectorDBがTrueの場合はVectorDBItemを取得
                VectorSearchProperties = [];
                if (_UseVectorDB) {
                    ObservableCollection<LibPythonAI.Model.VectorDB.VectorSearchProperty> items = QAChatStartupPropsInstance.ContentItem.GetFolder().GetVectorSearchProperties();
                    foreach (var item in items) {
                        VectorSearchProperties.Add(item);
                    }
                } else {
                    VectorSearchProperties.Clear();
                }

                OnPropertyChanged(nameof(UseVectorDB));
                OnPropertyChanged(nameof(VectorDBItemVisibility));
            }
        }
        //
        public Visibility VectorDBItemVisibility => Tools.BoolToVisibility(UseVectorDB);

        public Visibility SplitMOdeVisibility => Tools.BoolToVisibility(_splitMode != SplitOnTokenLimitExceedModeEnum.None);

        // UseFolderVectorSearchProperty
        // フォルダのベクトルDBを使用するか否か
        public bool UseFolderVectorSearchProperty {
            get {
                return QAChatStartupPropsInstance.ContentItem.UseFolderVectorSearchProperty;
            }
            set {
                QAChatStartupPropsInstance.ContentItem.UseFolderVectorSearchProperty = value;

                InitVectorDBProperties();
                OnPropertyChanged(nameof(UseFolderVectorSearchProperty));
                OnPropertyChanged(nameof(UseItemVectorSearchProperty));
                OnPropertyChanged(nameof(UseFolderVectorSearchPropertyVisibility));
                OnPropertyChanged(nameof(UseItemVectorSearchPropertyVisibility));
            }
        }
        // アイテムのベクトルDBを使用するか否か
        public bool UseItemVectorSearchProperty {
            get {
                return !UseFolderVectorSearchProperty;
            }
        }
        public Visibility UseFolderVectorSearchPropertyVisibility => Tools.BoolToVisibility(UseFolderVectorSearchProperty);

        public Visibility UseItemVectorSearchPropertyVisibility => Tools.BoolToVisibility(UseFolderVectorSearchProperty == false);



        private void InitVectorDBProperties() {
            VectorSearchProperties.Clear();
            if (UseVectorDB) {
                ObservableCollection<LibPythonAI.Model.VectorDB.VectorSearchProperty> items = [];
                // QAChatStartupPropsInstance.ContentItem.UseFolderVectorSearchProperty == Trueの場合
                if (UseFolderVectorSearchProperty) {
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


        #region AutoGen Group Chat
        // AutoGen関連のVisibility
        public Visibility AutoGenGroupChatVisibility => Tools.BoolToVisibility(_chatMode == OpenAIExecutionModeEnum.AutoGenGroupChat);

        // AutoGenGroupChatList
        public ObservableCollection<AutoGenGroupChat> AutoGenGroupChatList {
            get {
                ObservableCollection<AutoGenGroupChat> autoGenGroupChatList = [];
                foreach (var item in AutoGenGroupChat.GetAutoGenChatList()) {
                    autoGenGroupChatList.Add(item);
                }
                return autoGenGroupChatList;
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

        // ベクトルDBをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorSearchProperty != null) {
                // VectorDBItemsから削除
                VectorSearchProperties.Remove(SelectedVectorSearchProperty);
            }
            OnPropertyChanged(nameof(VectorSearchProperties));
        });

        // ベクトルDBを追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select,
                RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    VectorSearchProperties.Add(vectorDBItemBase);
                });

            OnPropertyChanged(nameof(VectorSearchProperties));
        });

        public ChatRequestContext CreateChatRequestContext(string promptText, string sessionToken) {
            // ベクトルDB検索結果最大値をVectorSearchPropertyに設定
            foreach (var item in VectorSearchProperties) {
                item.TopK = VectorDBSearchResultMax;
            }
            // AutoGenPropertiesにSessionTokenを設定
            AutoGenProperties.SessionToken = sessionToken;

            int splitTokenCount = int.Parse(SplitTokenCount);
            ChatRequestContext chatRequestContext = ChatRequestContext.CreateDefaultChatRequestContext(
                _chatMode, _splitMode, splitTokenCount, UseVectorDB, [.. VectorSearchProperties], AutoGenProperties, promptText
                );
            return chatRequestContext;
        }


    }
}
