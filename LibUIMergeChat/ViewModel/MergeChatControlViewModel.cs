using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Common;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;
using LibUIMergeChat.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.ViewModel.Chat;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.ViewModel.PromptTemplate;

namespace LibUIMergeChat.ViewModel {
    public class MergeChatControlViewModel : CommonViewModelBase {


        //初期化
        public MergeChatControlViewModel(MergeTargetPanelViewModel mergeTargetPanelViewModel) {

            // VectorDBItemsを設定 ApplicationFolderのベクトルDBを取得
            var folder = mergeTargetPanelViewModel.MergeTargetTreeViewControlViewModel.SelectedFolder;
            if (folder == null) {
                return;
            }
            Task.Run( () => {
                // ChatRequestContextViewModelを設定
                var item =  folder.Folder.GetVectorSearchProperties();
                ChatRequestContextViewModel.VectorSearchProperties = [.. item];
            });

            // MergeTargetPanelViewModelを設定
            MergeTargetPanelViewModel = mergeTargetPanelViewModel;

            // OutputFolderPath
            OutputFolder = mergeTargetPanelViewModel.MergeTargetTreeViewControlViewModel.SelectedFolder;

            // MergeTargetSummaryDataGridViewModel
            MergeTargetSummaryDataGridViewModel =  new((flag) => { });

            // MergeTargetSummaryDataGridViewModelのItemsを設定
            MergeTargetSummaryDataGridViewModel.Items = mergeTargetPanelViewModel.MergeTargetDataGridViewControlViewModel.CheckedItems;

            Task.Run(async () => {
                ExportItems = await CreateExportItems();
                OnPropertyChanged(nameof(ExportItems));
            });
        }

        // ExportItems
        public ObservableCollection<ContentItemDataDefinition> ExportItems { get; set; }



        // SessionToken
        public string SessionToken { get; set; } = Guid.NewGuid().ToString();

        public MergeTargetDataGridViewControlViewModel MergeTargetSummaryDataGridViewModel { get; set; } = null!; // 初期化時に設定されるため、null許容型ではない

        public MergeTargetPanelViewModel MergeTargetPanelViewModel { get; set; } = null!; // 初期化時に設定されるため、null許容型ではない

        public ChatRequestContextViewModel ChatRequestContextViewModel { get; set; } = new();


