using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
using PythonAILib.Utils.Python;
using QAChat.Control;
using QAChat.Resource;
using QAChat.Utils;
using QAChat.View.EditChatItemWindow;
using QAChat.View.PromptTemplateWindow;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel.PromptTemplateWindow;
using QAChat.ViewModel.VectorDBWindow;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.QAChatMain
{
    public class QAChatControlViewModel : CommonViewModelBase {
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
        }

        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;

        public QAChatStartupProps QAChatStartupProps { get; set; }


        // CollectionName
        private string? _CollectionName = null;
        public string? CollectionName {
            get {
                return _CollectionName;
            }
            set {
                _CollectionName = value;
                OnPropertyChanged(nameof(CollectionName));
            }
        }

        // 選択中のフォルダの全てのClipboardItem
        public ObservableCollection<ContentItem> ClipboardItems { get; set; } = new();


        public ChatRequest ChatController { get; set; } = new();

        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        }

        // Progress Indicatorの表示状態
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return _IsIndeterminate;
            }
            set {
                _IsIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }

        public int Mode {
            get {
                return (int)ChatController.ChatMode;
            }
            set {
                ChatController.ChatMode = (OpenAIExecutionModeEnum)value;
                OnPropertyChanged(nameof(Mode));
            }
        }

        public static ChatContentItem? SelectedItem { get; set; }

        public ObservableCollection<ChatContentItem> ChatHistory {
            get {
                return [.. ChatController.ChatHistory];
            }
            set {
                ChatController.ChatHistory = [.. value];
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
                return ChatController.ContentText;
            }
            set {
                ChatController.ContentText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        // プロンプトの文字列
        public string PromptText {
            get {
                return ChatController.PromptTemplateText;
            }
            set {
                ChatController.PromptTemplateText = value;
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
                // パラメーターJSONを作成
                string parametersJson = DebugUtil.CreateParameterJson(PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties(), [.. VectorDBItems], ChatController);
                return parametersJson;
            }
        }

        //
        public Visibility VectorDBItemVisibility {
            get {
                if (ChatController.ChatMode == OpenAIExecutionModeEnum.Normal) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }

        public TextWrapping TextWrapping {
            get {
                if (QAChatManager.Instance == null) {
                    return TextWrapping.NoWrap;
                }
                return QAChatManager.Instance.ConfigParams.GetTextWrapping();
            }
        }
        // チャット履歴を保存するか否かのフラグ
        private bool _SaveChatHistory = false;

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            PythonAILibManager? libManager = PythonAILibManager.Instance;
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResult? result = null;
                // プログレスバーを表示
                IsIndeterminate = true;

                // チャット内容を更新
                // UpdateChatHistoryList();
                if (ChatController.ChatMode == OpenAIExecutionModeEnum.AutoGen) {
                    StartAutoGenGroupChaCommandExecute();

                } else {

                    await Task.Run(() => {
                        // LangChainChat用。VectorDBItemsを設定
                        List<VectorDBItem> items = [.. VectorDBItems];
                        ChatController.VectorDBItems = items;

                        // OpenAIChat or LangChainChatを実行
                        result = ChatController.ExecuteChat(libManager.ConfigParams.GetOpenAIProperties());
                    });

                    if (result == null) {
                        LogWrapper.Error(StringResources.FailedToSendChat);
                        return;
                    }
                    // チャット内容を更新
                    UpdateChatHistoryList();

                    // inputTextをクリア
                    InputText = "";

                    // _SaveChatHistoryをTrueに設定
                    _SaveChatHistory = true;

                }

            } catch (Exception e) {
                LogWrapper.Error($"{StringResources.ErrorOccurredAndMessage}:\n{e.Message}\n{StringResources.StackTrace}:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // AutoGenのGroupChatを実行するコマンド
        private void StartAutoGenGroupChaCommandExecute() {
            // OpenAIPropertiesを取得
            OpenAIProperties openAIProperties = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties();
            // VectorDBItemsを取得
            List<VectorDBItem> vectorDBItems = [.. VectorDBItems];
            // メッセージを取得
            string message = InputText;
            // AutoGenGroupChatTest1を実行
            AutoGenProcessController.StartAutoGenGroupChatTest1(openAIProperties, vectorDBItems, ChatController, (responseJson) => {
                MainUITask.Run(() => {
                    // ChatHistoryに追加
                    ChatContentItem chatItem1 = new(ChatContentItem.UserRole, InputText);
                    ChatController.ChatHistory.Add(chatItem1);
                    // ChatHistoryに追加
                    string responseText = DebugUtil.ProcessAutoGenGroupChatResult(responseJson);

                    ChatContentItem chatItem2 = new(ChatContentItem.AssistantRole, responseText);
                    ChatController.ChatHistory.Add(chatItem2);

                    // チャット内容を更新
                    UpdateChatHistoryList();
                    // inputTextをクリア
                    InputText = "";
                    // _SaveChatHistoryをTrueに設定
                    _SaveChatHistory = true;
                });

            });
        }

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
            ChatController.ChatMode = (OpenAIExecutionModeEnum)index;
            // ModeがNormal以外の場合は、VectorDBItemを取得
            VectorDBItems = [];
            if (ChatController.ChatMode != OpenAIExecutionModeEnum.Normal) {
                List<VectorDBItem> items = QAChatStartupProps.ContentItem.ReferenceVectorDBItems;
                foreach (var item in items) {
                    VectorDBItems.Add(item);
                }
            }
            // VectorDBItemVisibilityを更新
            OnPropertyChanged(nameof(VectorDBItemVisibility));

        });

        // Tabが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> TabSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is TabControl tabControl) {
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
        public SimpleDelegateCommand<ChatContentItem> OpenChatItemCommand => new((chatItem) => {
            EditChatItemWindow.OpenEditChatItemWindow(chatItem);
        });

        // チャット内容をエクスポートするコマンド
        public SimpleDelegateCommand<object> ExportChatCommand => new((parameter) => {
            QAChatStartupProps.ExportChatCommand([.. ChatHistory]);
        });
        // 選択したチャット内容をクリップボードにコピーするコマンド
        public SimpleDelegateCommand<ChatContentItem> CopySelectedChatItemCommand => new((item) => {
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
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, (selectedItem) => {
                VectorDBItems.Add(selectedItem);
            });
        });

        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            QAChatStartupProps.SaveCommand(QAChatStartupProps.ContentItem, _SaveChatHistory);
            window.Close();
        });

        // GeneratedDebugCommand
        public string GeneratedDebugCommand {
            get {
                // ModeがNormalまたはOpenAIRAGの場合は、OpenAIChatを実行するコマンドを返す
                if (ChatController.ChatMode == OpenAIExecutionModeEnum.Normal || ChatController.ChatMode == OpenAIExecutionModeEnum.OpenAIRAG) {
                    // パラメーターファイルを作成
                    string parametersJson = DebugUtil.CreateParameterJson(PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties(), [.. VectorDBItems], ChatController);
                    File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);
                    return string.Join("\n\n", DebugUtil.CreateOpenAIChatCommandLine(DebugUtil.DebugRequestParametersFile));
                }
                // ModeがLangChainの場合は、LangChainChatを実行するコマンドを返す
                if (ChatController.ChatMode == OpenAIExecutionModeEnum.LangChain) {
                    // パラメーターファイルを作成
                    string parametersJson = DebugUtil.CreateParameterJson(PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties(), [.. VectorDBItems], ChatController);
                    File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);
                    return string.Join("\n\n", DebugUtil.CreateLangChainChatCommandLine(DebugUtil.DebugRequestParametersFile));
                }

                // ModeがAutoGenの場合は、AutoGenのGroupChatを実行するコマンドを返す
                if (ChatController.ChatMode == OpenAIExecutionModeEnum.AutoGen) {
                    // パラメーターファイルを作成
                    string parametersJson = DebugUtil.CreateParameterJson(PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties(), [.. VectorDBItems], ChatController);
                    File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);

                    return string.Join("\n\n", DebugUtil.CreateAutoGenGroupChatTest1CommandLine(DebugUtil.DebugRequestParametersFile, null));
                }

                return "";
            }
        }

    }

}
