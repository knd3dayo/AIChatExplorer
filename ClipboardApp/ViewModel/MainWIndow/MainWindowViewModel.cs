using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Control;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.ClipboardItemView;
using ClipboardApp.ViewModel.Common;
using ClipboardApp.ViewModel.MainWindow;
using ClipboardApp.ViewModel.MainWIndow;
using PythonAILib.Common;
using QAChat;


namespace ClipboardApp {
    public partial class MainWindowViewModel : ClipboardAppViewModelBase {
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
            RootFolderViewModelContainer = new();

            // データベースのチェックポイント処理
            ClipboardAppFactory.Instance.GetClipboardDBController().GetDatabase().Checkpoint();

            // DBのバックアップの取得
            BackupController.BackupNow();

            // ClipboardControllerのOnClipboardChangedに処理をセット
            ClipboardController.OnClipboardChanged = (e) => {
                // CopiedItemsをクリア
                CopiedObjects.Clear();
            };

            // TabItemsの初期化
            MainPanel mainPanel = new() {
                DataContext = this
            };

            ClipboardAppTabContainer container = new("main", mainPanel);
            container.CloseButtonVisibility = Visibility.Collapsed;
            TabItems.Add(container);
        }

        public ObservableCollection<ClipboardAppTabContainer> TabItems { get; set; } = [];

        public string TabName { get; set; } = "main";

        // Null許容型
        [AllowNull]
        public RootFolderViewModelContainer RootFolderViewModelContainer { get; set; }

        public static MainWindowViewModel ActiveInstance { get; set; } = new MainWindowViewModel();

        // ClipboardController
        public static ClipboardController ClipboardController { get; } = new();

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


    }

}
