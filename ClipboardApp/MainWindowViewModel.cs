using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.Views.ClipboardItemView;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon;
using WpfAppCommon.Control;
using WpfAppCommon.Factory;
using WpfAppCommon.Factory.Default;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;


namespace ClipboardApp {

    public class MainWindowViewModel : MyWindowViewModel {


        // プログレスインジケータ表示更新用のアクション
        // 
        public static Action<bool> UpdateProgressCircleVisibility { get; set; } = (visible) => { };

        // Tools
        public static Tools? Tools { get; private set; }

        // Progress Indicatorの表示フラグ
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return _IsIndeterminate;
            }
            set {
                _IsIndeterminate = value;
                OnPropertyChanged("IsIndeterminate");
            }
        }

        // クリップボード監視が開始されている場合は「停止」、停止されている場合は「開始」を返す
        public string ClipboardMonitorButtonText {
            get {
                return IsClipboardMonitor ? "停止" : "開始";
            }
        }
        // クリップボード監視を開始、終了するフラグ
        private bool _isClipboardMonitor = false;
        public bool IsClipboardMonitor {
            get {
                return _isClipboardMonitor;
            }
            set {
                _isClipboardMonitor = value;
                OnPropertyChanged(nameof(IsClipboardMonitor));
                if (value) {
                    ClipboardAppFactory.Instance.GetClipboardController().Start((actionMessage) => {
                        // クリップボードが変更された時の処理
                        SelectedFolder?.Load();
                    });

                    Tools.Info("クリップボード監視を開始しました");
                } else {
                    ClipboardAppFactory.Instance.GetClipboardController().Stop();
                    Tools.Info("クリップボード監視を停止しました");
                }
            }
        }
        // ClipboardFolder

        public static ObservableCollection<ClipboardFolderViewModel> ClipboardItemFolders { get; set; } = new ObservableCollection<ClipboardFolderViewModel>();

        // RootFolderのClipboardViewModel
        public static ClipboardFolderViewModel? RootFolderViewModel { get; private set; } 

        // Cutフラグ
        public bool CutFlag { get; set; } = false;
        // 選択中のアイテム(複数選択)
        public ObservableCollection<ClipboardItemViewModel> SelectedItems { get; set; } = new ObservableCollection<ClipboardItemViewModel>();

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
                _selectedFolder?.Load();

                OnPropertyChanged(nameof(SelectedFolder));
            }
        }
        // Ctrl + C or X が押された時のClipboardItem
        public List<ClipboardItemViewModel> CopiedItems { get; set; } = new List<ClipboardItemViewModel>();

        // Ctrl + C or X  が押された時のClipboardItemFolder
        public ClipboardFolderViewModel? CopiedItemFolder { get; set; } = null;

        //-----
        // ClipboardItemContextMenuItems
        //-----
        public ClipboardItemFolderContextMenuItems? ClipboardItemContextMenuItems { get; set; } = null;

        public ObservableCollection<ClipboardAppMenuItem> ClipboardItemFolderContextMenuItems { get; set; } = new ObservableCollection<ClipboardAppMenuItem>();

        // 表示・非表示の設定
        public Visibility UsePythonVisibility {
            get {
                return ClipboardAppConfig.PythonExecute == 0 ? Visibility.Collapsed : Visibility.Visible;
            }
        }
        public Visibility UseOpenAIVisibility {
            get {
                if (ClipboardAppConfig.UseOpenAI && ClipboardAppConfig.PythonExecute != 0) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public MainWindowViewModel() {
            // データベースのチェックポイント処理
            DefaultClipboardDBController.GetClipboardDatabase().Checkpoint();
            // ステータスバークリック時に実行するコマンド
            MyStatusBarViewModel.OpenStatusMessageWindow = MainWindowCommand.OpenStatusMessageWindowCommand;

            // フォルダ階層を再描写する
            ReloadFolder();

            // Python処理機能の初期化
            string pythonDLLPath = WpfAppCommon.Properties.Settings.Default.PythonDllPath;
            PythonExecutor.Init(pythonDLLPath);

            // コンテキストメニューの初期化
            ClipboardItemContextMenuItems = new ClipboardItemFolderContextMenuItems(this);

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

        // フォルダ階層を再描写する
        public void ReloadFolder() {
            ClipboardItemFolders.Clear();
            ClipboardItemFolders.Add(new ClipboardFolderViewModel(this, ClipboardFolder.RootFolder));
            ClipboardItemFolders.Add(new ClipboardFolderViewModel(this, ClipboardFolder.SearchRootFolder));
            OnPropertyChanged("ClipboardItemFolders");
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

        private void ScrollToTop() {

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
            MainWindowCommand.ExitCommand();
        });

        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand ToggleClipboardMonitor => new((parameter) => {
            MainWindowCommand.ToggleClipboardMonitorCommand(this);
        });

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand FolderSelectionChangedCommand => new((parameter) => {
            MainWindowCommand.FolderSelectionChangedCommand(this, parameter);
        });

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand ClipboardItemSelectionChangedCommand => new((parameter) => {
            MainWindowCommand.ClipboardItemSelectionChangedCommand(this, parameter);
        });


        // クリップボードアイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public SimpleDelegateCommand CreateItemCommand => new((parameter) => {
            MainWindowCommand.CreateItemCommand(this, parameter);
        });

        // OpenOpenAIWindowCommand メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand OpenOpenAIWindowCommand => new((parameter) => {
            MainWindowCommand.OpenOpenAIWindowCommand();
        });
        // OpenRAGManagementWindowCommandメニュー　「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand OpenRAGManagementWindowCommand => new((parameter) => {
            MainWindowCommand.OpenRAGManagementWindowCommand();
        });

        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand SearchCommand => new((parameter) => {
            MainWindowCommand.SearchCommand(this);
        });


        // Ctrl + R が押された時の処理
        public SimpleDelegateCommand ReloadCommand => new((parameter) => {
            MainWindowCommand.ReloadCommand(this);
        });

        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand DeleteDisplayedItemCommand => new((parameter) => {
            MainWindowCommand.DeleteDisplayedItemCommand(this);
        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand DeleteSelectedItemCommand => new((parameter) => {
            MainWindowCommand.DeleteSelectedItemCommand(this);
        });


        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand SettingCommand => new((parameter) => {
            MainWindowCommand.SettingCommand();
        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand ChangePinCommand => new((parameter) => {
            MainWindowCommand.ChangePinCommand(this);
        });

        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemCommand => new((parameter) => {
            MainWindowCommand.OpenSelectedItemCommand(this);
        });

        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemAsFileCommand => new((parameter) => {
            MainWindowCommand.OpenSelectedItemAsFileCommand(this);
        });

        // 選択したアイテムを新規として開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemAsNewFileCommand => new((parameter) => {
            MainWindowCommand.OpenSelectedItemAsNewFileCommand(this);
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand CutItemCommand => new((parameter) => {
            MainWindowCommand.CutItemCommand(this);
        });
        // Ctrl + C が押された時の処理
        public SimpleDelegateCommand CopyToClipboardCommand => new((parameter) => {
            MainWindowCommand.CopyToClipboardCommand(this);
        });
        // Ctrl + V が押された時の処理
        public SimpleDelegateCommand PasteFromClipboardCommand => new((parameter) => {
            MainWindowCommand.PasteFromClipboardCommand(this);
        });

        // Ctrl + M が押された時の処理
        public SimpleDelegateCommand MergeItemCommand => new((parameter) => {
            MainWindowCommand.MergeItemCommand(this);
        });
        // Ctrl + Shift + M が押された時の処理
        public SimpleDelegateCommand MergeItemWithHeaderCommand => new((parameter) => {
            MainWindowCommand.MergeItemWithHeaderCommand(this);
        });

        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListPythonScriptWindowCommand => new((parameter) => {
            MainWindowCommand.OpenListPythonScriptWindowCommand(parameter);
        });

        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListPromptTemplateWindowCommand => new((parameter) => {
            MainWindowCommand.OpenListPromptTemplateWindowCommand(this);
        });
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListAutoProcessRuleWindowCommand => new((parameter) => {
            MainWindowCommand.OpenListAutoProcessRuleWindowCommand();
        });
        // メニューの「タグ編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenTagWindowCommand => new((parameter) => {
            MainWindowCommand.OpenTagWindowCommand();
        });

        // コンテキストメニューの「ファイルのパスを分割」の実行用コマンド
        public SimpleDelegateCommand SplitFilePathCommand => new((parameter) => {
            MainWindowCommand.SplitFilePathCommand(this);
        });

    }

}
