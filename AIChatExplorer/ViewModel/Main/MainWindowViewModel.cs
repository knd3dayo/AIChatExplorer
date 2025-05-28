using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.Model.Item;
using AIChatExplorer.Model.Main;
using AIChatExplorer.View.Help;
using AIChatExplorer.View.Main;
using AIChatExplorer.ViewModel.Folders.Search;
using AIChatExplorer.ViewModel.Settings;
using LibPythonAI.Data;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.VectorDB;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.AutoGen;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;

namespace AIChatExplorer.ViewModel.Main {
    public partial class MainWindowViewModel : CommonViewModelBase {
        public MainWindowViewModel() {


            Instance = this;

            // PythonAILibManagerの初期化
            InitPythonAILibManager();

            // Commandの初期化
            Commands = new(UpdateIndeterminate, () => {
                // 現在選択中のフォルダをReload
                MainUITask.Run(() => {
                    MainPanelTreeViewControlViewModel?.SelectedFolder?.LoadFolderCommand.Execute();
                });
            });

            // フォルダの初期化
            RootFolderViewModelContainer = new(Commands);

            // ClipboardControllerのOnClipboardChangedに処理をセット
            ClipboardController.Instance.OnClipboardChanged = (e) => {
                // CopiedItemsをクリア
                ClipboardController.Instance.CopiedObjects.Clear();
            };


            // MainPanelDataGridViewControlViewModelの初期化
            MainPanelDataGridViewControlViewModel = new(Commands) {
                UpdateIndeterminateAction = UpdateIndeterminate,

            };

            // MainPanelTreeViewControlViewModelの初期化
            MainPanelTreeViewControlViewModel = new(Commands) {
                UpdateIndeterminateAction = UpdateIndeterminate,
                RootFolderViewModelContainer = RootFolderViewModelContainer,
                SelectedFolderChangedAction = ((selectedFolder) => {
                    MainPanelDataGridViewControlViewModel.SelectedFolder = selectedFolder;
                })
            };

            MainPanelViewModel mainPanelViewModel = new(Commands) {
                MainPanelTreeViewControlViewModel = MainPanelTreeViewControlViewModel,
                MainPanelDataGridViewControlViewModel = MainPanelDataGridViewControlViewModel
            };

            // TabItemsの初期化
            MainPanel mainPanel = new() {
                DataContext = mainPanelViewModel
            };

            MainTabContent container = new("main", mainPanel) {
                CloseButtonVisibility = Visibility.Collapsed
            };
            MainTabManager.TabItems.Add(container);

        }

