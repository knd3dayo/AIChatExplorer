using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ClipboardApp.Common;
using ClipboardApp.Factory;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Item;
using ClipboardApp.Settings;
using ClipboardApp.View.Help;
using ClipboardApp.View.Main;
using ClipboardApp.ViewModel.Common;
using ClipboardApp.ViewModel.Folders.Clipboard;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;
using QAChat;
using QAChat.Resource;
using QAChat.View.AutoGen;
using QAChat.View.AutoProcessRule;
using QAChat.View.PromptTemplate;
using QAChat.View.Tag;
using QAChat.ViewModel.PromptTemplate;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Main {
    public partial class MainWindowViewModel : ClipboardAppViewModelBase {
        public MainWindowViewModel() { }
        public void Init() {

            /**
#if DEBUG
            System.Diagnostics.PresentationTraceSources.DataBindingSource.Switch.Level = System.Diagnostics.SourceLevels.Critical;
#endif
            **/

            Instance = this;

            //　環境変数HTTP_PROXY,HTTPS_PROXYの設定
            if (!string.IsNullOrEmpty(ClipboardAppConfig.Instance.ProxyURL)) {
                Environment.SetEnvironmentVariable("HTTP_PROXY", ClipboardAppConfig.Instance.ProxyURL);
                Environment.SetEnvironmentVariable("HTTPS_PROXY", ClipboardAppConfig.Instance.ProxyURL);
            }
            // 環境変数NO_PROXYの設定
            if (!string.IsNullOrEmpty(ClipboardAppConfig.Instance.NoProxyList)) {
                Environment.SetEnvironmentVariable("NO_PROXY", ClipboardAppConfig.Instance.NoProxyList);
            }

            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };
            ClipboardAppPythonAILibConfigParams configParams = new();
            PythonAILibManager.Init(configParams);
            QAChatManager.Init(configParams);
            // フォルダの初期化
            RootFolderViewModelContainer = new();

            // データベースのチェックポイント処理
            ClipboardAppFactory.Instance.GetClipboardDBController().GetDatabase().Checkpoint();

            // DBのバックアップの取得
            BackupController.BackupNow();

            // ClipboardControllerのOnClipboardChangedに処理をセット
            ClipboardController.Instance.OnClipboardChanged = (e) => {
                // CopiedItemsをクリア
                ClipboardController.Instance.CopiedObjects.Clear();
            };

            // MainPanelDataGridViewControlViewModelの初期化
            MainPanelDataGridViewControlViewModel = new() {
                UpdateIndeterminateAction = UpdateIndeterminate,

            };
            // MainPanelTreeViewControlViewModelの初期化
            MainPanelTreeViewControlViewModel = new() {
                UpdateIndeterminateAction = UpdateIndeterminate,
                RootFolderViewModelContainer = RootFolderViewModelContainer,
                SelectedFolderChangedAction = ((selectedFolder) => {
                    MainPanelDataGridViewControlViewModel.SelectedFolder = selectedFolder;
                })
            };

            MainPanelViewModel mainPanelViewModel = new() {
                MainPanelTreeViewControlViewModel = MainPanelTreeViewControlViewModel,
                MainPanelDataGridViewControlViewModel = MainPanelDataGridViewControlViewModel
            };

            // TabItemsの初期化
            MainPanel mainPanel = new() {
                DataContext = mainPanelViewModel
            };

            ClipboardAppTabContainer container = new("main", mainPanel) {
                CloseButtonVisibility = Visibility.Collapsed
            };
            TabItems.Add(container);


            // AutoGenPropertiesの初期化
            AutoGenProperties.Init();

        }

        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; } = new();

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; } = new();

        public ObservableCollection<ClipboardAppTabContainer> TabItems { get; set; } = [];


        // メインウィンドウにアイテムのタブを追加
        public void AddTabItem(ClipboardAppTabContainer tabItem) {
            if (ThisWindow == null) {
                return;
            }
            // ClipboardAppTabContainerのHeaderWidthを設定. 現在のタブ数 * ClipboardAppTabContainerのHeaderWidth > ThisWindow.Widthの場合はThisWindow.Widthを超えないようにする
            double tabControlWidth = ThisWindow.ActualWidth - 500;
            if ((TabItems.Count + 1) * ClipboardAppTabContainer.HeaderWidthStatic > tabControlWidth) {
                ClipboardAppTabContainer.HeaderWidthStatic = tabControlWidth / (TabItems.Count + 1);
                for (int i = 1; i < TabItems.Count; i++) {
                    TabItems[i].HeaderWidth = ClipboardAppTabContainer.HeaderWidthStatic;
                }
            }

            TabItems.Add(tabItem);
            OnPropertyChanged(nameof(TabItems));
            SelectedTabItem = tabItem;
        }
        // メインウィンドウからアイテムのタブを削除

        public void RemoveTabItem(ClipboardAppTabContainer tabItem) {
            TabItems.Remove(tabItem);
            OnPropertyChanged(nameof(TabItems));
        }

        // SelectedTabItem
        private ClipboardAppTabContainer? _selectedTabItem;
        public ClipboardAppTabContainer? SelectedTabItem {
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
        public ClipboardAppRootFolderViewModelContainer RootFolderViewModelContainer { get; set; }

        public static MainWindowViewModel Instance { get; set; } = new MainWindowViewModel();

        // プログレスインジケータ表示更新用のアクション
        // 
        public static Action<bool> UpdateProgressCircleVisibility { get; set; } = (visible) => { };

        public bool IsIndeterminate { get; private set; } = false;
        public void UpdateIndeterminate(bool visible) {
            IsIndeterminate = visible;
            OnPropertyChanged(nameof(IsIndeterminate));
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

        public SimpleDelegateCommand<ClipboardAppTabContainer> CloseTabCommand => new((tabItem) => {
            if (tabItem == null) {
                return;
            }
            if (TabItems.Count == 1) {
                return;
            }
            TabItems.Remove(tabItem);
            OnPropertyChanged(nameof(TabItems));
        });

        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleClipboardMonitor => new((parameter) => {
            ClipboardItemViewModelCommands.StartStopClipboardMonitorCommand();
        });


        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public static void OpenListPromptTemplateWindowCommandExecute(MainWindowViewModel windowViewModel) {
            // ListPromptTemplateWindowを開く
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Edit, (promptTemplateWindowViewModel, OpenAIExecutionModeEnum) => {
                // PromptTemplate = promptTemplateWindowViewModel.ClipboardPromptItem;
            });
        }
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public void OpenListAutoProcessRuleWindowCommandExecute() {
            // ListAutoProcessRuleWindowを開く
            ListAutoProcessRuleWindow.OpenListAutoProcessRuleWindow(PythonAILibUI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels);

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
            ListAutoGenItemWindow.OpenListAutoGenItemWindow(PythonAILibUI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels);
        });

        // バージョン情報画面を開く処理
        public SimpleDelegateCommand<object> OpenVersionInfoCommand => new((parameter) => {
            VersionWindow.OpenVersionWindow();
        });

        // OpenOpenAIWindowCommandExecute メニューの「OpenAIチャット」をクリックしたときの処理。
        // チャット履歴フォルダーに新規作成
        public SimpleDelegateCommand<object> OpenOpenAIWindowCommand => new((parameter) => {

            ClipboardItemViewModelCommands commands = new();

            commands.OpenOpenAIChatWindowCommand(null);

        });


        // OpenImageChatWindow
        public SimpleDelegateCommand<object> OpenImageChatWindow => new((parameter) => {
            // チャット履歴フォルダーに新規作成
            ClipboardItem dummyItem = new(RootFolderViewModelContainer.ChatRootFolderViewModel.Folder.Id);
            ClipboardItemViewModelCommands commands = new();
            commands.OpenImageChatWindowCommand(dummyItem, () => {
                RootFolderViewModelContainer.ChatRootFolderViewModel.LoadFolderCommand.Execute();
            });
        });

        // OpenMergeChatWindow
        public SimpleDelegateCommand<object> OpenMergeChatWindow => new((parameter) => {
            ClipboardItemViewModelCommands commands = new();
            ClipboardItem dummyItem = new(RootFolderViewModelContainer.ChatRootFolderViewModel.Folder.Id);
            commands.OpenMergeChatWindowCommand(dummyItem, () => {
                RootFolderViewModelContainer.ChatRootFolderViewModel.LoadFolderCommand.Execute();
            });
        });


        // OpenVectorSearchWindowCommand メニューの「ベクトル検索」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorSearchWindowCommand => new((parameter) => {
            ClipboardFolderViewModel folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            ClipboardItemViewModelCommands commands = new();
            commands.OpenVectorSearchWindowCommand(folderViewModel.Folder);
        });

        // OpenRAGManagementWindowCommandメニュー　「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenRAGManagementWindowCommand => new((parameter) => {
            ClipboardItemViewModelCommands commands = new();
            commands.OpenRAGManagementWindowCommand();
        });
        // OpenVectorDBManagementWindowCommandメニュー　「ベクトルDB管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorDBManagementWindowCommand => new((parameter) => {
            ClipboardItemViewModelCommands commands = new();
            commands.OpenVectorDBManagementWindowCommand();
        });

        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            ClipboardItemViewModelCommands commands = new();
            commands.SettingCommandExecute();
        });

        #endregion


        #region Window全体のInputBinding用のコマンド
        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand<object> SearchCommand => new((parameter) => {

            ClipboardFolderViewModel folderViewModel = MainPanelTreeViewControlViewModel.SelectedFolder ?? RootFolderViewModelContainer.RootFolderViewModel;
            ClipboardItemViewModelCommands commands = new();
            if (folderViewModel.Folder is not ClipboardFolder clipboardFolder) {
                return;
            }
            commands.OpenSearchWindowCommand(clipboardFolder, () => { folderViewModel.LoadFolderCommand.Execute(); });
        });

        #endregion


    }

}
