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
using QAChat.Resource;
using QAChat.View.PromptTemplate;
using QAChat.View.QAChatMain;
using QAChat.ViewModel.PromptTemplate;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.QAChatMain {
    public class QAChatControlViewModel : QAChatViewModelBase {
        //初期化
        public QAChatControlViewModel(QAChatStartupProps props) {

            QAChatStartupProps = props;
            // VectorDBItemsを設定 ClipboardFolderのベクトルDBを取得
            List<VectorDBItem> vectorDBItems = props.ContentItem.ReferenceVectorDBItems;
            VectorDBItems = new(vectorDBItems);

            // InputTextを設定
            InputText = QAChatStartupProps.ContentItem?.Content ?? "";
            // ClipboardItemがある場合は、ChatItemsを設定
            if (QAChatStartupProps.ContentItem != null) {
                ChatHistory = [.. QAChatStartupProps.ContentItem.ChatItems];
            }
            // AutoGenGroupChatを設定
            SelectedAutoGenGroupChat = AutoGenGroupChat.FindAll().FirstOrDefault();
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
        public int Mode {
            get {
                return (int)ChatRequest.ChatMode;
            }
            set {
                ChatRequest.ChatMode = (OpenAIExecutionModeEnum)value;
                OnPropertyChanged(nameof(Mode));
            }
        }

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

        private ObservableCollection<VectorDBItem> _vectorDBItemBases = [];
        public ObservableCollection<VectorDBItem> VectorDBItems {
            get {
                return _vectorDBItemBases;
            }
            set {
                _vectorDBItemBases = value;
                OnPropertyChanged(nameof(VectorDBItems));
            }
        }

        private VectorDBItem? _SelectedVectorDBItem = null;
        public VectorDBItem? SelectedVectorDBItem {
            get {
                return _SelectedVectorDBItem;
            }
            set {
                _SelectedVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedVectorDBItem));
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
        public string PromptText {
            get {
                return ChatRequest.PromptTemplateText;
            }
            set {
                ChatRequest.PromptTemplateText = value;
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

        public string PreviewJson {
            get {
                ChatRequestContext chatRequestContext = ChatRequestContext.CreateDefaultChatRequestContext([.. VectorDBItems], SelectedAutoGenGroupChat);
                return DebugUtil.CreateParameterJson(chatRequestContext, ChatRequest);
            }
        }

        //
        public Visibility VectorDBItemVisibility => Tools.BoolToVisibility(ChatRequest.ChatMode != OpenAIExecutionModeEnum.Normal);


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
                    // VectorDBItemsを設定
                    List<VectorDBItem> items = [.. VectorDBItems];

                    // ★TODO AutoGenPropertiesを実行時に指定可能にする
                    AutoGenProperties autoGenProperties = new() {
                        WorkDir = libManager.ConfigParams.GetAutoGenWorkDir(),
                    };
                    // ChatRequestContextを設定
                    ChatRequestContext chatRequestContext = new() {
                        VectorDBItems = items,
                        OpenAIProperties = libManager.ConfigParams.GetOpenAIProperties(),
                        AutoGenProperties = autoGenProperties
                    };

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

        // モードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            ChatRequest.ChatMode = (OpenAIExecutionModeEnum)index;
            // ModeがNormal以外の場合は、VectorDBItemを取得
            VectorDBItems = [];
            if (ChatRequest.ChatMode != OpenAIExecutionModeEnum.Normal) {
                List<VectorDBItem> items = QAChatStartupProps.ContentItem.ReferenceVectorDBItems;
                foreach (var item in items) {
                    VectorDBItems.Add(item);
                }
            }
            // VectorDBItemVisibilityを更新
            OnPropertyChanged(nameof(VectorDBItemVisibility));
            // AutoGenVisibilityを更新
            OnPropertyChanged(nameof(AutoGenGroupChatVisibility));
            // AutoGenVisibilityを更新
            OnPropertyChanged(nameof(AutoGenNormalChatVisibility));
            // AutoGenVisibilityを更新
            OnPropertyChanged(nameof(AutoGenNestedChatVisibility));

        });

        // Tabが変更されたときの処理       
        public SimpleDelegateCommand<RoutedEventArgs> TabSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is TabControl tabControl) {
                // リクエストのメッセージをアップデート
                ChatRequest.UpdateMessage();
                // タブが変更されたときの処理
                if (tabControl.SelectedIndex == 1) {
                    // プレビュー(JSON)タブが選択された場合、プレビューJSONを更新
                    OnPropertyChanged(nameof(PreviewJson));
                }
                if (tabControl.SelectedIndex == 2) {
                    // デバッグタブが選択された場合、デバッグコマンドを更新
                    OnPropertyChanged(nameof(GeneratedDebugCommand));
                }
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
            if (SelectedVectorDBItem != null) {
                // 元のVectorDBItemsから削除
                QAChatStartupProps.ContentItem.ReferenceVectorDBItems.Remove(SelectedVectorDBItem);
                // VectorDBItemsから削除
                VectorDBItems.Remove(SelectedVectorDBItem);
            }
            OnPropertyChanged(nameof(VectorDBItems));
        });

        // ベクトルDBを追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            QAChatStartupProps.SelectVectorDBItemAction(VectorDBItems);
            // 元のVectorDBItemsに追加
            QAChatStartupProps.ContentItem.ReferenceVectorDBItems = [.. VectorDBItems];

            OnPropertyChanged(nameof(VectorDBItems));
        });

        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            QAChatStartupProps.EditVectorDBItemAction(VectorDBItems);
            // 元のVectorDBItemsに追加
            QAChatStartupProps.ContentItem.ReferenceVectorDBItems = [.. VectorDBItems];
        });

        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            QAChatStartupProps.SaveCommand(QAChatStartupProps.ContentItem, true);
            window.Close();
        });


        // GeneratedDebugCommand
        public string GeneratedDebugCommand {
            get {
                ChatRequestContext chatRequestContext = ChatRequestContext.CreateDefaultChatRequestContext([.. VectorDBItems], SelectedAutoGenGroupChat);
                return DebugUtil.CreateChatCommandLine(chatRequestContext, ChatRequest);
            }
        }


        #region AutoGen Normal Chat
        // AutoGen関連のVisibility
        public Visibility AutoGenNormalChatVisibility => Tools.BoolToVisibility(ChatRequest.ChatMode == OpenAIExecutionModeEnum.AutoGenNormalChat);

        // AutoGenNormalChatList
        public ObservableCollection<AutoGenNormalChat> AutoGenNormalChatList {
            get {
                ObservableCollection<AutoGenNormalChat> autoGenNormalChatList = [];
                foreach (var item in AutoGenNormalChat.FindAll()) {
                    autoGenNormalChatList.Add(item);
                }
                return autoGenNormalChatList;
            }
        }

        // SelectedAutoGenNormalChat
        private AutoGenNormalChat? _SelectedAutoGenNormalChat = null;
        public AutoGenNormalChat? SelectedAutoGenNormalChat {
            get {
                return _SelectedAutoGenNormalChat;
            }
            set {
                _SelectedAutoGenNormalChat = value;
                OnPropertyChanged(nameof(SelectedAutoGenNormalChat));
            }
        }
        // AutoGenNormalChatSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenNormalChatSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is ComboBox comboBox) {
                // 選択されたComboBoxItemのIndexを取得
                int index = comboBox.SelectedIndex;
                SelectedAutoGenNormalChat = AutoGenNormalChatList[index];
            }
        });
        #endregion

        #region AutoGen Group Chat
        // AutoGen関連のVisibility
        public Visibility AutoGenGroupChatVisibility => Tools.BoolToVisibility(ChatRequest.ChatMode == OpenAIExecutionModeEnum.AutoGenGroupChat);

        // AutoGenGroupChatList
        public ObservableCollection<AutoGenGroupChat> AutoGenGroupChatList {
            get {
                ObservableCollection<AutoGenGroupChat> autoGenGroupChatList = [];
                foreach (var item in AutoGenGroupChat.FindAll()) {
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
        #endregion

        #region AutoGen Nested Chat
        // AutoGen関連のVisibility
        public Visibility AutoGenNestedChatVisibility  => Tools.BoolToVisibility(ChatRequest.ChatMode == OpenAIExecutionModeEnum.AutoGenNestedChat);

        // AutoGenNestedChatList
        public ObservableCollection<AutoGenNestedChat> AutoGenNestedChatList {
            get {
                ObservableCollection<AutoGenNestedChat> autoGenNestedChatList = [];
                foreach (var item in AutoGenNestedChat.FindAll()) {
                    autoGenNestedChatList.Add(item);
                }
                return autoGenNestedChatList;
            }
        }
        // SelectedAutoGenNestedChat
        private AutoGenNestedChat? _SelectedAutoGenNestedChat = null;
        public AutoGenNestedChat? SelectedAutoGenNestedChat {
            get {
                return _SelectedAutoGenNestedChat;
            }
            set {
                _SelectedAutoGenNestedChat = value;
                OnPropertyChanged(nameof(SelectedAutoGenNestedChat));
            }
        }
        // AutoGenNestedChatSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenNestedChatSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is ComboBox comboBox) {
                // 選択されたComboBoxItemのIndexを取得
                int index = comboBox.SelectedIndex;
                SelectedAutoGenNestedChat = AutoGenNestedChatList[index];
            }
        });
        #endregion


    }

}
