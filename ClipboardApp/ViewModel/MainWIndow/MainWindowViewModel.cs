using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ClipboardApp.Control;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.VectorDBView;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.ClipboardItemView;
using ClipboardApp.ViewModel.Folder;
using ClipboardApp.ViewModel.MainWindow;
using ClipboardApp.ViewModel.Search;
using QAChat;
using QAChat.Control;
using QAChat.Resource;
using WpfAppCommon.Control.Editor;


namespace ClipboardApp {
    public partial class MainWindowViewModel : ClipboardAppViewModelBase, IMainPanelImplementer {
        public MainWindowViewModel() { }
        public void Init() {

            ActiveInstance = this;

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
            InitClipboardFolders();

            // データベースのチェックポイント処理
            ClipboardAppFactory.Instance.GetClipboardDBController().GetDatabase().Checkpoint();

            // DBのバックアップの取得
            BackupController.BackupNow();

            // ClipboardControllerのOnClipboardChangedに処理をセット
            ClipboardController.OnClipboardChanged = (e) => {
                // CopiedItemsをクリア
                CopiedObjects.Clear();
            };
        }

        private void InitClipboardFolders() {
            RootFolderViewModel = new ClipboardFolderViewModel(ClipboardFolder.RootFolder);
            SearchRootFolderViewModel = new SearchFolderViewModel(ClipboardFolder.SearchRootFolder);
            ChatRootFolderViewModel = new ChatFolderViewModel(ClipboardFolder.ChatRootFolder);
            FolderViewModels.Add(RootFolderViewModel);
            FolderViewModels.Add(SearchRootFolderViewModel);
            FolderViewModels.Add(ChatRootFolderViewModel);

            OnPropertyChanged(nameof(FolderViewModels));
        }

        public static MainWindowViewModel ActiveInstance { get; set; } = new MainWindowViewModel();

        // RootFolderのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public ClipboardFolderViewModel RootFolderViewModel { get; private set; }

        // 検索フォルダのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public SearchFolderViewModel SearchRootFolderViewModel { get; private set; }

        // チャットフォルダのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public ChatFolderViewModel ChatRootFolderViewModel { get; private set; }


        // ClipboardController
        public static ClipboardController ClipboardController { get; } = new();

        // プログレスインジケータ表示更新用のアクション
        // 
        public static Action<bool> UpdateProgressCircleVisibility { get; set; } = (visible) => { };

        // Progress Indicatorの表示フラグ
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return _IsIndeterminate;
            }
            set {
                _IsIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
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

        // Windows通知監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string WindowsNotificationMonitorButtonText {
            get {
                return IsWindowsNotificationMonitorActive ? StringResources.StopNotificationWatch : StringResources.StartNotificationWatch;
            }
        }
        // Windows通知監視が実行中であるかどうか
        public bool IsWindowsNotificationMonitorActive { get; set; } = false;

        // AutoGen Studioが実行中であるかどうか
        public bool IsAutoGenStudioRunning { get; set; } = false;
        // AutoGen Studioが実行中の場合は「停止」、停止されている場合は「開始」を返す
        public string AutoGenStudioIsRunningButtonText {
            get {
                return IsAutoGenStudioRunning ? StringResources.StopAutoGenStudio : StringResources.StartAutoGenStudio;
            }
        }


        // ClipboardFolder

        public ObservableCollection<ClipboardFolderViewModel> FolderViewModels { get; set; } = [];


        // Cutフラグ
        public enum CutFlagEnum {
            None,
            Item,
            Folder
        }
        public CutFlagEnum CutFlag { get; set; } = CutFlagEnum.None;

