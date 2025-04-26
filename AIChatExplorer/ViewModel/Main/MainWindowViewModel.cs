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
using LibPythonAI.Model.VectorDB;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.AutoGen;
using LibUIPythonAI.View.AutoProcessRule;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.View.Tag;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.ViewModel.PromptTemplate;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;

namespace AIChatExplorer.ViewModel.Main {
    public partial class MainWindowViewModel : AppViewModelBase {
        public MainWindowViewModel() { }
        public void Init() {


            Instance = this;

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

            // VectorDBItemのLoad
            Task.Run(() => {
                VectorDBItem.LoadItems();
            });


            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };
            // Commandの初期化
            Commands = new(UpdateIndeterminate, () => {
                // 現在選択中のフォルダをReload
                MainUITask.Run(() => {
                    MainPanelTreeViewControlViewModel.SelectedFolder?.LoadFolderCommand.Execute();
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

            AppTabContainer container = new("main", mainPanel) {
                CloseButtonVisibility = Visibility.Collapsed
            };
            TabItems.Add(container);


            // AutoGenPropertiesの初期化
            AutoGenProperties.Init();

        }

        public AppViewModelCommands Commands { get; set; }
        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; }

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; }

        public ObservableCollection<AppTabContainer> TabItems { get; set; } = [];


        // メインウィンドウにアイテムのタブを追加
        public void AddTabItem(AppTabContainer tabItem) {
            if (ThisWindow == null) {
                return;
            }
            // HeaderWidthを設定. 現在のタブ数 * AIChatExplorerTabContainerのHeaderWidth > ThisWindow.Widthの場合はThisWindow.Widthを超えないようにする
            double tabControlWidth = ThisWindow.ActualWidth - 500;
            if ((TabItems.Count + 1) * AppTabContainer.HeaderWidthStatic > tabControlWidth) {
                AppTabContainer.HeaderWidthStatic = tabControlWidth / (TabItems.Count + 1);
                for (int i = 1; i < TabItems.Count; i++) {
                    TabItems[i].HeaderWidth = AppTabContainer.HeaderWidthStatic;
                }
            }

            TabItems.Add(tabItem);
            OnPropertyChanged(nameof(TabItems));
            SelectedTabItem = tabItem;
        }
        // メインウィンドウからアイテムのタブを削除

        public void RemoveTabItem(AppTabContainer tabItem) {
            TabItems.Remove(tabItem);
            OnPropertyChanged(nameof(TabItems));
        }

        // SelectedTabItem
        private AppTabContainer? _selectedTabItem;
        public AppTabContainer? SelectedTabItem {
            get {
                return _selectedTabItem;
            }
            set {
                _selectedTabItem = value;
                OnPropertyChanged(nameof(SelectedTabItem));
            }
        }

        // Null許容型
        [AllowNull]
        public FolderViewModelManager RootFolderViewModelContainer { get; set; }

        public static MainWindowViewModel Instance { get; set; } = new MainWindowViewModel();

        // プログレスインジケータ表示更新用のアクション
        // 
        public static Action<bool> UpdateProgressCircleVisibility { get; set; } = (visible) => { };

        public bool IsIndeterminate { get; private set; } = false;
        public void UpdateIndeterminate(bool visible) {
            IsIndeterminate = visible;
            OnPropertyChanged(nameof(IsIndeterminate));
            // SelectedItemを更新
            if (visible == false) {
                OnPropertyChanged(nameof(MainPanelDataGridViewControlViewModel.SelectedItem));
            }
        }

        // クリップボード監視が実行中であるかどうか
        public bool IsClipboardMonitoringActive { get; set; } = false;

        // クリップボード監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string ClipboardMonitorButtonText {
            get {
                return IsClipboardMonitoringActive ? StringResources.StopClipboardWatch : StringResources.StartClipboardWatch;
            }
        }

        /// <summary>
        /// 外部からプロパティの変更を通知する
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName) {
            OnPropertyChanged(propertyName);
        }

