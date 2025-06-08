using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Common;
using LibPythonAI.Model.AutoGen;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folder;
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
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.ViewModel.PromptTemplate;
using LibUIPythonAI.ViewModel.VectorDB;

namespace LibUIMergeChat.ViewModel {
    public class MergeChatControlViewModel : CommonViewModelBase {


        //初期化
        public MergeChatControlViewModel(MergeTargetPanelViewModel mergeTargetPanelViewModel) {

            // VectorDBItemsを設定 ApplicationFolderのベクトルDBを取得
            var folder = mergeTargetPanelViewModel.MergeTargetTreeViewControlViewModel.SelectedFolder;
            if (folder == null) {
                return;
            }
            VectorSearchProperties = [.. folder.Folder.GetVectorSearchProperties()];

            // MergeTargetPanelViewModelを設定
            MergeTargetPanelViewModel = mergeTargetPanelViewModel;

            // OutputFolderPath
            OutputFolder = mergeTargetPanelViewModel.MergeTargetTreeViewControlViewModel.SelectedFolder;

        }

        // SessionToken
        public string SessionToken { get; set; } = Guid.NewGuid().ToString();

        public MergeTargetPanelViewModel MergeTargetPanelViewModel { get; set; }

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

        public static ChatMessage? SelectedItem { get; set; }


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

        // RAGMode
        private RAGModeEnum _ragMode = RAGModeEnum.None;
        public int RAGMode {
            get {
                return (int)_ragMode;
            }
            set {
                _ragMode = (RAGModeEnum)value;

                OnPropertyChanged(nameof(RAGMode));
                UpdateVectorDBProperties();

            }
        }
        
        private void UpdateVectorDBProperties() {
            if (_ragMode != RAGModeEnum.None) {
                var folder = MergeTargetPanelViewModel.MergeTargetTreeViewControlViewModel.SelectedFolder;
                if (folder == null) {
                    return;
                }
                List<LibPythonAI.Model.VectorDB.VectorSearchItem> items = [.. folder.Folder.GetVectorSearchProperties()];
                foreach (var item in items) {
                    VectorSearchProperties.Add(item);
                }
            } else {
                VectorSearchProperties.Clear();
            }
            OnPropertyChanged(nameof(VectorDBItemVisibility));
        }


        private ChatRequestContext CreateChatRequestContext() {
            int splitTokenCount = int.Parse(SplitTokenCount);
            // ベクトルDB検索結果最大値をVectorSearchItemに設定
            foreach (var item in VectorSearchProperties) {
                item.TopK = VectorDBSearchResultMax;
            }
            ChatRequestContext chatRequestContext = ChatRequestContext.CreateDefaultChatRequestContext(
                _chatMode, _splitMode, splitTokenCount, _ragMode, [.. VectorSearchProperties], AutoGenProperties, PreProcessPromptText
                );
            return chatRequestContext;
        }

        //


        public Visibility VectorDBItemVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(_ragMode != RAGModeEnum.None);

        public Visibility SplitMOdeVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(_splitMode != SplitModeEnum.None);


        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {

            PythonAILibManager? libManager = PythonAILibManager.Instance;

            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResponse? result = null;
                // プログレスバーを表示
                CommonViewModelProperties.UpdateIndeterminate(true);

                ObservableCollection<ContentItemViewModel> itemViewModels = MergeTargetPanelViewModel.MergeTargetDataGridViewControlViewModel.CheckedItemsInMergeTargetSelectedDataGrid;
                // itemViewModelsからContentItemをSelect
                List<ContentItemWrapper> items = itemViewModels.Select(x => x.ContentItem).ToList();
                // チャット内容を更新
                await Task.Run(async () => {
                    ChatRequestContext chatRequestContext = CreateChatRequestContext();
                    // SplitModeが有効な場合で、PromptTextが空の場合はエラー
                    if (_splitMode != SplitModeEnum.None && string.IsNullOrEmpty(PreProcessPromptText)) {
                        LogWrapper.Error(CommonStringResources.Instance.PromptTextIsNeededWhenSplitModeIsEnabled);
                        CommonViewModelProperties.UpdateIndeterminate(false);
                        return;
                    }
                    // MergeChatUtil.MergeChatを実行
                    result = await MergeChatUtil.MergeChat(chatRequestContext, items, PreProcessPromptText, PostProcessPromptText, SessionToken, [.. ExportItems]);
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


                    OutputFolder.Folder.AddItem(contentItemWrapper, true, (item) => {
                        CommonViewModelProperties.UpdateIndeterminate(false);
                        LogWrapper.Info(CommonStringResources.Instance.SavedChatResult);
                        // OutputFolderを再読み込みした後、Closeを実行
                        OutputFolder.LoadFolderCommand.Execute();
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

        // Splitモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> SplitModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            SplitMode = comboBox.SelectedIndex;
            // SplitMOdeVisibility
            OnPropertyChanged(nameof(SplitMOdeVisibility));

        });
        // RAGモードが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> RAGModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            RAGMode = comboBox.SelectedIndex;
            // VectorDBItemVisibility
            OnPropertyChanged(nameof(VectorDBItemVisibility));
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
                // タブが変更されたときの処理
                MergeTargetPanelViewModel.UpdateCheckedItems();
                // リクエストのメッセージをアップデート
                ChatRequestContext chatRequestContext = CreateChatRequestContext();
                ChatRequest request = new() {
                    // request.Temperature;
                    Temperature = Temperature
                };
                ChatUtil.PrepareNormalRequest(chatRequestContext, request);
                // SelectedTabIndexを更新
                SelectedTabIndex = tabControl.SelectedIndex;
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
            FolderSelectWindow.OpenFolderSelectWindow(RootFolderViewModelContainer.FolderViewModels, (folderViewModel, isSelect) => {
                if (isSelect) {
                    OutputFolder = folderViewModel;
                }
            });
        });

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
                RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    VectorSearchProperties.Add(vectorDBItemBase);
                });

            OnPropertyChanged(nameof(VectorSearchProperties));
        });

        // ExportItems
        public ObservableCollection<ExportImportItem> ExportItems { get; set; } = CreateExportItems();

        private static ObservableCollection<ExportImportItem> CreateExportItems() {
            // PromptItemの設定 出力タイプがテキストコンテンツのものを取得
            List<PromptItem> promptItems = PromptItem.GetPromptItems().Where(item => item.PromptResultType == PromptResultTypeEnum.TextContent).ToList();

            ObservableCollection<ExportImportItem> items = [
                new ExportImportItem("Properties", CommonStringResources.Instance.Properties, true, false),
                new ExportImportItem("Text", CommonStringResources.Instance.Text, true, false),
            ];
            foreach (PromptItem promptItem in promptItems) {
                items.Add(new ExportImportItem(promptItem.Name, promptItem.Description, false, true));
            }
            return items;
        }


    }

}
