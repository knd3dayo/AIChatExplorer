using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Folders.ClipboardHistory;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.View.Help;
using AIChatExplorer.View.Main;
using AIChatExplorer.ViewModel.Content;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Folders.Search;
using LibPythonAI.Model.Folders;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Main {
    public partial class MainWindowViewModel : CommonViewModelBase {
        public MainWindowViewModel() {
            Instance = this;

            // Commandの初期化
            Commands = InitCommands();

            // フォルダの初期化
            RootFolderViewModelContainer = new(Commands);

            // ClipboardControllerのOnClipboardChangedに処理をセット
            InitClipboardController();

            // MainPanelDataGridViewControlViewModel,MainPanelTreeViewControlViewModelの初期化
            (MainPanelDataGridViewControlViewModel, MainPanelTreeViewControlViewModel) = InitMainPanelControls();

            // パネルの初期化
            InitPanel();
        }

        // Commandsの初期化
        private AppViewModelCommandExecutes InitCommands() {
            AppViewModelCommandExecutes commands = new(UpdateIndeterminate, () => {
                // 現在選択中のフォルダをReload
                MainUITask.Run(() => {
                    MainPanelTreeViewControlViewModel?.SelectedFolder?.FolderCommands.LoadFolderCommand.Execute();
                });
            });
            return commands;
        }
        // ClipboardControllerの初期化
        private void InitClipboardController() {
            // ClipboardControllerの初期化
            ClipboardController.Instance.OnClipboardChanged = (e) => {
                // CopiedItemsをクリア
                ClipboardController.Instance.CopiedObjects.Clear();
            };
        }
        private (MainPanelDataGridViewControlViewModel, MainPanelTreeViewControlViewModel) InitMainPanelControls() {
            // MainPanelDataGridViewControlViewModelの初期化
            MainPanelDataGridViewControlViewModel dataGridViewModel = new(Commands) {
                UpdateIndeterminateAction = UpdateIndeterminate,
            };
            // MainPanelTreeViewControlViewModelの初期化
            MainPanelTreeViewControlViewModel treeViewModel = new(Commands) {
                UpdateIndeterminateAction = UpdateIndeterminate,
                RootFolderViewModelContainer = RootFolderViewModelContainer,
                SelectedFolderChangedAction = ((selectedFolder) => {
                    MainPanelDataGridViewControlViewModel.SelectedFolder = selectedFolder;
                })
            };
            return (dataGridViewModel, treeViewModel);
        }
        private void InitPanel() {
            MainPanelViewModel mainPanelViewModel = new(Commands) {
                MainPanelTreeViewControlViewModel = MainPanelTreeViewControlViewModel,
                MainPanelDataGridViewControlViewModel = MainPanelDataGridViewControlViewModel
            };

            MainPanel mainPanel = new() {
                DataContext = mainPanelViewModel
            };

            // TabItemsの初期化
            MainTabContent container = new("main", mainPanel) {
                CloseButtonVisibility = Visibility.Collapsed
            };
            MainTabManager.TabItems.Add(container);
        }

        public AppViewModelCommandExecutes Commands { get; set; }

        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; }

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; }

        // Null許容型
        public FolderViewModelManager RootFolderViewModelContainer { get; set; }

        [AllowNull]
        public static MainWindowViewModel Instance { get; set; }

        public MainTabManager MainTabManager { get; } = new();

        // クリップボード監視が実行中であるかどうか
        public bool IsClipboardMonitoringActive { get; set; } = false;


        // クリップボード監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string ClipboardMonitorButtonText {
            get {
                return IsClipboardMonitoringActive ? CommonStringResources.Instance.StopClipboardWatch : CommonStringResources.Instance.StartClipboardWatch;
            }
        }

        // 画面監視が実行中であるかどうか
        public bool IsScreenMonitoringActive { get; set; } = false;

        // 画面監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string ScreenMonitorButtonText {
            get {
                return IsScreenMonitoringActive ? CommonStringResources.Instance.StopScreenWatch : CommonStringResources.Instance.StartScreenWatch;
            }
        }
        // IsIntegratedMonitorActive
        public bool IsIntegratedMonitorActive { get; set; } = false;
        // 監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string IntegratedMonitorButtonText {
            get {
                return IsIntegratedMonitorActive ? CommonStringResources.Instance.StopIntegratedMonitorWatch : CommonStringResources.Instance.StartIntegratedMonitorWatch;
            }
        }

        /// <summary>
        /// 外部からプロパティの変更を通知する
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName) {
            OnPropertyChanged(propertyName);
        }

        // プログレスインジケータ表示更新用のアクション

        public void UpdateIndeterminate(bool visible) {
            CommonViewModelProperties.UpdateIndeterminate(visible);
            if (visible == false) {
                OnPropertyChanged(nameof(MainPanelDataGridViewControlViewModel.SelectedItem));
            }
        }

        // アイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public SimpleDelegateCommand<object> CreateItemCommand => MainPanelTreeViewControlViewModel.CreateItemCommand;

        // メニューの「フォルダ作成」をクリックしたときの処理
        public SimpleDelegateCommand<object> CreateFolderCommand => MainPanelTreeViewControlViewModel.CreateFolderCommand;

        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListPromptTemplateWindowCommand => new((parameter) => {
            AppViewModelCommandExecutes.OpenListPromptTemplateWindowCommandExecute();
        });
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListAutoProcessRuleWindowCommand => new((parameter) => {
            AppViewModelCommandExecutes.OpenListAutoProcessRuleWindowCommandExecute();
        });

        // メニューの「タグ編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenTagWindowCommand => new((parameter) => {
            AppViewModelCommandExecutes.OpenTagWindowCommandExecute();
        });


        // バージョン情報画面を開く処理
        public SimpleDelegateCommand<object> OpenVersionInfoCommand => new((parameter) => {
            VersionWindow.OpenVersionWindow();
        });

        // OpenImageChatWindow
        public SimpleDelegateCommand<object> OpenImageChatWindow => new((parameter) => {
            // チャット履歴フォルダーに新規作成
            var viewModel = RootFolderViewModelContainer.ChatRootFolderViewModel;
            if (viewModel == null) {
                // チャット履歴フォルダーが存在しない場合は、何もしない
                return;
            }
            ApplicationItem dummyItem = new(viewModel.Folder.Entity);
            AppViewModelCommandExecutes.OpenImageChatWindowCommand(dummyItem, () => {
                viewModel.FolderCommands.LoadFolderCommand.Execute();
            });
        });

        // OpenMergeChatWindow
        public SimpleDelegateCommand<object> OpenFolderMergeChatWindow => new((parameter) => {

            ContentFolderViewModel? folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.GetApplicationRootFolderViewModel();
            if (folderViewModel == null) {
                // フォルダーが選択されていない場合は、何もしない
                return;
            }
            AppViewModelCommandExecutes.OpenMergeChatWindowCommand(folderViewModel, []);

        });

        // OpenNormalChatWindow
        public SimpleDelegateCommand<object> OpenNormalChatWindow => new((parameter) => {
            // チャット履歴用のItemの設定
            ApplicationFolderViewModel? chatFolderViewModel = MainWindowViewModel.Instance.RootFolderViewModelContainer.ChatRootFolderViewModel;
            if (chatFolderViewModel == null) {
                // チャット履歴フォルダーが存在しない場合は、何もしない
                return;
            }
            // チャット履歴用のItemの設定
            ApplicationItem item = new(chatFolderViewModel.Folder.Entity) {
                // TEMPORARY_ITEM_ID
                Id = ApplicationItem.TEMPORARY_ITEM_ID,
                // タイトルを日付 + 元のタイトルにする
                Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + CommonStringResources.Instance.ChatHeader + CommonStringResources.Instance.NoTitle
            };
            ApplicationItemViewModel applicationItemViewModel = new(chatFolderViewModel, item);

            AppViewModelCommandExecutes.OpenNormalChatWindowCommandExecute(applicationItemViewModel);
        });

        public SimpleDelegateCommand<object> OpenSelectedItemsMergeChatWindow => new((parameter) => {
            ContentFolderViewModel? folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.GetApplicationRootFolderViewModel();
            if (folderViewModel == null) {
                // フォルダーが選択されていない場合は、何もしない
                return;
            }
            ObservableCollection<ContentItemViewModel> selectedItems = [.. MainPanelDataGridViewControlViewModel.SelectedItems];
            AppViewModelCommandExecutes.OpenMergeChatWindowCommand(folderViewModel, selectedItems);

        });


        // OpenVectorSearchWindowCommand メニューの「ベクトル検索」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorSearchWindowCommand => new(async (parameter) => {
            ContentFolderViewModel? folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.GetApplicationRootFolderViewModel();
            if (folderViewModel == null) {
                // フォルダーが選択されていない場合は、何もしない
                return;
            }
            await AppViewModelCommandExecutes.OpenFolderVectorSearchWindowCommandExecute(folderViewModel);
        });


        // OpenVectorDBManagementWindowCommandメニュー　「ベクトルDB管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorDBManagementWindowCommand => new((parameter) => {
            AppViewModelCommandExecutes.OpenVectorDBManagementWindowCommand();
        });

        // メニューの「設定」をクリックしたときの処理
        public SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            AppViewModelCommandExecutes.SettingCommandExecute();
        });


        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand<object> SearchCommand => new( (parameter) => {
            // 子フォルダを作成
            var parentFolder =  FolderManager.GetSearchRootFolder();
            SearchFolder folder = parentFolder.CreateChild("New Folder");

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(folder, Commands);

            AppViewModelCommandExecutes.OpenSearchWindowCommandExecute(searchFolderViewModel, async () => {
                // 保存と再読み込み
                searchFolderViewModel.ParentFolderViewModel = MainPanelTreeViewControlViewModel.RootFolderViewModelContainer.GetSearchRootFolderViewModel();
                searchFolderViewModel.FolderCommands.SaveFolderCommand.Execute(null);
                // 親フォルダを保存
                MainPanelTreeViewControlViewModel.RootFolderViewModelContainer.GetSearchRootFolderViewModel()?.FolderCommands.SaveFolderCommand.Execute(null);
                // Load
                await MainPanelTreeViewControlViewModel.RootFolderViewModelContainer.GetSearchRootFolderViewModel().LoadFolderExecuteAsync(
                () => {
                    Commands.UpdateIndeterminate(true);
                },
                () => {
                    MainUITask.Run(() => {
                        Commands.UpdateIndeterminate(false);

                        MainPanelTreeViewControlViewModel.SelectedTreeViewItemChangeCommandExecute(searchFolderViewModel);
                        // SelectedFolder に　SearchFolderViewModelを設定
                        MainPanelTreeViewControlViewModel.SelectedFolder = searchFolderViewModel;
                    });
                });

            });
        });
    }

}
