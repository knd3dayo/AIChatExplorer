using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
using PythonAILib.Utils.Python;
using QAChat.Model;
using QAChat.View.PromptTemplate;
using QAChat.View.QAChatMain;
using QAChat.View.VectorDB;
using QAChat.ViewModel;
using QAChat.ViewModel.PromptTemplate;
using QAChat.ViewModel.VectorDB;
using WpfAppCommon.Utils;

namespace MergeChat.ViewModel {
    public class MergeChatControlViewModel : QAChatViewModelBase {


        //初期化
        public MergeChatControlViewModel(QAChatStartupProps props) {

            QAChatStartupProps = props;
            // VectorDBItemsを設定 ClipboardFolderのベクトルDBを取得
            VectorSearchProperties = [.. props.ContentItem.GetFolder<ContentFolder>().GetVectorSearchProperties()];

            // InputTextを設定
            InputText = QAChatStartupProps.ContentItem?.Content ?? "";
            // ClipboardItemがある場合は、ChatItemsを設定
            if (QAChatStartupProps.ContentItem != null) {
                ChatHistory = [.. QAChatStartupProps.ContentItem.ChatItems];
            }
            // AutoGenPropertiesを設定
            _autoGenProperties = new();
            _autoGenProperties.AutoGenDBPath = PythonAILibManager.Instance.ConfigParams.GetAutoGenDBPath();
            _autoGenProperties.WorkDir = PythonAILibManager.Instance.ConfigParams.GetAutoGenWorkDir();
            _autoGenProperties.VenvPath = PythonAILibManager.Instance.ConfigParams.GetPathToVirtualEnv();

            // AutoGenGroupChatを設定
            SelectedAutoGenGroupChat = AutoGenGroupChat.GetAutoGenChatList().FirstOrDefault();

        }

        public QAChatStartupProps QAChatStartupProps { get; set; }

        public ChatRequest ChatRequest { get; set; } = new();

        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        }

