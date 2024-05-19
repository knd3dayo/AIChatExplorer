using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ClipboardApp.Control;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.Views.ClipboardItemView;
using WpfAppCommon;
using WpfAppCommon.Control;
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

        public MainWindowViewModel() {
            // データベースのチェックポイント処理
            DefaultClipboardDBController.GetClipboardDatabase().Checkpoint();
            // ステータスバークリック時に実行するコマンド
            MyStatusBarViewModel.OpenStatusMessageWindow = OpenStatusMessageWindowCommand;

            // フォルダ階層を再描写する
            ReloadFolder();

            // Python処理機能の初期化
            PythonExecutor.Init(ClipboardAppConfig.PythonDllPath);

            // DBのバックアップの取得
            IBackupController backupController = ClipboardAppFactory.Instance.GetBackupController();
            backupController.BackupNow();

            // ProgressIndicatorの表示更新用のアクションをセット
            UpdateProgressCircleVisibility = (visible) => {
                IsIndeterminate = visible;
            };
            // RootFolderのViewModel
            RootFolderViewModel = new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder);
        }

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

        public static ObservableCollection<ClipboardFolderViewModel> ClipboardItemFolders { get; set; } = [];

        // RootFolderのClipboardViewModel
        public static ClipboardFolderViewModel? RootFolderViewModel { get; private set; } 

        // Cutフラグ
        public bool CutFlag { get; set; } = false;
        // 選択中のアイテム(複数選択)
        public ObservableCollection<ClipboardItemViewModel> SelectedItems { get; set; } = [];

        // 選択中のアイテム
        private ClipboardItemViewModel? _selectedItem = null;
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
        private ClipboardFolderViewModel? _selectedFolder = null;
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
        public List<ClipboardItemViewModel> CopiedItems { get; set; } = [];

        /// <summary>
        /// コピーされたアイテムのフォルダ
        /// </summary>
        // Ctrl + C or X  が押された時のClipboardItemFolder
        public ClipboardFolderViewModel? CopiedItemFolder { get; set; } = null;

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

        // フォルダ階層を再描写する
        public void ReloadFolder() {
            ClipboardItemFolders.Clear();
            ClipboardItemFolders.Add(new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder));
            ClipboardItemFolders.Add(new ClipboardFolderViewModel(this, ClipboardFolder.SearchRootFolder));
            OnPropertyChanged(nameof(ClipboardItemFolders));
        }
        // ClipboardItemを再描写する
        public void ReloadClipboardItems() {
            if (SelectedFolder == null) {
                return;
            }
            SelectedFolder.Load();
            // ListBoxの先頭にスクロール
            ScrollToTop();
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

        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------
        // アプリケーションを終了する。
        // Ctrl + Q が押された時の処理
        // メニューの「終了」をクリックしたときの処理

        public static SimpleDelegateCommand ExitCommand => new((parameter) => {
            ExitCommandExecute();
        });

        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand ToggleClipboardMonitor => new((parameter) => {
            ToggleClipboardMonitorCommand(this);
        });

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand FolderSelectionChangedCommand => new((parameter) => {
            FolderSelectionChangedCommandExecute(this, parameter);
        });

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand ClipboardItemSelectionChangedCommand => new((parameter) => {
            ClipboardItemSelectionChangedCommandExecute(this, parameter);
        });


        // クリップボードアイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public SimpleDelegateCommand CreateItemCommand => new((parameter) => {
            ClipboardItemViewModel.CreateItemCommandExecute(this.SelectedFolder);
        });

        // OpenOpenAIWindowCommandExecute メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand OpenOpenAIWindowCommand => new((parameter) => {
            OpenOpenAIWindowCommandExecute();
        });
        // OpenScreenshotCheckerWindowExecute メニューの「画像エビデンスチェッカー」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand OpenScreenshotCheckerWindow => new((parameter) => {
            OpenScreenshotCheckerWindowExecute();
        });

        // OpenRAGManagementWindowCommandメニュー　「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand OpenRAGManagementWindowCommand => new((parameter) => {
            OpenRAGManagementWindowCommandExecute();
        });
        // OpenVectorDBManagementWindowCommandメニュー　「ベクトルDB管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand OpenVectorDBManagementWindowCommand => new((parameter) => {
            OpenVectorDBManagementWindowCommandExecute();
        });

        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand SearchCommand => new((parameter) => {
            SearchCommandExecute(this);
        });


        // Ctrl + R が押された時の処理
        public SimpleDelegateCommand ReloadCommand => new((parameter) => {
            ReloadCommandExecute(this);
        });

        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand DeleteDisplayedItemCommand => new((parameter) => {
            DeleteDisplayedItemCommandExecute(this);
        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand DeleteSelectedItemCommand => new((parameter) => {
            DeleteSelectedItemCommandExecute(this);
        });


        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand SettingCommand => new((parameter) => {
            SettingCommandExecute();
        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand ChangePinCommand => new((parameter) => {
            ChangePinCommandExecute(this);
        });

        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemCommand => new((parameter) => {
            ClipboardItemViewModel.OpenItemCommandExecute(this.SelectedFolder, this.SelectedItem);

        });

        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemAsFileCommand => new((parameter) => {
            ClipboardItemViewModel.OpenSelectedItemAsFileCommandExecute(this.SelectedItem);
        });

        // 選択したアイテムのフォルダを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenFolderCommand => new((parameter) => {
            ClipboardItemViewModel.OpenFolderCommandExecute(this.SelectedItem);
        });
        // 選択したアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenFileCommand => new((parameter) => {
            ClipboardItemViewModel.OpenFileCommandExecute(this.SelectedItem);
        });
        // 選択したアイテムを一時フォルダで開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenFileInTempFolderCommand => new((parameter) => {
            ClipboardItemViewModel.OpenFileInTempFolderCommandExecute(this.SelectedItem);
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand CutItemCommand => new((parameter) => {
            CutItemCommandExecute(this);
        });
        // Ctrl + C が押された時の処理
        public SimpleDelegateCommand CopyToClipboardCommand => new((parameter) => {
            CopyToClipboardCommandExecute(this);
        });
        // Ctrl + V が押された時の処理
        public SimpleDelegateCommand PasteFromClipboardCommand => new((parameter) => {
            PasteFromClipboardCommandExecute(this);
        });

        // Ctrl + M が押された時の処理
        public SimpleDelegateCommand MergeItemCommand => new((parameter) => {
            MergeItemCommandExecute(this);
        });
        // Ctrl + Shift + M が押された時の処理
        public SimpleDelegateCommand MergeItemWithHeaderCommand => new((parameter) => {
            MergeItemWithHeaderCommandExecute(this);
        });

        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListPythonScriptWindowCommand => new((parameter) => {
            OpenListPythonScriptWindowCommandExecute(parameter);
        });

        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListPromptTemplateWindowCommand => new((parameter) => {
            OpenListPromptTemplateWindowCommandExecute(this);
        });
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListAutoProcessRuleWindowCommand => new((parameter) => {
            OpenListAutoProcessRuleWindowCommandExecute();
        });
        // メニューの「タグ編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenTagWindowCommand => new((parameter) => {
            OpenTagWindowCommandExecute();
        });

        // コンテキストメニューの「ファイルのパスを分割」の実行用コマンド
        public SimpleDelegateCommand SplitFilePathCommand => new((parameter) => {
            SplitFilePathCommandExecute(this);
        });

    }

}
