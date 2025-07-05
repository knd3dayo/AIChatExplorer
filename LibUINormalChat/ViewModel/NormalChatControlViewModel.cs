using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Common;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folder;
using LibPythonAI.Model.Prompt;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;
using LibUINormalChat.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Chat;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.ViewModel.Chat;
using LibUIPythonAI.ViewModel.PromptTemplate;

namespace LibUINormalChat.ViewModel {
    public class NormalChatControlViewModel : CommonViewModelBase {

        //初期化
        public NormalChatControlViewModel(RelatedItemsPanelViewModel relatedItemsPanelViewModel, QAChatStartupProps qAChatStartupPropsInstance) {

            QAChatStartupPropsInstance = qAChatStartupPropsInstance;
            // ChatRequestを初期化
            ChatRequest = new();

            // InputTextを設定
            InputText = QAChatStartupPropsInstance.ContentItem.Content;

            // ApplicationItemがある場合は、ChatItemsを設定
            if (QAChatStartupPropsInstance.ContentItem != null) {
                ChatRequest.ChatHistory = [.. QAChatStartupPropsInstance.ContentItem.ChatItems];
            }

            ChatHistoryViewModel = new(ChatRequest);

            // VectorDBItemsを設定 ApplicationFolderのベクトルDBを取得
            var folder = relatedItemsPanelViewModel.RelatedItemTreeViewControlViewModel.SelectedFolder;
            if (folder == null) {
                return;
            }
            Task.Run(async () => {
                // ChatRequestContextViewModelを設定
                var item = await folder.Folder.GetVectorSearchProperties();
                ChatRequestContextViewModel.VectorSearchProperties = [.. item];
            });

            // RelatedItemsPanelViewModelを設定
            RelatedItemsPanelViewModel = relatedItemsPanelViewModel;

            // RelatedItemSummaryDataGridViewModel
            RelatedItemSummaryDataGridViewModel = new((flag) => { });

            // RelatedItemSummaryDataGridViewModelのItemsを設定
            RelatedItemSummaryDataGridViewModel.Items = relatedItemsPanelViewModel.RelatedItemsDataGridViewModel.CheckedItems;
            QAChatStartupPropsInstance = qAChatStartupPropsInstance;

            Task.Run(async () => {
                // DataDefinitionsを初期化
                DataDefinitions = await CreateExportItems();
            }).ContinueWith((task) => {
                // DataDefinitionsの変更を通知
                OnPropertyChanged(nameof(DataDefinitions));
            });
        }

        // ChatRequest
        private ChatRequest ChatRequest { get; set; }

        private QAChatStartupProps QAChatStartupPropsInstance { get; set; }

        private bool ChatExecuted { get; set; } = false;


        // DataDefinitions
        public ObservableCollection<ContentItemDataDefinition> DataDefinitions { get; set; } = [];

        public RelatedItemDataGridViewModel RelatedItemSummaryDataGridViewModel { get; set; } = null!; // 初期化時に設定されるため、null許容型ではない

        public RelatedItemsPanelViewModel RelatedItemsPanelViewModel { get; set; } = null!; // 初期化時に設定されるため、null許容型ではない

        public ChatRequestContextViewModel ChatRequestContextViewModel { get; set; } = new();



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


        public static ChatMessage? SelectedItem { get; set; }

        public ChatHistoryViewModel ChatHistoryViewModel { get; set; }

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
        public string PromptTemplateText {
            get {
                return ChatRequestContextViewModel.PromptTemplateText;
            }
            set {
                ChatRequestContextViewModel.PromptTemplateText = value;
                OnPropertyChanged(nameof(PromptTemplateText));
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

        // SelectedTabIndex
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


        public string PreviewJson {
            get => CreateChatRequestJson();
        }
        // GeneratedDebugCommand
        public string GeneratedDebugCommand {
            get {
                ChatRequestContext chatRequestContext = ChatRequestContextViewModel.GetChatRequestContext();
                ChatUtil.PrepareNormalRequest(chatRequestContext, ChatRequest);
                return string.Join("\n\n", DebugUtil.CreateChatCommandLine(OpenAIExecutionModeEnum.Normal, chatRequestContext, ChatRequest));
            }
        }


        public Visibility MarkdownVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView);
        public Visibility TextVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView == false);
        // DebugCommandVisibility
        public Visibility DebugCommandVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(SelectedTabIndex == 2);



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