        // Temperature
        public double Temperature {
            get {
                return ChatRequest.Temperature;
            }
            set {
                ChatRequest.Temperature = value;
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
                    int count = Int32.Parse(value);
                    _SplitTokenCount = count;
                } catch (Exception) {
                    return;
                }
                OnPropertyChanged(nameof(SplitTokenCount));
            }
        }

        // VectorDBSearchResultMax
        public int VectorDBSearchResultMax { get; set; } = 10;

        public static ChatMessage? SelectedItem { get; set; }

        public ObservableCollection<ChatMessage> ChatHistory {
            get {
                return [.. ChatRequest.ChatHistory];
            }
            set {
                ChatRequest.ChatHistory = [.. value];
                OnPropertyChanged(nameof(ChatHistory));
            }

        }

        private ObservableCollection<VectorSearchProperty> _vectorSearchProperties = [];
        public ObservableCollection<VectorSearchProperty> VectorSearchProperties {
            get {
                return _vectorSearchProperties;
            }
            set {
                _vectorSearchProperties = value;
                OnPropertyChanged(nameof(VectorSearchProperties));
            }
        }

        private VectorSearchProperty? _selectedVectorSearchProperty = null;
        public VectorSearchProperty? SelectedVectorSearchProperty {
            get {
                return _selectedVectorSearchProperty;
            }
            set {
                _selectedVectorSearchProperty = value;
                OnPropertyChanged(nameof(SelectedVectorSearchProperty));
            }
        }

        public string InputText {
            get {
                return ChatRequest.ContentText;
            }
            set {
                ChatRequest.ContentText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        // プロンプトの文字列
        private string _PromptText = "";
        public string PromptText {
            get {
                return _PromptText;
            }
            set {
                _PromptText = value;
                OnPropertyChanged(nameof(PromptText));
            }
        }

        // SelectedContextItem
        private ContentItem? _SelectedContextItem = null;
        public ContentItem? SelectedContextItem {
            get {
                return _SelectedContextItem;
            }
            set {
                _SelectedContextItem = value;
                OnPropertyChanged(nameof(SelectedContextItem));
            }
        }
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
                    List<VectorSearchProperty> items = QAChatStartupProps.ContentItem.GetFolder<ContentFolder>().GetVectorSearchProperties();
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

        public string PreviewJson {
            get {
                // ベクトルDB検索結果最大値をVectorSearchPropertyに設定
                foreach (var item in VectorSearchProperties) {
                    item.TopK = VectorDBSearchResultMax;
                }
                ChatRequestContext chatRequestContext = CreateChatRequestContext();
                ChatRequest.PrepareNormalRequest(chatRequestContext, ChatRequest);
                return DebugUtil.CreateParameterJson(chatRequestContext, ChatRequest);
            }
        }
        // GeneratedDebugCommand
        public string GeneratedDebugCommand {
            get {
                // ベクトルDB検索結果最大値をVectorSearchPropertyに設定
                foreach (var item in VectorSearchProperties) {
                    item.TopK = VectorDBSearchResultMax;
                }
                ChatRequestContext chatRequestContext = CreateChatRequestContext();
                ChatRequest.PrepareNormalRequest(chatRequestContext, ChatRequest);
                return string.Join("\n\n", DebugUtil.CreateChatCommandLine(chatRequestContext, ChatRequest));
            }
        }

        private ChatRequestContext CreateChatRequestContext() {
            int splitTokenCount = Int32.Parse(SplitTokenCount);
            ChatRequestContext chatRequestContext = ChatRequestContext.CreateDefaultChatRequestContext(
                _chatMode, _splitMode, splitTokenCount, UseVectorDB, [.. VectorSearchProperties], AutoGenProperties, PromptText
                );
            return chatRequestContext;
        }

        //
        public Visibility VectorDBItemVisibility => Tools.BoolToVisibility(UseVectorDB);

        public Visibility SplitMOdeVisibility => Tools.BoolToVisibility(_splitMode != SplitOnTokenLimitExceedModeEnum.None);

        // DebugCommandVisibility
        public Visibility DebugCommandVisibility => Tools.BoolToVisibility(SelectedTabIndex == 2);

        // DebugCommand
        public SimpleDelegateCommand<object> DebugCommand => new((parameter) => {
            // ベクトルDB検索結果最大値をVectorSearchPropertyに設定
            foreach (var item in VectorSearchProperties) {
                item.TopK = VectorDBSearchResultMax;
            }
            ChatRequestContext chatRequestContext = CreateChatRequestContext();
            ChatRequest.PrepareNormalRequest(chatRequestContext, ChatRequest);
            DebugUtil.ExecuteDebugCommand(DebugUtil.CreateChatCommandLine(chatRequestContext, ChatRequest));
        });
        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {

            PythonAILibManager? libManager = PythonAILibManager.Instance;

            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResult? result = null;
                // プログレスバーを表示
                IsIndeterminate = true;

                // チャット内容を更新
                await Task.Run(() => {

                    // ベクトルDB検索結果最大値をVectorSearchPropertyに設定
                    foreach (var item in VectorSearchProperties) {
                        item.TopK = VectorDBSearchResultMax;
                    }
                    ChatRequestContext chatRequestContext = CreateChatRequestContext();
                    // SplitModeが有効な場合で、PromptTextが空の場合はエラー
                    if (_splitMode != SplitOnTokenLimitExceedModeEnum.None && string.IsNullOrEmpty(PromptText)) {
                        LogWrapper.Error(StringResources.PromptTextIsNeededWhenSplitModeIsEnabled);
                        return;
                    }
                    // OpenAIChat or LangChainChatを実行
                    result = ChatRequest.ExecuteChat(chatRequestContext, (message) => {
                        MainUITask.Run(() => {
                            // チャット内容を更新
                            UpdateChatHistoryList();
                        });
                    });

                });

                if (result == null) {
                    LogWrapper.Error(StringResources.FailedToSendChat);
                    return;
                }
                // チャット内容を更新
                UpdateChatHistoryList();

                // inputTextをクリア
                InputText = "";

            } catch (Exception e) {
                LogWrapper.Error($"{StringResources.ErrorOccurredAndMessage}:\n{e.Message}\n{StringResources.StackTrace}:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // チャット内容のリストを更新するメソッド
        public void UpdateChatHistoryList() {
            // ClipboardItemがある場合はClipboardItemのChatItemsを更新
            QAChatStartupProps.ContentItem.ChatItems = [.. ChatHistory];
            OnPropertyChanged(nameof(ChatHistory));

            // ListBoxの一番最後のアイテムに移動
            UserControl? userControl = (UserControl?)ThisWindow?.FindName("QAChtControl");
            if (userControl != null) {

                ListBox? listBox = (ListBox?)userControl.FindName("ChatContentList");
                if (listBox != null) {
                    listBox.SelectedIndex = listBox.Items.Count - 1;
                    listBox.ScrollIntoView(listBox.SelectedItem);
                }
            }
        }
        // チャット履歴をクリアコマンド
        public SimpleDelegateCommand<object> ClearChatContentsCommand => new((parameter) => {
            ChatHistory = [];
            // ClipboardItemがある場合は、ChatItemsをクリア
            QAChatStartupProps.ContentItem.ChatItems = [];
            OnPropertyChanged(nameof(ChatHistory));
        });

        // 本文を再読み込みコマンド
        public SimpleDelegateCommand<object> ReloadInputTextCommand => new((parameter) => {
            InputText = QAChatStartupProps.ContentItem?.Content ?? "";
            OnPropertyChanged(nameof(InputText));
        });

        // 本文をクリアコマンド
        public SimpleDelegateCommand<object> ClearInputTextCommand => new((parameter) => {
            InputText = "";
            OnPropertyChanged(nameof(InputText));

            PromptText = "";
            OnPropertyChanged(nameof(PromptText));

        });

        // Chatモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ChatModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            ChatMode = comboBox.SelectedIndex;
            // ChatModeVisibility
            OnPropertyChanged(nameof(AutoGenGroupChatVisibility));


        });

        // Splitモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> SplitModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            SplitMode = comboBox.SelectedIndex;
            // SplitMOdeVisibility
            OnPropertyChanged(nameof(SplitMOdeVisibility));

        });

        private int _SelectedTabIndex = 0;
        public int SelectedTabIndex {
            get {
                return _SelectedTabIndex;
            }
            set {
                _SelectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }


        // Tabが変更されたときの処理       
        public SimpleDelegateCommand<RoutedEventArgs> TabSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is TabControl tabControl) {
                // リクエストのメッセージをアップデート
                ChatRequestContext chatRequestContext = CreateChatRequestContext();

                ChatRequest.PrepareNormalRequest(chatRequestContext, ChatRequest);
                // SelectedTabIndexを更新
                SelectedTabIndex = tabControl.SelectedIndex;
                // タブが変更されたときの処理
                if (tabControl.SelectedIndex == 1) {
                    // プレビュー(JSON)タブが選択された場合、プレビューJSONを更新
                    OnPropertyChanged(nameof(PreviewJson));
                }
                if (tabControl.SelectedIndex == 2) {
                    // デバッグタブが選択された場合、デバッグコマンドを更新
                    OnPropertyChanged(nameof(GeneratedDebugCommand));
                }
                OnPropertyChanged(nameof(DebugCommandVisibility));
            }
        });

        // プロンプトテンプレート画面を開くコマンド
        public SimpleDelegateCommand<object> PromptTemplateCommand => new((parameter) => {
            PromptTemplateCommandExecute(parameter);
        });

        // チャットアイテムを編集するコマンド
        public SimpleDelegateCommand<ChatMessage> OpenChatItemCommand => new((chatItem) => {
            EditChatItemWindow.OpenEditChatItemWindow(chatItem);
        });

        // チャット内容をエクスポートするコマンド
        public SimpleDelegateCommand<object> ExportChatCommand => new((parameter) => {
            QAChatStartupProps.ExportChatCommand([.. ChatHistory]);
        });
        // 選択したチャット内容をクリップボードにコピーするコマンド
        public SimpleDelegateCommand<ChatMessage> CopySelectedChatItemCommand => new((item) => {
            string text = $"{item.Role}:\n{item.Content}";
            Clipboard.SetText(text);

        });
        // 全てのチャット内容をクリップボードにコピーするコマンド
        public SimpleDelegateCommand<object> CopyAllChatItemCommand => new((parameter) => {
            string text = "";
            foreach (var item in ChatHistory) {
                text += $"{item.Role}:\n{item.Content}\n";
            }
            Clipboard.SetText(text);
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
                PythonAILibUI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    VectorSearchProperties.Add(vectorDBItemBase);
                });

            OnPropertyChanged(nameof(VectorSearchProperties));
        });


        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            QAChatStartupProps.SaveCommand(QAChatStartupProps.ContentItem, true);
            window.Close();
        });

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
                    AutoGenProperties.AutoGenGroupChat = _SelectedAutoGenGroupChat;
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

    }

}