        public static SimpleDelegateCommand<object> ExitCommand => new((parameter) => {
            // Display exit confirmation dialog. If Yes, exit the application
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmExit, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                Application.Current.Shutdown();
            }
        });

        public SimpleDelegateCommand<AppTabContainer> CloseTabCommand => new((tabItem) => {
            if (tabItem == null || TabItems.Count == 1) {
                return;
            }
            TabItems.Remove(tabItem);
            OnPropertyChanged(nameof(TabItems));
        });

        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleClipboardMonitor => new((parameter) => {
            Commands.StartStopClipboardMonitorCommand();
        });


        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public static void OpenListPromptTemplateWindowCommandExecute(MainWindowViewModel windowViewModel) {
            // ListPromptTemplateWindowを開く
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Edit, (promptTemplateWindowViewModel, OpenAIExecutionModeEnum) => { });
        }
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public void OpenListAutoProcessRuleWindowCommandExecute() {
            // ListAutoProcessRuleWindowを開く
            ListAutoProcessRuleWindow.OpenListAutoProcessRuleWindow(LibUIPythonAI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels);

        }
        // メニューの「タグ編集」をクリックしたときの処理
        public static void OpenTagWindowCommandExecute() {
            // TagWindowを開く
            TagWindow.OpenTagWindow(null, () => { });

        }
        #region 別Windowを開く処理


        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListPromptTemplateWindowCommand => new((parameter) => {
            OpenListPromptTemplateWindowCommandExecute(this);
        });
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListAutoProcessRuleWindowCommand => new((parameter) => {
            OpenListAutoProcessRuleWindowCommandExecute();
        });
        // メニューの「タグ編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenTagWindowCommand => new((parameter) => {
            OpenTagWindowCommandExecute();
        });
        // メニューの「AutoGen定義編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListAutoGenItemWindowCommand => new((parameter) => {
            ListAutoGenItemWindow.OpenListAutoGenItemWindow(LibUIPythonAI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels);
        });

        // バージョン情報画面を開く処理
        public SimpleDelegateCommand<object> OpenVersionInfoCommand => new((parameter) => {
            VersionWindow.OpenVersionWindow();
        });

        // OpenOpenAIWindowCommandExecute メニューの「OpenAIチャット」をクリックしたときの処理。
        // チャット履歴フォルダーに新規作成
        public SimpleDelegateCommand<object> OpenOpenAIWindowCommand => new((parameter) => {

            Commands.OpenOpenAIChatWindowCommandExecute();

        });


        // OpenImageChatWindow
        public SimpleDelegateCommand<object> OpenImageChatWindow => new((parameter) => {
            // チャット履歴フォルダーに新規作成
            ClipboardItem dummyItem = new(RootFolderViewModelContainer.ChatRootFolderViewModel.Folder.Entity);
            Commands.OpenImageChatWindowCommand(dummyItem, () => {
                RootFolderViewModelContainer.ChatRootFolderViewModel.LoadFolderCommand.Execute();
            });
        });

        // OpenMergeChatWindow
        public SimpleDelegateCommand<object> OpenFolderMergeChatWindow => new((parameter) => {

            ContentFolderViewModel folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            Commands.OpenMergeChatWindowCommand(folderViewModel, []);

        });

        public SimpleDelegateCommand<object> OpenSelectedItemsMergeChatWindow => new((parameter) => {
            ContentFolderViewModel folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            ObservableCollection<ContentItemViewModel> selectedItems = [.. MainPanelDataGridViewControlViewModel.SelectedItems];
            Commands.OpenMergeChatWindowCommand(folderViewModel, selectedItems);

        });


        // OpenVectorSearchWindowCommand メニューの「ベクトル検索」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorSearchWindowCommand => new((parameter) => {
            ContentFolderViewModel folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            Commands.OpenFolderVectorSearchWindowCommandExecute(folderViewModel);
        });

        // OpenRAGManagementWindowCommandメニュー　「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenRAGManagementWindowCommand => new((parameter) => {
            Commands.OpenRAGManagementWindowCommand();
        });
        // OpenVectorDBManagementWindowCommandメニュー　「ベクトルDB管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorDBManagementWindowCommand => new((parameter) => {
            Commands.OpenVectorDBManagementWindowCommand();
        });

        // メニューの「設定」をクリックしたときの処理
        public SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            Commands.SettingCommandExecute();
        });

        #endregion


        #region Window全体のInputBinding用のコマンド
        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand<object> SearchCommand => new((parameter) => {
            // 子フォルダを作成
            SearchFolder folder = AIChatExplorerFolderManager.SearchRootFolder.CreateChild("New Folder");

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(folder, Commands);

            Commands.OpenSearchWindowCommand(searchFolderViewModel, () => {
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