        private void PreProcessPromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PreProcessPromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        }
        private void PostProcessPromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PostProcessPromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        }

        // プロンプトの文字列
        private string _PreProcessPromptText = "";
        public string PreProcessPromptText {
            get {
                return _PreProcessPromptText;
            }
            set {
                _PreProcessPromptText = value;
                OnPropertyChanged(nameof(PreProcessPromptText));
            }
        }
        // 事後処理用のプロンプトの文字列
        private string _PostProcessPromptText = "";
        public string PostProcessPromptText {
            get {
                return _PostProcessPromptText;
            }
            set {
                _PostProcessPromptText = value;
                OnPropertyChanged(nameof(PostProcessPromptText));
            }
        }

        // OutputFolderPath
        private ContentFolderViewModel? _OutputFolder;
        public ContentFolderViewModel? OutputFolder {
            get {
                return _OutputFolder;
            }
            set {
                _OutputFolder = value;
                OnPropertyChanged(nameof(OutputFolder));
            }
        }

        // Temperature
        private double _Temperature = 0.5;
        public double Temperature {
            get {
                return _Temperature;
            }
            set {
                _Temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {

            PythonAILibManager? libManager = PythonAILibManager.Instance;

            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResponse? result = null;
                // プログレスバーを表示
                CommonViewModelProperties.UpdateIndeterminate(true);

                ObservableCollection<ContentItemViewModel> itemViewModels = MergeTargetPanelViewModel?.MergeTargetDataGridViewControlViewModel.CheckedItemsInMergeTargetSelectedDataGrid ?? [];
                // itemViewModelsからContentItemをSelect
                List<ContentItemWrapper> items = itemViewModels?.Select(x => x.ContentItem).ToList() ?? [];
                // チャット内容を更新
                await Task.Run(async () => {
                    ChatRequestContext context = ChatRequestContextViewModel.GetChatRequestContext();

                    // SplitModeが有効な場合で、PromptTextが空の場合はエラー
                    if (context.SplitMode != SplitModeEnum.None && string.IsNullOrEmpty(PreProcessPromptText)) {
                        LogWrapper.Error(CommonStringResources.Instance.PromptTextIsNeededWhenSplitModeIsEnabled);
                        CommonViewModelProperties.UpdateIndeterminate(false);
                        return;
                    }
                    // MergeChatUtil.MergeChatを実行
                    result = await MergeChatUtil.MergeChat(context, items, PreProcessPromptText, PostProcessPromptText, SessionToken, [.. ExportItems]);
                });

                if (result == null) {
                    LogWrapper.Error(CommonStringResources.Instance.FailedToSendChat);
                    CommonViewModelProperties.UpdateIndeterminate(false);
                    return;
                }
                // チャット結果をOutputFolderに保存
                if (OutputFolder != null) {
                    ContentItemWrapper contentItemWrapper = new() {
                        Content = result.Output,
                        SourceType = ContentSourceType.Application
                    };


                    await OutputFolder.Folder.AddItemAsync(contentItemWrapper, true, (item) => {
                        CommonViewModelProperties.UpdateIndeterminate(false);
                        LogWrapper.Info(CommonStringResources.Instance.SavedChatResult);
                        // OutputFolderを再読み込みした後、Closeを実行
                        OutputFolder.FolderCommands.LoadFolderCommand.Execute();
                        // Close
                        MainUITask.Run(() => {
                            CloseCommand.Execute();
                        });
                    });
                }


            } catch (Exception e) {
                LogWrapper.Error($"{CommonStringResources.Instance.ErrorOccurredAndMessage}:\n{e.Message}\n{CommonStringResources.Instance.StackTrace}:\n{e.StackTrace}");
            }

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
                ChatRequestContext chatRequestContext = ChatRequestContextViewModel.GetChatRequestContext();
                ChatRequest request = new() {
                    // request.Temperature;
                    Temperature = Temperature
                };
                ChatUtil.PrepareNormalRequest(chatRequestContext, request);
                // SelectedTabIndexを更新
                SelectedTabIndex = tabControl.SelectedIndex;

                // OnPropertyChanged(nameof(MergeTargetSummaryDataGridViewModel.ContentItems));
            }
        });

        // プロンプトテンプレート画面を開くコマンド
        public SimpleDelegateCommand<object> PreProcessPromptTemplateCommand => new((parameter) => {
            PreProcessPromptTemplateCommandExecute(parameter);
        });

        // 事後処理用のプロンプトテンプレート画面を開くコマンド
        public SimpleDelegateCommand<object> PostProcessPromptTemplateCommand => new((parameter) => {
            PostProcessPromptTemplateCommandExecute(parameter);
        });

        // 出力先フォルダを選択するコマンド
        public SimpleDelegateCommand<object> SelectOutputFolderCommand => new((parameter) => {
            // フォルダを選択
            FolderSelectWindow.OpenFolderSelectWindow(FolderViewModelManagerBase.FolderViewModels, (folderViewModel, isSelect) => {
                if (isSelect) {
                    OutputFolder = folderViewModel;
                }
            });
        });


        private static async Task<ObservableCollection<ContentItemDataDefinition>> CreateExportItems() {
            // PromptItemの設定 出力タイプがテキストコンテンツのものを取得
            List<PromptItem> promptItems = await PromptItem.GetPromptItems();
            promptItems = promptItems.Where(item => item.PromptResultType == PromptResultTypeEnum.TextContent).ToList();

            ObservableCollection<ContentItemDataDefinition> items = [ .. ContentItemDataDefinition.CreateDefaultDataDefinitions()];

            foreach (PromptItem promptItem in promptItems) {
                items.Add(new ContentItemDataDefinition(promptItem.Name, promptItem.Description, false, true));
            }
            return items;
        }


    }

}
