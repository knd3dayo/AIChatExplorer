using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.ViewModel;
using PythonAILib.PythonIF;
using WpfAppCommon;
using WpfAppCommon.Factory;
using WpfAppCommon.Factory.Default;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;


namespace ClipboardApp
{

    public partial class MainWindowViewModel : MyWindowViewModel {

        public MainWindowViewModel() {
            RootFolderViewModel = new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder);
            Init();

        }
        public void Init() {
            ClipboardItemFolders.Add(RootFolderViewModel);
            ClipboardItemFolders.Add(new SearchFolderViewModel(this, ClipboardFolder.SearchRootFolder));
            ClipboardItemFolders.Add(new ChatFolderViewModel(this, ClipboardFolder.ChatRootFolder));
            ClipboardItemFolders.Add(new ImageCheckFolderViewModel(this, ClipboardFolder.ImageCheckRootFolder));
            OnPropertyChanged(nameof(ClipboardItemFolders));

            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };

            ActiveInstance = this;


            // データベースのチェックポイント処理
            DefaultClipboardDBController.GetClipboardDatabase().Checkpoint();

            // Python処理機能の初期化
            PythonExecutor.Init(ClipboardAppConfig.PythonDllPath);

            // DBのバックアップの取得
            IBackupController backupController = ClipboardAppFactory.Instance.GetBackupController();
            backupController.BackupNow();
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// 
        public static MainWindowViewModel? ActiveInstance { get; set; }

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
                SelectedItemStatic = value;
            }
        }
        public static ClipboardItemViewModel? SelectedItemStatic { get; set; } = null;

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


        // 表示・非表示の設定
        /// <summary>
        /// Pythonが有効な場合はVisible、無効な場合はCollapsed
        /// </summary>
        public static Visibility UsePythonVisibility {
            get {
                return ClipboardAppConfig.PythonExecute == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        /// <summary>
        /// OpenAIが有効な場合はVisible、無効な場合はCollapsed
        public static Visibility UseOpenAIVisibility {
            get {
                if (ClipboardAppConfig.UseOpenAI && ClipboardAppConfig.PythonExecute != 0) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        // コンパクトモードでプレビューを表示するかどうか
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
                MessageBox.Show("アプリケーションを再起動すると、表示モードが変更されます。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public static Visibility CompactModeVisibility {
            get {
                return ClipboardAppConfig.CompactViewMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // ExpandModeVisibility
        public static Visibility ExpandModeVisibility {
            get {
                return ClipboardAppConfig.CompactViewMode ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        /// <summary>
        /// ListBoxの先頭にスクロール
        /// </summary>
        private static void ScrollToTop() {

            ListBox? listBox = Application.Current.MainWindow.FindName("listBox1") as ListBox;
            // ListBoxの先頭にスクロール
            if (listBox?.Items.Count > 0) {
                listBox?.ScrollIntoView(listBox.Items[0]);
            }
        }
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


        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------
        // アプリケーションを終了する。
        // Ctrl + Q が押された時の処理
        // メニューの「終了」をクリックしたときの処理

        public static SimpleDelegateCommand<object> ExitCommand => new((parameter) => {
            ExitCommandExecute();
        });

        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleClipboardMonitor => new((parameter) => {
            ToggleClipboardMonitorCommand(this);
        });
        // Windows通知監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleWindowsNotificationMonitor => new((parameter) => {
            ToggleWindowsNotificationMonitorCommand(this);
        });

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new(FolderSelectionChangedCommandExecute);

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ClipboardItemSelectionChangedCommand => new(ClipboardItemSelectionChangedCommandExecute);



    }

}
