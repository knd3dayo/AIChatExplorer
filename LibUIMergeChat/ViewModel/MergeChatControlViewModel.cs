using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.ViewModel;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.ViewModel.PromptTemplate;
using LibUIPythonAI.ViewModel.VectorDB;
using LibUIPythonAI.View.PromptTemplate;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.Utils.Python;
using LibUIPythonAI.Resource;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Folder;
using LibUIMergeChat.Common;
using LibUIPythonAI.Utils;
using LibPythonAI.Utils.Common;
using LibPythonAI.Model.Content;
using LibPythonAI.Data;

namespace LibUIMergeChat.ViewModel {
    public class MergeChatControlViewModel : ChatViewModelBase {


        //初期化
        public MergeChatControlViewModel(MergeTargetPanelViewModel mergeTargetPanelViewModel) {

            // VectorDBItemsを設定 ClipboardFolderのベクトルDBを取得
            VectorSearchProperties = [.. mergeTargetPanelViewModel.MergeTargetTreeViewControlViewModel.SelectedFolder?.Folder.GetVectorSearchProperties()];

            // AutoGenPropertiesを設定
            _autoGenProperties = new();
            _autoGenProperties.AutoGenDBPath = PythonAILibManager.Instance.ConfigParams.GetAutoGenDBPath();
            _autoGenProperties.WorkDir = PythonAILibManager.Instance.ConfigParams.GetAutoGenWorkDir();
            _autoGenProperties.VenvPath = PythonAILibManager.Instance.ConfigParams.GetPathToVirtualEnv();

            // AutoGenGroupChatを設定
            SelectedAutoGenGroupChat = AutoGenGroupChat.GetAutoGenChatList().FirstOrDefault();

            // MergeTargetPanelViewModelを設定
            MergeTargetPanelViewModel = mergeTargetPanelViewModel;

            // OutputFolderPath
            OutputFolder = mergeTargetPanelViewModel.MergeTargetTreeViewControlViewModel.SelectedFolder;

        }

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

        public static ChatMessage? SelectedItem { get; set; }


        private ObservableCollection<VectorDBProperty> _vectorSearchProperties = [];
        public ObservableCollection<VectorDBProperty> VectorSearchProperties {
            get {
                return _vectorSearchProperties;
            }
            set {
                _vectorSearchProperties = value;
                OnPropertyChanged(nameof(VectorSearchProperties));
            }
        }

        private VectorDBProperty? _selectedVectorSearchProperty = null;
        public VectorDBProperty? SelectedVectorSearchProperty {
            get {
                return _selectedVectorSearchProperty;
            }
            set {
                _selectedVectorSearchProperty = value;
                OnPropertyChanged(nameof(SelectedVectorSearchProperty));
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
                    List<VectorDBProperty> items = [.. MergeTargetPanelViewModel.MergeTargetTreeViewControlViewModel.SelectedFolder?.Folder.GetVectorSearchProperties()];
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

        private ChatRequestContext CreateChatRequestContext() {
            int splitTokenCount = int.Parse(SplitTokenCount);
            // ベクトルDB検索結果最大値をVectorSearchPropertyに設定
            foreach (var item in VectorSearchProperties) {
                item.TopK = VectorDBSearchResultMax;
            }
            ChatRequestContext chatRequestContext = ChatRequestContext.CreateDefaultChatRequestContext(
                _chatMode, _splitMode, splitTokenCount, UseVectorDB, [.. VectorSearchProperties], AutoGenProperties, PreProcessPromptText
                );
            return chatRequestContext;
        }

        //
        public Visibility VectorDBItemVisibility => Tools.BoolToVisibility(UseVectorDB);

        public Visibility SplitMOdeVisibility => Tools.BoolToVisibility(_splitMode != SplitOnTokenLimitExceedModeEnum.None);


        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {

            PythonAILibManager? libManager = PythonAILibManager.Instance;

            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                ChatResult? result = null;
                // プログレスバーを表示
                UpdateIndeterminate(true);

                ObservableCollection<ContentItemViewModel> itemViewModels = MergeTargetPanelViewModel.MergeTargetDataGridViewControlViewModel.CheckedItemsInMergeTargetSelectedDataGrid;
                // itemViewModelsからContentItemをSelect
                List<ContentItemWrapper> items = itemViewModels.Select(x => x.ContentItem).ToList();
                // チャット内容を更新
                await Task.Run(() => {
                    ChatRequestContext chatRequestContext = CreateChatRequestContext();
                    // SplitModeが有効な場合で、PromptTextが空の場合はエラー
                    if (_splitMode != SplitOnTokenLimitExceedModeEnum.None && string.IsNullOrEmpty(PreProcessPromptText)) {
                        LogWrapper.Error(StringResources.PromptTextIsNeededWhenSplitModeIsEnabled);
                        UpdateIndeterminate(false);
                        return;
                    }
                    // MergeChatUtil.MergeChatを実行
                    result = MergeChatUtil.MergeChat(chatRequestContext, items, PreProcessPromptText, PostProcessPromptText, [.. ExportItems]);
                });

                if (result == null) {
                    LogWrapper.Error(StringResources.FailedToSendChat);
                    UpdateIndeterminate(false);
                    return;
                }
                // チャット結果をOutputFolderに保存
                if (OutputFolder != null) {
                    ContentItemEntity contentItem = new() {
                        Content = result.Output,
                    };
                    ContentItemWrapper contentItemWrapper = new(contentItem) {
                        SourceType = ContentSourceType.Application
                    };


                    OutputFolder.Folder.AddItem(contentItemWrapper, true, (item) => {
                        UpdateIndeterminate(false);
                        LogWrapper.Info(StringResources.SavedChatResult);
                        // OutputFolderを再読み込みした後、Closeを実行
                        OutputFolder.LoadFolderCommand.Execute();
                        // Close
                        MainUITask.Run(() => {
                            CloseCommand.Execute();
                        });
                    });
                }


            } catch (Exception e) {
                LogWrapper.Error($"{StringResources.ErrorOccurredAndMessage}:\n{e.Message}\n{StringResources.StackTrace}:\n{e.StackTrace}");
            }

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
