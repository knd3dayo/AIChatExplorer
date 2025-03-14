using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Chat;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.ViewModel.PromptTemplate;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Utils.Python;
using LibPythonAI.Utils.Common;
using LibPythonAI.Model.Content;
using PythonAILib.PythonIF;

namespace LibUIPythonAI.ViewModel.Chat {
    public class ChatControlViewModel : ChatViewModelBase {

        //初期化
        public ChatControlViewModel(QAChatStartupProps props) {

            QAChatStartupPropsInstance = props;

            // InputTextを設定
            InputText = QAChatStartupPropsInstance.ContentItem?.Content ?? "";
            // ClipboardItemがある場合は、ChatItemsを設定
            if (QAChatStartupPropsInstance.ContentItem != null) {
                ChatHistory = [.. QAChatStartupPropsInstance.ContentItem.ChatItems];
            }
            // ChatContextPanelViewModelを設定
            ChatContextViewModelInstance = new ChatContextViewModel(QAChatStartupPropsInstance);

        }

        // ChatContextPanelViewModel
        public ChatContextViewModel ChatContextViewModelInstance { get; set; }

        public QAChatStartupProps QAChatStartupPropsInstance { get; set; }

        public ChatRequest ChatRequest { get; set; } = new();

        public bool ChatExecuted { get; set; } = false;
        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        }

        public string SessionToken { get; set; } = Guid.NewGuid().ToString();

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
        private ContentItemWrapper? _SelectedContextItem = null;
        public ContentItemWrapper? SelectedContextItem {
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
                ChatRequestContext chatRequestContext = ChatContextViewModelInstance.CreateChatRequestContext(PromptText, SessionToken);
                ChatUtil.PrepareNormalRequest(chatRequestContext, ChatRequest);
                return DebugUtil.CreateParameterJson(chatRequestContext, ChatRequest);
            }
        }
        // GeneratedDebugCommand
        public string GeneratedDebugCommand {
            get {
                ChatRequestContext chatRequestContext = ChatContextViewModelInstance.CreateChatRequestContext(PromptText, SessionToken);
                ChatUtil.PrepareNormalRequest(chatRequestContext, ChatRequest);
                return string.Join("\n\n", DebugUtil.CreateChatCommandLine(chatRequestContext, ChatRequest));
            }
        }

        // DebugCommandVisibility
        public Visibility DebugCommandVisibility => Tools.BoolToVisibility(SelectedTabIndex == 2);

        // DebugCommand
        public SimpleDelegateCommand<object> DebugCommand => new((parameter) => {
            ChatRequestContext chatRequestContext = ChatContextViewModelInstance.CreateChatRequestContext(PromptText, SessionToken);
            ChatUtil.PrepareNormalRequest(chatRequestContext, ChatRequest);
            DebugUtil.ExecuteDebugCommand(DebugUtil.CreateChatCommandLine(chatRequestContext, ChatRequest));
        });

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {

            PythonAILibManager? libManager = PythonAILibManager.Instance;

            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResult? result = null;
                // プログレスバーを表示
                UpdateIndeterminate(true);

                // チャット内容を更新
                await Task.Run(() => {

                    ChatRequest.Temperature = ChatContextViewModelInstance.Temperature;

                    ChatRequestContext chatRequestContext = ChatContextViewModelInstance.CreateChatRequestContext(PromptText, SessionToken);
                    // SplitModeが有効な場合で、PromptTextが空の場合はエラー
                    SplitOnTokenLimitExceedModeEnum _splitMode = (SplitOnTokenLimitExceedModeEnum)ChatContextViewModelInstance.SplitMode;
                    if (_splitMode != SplitOnTokenLimitExceedModeEnum.None && string.IsNullOrEmpty(PromptText)) {
                        LogWrapper.Error(StringResources.PromptTextIsNeededWhenSplitModeIsEnabled);
                        return;
                    }
                    // OpenAIChat or LangChainChatを実行
                    result = ChatUtil.ExecuteChat(ChatRequest, chatRequestContext, (message) => {
                        MainUITask.Run(() => {
                            // チャット内容を更新
                            UpdateChatHistoryList();
                        });
                    });

                });
                UpdateIndeterminate(false);

                if (result == null) {
                    LogWrapper.Error(StringResources.FailedToSendChat);
                    return;
                }
                // チャット内容を更新
                UpdateChatHistoryList();

                // inputTextをクリア
                InputText = "";
                // ChatExecutedをTrueに設定
                ChatExecuted = true;

            } catch (Exception e) {
                LogWrapper.Error($"{StringResources.ErrorOccurredAndMessage}:\n{e.Message}\n{StringResources.StackTrace}:\n{e.StackTrace}");
            }

        });

        // チャット内容のリストを更新するメソッド
        public void UpdateChatHistoryList() {
            // ClipboardItemがある場合はClipboardItemのChatItemsを更新
            QAChatStartupPropsInstance.ContentItem.ChatItems.Clear();
            QAChatStartupPropsInstance.ContentItem.ChatItems.AddRange( [.. ChatHistory]);
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
        // AutoGeChatのキャンセルコマンド
        public SimpleDelegateCommand<object> CancelAutoGenChatCommand => new((parameter) => {
            Task.Run(() => {
                ChatUtil.CancelAutoGenChat(SessionToken);
            }).ContinueWith((task) => {
                UpdateIndeterminate(false);
            });
        });
        // チャット履歴をクリアコマンド
        public SimpleDelegateCommand<object> ClearChatContentsCommand => new((parameter) => {
            ChatHistory = [];
            // ClipboardItemがある場合は、ChatItemsをクリア
            QAChatStartupPropsInstance.ContentItem.ChatItems.Clear();
            OnPropertyChanged(nameof(ChatHistory));
        });

        // 本文を再読み込みコマンド
        public SimpleDelegateCommand<object> ReloadInputTextCommand => new((parameter) => {
            InputText = QAChatStartupPropsInstance.ContentItem?.Content ?? "";
            OnPropertyChanged(nameof(InputText));
        });

        // 本文をクリアコマンド
        public SimpleDelegateCommand<object> ClearInputTextCommand => new((parameter) => {
            InputText = "";
            OnPropertyChanged(nameof(InputText));

            PromptText = "";
            OnPropertyChanged(nameof(PromptText));

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
                ChatRequestContext chatRequestContext = ChatContextViewModelInstance.CreateChatRequestContext(PromptText, SessionToken);
                ChatRequest.Temperature = ChatContextViewModelInstance.Temperature;
                ChatUtil.PrepareNormalRequest(chatRequestContext, ChatRequest);
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
            QAChatStartupPropsInstance.ExportChatCommand([.. ChatHistory]);
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


        public SimpleDelegateCommand<Window> SaveAndCloseCommand => new((window) => {
            // Chatを実行した場合は 、ContentItemを更新

            QAChatStartupPropsInstance.CloseCommand(QAChatStartupPropsInstance.ContentItem, ChatExecuted);
            window.Close();
        });

    }

}
