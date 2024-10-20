using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.SelectVectorDBView;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.Folder;
using ClipboardApp.ViewModel.Search;
using PythonAILib.Model.Content;
using QAChat;
using QAChat.Control;


namespace ClipboardApp {
    public partial class MainWindowViewModel : ClipboardAppViewModelBase {
        public MainWindowViewModel() { }
        public void Init() {

            ActiveInstance = this;

            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };
            ClipboardAppPythonAILibConfigParams configParams = new();
            PythonAILibManager.Init(configParams);
            QAChatManager.Init(configParams);
            // フォルダの初期化
            RootFolderViewModel = new ClipboardFolderViewModel(ClipboardFolder.RootFolder);
            ImageCheckRootFolderViewModel = new ImageCheckFolderViewModel(ClipboardFolder.ImageCheckRootFolder);
            InitClipboardFolders();

            // データベースのチェックポイント処理
            ClipboardAppFactory.Instance.GetClipboardDBController().GetDatabase().Checkpoint();

            // DBのバックアップの取得
            BackupController.BackupNow();
        }

        private void InitClipboardFolders() {
            RootFolderViewModel = new ClipboardFolderViewModel(ClipboardFolder.RootFolder);
            ImageCheckRootFolderViewModel = new ImageCheckFolderViewModel(ClipboardFolder.ImageCheckRootFolder);
            SearchRootFolderViewModel = new SearchFolderViewModel(ClipboardFolder.SearchRootFolder);
            ChatRootFolderViewModel = new ChatFolderViewModel(ClipboardFolder.ChatRootFolder);
            ClipboardItemFolders.Add(RootFolderViewModel);
            ClipboardItemFolders.Add(SearchRootFolderViewModel);
            ClipboardItemFolders.Add(ChatRootFolderViewModel);
            ClipboardItemFolders.Add(ImageCheckRootFolderViewModel);

            OnPropertyChanged(nameof(ClipboardItemFolders));
        }

        public static MainWindowViewModel ActiveInstance { get; set; } = new MainWindowViewModel();

        // RootFolderのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public ClipboardFolderViewModel RootFolderViewModel { get; private set; }

        // 画像チェックフォルダのClipboardViewModel
        // Null非許容を無視
        [AllowNull]
        public ImageCheckFolderViewModel ImageCheckRootFolderViewModel { get; private set; }

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

        // クリップボード監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string ClipboardMonitorButtonText {
            get {
                return IsClipboardMonitor ? StringResources.StopClipboardWatch : StringResources.StartClipboardWatch;
            }
        }
        // クリップボード監視を開始、終了するフラグ
        public bool IsClipboardMonitor { get; set; } = false;

        // Windows通知監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string WindowsNotificationMonitorButtonText {
            get {
                return IsWindowsNotificationMonitor ? StringResources.StopNotificationWatch : StringResources.StartNotificationWatch;
            }
        }
        // Windows通知監視が開始、終了するフラグ
        public bool IsWindowsNotificationMonitor { get; set; } = false;

        // ClipboardFolder

        public ObservableCollection<ClipboardFolderViewModel> ClipboardItemFolders { get; set; } = [];

        // Cutフラグ
        private bool _CutFlag = false;
        public bool CutFlag {
            get {
                return _CutFlag;
            }
            set {
                _CutFlag = value;
                OnPropertyChanged(nameof(CutFlag));
            }
        }

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
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }
        /// <summary>
        /// コピーされたアイテム
        /// </summary>
        // Ctrl + C or X が押された時のClipboardItem
        private ObservableCollection<ClipboardItemViewModel> _copiedItems = [];
        public ObservableCollection<ClipboardItemViewModel> CopiedItems {
            get {
                return _copiedItems;
            }
            set {
                _copiedItems = value;
                // MainWindowModelのCopiedItemsにもセット
                OnPropertyChanged(nameof(CopiedItems));
            }
        }


        /// <summary>
        /// コピーされたアイテムのフォルダ
        /// </summary>
        // Ctrl + C or X  が押された時のClipboardItemFolder
        private ClipboardFolderViewModel? _copiedItemFolder;
        public ClipboardFolderViewModel? CopiedItemFolder {
            get {
                return _copiedItemFolder;
            }
            set {
                _copiedItemFolder = value;
                OnPropertyChanged(nameof(CopiedItemFolder));
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
            }
        }

        // プレビューモード　プレビューを表示するかどうか
        public static Visibility PreviewModeVisibility {
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
                // フォルダ選択アクション
                SelectVectorDBItemAction = (vectorDBItems) => {
                    SelectVectorDBWindow.OpenSelectVectorDBWindow(ActiveInstance.RootFolderViewModel, false, (selectedItems) => {
                        foreach (var item in selectedItems) {
                            vectorDBItems.Add(item);
                        }
                    });

                },
                // Saveアクション
                SaveCommand = (item) => {
                    clipboardItem = (ClipboardItem)item;
                    // ClipboardItemを保存
                    clipboardItem.Save();

                    //ChatHistoryItemがある場合は保存
                    // チャット履歴用のItemの設定
                    // チャット履歴を保存する。チャット履歴に同一階層のフォルダを作成して、Itemをコピーする。
                    ClipboardFolder chatFolder = ClipboardFolder.GetAnotherTreeFolder(clipboardItem.GetFolder(), ClipboardFolder.ChatRootFolder, true);
                    ClipboardItem chatHistoryItem = new(chatFolder.Id);

                    clipboardItem.CopyTo(chatHistoryItem);
                    chatHistoryItem.Save();
                }
            };

            return props;
        }

    }

}