        public AppViewModelCommands Commands { get; set; }
        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; }

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; }



        // Null許容型
        [AllowNull]
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

        /// <summary>
        /// 外部からプロパティの変更を通知する
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName) {
            OnPropertyChanged(propertyName);
        }

        // PythonAILibManagerの初期化
        private static void InitPythonAILibManager() {
            //　環境変数HTTP_PROXY,HTTPS_PROXYの設定
            if (!string.IsNullOrEmpty(AIChatExplorerConfig.Instance.ProxyURL)) {
                Environment.SetEnvironmentVariable("HTTP_PROXY", AIChatExplorerConfig.Instance.ProxyURL);
                Environment.SetEnvironmentVariable("HTTPS_PROXY", AIChatExplorerConfig.Instance.ProxyURL);
            }
            // 環境変数NO_PROXYの設定
            if (!string.IsNullOrEmpty(AIChatExplorerConfig.Instance.NoProxyList)) {
                Environment.SetEnvironmentVariable("NO_PROXY", AIChatExplorerConfig.Instance.NoProxyList);
            }

            AIChatExplorerPythonAILibConfigParams configParams = new();
            PythonAILibManager.Init(configParams);


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
            AppViewModelCommands.OpenListPromptTemplateWindowCommandExecute();
        });
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListAutoProcessRuleWindowCommand => new((parameter) => {
            AppViewModelCommands.OpenListAutoProcessRuleWindowCommandExecute();
        });

        // メニューの「タグ編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenTagWindowCommand => new((parameter) => {
            AppViewModelCommands.OpenTagWindowCommandExecute();
        });
        // メニューの「AutoGen定義編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListAutoGenItemWindowCommand => new((parameter) => {
            ListAutoGenItemWindow.OpenListAutoGenItemWindow(LibUIPythonAI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels);
        });

        // バージョン情報画面を開く処理
        public SimpleDelegateCommand<object> OpenVersionInfoCommand => new((parameter) => {
            VersionWindow.OpenVersionWindow();
        });

        // メニューの「OpenAIチャット」をクリックしたときの処理。
        // チャット履歴フォルダーに新規作成
        public SimpleDelegateCommand<object> OpenOpenAIWindowCommand => new((parameter) => {

            AppViewModelCommands.OpenOpenAIChatWindowCommandExecute();

        });

        // メニューの「AutoGenチャット」をクリックしたときの処理。
        // チャット履歴フォルダーに新規作成
        public SimpleDelegateCommand<object> OpenAutoGenChatWindow => new((parameter) => {

            AppViewModelCommands.OpenAutoGenChatWindowCommandExecute();

        });

        // OpenImageChatWindow
        public SimpleDelegateCommand<object> OpenImageChatWindow => new((parameter) => {
            // チャット履歴フォルダーに新規作成
            ApplicationItem dummyItem = new(RootFolderViewModelContainer.ChatRootFolderViewModel.Folder.Entity);
            AppViewModelCommands.OpenImageChatWindowCommand(dummyItem, () => {
                RootFolderViewModelContainer.ChatRootFolderViewModel.LoadFolderCommand.Execute();
            });
        });

        // OpenMergeChatWindow
        public SimpleDelegateCommand<object> OpenFolderMergeChatWindow => new((parameter) => {

            ContentFolderViewModel folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            AppViewModelCommands.OpenMergeChatWindowCommand(folderViewModel, []);

        });

        public SimpleDelegateCommand<object> OpenSelectedItemsMergeChatWindow => new((parameter) => {
            ContentFolderViewModel folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            ObservableCollection<ContentItemViewModel> selectedItems = [.. MainPanelDataGridViewControlViewModel.SelectedItems];
            AppViewModelCommands.OpenMergeChatWindowCommand(folderViewModel, selectedItems);

        });


        // OpenVectorSearchWindowCommand メニューの「ベクトル検索」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorSearchWindowCommand => new((parameter) => {
            ContentFolderViewModel folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            AppViewModelCommands.OpenFolderVectorSearchWindowCommandExecute(folderViewModel);
        });


        // OpenVectorDBManagementWindowCommandメニュー　「ベクトルDB管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorDBManagementWindowCommand => new((parameter) => {
            AppViewModelCommands.OpenVectorDBManagementWindowCommand();
        });

        // メニューの「設定」をクリックしたときの処理
        public SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            AppViewModelCommands.SettingCommandExecute();
        });


        #region Window全体のInputBinding用のコマンド
        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand<object> SearchCommand => new((parameter) => {
            // 子フォルダを作成
            SearchFolder folder = FolderManager.SearchRootFolder.CreateChild("New Folder");

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(folder, Commands);

            AppViewModelCommands.OpenSearchWindowCommandExecute(searchFolderViewModel, () => {
                // 保存と再読み込み
                searchFolderViewModel.ParentFolderViewModel = MainPanelTreeViewControlViewModel.RootFolderViewModelContainer.SearchRootFolderViewModel;
                searchFolderViewModel.SaveFolderCommand.Execute(null);
                // 親フォルダを保存
                MainPanelTreeViewControlViewModel.RootFolderViewModelContainer.SearchRootFolderViewModel.SaveFolderCommand.Execute(null);
                // Load
                MainPanelTreeViewControlViewModel.RootFolderViewModelContainer.SearchRootFolderViewModel.LoadFolderExecute(
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

        #endregion


    }

}
