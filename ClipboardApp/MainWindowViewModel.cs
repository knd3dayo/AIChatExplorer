using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using PythonAILib.PythonIF;
using WpfAppCommon;
using WpfAppCommon.Factory;
using WpfAppCommon.Factory.Default;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;


namespace ClipboardApp {

    public partial class MainWindowViewModel : MyWindowViewModel {

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// 
        public static MainWindowViewModel? ActiveInstance { get; set; }

        public override void OnActivatedAction() {

            ActiveInstance = this;

        }
        public MainWindowViewModel() {
            Init();

        }
        public void Init() {
            ClipboardItemFolders.Add(new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder));
            ClipboardItemFolders.Add(new SearchFolderViewModel(this, ClipboardFolder.SearchRootFolder));
            ClipboardItemFolders.Add(new ImageCheckFolderViewModel(this, ClipboardFolder.ImageCheckRootFolder));
            OnPropertyChanged(nameof(ClipboardItemFolders));

            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };
            // RootFolderのViewModel
            RootFolderViewModel = new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder);

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
                return IsClipboardMonitor ? StringResources.Stop : StringResources.Start;
            }
        }
        // クリップボード監視を開始、終了するフラグ
        public bool IsClipboardMonitor { get; set; } = false;

        // ClipboardFolder

        public ObservableCollection<ClipboardFolderViewModel> ClipboardItemFolders { get; set; } = [];

        // RootFolderのClipboardViewModel
        public static ClipboardFolderViewModel? RootFolderViewModel { get; private set; }

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
        // コンパクトモード表示するかどうか
        public bool CompactMode {
            get {
                return WpfAppCommon.Properties.Settings.Default.CompactViewMode;
            }
            set {
                // Save
                WpfAppCommon.Properties.Settings.Default.CompactViewMode = value;
                WpfAppCommon.Properties.Settings.Default.Save();
                OnPropertyChanged(nameof(CompactMode));
                // アプリケーション再起動後に反映されるようにメッセージを表示
                MessageBox.Show("アプリケーションを再起動すると、表示モードが変更されます。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        //　プレビューモード表示するかどうか
        public bool PreviewMode {
            get {
                return WpfAppCommon.Properties.Settings.Default.PreviewMode;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.PreviewMode = value;
                // Save
                WpfAppCommon.Properties.Settings.Default.Save();

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

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new(FolderSelectionChangedCommandExecute);

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ClipboardItemSelectionChangedCommand => new(ClipboardItemSelectionChangedCommandExecute);


        // クリップボードアイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public SimpleDelegateCommand<object> CreateItemCommand => new((parameter) => {
            if (this.SelectedFolder == null) {
                LogWrapper.Error("フォルダが選択されていません。");
                return;
            }
            this.SelectedFolder.CreateItemCommandExecute();
        });

        // OpenOpenAIWindowCommandExecute メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenOpenAIWindowCommand => new((parameter) => {
            OpenOpenAIWindowCommandExecute();
        });
        // OpenScreenshotCheckerWindowExecute メニューの「画像エビデンスチェッカー」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenScreenshotCheckerWindow => new((parameter) => {
            OpenScreenshotCheckerWindowExecute();
        });

        // OpenRAGManagementWindowCommandメニュー　「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenRAGManagementWindowCommand => new((parameter) => {
            OpenRAGManagementWindowCommandExecute();
        });
        // OpenVectorDBManagementWindowCommandメニュー　「ベクトルDB管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorDBManagementWindowCommand => new((parameter) => {
            OpenVectorDBManagementWindowCommandExecute();
        });

        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand<object> SearchCommand => new((parameter) => {
            SearchCommandExecute(SelectedFolder);
        });


        // Ctrl + R が押された時の処理
        public SimpleDelegateCommand<object> ReloadCommand => new((parameter) => {
            ReloadCommandExecute(this);
        });

        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            DeleteDisplayedItemCommandExecute(this);
        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteSelectedItemCommand => new((parameter) => {
            DeleteSelectedItemCommandExecute(this);
        });


        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            SettingCommandExecute();
        });


        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            ClipboardItemViewModel.OpenItemCommandExecute(this.SelectedFolder, this.SelectedItem);

        });

        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((parameter) => {
            ClipboardItemViewModel.OpenContentAsFileCommandExecute(this.SelectedItem);
        });

        // 選択したアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenFileCommand => new((parameter) => {
            ClipboardItemViewModel.OpenFileCommandExecute(this.SelectedItem);
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutItemCommand => new((parameter) => {
            CutItemCommandExecute(this);
        });
        // Ctrl + C が押された時の処理
        public SimpleDelegateCommand<object> CopyItemCommand => new((parameter) => {
            CopyToClipboardCommandExecute(this);
        });
        // Ctrl + V が押された時の処理
        public SimpleDelegateCommand<object> PasteItemCommand => new((parameter) => {
            PasteFromClipboardCommandExecute(this);
        });

        // Ctrl + M が押された時の処理
        public SimpleDelegateCommand<object> MergeItemCommand => new((parameter) => {
            MergeItemCommandExecute(this);
        });
        // Ctrl + Shift + M が押された時の処理
        public SimpleDelegateCommand<object> MergeItemWithHeaderCommand => new((parameter) => {
            MergeItemWithHeaderCommandExecute(this);
        });

        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListPythonScriptWindowCommand => new((parameter) => {
            OpenListPythonScriptWindowCommandExecute(parameter);
        });

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

    }

}