                    // SplitModeが有効な場合で、PromptTextが空の場合はエラー
                    if (!IsSplitModePreconditionMet()) {
                        CommonViewModelProperties.UpdateIndeterminate(false);
                        return;
                    }
                    // チャットを実行
                    result = await ExecuteChat();
                    if (result == null) {
                        CommonViewModelProperties.UpdateIndeterminate(false);
                        LogWrapper.Error(CommonStringResources.Instance.FailedToSendChat);
                        return;
                    }
                    // チャット内容を更新
                    AfterSendChatCommand();
                });

            } catch (Exception e) {
                LogWrapper.Error($"{CommonStringResources.Instance.ErrorOccurredAndMessage}:\n{e.Message}\n{CommonStringResources.Instance.StackTrace}:\n{e.StackTrace}");
            }

        });


        // Tabが変更されたときの処理       
        public SimpleDelegateCommand<RoutedEventArgs> TabSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is TabControl tabControl) {
                // SelectedTabIndexを更新
                SelectedTabIndex = tabControl.SelectedIndex;
                OnPropertyChanged(nameof(PreviewJson));
                OnPropertyChanged(nameof(GeneratedDebugCommand));
            }
        });

        private static async Task<ObservableCollection<ContentItemDataDefinition>> CreateExportItems() {
            // PromptItemの設定 出力タイプがテキストコンテンツのものを取得
            List<PromptItem> promptItems = await PromptItem.GetPromptItems();
            promptItems = promptItems.Where(item => item.PromptResultType == PromptResultTypeEnum.TextContent).ToList();

            ObservableCollection<ContentItemDataDefinition> items = [.. ContentItemDataDefinition.CreateDefaultDataDefinitions()];
            foreach (PromptItem promptItem in promptItems) {
                items.Add(new ContentItemDataDefinition(promptItem.Name, promptItem.Description, false, true));
            }
            return items;
        }
        // プロンプトテンプレート画面を開くコマンド
        public SimpleDelegateCommand<object> PromptTemplateCommand => new((parameter) => {
            PromptTemplateCommandExecute(parameter);
        });

        // DebugCommand
        public SimpleDelegateCommand<object> DebugCommand => new((parameter) => {
            ChatRequestContext chatRequestContext = ChatRequestContextViewModel.GetChatRequestContext();
            ChatUtil.PrepareNormalRequest(chatRequestContext, ChatRequest);
            DebugUtil.ExecuteDebugCommand(DebugUtil.CreateChatCommandLine(OpenAIExecutionModeEnum.Normal, chatRequestContext, ChatRequest));
        });

        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PromptTemplateText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        }

        // チャットアイテムを編集するコマンド
        public SimpleDelegateCommand<ChatMessage> OpenChatItemCommand => new((chatItem) => {
            EditChatItemWindow.OpenEditChatItemWindow(chatItem);
        });

        // チャット内容をエクスポートするコマンド
        public SimpleDelegateCommand<object> ExportChatCommand => new((parameter) => {
            QAChatStartupPropsInstance.ExportChatCommand([.. ChatHistoryViewModel.ChatHistory]);
        });
        // 選択したチャット内容をクリップボードにコピーするコマンド
        public SimpleDelegateCommand<ChatMessage> CopySelectedChatItemCommand => new((item) => {
            string text = $"{item.Role}:\n{item.Content}";
            Clipboard.SetText(text);

        });
        // 全てのチャット内容をクリップボードにコピーするコマンド
        public SimpleDelegateCommand<object> CopyAllChatItemCommand => new((parameter) => {
            string text = "";
            foreach (var item in ChatHistoryViewModel.ChatHistory) {
                text += $"{item.Role}:\n{item.Content}\n";
            }
            Clipboard.SetText(text);
        });

        private string CreateChatRequestJson() {
            ChatRequestContext chatRequestContext = ChatRequestContextViewModel.GetChatRequestContext();
            // ChatRequestのコピーを作成
            ChatRequest chatRequest = ChatRequest.Copy();
            // 関連アイテムを適用
            chatRequest.ApplyReletedItems(CreateChateRelatedItems());
            ChatUtil.PrepareNormalRequest(chatRequestContext, chatRequest);
            return DebugUtil.CreateParameterJson(OpenAIExecutionModeEnum.Normal, chatRequestContext, chatRequest);
        }

        // SplitModeの前提をチェックするメソッド
        private bool IsSplitModePreconditionMet() {
            // SplitModeが有効な場合、PromptTemplateTextが空でないことを確認
            ChatRequestContext chatRequestContext = ChatRequestContextViewModel.GetChatRequestContext();
            if (chatRequestContext.SplitMode != SplitModeEnum.None && string.IsNullOrEmpty(PromptTemplateText)) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTextIsNeededWhenSplitModeIsEnabled);
                return false;
            }
            return true;
        }
        // チャット内容のリストを更新するメソッド
        private void UpdateChatHistoryPosition() {
            OnPropertyChanged(nameof(ChatHistoryViewModel));
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

        private void AfterSendChatCommand() {
            MainUITask.Run(() => {
                CommonViewModelProperties.UpdateIndeterminate(false);
                // チャット内容を更新
                UpdateChatHistoryPosition();
                // inputTextをクリア
                InputText = "";
                // プロンプトテンプレートをクリア
                PromptTemplateText = "";
                // ChatExecutedをTrueに設定
                ChatExecuted = true;
            });

        }
        private ChatRelatedItems CreateChateRelatedItems() {
            // 参照アイテム情報を設定
            List<ContentItemWrapper> items = RelatedItemSummaryDataGridViewModel.Items.Select(x => x.ContentItem).ToList();
            List<ContentItemDataDefinition> dataDefinitions = DataDefinitions.Where(x => x.IsChecked).ToList();
            bool sendRelatedItemsOnlyFirstRequest = ChatRequestContextViewModel.SendRelatedItemsOnlyFirstRequest == 0;

            ChatRelatedItems chatRelatedItems = new() {
                ContentItems = items,
                DataDefinitions = dataDefinitions,
                SendRelatedItemsOnlyFirstRequest = sendRelatedItemsOnlyFirstRequest
            };
            return chatRelatedItems;
        }

        private async Task<ChatResponse?> ExecuteChat() {
            // ChatRequestContextを準備
            ChatRequestContext chatRequestContext = ChatRequestContextViewModel.GetChatRequestContext();

            // 参照アイテム情報を設定
            ChatRelatedItems chatRelatedItems = CreateChateRelatedItems();
            // OpenAIChatAsync or LangChainChatを実行
            ChatResponse? result = await NormalChatUtil.ExecuteChat(ChatRequest, chatRequestContext, chatRelatedItems, (message) => { });

            return result;

        }

    }

}
