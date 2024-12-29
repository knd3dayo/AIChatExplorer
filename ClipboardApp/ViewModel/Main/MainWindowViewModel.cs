using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using ClipboardApp.Factory;
using ClipboardApp.Model;
using ClipboardApp.View.Main;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Common;
using ClipboardApp.ViewModel.Main;
using PythonAILib.Common;
using QAChat;
using WpfAppCommon.Utils;
using PythonAILib.Model.AutoGen;
using ClipboardApp.Common;

namespace ClipboardApp
{
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
                CopiedObjects.Clear();
            };

            // TabItemsの初期化
            MainPanel mainPanel = new() {
                DataContext = this
            };

            ClipboardAppTabContainer container = new("main", mainPanel) {
                CloseButtonVisibility = Visibility.Collapsed
            };
            TabItems.Add(container);

            // AutoGenPropertiesの初期化
            PythonAILibManager? libManager = PythonAILibManager.Instance;
            Task.Run(() => {
                AutoGenProperties.Init(libManager.ConfigParams.GetOpenAIProperties());
            });

        }

 
        public ObservableCollection<ClipboardAppTabContainer> TabItems { get; set; } = [];

        public void AddTabItem(ClipboardAppTabContainer tabItem) {
            if (ThisWindow == null) {
                return;
            }
            // ClipboardAppTabContainerのHeaderWidthを設定. 現在のタブ数 * ClipboardAppTabContainerのHeaderWidth > ThisWindow.Widthの場合はThisWindow.Widthを超えないようにする
            double tabControlWidth = ThisWindow.ActualWidth - 500;
            if ( (TabItems.Count + 1) * ClipboardAppTabContainer.HeaderWidthStatic > tabControlWidth ) {
                ClipboardAppTabContainer.HeaderWidthStatic = tabControlWidth / (TabItems.Count + 1);
                for (int i = 1; i < TabItems.Count; i++) {
                    TabItems[i].HeaderWidth = ClipboardAppTabContainer.HeaderWidthStatic;
                }
            }

            TabItems.Add(tabItem);
            OnPropertyChanged(nameof(TabItems));
            SelectedTabItem = tabItem;
        }

        public void RemoveTabItem(ClipboardAppTabContainer tabItem) {
            TabItems.Remove(tabItem);
            OnPropertyChanged(nameof(TabItems));
        }

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
        public RootFolderViewModelContainer RootFolderViewModelContainer { get; set; }

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
