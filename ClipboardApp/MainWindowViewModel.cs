using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.ViewModel;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using WpfAppCommon;
using WpfAppCommon.Factory;
using WpfAppCommon.Factory.Default;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;


namespace ClipboardApp
{

    public partial class MainWindowViewModel : MyWindowViewModel {

        public MainWindowViewModel() {
            // 旧バージョンのRootFolderの移行
            ClipboardFolder.MigrateRootFolder();

            RootFolderViewModel = new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder);
            Init();

        }
        private void Init() {

            ActiveInstance = this;

            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };

            PythonAILibStringResources.Lang = ClipboardAppConfig.ActualLang;

            
            // フォルダの初期化
            InitClipboardFolders();



            // Python処理機能の初期化
            PythonExecutor.Init(ClipboardAppConfig.PythonDllPath, ClipboardAppConfig.PythonVenvPath);

            // データベースのチェックポイント処理
            DefaultClipboardDBController.GetClipboardDatabase().Checkpoint();

            // DBのバックアップの取得
            IBackupController backupController = ClipboardAppFactory.Instance.GetBackupController();
            backupController.BackupNow();
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);

        }

        private void InitClipboardFolders() {

            ClipboardItemFolders.Add(RootFolderViewModel);
            ClipboardItemFolders.Add(new SearchFolderViewModel(this, ClipboardFolder.SearchRootFolder));
            ClipboardItemFolders.Add(new ChatFolderViewModel(this, ClipboardFolder.ChatRootFolder));
            ClipboardItemFolders.Add(new ImageCheckFolderViewModel(this, ClipboardFolder.ImageCheckRootFolder));
            OnPropertyChanged(nameof(ClipboardItemFolders));
        }


        public static MainWindowViewModel? ActiveInstance { get; set; }

        /// <summary>
        /// ウィンドウがアクティブになった時の処理
        /// </summary>
        public override void OnActivatedAction() {

            ActiveInstance = this;

        }
        // RootFolderのClipboardViewModel
        public ClipboardFolderViewModel RootFolderViewModel { get; private set; }

        public void ReLoadRootFolders() {
            foreach (var folder in ClipboardItemFolders) {
                folder.LoadFolderCommand.Execute();
            }
        }

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
                return ClipboardAppConfig.TextWrapping == System.Windows.TextWrapping.Wrap;
            }
            set {
                if (value) {
                    ClipboardAppConfig.TextWrapping = System.Windows.TextWrapping.Wrap;
                } else {
                    ClipboardAppConfig.TextWrapping = System.Windows.TextWrapping.NoWrap;
                }
                // Save
                ClipboardAppConfig.Save();
                OnPropertyChanged(nameof(TextWrapping));
            }
        }

        // プレビューモード　プレビューを表示するかどうか
        public static Visibility PreviewModeVisibility {
            get {
                return ClipboardAppConfig.PreviewMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }


        
        //　プレビューモード表示するかどうか
        public bool PreviewMode {
            get {
                return ClipboardAppConfig.PreviewMode;
            }
            set {
                ClipboardAppConfig.PreviewMode = value;
                // Save
                ClipboardAppConfig.Save();

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

        public TextSelector TextSelector { get; } = new();

        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand<TextBox> SelectTextCommand => new((textBox) => {

            // テキスト選択
            TextSelector.SelectText(textBox);
            return;
        });
        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand<TextBox> ExecuteSelectedTextCommand => new((textbox) => {

            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(textbox);

        });


        // 開発中の機能を有効にするかどうか
        public bool EnableDevFeatures {
            get {
                return ClipboardAppConfig.EnableDevFeatures;
            }
            set {
                ClipboardAppConfig.EnableDevFeatures = value;
                // Save
                ClipboardAppConfig.Save();
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
            }
        }
        // 開発中機能の表示
        public Visibility EnableDevFeaturesVisibility {
            get {
                return ClipboardAppConfig.EnableDevFeatures ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------


    }

}