        // 選択中のアイテム(複数選択)
        private ObservableCollection<ClipboardItemViewModel> _selectedItems = [];
        public ObservableCollection<ClipboardItemViewModel> SelectedItems {
            get {
                return _selectedItems;

            }
            set {
                _selectedItems = value;

                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        // 選択中のアイテム
        private ClipboardItemViewModel? _selectedItem;
        public ClipboardItemViewModel? SelectedItem {
            get {
                return _selectedItem;
            }
            set {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        // 選択中のフォルダ
        private ClipboardFolderViewModel? _selectedFolder;
        public ClipboardFolderViewModel? SelectedFolder {
            get {

                return _selectedFolder;
            }
            set {
                if (value == null) {
                    _selectedFolder = null;
                } else {
                    _selectedFolder = value;
                }
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }
        /// <summary>
        /// コピーされたアイテム
        /// </summary>
        // Ctrl + C or X が押された時のClipboardItem or ClipboardFolder
        public List<object> CopiedObjects { get; set; } = [];

        /// <summary>
        /// コピーされたアイテムのフォルダ
        /// </summary>
        // Ctrl + C or X  が押された時のClipboardItemFolder
        private ClipboardFolderViewModel? _copiedFolder;
        public ClipboardFolderViewModel? CopiedFolder {
            get {
                return _copiedFolder;
            }
            set {
                _copiedFolder = value;
                OnPropertyChanged(nameof(CopiedFolder));
            }
        }

        // テキストを右端で折り返すかどうか
        public bool TextWrapping {
            get {
                return ClipboardAppConfig.Instance.TextWrapping == System.Windows.TextWrapping.Wrap;
            }
            set {
                if (value) {
                    ClipboardAppConfig.Instance.TextWrapping = System.Windows.TextWrapping.Wrap;
                } else {
                    ClipboardAppConfig.Instance.TextWrapping = System.Windows.TextWrapping.NoWrap;
                }
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(TextWrapping));
                if (TextWrapping) {
                    CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.WrapWithThreshold;
                } else {
                    if (value) {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
                    } else {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.NoWrap;
                    }
                }
            }
        }
        // AutoTextWrapping
        public bool AutoTextWrapping {
            get {
                return ClipboardAppConfig.Instance.AutoTextWrapping;
            }
            set {
                ClipboardAppConfig.Instance.AutoTextWrapping = value;
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(AutoTextWrapping));
                // CommonViewModelBaseのTextWrappingModeを更新
                if (value) {
                    CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.WrapWithThreshold;
                } else {
                    if (TextWrapping) {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
                    } else {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.NoWrap;
                    }
                }
            }
        }

        // プレビューモード　プレビューを表示するかどうか
        public Visibility PreviewModeVisibility {
            get {
                return ClipboardAppConfig.Instance.PreviewMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        //　プレビューモード表示するかどうか
        public bool PreviewMode {
            get {
                return ClipboardAppConfig.Instance.PreviewMode;
            }
            set {
                ClipboardAppConfig.Instance.PreviewMode = value;
                // Save
                ClipboardAppConfig.Instance.Save();

                OnPropertyChanged(nameof(PreviewMode));
                OnPropertyChanged(nameof(PreviewModeVisibility));
                // アプリケーション再起動後に反映されるようにメッセージを表示
                MessageBox.Show(StringResources.DisplayModeWillChangeWhenYouRestartTheApplication, StringResources.Information, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 外部からプロパティの変更を通知する
        /// </summary>
        /// <param name="propertyName"></param>
        public void NotifyPropertyChanged(string propertyName) {
            OnPropertyChanged(propertyName);
        }

        // 開発中の機能を有効にするかどうか
        public bool EnableDevFeatures {
            get {
                return ClipboardAppConfig.Instance.EnableDevFeatures;
            }
            set {
                ClipboardAppConfig.Instance.EnableDevFeatures = value;
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
            }
        }
        // 開発中機能の表示
        public Visibility EnableDevFeaturesVisibility {
            get {
                return ClipboardAppConfig.Instance.EnableDevFeatures ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public static QAChatStartupProps CreateQAChatStartupProps(ClipboardItem clipboardItem) {

            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChatStartupProps props = new(clipboardItem) {
                // VenvPath
                VenvPath = ClipboardAppConfig.Instance.PythonVenvPath,
                // フォルダ選択アクション
                SelectVectorDBItemAction = (vectorDBItems) => {
                    SelectVectorDBWindow.OpenSelectVectorDBWindow(ActiveInstance.RootFolderViewModel, true, (selectedItems) => {
                        foreach (var item in selectedItems) {
                            vectorDBItems.Add(item);
                        }
                    });

                },
                // Saveアクション
                SaveCommand = (item, saveChatHistory) => {
                    clipboardItem = (ClipboardItem)item;
                    // ClipboardItemを保存
                    clipboardItem.Save();
                    // チャット履歴を保存するフラグが立っている場合で、チャット履歴以外のフォルダの場合
                    if (saveChatHistory && clipboardItem.GetFolder().FolderType != FolderTypeEnum.Chat) {
                        // チャット履歴用のItemの設定
                        ClipboardFolder chatFolder = MainWindowViewModel.ActiveInstance.ChatRootFolderViewModel.ClipboardItemFolder;
                        ClipboardItem chatHistoryItem = new(chatFolder.Id);
                        clipboardItem.CopyTo(chatHistoryItem);
                        // タイトルを日付 + 元のタイトルにする
                        chatHistoryItem.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";
                        if (!string.IsNullOrEmpty(clipboardItem.Description)) {
                            chatHistoryItem.Description += " " + clipboardItem.Description;
                        }
                        chatHistoryItem.Save();
                    }

                },
                // ExportChatアクション
                ExportChatCommand = (chatHistory) => {
                    ClipboardFolderViewModel? folderViewModel = MainWindowViewModel.ActiveInstance.SelectedFolder ?? MainWindowViewModel.ActiveInstance.RootFolderViewModel;

                    FolderSelectWindow.OpenFolderSelectWindow(folderViewModel, (folder) => {
                        ClipboardItem chatHistoryItem = new(folder.ClipboardItemFolder.Id);
                        // タイトルを日付 + 元のタイトルにする
                        chatHistoryItem.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";
                        if (!string.IsNullOrEmpty(clipboardItem.Description)) {
                            chatHistoryItem.Description += " " + clipboardItem.Description;
                        }
                        // chatHistoryItemの内容をテキスト化
                        string chatHistoryText = "";
                        foreach (var item in chatHistory) {
                            chatHistoryText += $"--- {item.Role} ---\n";
                            chatHistoryText += item.ContentWithSources + "\n\n";
                        }
                        chatHistoryItem.Content = chatHistoryText;
                        chatHistoryItem.Save();

                    });

                }
            };

            return props;
        }

    }

}
