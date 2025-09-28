using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Python;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Chat;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.ViewModel.PromptTemplate;
using System.ComponentModel;
using LibUIPythonAI.Resource;
using LibUIPythonAI.ViewModel.Chat;
using LibPythonAI.Model.Chat;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Common;

namespace LibUIAutoGenChat.ViewModel.Chat {
    public class AutoGenChatControlViewModel : CommonViewModelBase {

        //初期化
        public AutoGenChatControlViewModel(QAChatStartupPropsBase props) {

            QAChatStartupPropsInstance = props;

            // InputTextを設定
            InputText = QAChatStartupPropsInstance.GetContentItem().Content;
            // ApplicationItemがある場合は、ChatItemsを設定
            if (QAChatStartupPropsInstance.GetContentItem() != null) {
                ChatHistory = [.. QAChatStartupPropsInstance.GetContentItem().ChatItems];
            }
            // ChatContextPanelViewModelを設定
            ChatContextViewModelInstance = new AutoGenChatContextViewModel(QAChatStartupPropsInstance);

            CommonViewModelProperties.PropertyChanged  += OnPropertyChanged;

        }

        // OnPropertyChanged
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CommonViewModelProperties.MarkdownView)) {
                OnPropertyChanged(nameof(ChatHistory));
            }
        }

        // ChatContextPanelViewModel
        public AutoGenChatContextViewModel ChatContextViewModelInstance { get; set; }

        public QAChatStartupPropsBase QAChatStartupPropsInstance { get; set; }

        // ChatRequest
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


        public Visibility MarkdownVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView);

        public Visibility TextVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView == false);




        // DebugCommandVisibility
        public Visibility DebugCommandVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(SelectedTabIndex == 2);


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



        // チャット内容のリストを更新するメソッド
        public void UpdateChatHistoryList() {
            // ApplicationItemがある場合はApplicationItemのChatItemsを更新
            QAChatStartupPropsInstance.GetContentItem().ChatItems.Clear();
            QAChatStartupPropsInstance.GetContentItem().ChatItems.AddRange([.. ChatHistory]);
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

        public string PreviewJson {
            get {
                ChatRequestContext chatRequestContext = ChatContextViewModelInstance.CreateChatRequestContext(PromptText, SessionToken);
                ChatUtil.PrepareNormalRequest(chatRequestContext, ChatRequest);
                return DebugUtil.CreateParameterJson((OpenAIExecutionModeEnum)ChatContextViewModelInstance.ChatMode, chatRequestContext, ChatRequest);
            }
        }
        // GeneratedDebugCommand
        public string GeneratedDebugCommand {
            get {
                ChatRequestContext chatRequestContext = ChatContextViewModelInstance.CreateChatRequestContext(PromptText, SessionToken);
                ChatUtil.PrepareNormalRequest(chatRequestContext, ChatRequest);
                return string.Join("\n\n", DebugUtil.CreateChatCommandLine((OpenAIExecutionModeEnum)ChatContextViewModelInstance.ChatMode, chatRequestContext, ChatRequest));
            }
        }

        // DebugCommand
        public SimpleDelegateCommand<object> DebugCommand => new((parameter) => {
            ChatRequestContext chatRequestContext = ChatContextViewModelInstance.CreateChatRequestContext(PromptText, SessionToken);
            ChatUtil.PrepareNormalRequest(chatRequestContext, ChatRequest);
            DebugUtil.ExecuteDebugCommand(DebugUtil.CreateChatCommandLine((OpenAIExecutionModeEnum)ChatContextViewModelInstance.ChatMode, chatRequestContext, ChatRequest));
        });

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {

            PythonAILibManager? libManager = PythonAILibManager.Instance;

            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResponse? result = null;
                // プログレスバーを表示
                CommonViewModelProperties.UpdateIndeterminate(true);

                // チャット内容を更新
                await Task.Run(async () => {

                    ChatRequest.Temperature = ChatContextViewModelInstance.Temperature;

                    ChatRequestContext chatRequestContext = ChatContextViewModelInstance.CreateChatRequestContext(PromptText, SessionToken);
                    // SplitModeが有効な場合で、PromptTextが空の場合はエラー
                    SplitModeEnum _splitMode = (SplitModeEnum)ChatContextViewModelInstance.SplitMode;
                    if (_splitMode != SplitModeEnum.None && string.IsNullOrEmpty(PromptText)) {
                        LogWrapper.Error(CommonStringResources.Instance.PromptTextIsNeededWhenSplitModeIsEnabled);
                        return;
                    }
                    // ChatMode
                    OpenAIExecutionModeEnum chatMode = (OpenAIExecutionModeEnum)ChatContextViewModelInstance.ChatMode;
                    // OpenAIChatAsync or LangChainChatを実行
                    result = await ChatUtil.ExecuteChat(chatMode, ChatRequest, chatRequestContext, (message) => {
                        MainUITask.Run(() => {
                            // チャット内容を更新
                            UpdateChatHistoryList();
                        });
                    });

                });
                CommonViewModelProperties.UpdateIndeterminate(false);

                if (result == null) {
                    LogWrapper.Error(CommonStringResources.Instance.FailedToSendChat);
                    return;
                }
                // チャット内容を更新
                UpdateChatHistoryList();

                // inputTextをクリア
                InputText = "";
                // ChatExecutedをTrueに設定
                ChatExecuted = true;

            } catch (Exception e) {
                LogWrapper.Error($"{CommonStringResources.Instance.ErrorOccurredAndMessage}:\n{e.Message}\n{CommonStringResources.Instance.StackTrace}:\n{e.StackTrace}");
            }

        });


        // AutoGeChatのキャンセルコマンド
        public SimpleDelegateCommand<object> CancelAutoGenChatCommand => new((parameter) => {
            Task.Run(() => {
                ChatUtil.CancelAutoGenChat(SessionToken);
            }).ContinueWith((task) => {
                CommonViewModelProperties.UpdateIndeterminate(false);
            });
        });
        // チャット履歴をクリアコマンド
        public SimpleDelegateCommand<object> ClearChatContentsCommand => new((parameter) => {
            ChatHistory = [];
            // ApplicationItemがある場合は、ChatItemsをクリア
            QAChatStartupPropsInstance.GetContentItem().ChatItems.Clear();
            OnPropertyChanged(nameof(ChatHistory));
        });

        // 本文を再読み込みコマンド
        public SimpleDelegateCommand<object> ReloadInputTextCommand => new((parameter) => {
            InputText = QAChatStartupPropsInstance.GetContentItem()?.Content ?? "";
            OnPropertyChanged(nameof(InputText));
        });

        // 本文をクリアコマンド
        public SimpleDelegateCommand<object> ClearInputTextCommand => new((parameter) => {
            InputText = "";
            OnPropertyChanged(nameof(InputText));

            PromptText = "";
            OnPropertyChanged(nameof(PromptText));

        });

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
            CommonViewModelProperties.PropertyChanged -= OnPropertyChanged;

            QAChatStartupPropsInstance.SaveCommand(QAChatStartupPropsInstance.GetContentItem(), ChatExecuted);
            window.Close();
        });

    }

}
