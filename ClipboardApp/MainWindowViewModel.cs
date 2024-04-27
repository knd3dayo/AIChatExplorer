using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.AutoProcessRuleView;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.PythonScriptView.PythonScriptView;
using ClipboardApp.View.StatusMessageView;
using ClipboardApp.View.TagView;
using ClipboardApp.Views.ClipboardItemView;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon;
using WpfAppCommon.Factory;
using WpfAppCommon.Factory.Default;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;


namespace ClipboardApp {

    public class MainWindowViewModel : ObservableObject {

        // static properties
        // ステータスバーのテキスト
        public static StatusText StatusText { get; } = new StatusText();
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
        private ClipboardFolderViewModel? _selectedFolder = new ClipboardFolderViewModel(ClipboardFolder.RootFolder);
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

        public MainWindowViewModel() {
            // データベースのチェックポイント処理
            DefaultClipboardDBController.GetClipboardDatabase().Checkpoint();

            // ロギング設定
            Tools.StatusText = StatusText;

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


        }

        // フォルダ階層を再描写する
        public void ReloadFolder() {
            ClipboardItemFolders.Clear();
            ClipboardItemFolders.Add(new ClipboardFolderViewModel(ClipboardFolder.RootFolder));
            ClipboardItemFolders.Add(new ClipboardFolderViewModel(ClipboardFolder.SearchRootFolder));
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

        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------
        // アプリケーションを終了する。
        // Ctrl + Q が押された時の処理
        // メニューの「終了」をクリックしたときの処理

        public static SimpleDelegateCommand ExitCommand => new((parameter) => {
            // 終了確認ダイアログを表示。Yesならアプリケーションを終了
            MessageBoxResult result = MessageBox.Show("終了しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {

                Application.Current.Shutdown();
            }
        });

        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand ToggleClipboardMonitor => new((parameter) => {
            IsClipboardMonitor = !IsClipboardMonitor;
            OnPropertyChanged(nameof(ClipboardMonitorButtonText));
        });

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand FolderSelectionChangedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ClipboardFolderViewModel clipboardItemFolderViewModel = (ClipboardFolderViewModel)treeView.SelectedItem;
            SelectedFolder = clipboardItemFolderViewModel;
        });

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand ClipboardItemSelectionChangedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            ListBox listBox = (ListBox)routedEventArgs.OriginalSource;
            ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)listBox.SelectedItem;
            SelectedItems.Clear();
            foreach (ClipboardItemViewModel item in listBox.SelectedItems) {
                SelectedItems.Add(item);
            }
            SelectedItem = clipboardItemViewModel;
        });


        // クリップボードアイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public SimpleDelegateCommand CreateItemCommand => new((parameter) => {
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardItemCommands.CreateItemCommandExecute(SelectedFolder);
        });

        // OpenOpenAIWindowCommand メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand OpenOpenAIWindowCommand => new((parameter) => {
            ClipboardItemCommands.OpenOpenAIChatWindowExecute(null);
        });


        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand SearchCommand => new((parameter) => {
            // 選択中のフォルダがない場合でも処理をする
            ClipboardFolderCommands.SearchCommandExecute(SelectedFolder);
        });


        // Ctrl + R が押された時の処理
        public SimpleDelegateCommand ReloadCommand => new((parameter) => {
            if (SelectedFolder == null) {
                return;
            }
            // -- 処理完了までProgressIndicatorを表示
            try {
                IsIndeterminate = true;
                ClipboardFolderCommands.ReloadCommandExecute(SelectedFolder);
            } finally {
                IsIndeterminate = false;
            }
        });

        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand DeleteDisplayedItemCommand => new((parameter) => {
            if (SelectedFolder == null) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardFolderCommands.DeleteDisplayedItemCommandExecute(SelectedFolder);
        });


        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand DeleteSelectedItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }

            ClipboardItemCommands.DeleteSelectedItemCommandExecute(SelectedFolder, SelectedItems);
        });


        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand SettingCommand => new((parameter) => {
            SettingWindow settingWindow = new();
            settingWindow.ShowDialog();
        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand ChangePinCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.ChangePinCommandExecute(SelectedFolder, SelectedItems);
        });
        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.OpenItemCommandExecute(SelectedFolder, SelectedItem);
        });
        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemAsFileCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            ClipboardItemCommands.OpenSelectedItemAsFileCommandExecute(SelectedItem);
        });

        // 選択したアイテムを新規として開く処理 複数アイテム処理不可
        public SimpleDelegateCommand OpenSelectedItemAsNewFileCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            ClipboardItemCommands.OpenSelectedItemAsNewFileCommandExecute(SelectedItem);
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand CutItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            // Cut Flagを立てる
            CutFlag = true;
            // CopiedItemsに選択中のアイテムをセット
            CopiedItems.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                CopiedItems.Add(item);
            }
            CopiedItemFolder = SelectedFolder;
            Tools.Info("切り取りしました");

        });
        // Ctrl + C が押された時の処理
        public SimpleDelegateCommand CopyToClipboardCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            // Cutフラグをもとに戻す
            CutFlag = false;
            // CopiedItemsに選択中のアイテムをセット
            CopiedItems.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                CopiedItems.Add(item);
            }
            CopiedItemFolder = SelectedFolder;
            try {
                ClipboardAppFactory.Instance.GetClipboardController().SetDataObject(SelectedItem.ClipboardItem);
                Tools.Info("コピーしました");

            } catch (Exception e) {
                string message = $"エラーが発生しました。\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
                Tools.Error(message);
            }

        });
        // Ctrl + V が押された時の処理
        public SimpleDelegateCommand PasteFromClipboardCommand => new((parameter) => {
            // コピー元のアイテムがない場合は処理をしない
            if (CopiedItems.Count == 0) {
                Tools.Info("コピー元のアイテムがない");
                return;
            }
            // コピー元のフォルダがない場合は処理をしない
            if (CopiedItemFolder == null) {
                Tools.Error("コピー元のフォルダがない");
                return;
            }
            // 貼り付け先のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("貼り付け先のフォルダがない");
                return;
            }
            ClipboardItemCommands.PasteClipboardItemCommandExecute(
                this.CutFlag,
                CopiedItems,
                CopiedItemFolder,
                SelectedFolder
                );
            // Cutフラグをもとに戻す
            CutFlag = false;
            // 貼り付け後にコピー選択中のアイテムをクリア
            CopiedItems.Clear();
            CopiedItemFolder = null;

        });

        // Ctrl + M が押された時の処理
        public SimpleDelegateCommand MergeItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems,
                false
                );
        });
        // Ctrl + Shift + M が押された時の処理
        public SimpleDelegateCommand MergeItemWithHeaderCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemCommands.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems,
                true
                );
        });

        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListPythonScriptWindowCommand => new (PythonCommands.OpenListPythonScriptWindowCommandExecute);

        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListPromptTemplateWindowCommand => new((parameter) => {
            ListPromptTemplateWindow listPromptTemplateWindow = new ListPromptTemplateWindow();
            ListPromptTemplateWindowViewModel listPromptTemplateWindowViewModel = (ListPromptTemplateWindowViewModel)listPromptTemplateWindow.DataContext;
            listPromptTemplateWindowViewModel.InitializeEdit((promptTemplateWindowViewModel) => {
                // PromptTemplate = promptTemplateWindowViewModel.PromptItem;
            });
            listPromptTemplateWindow.ShowDialog();
        });
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenListAutoProcessRuleWindowCommand => new((parameter) => {
            ListAutoProcessRuleWindow ListAutoProcessRuleWindow = new ListAutoProcessRuleWindow();
            ListAutoProcessRuleWindowViewModel ListAutoProcessRuleWindowViewModel = (ListAutoProcessRuleWindowViewModel)ListAutoProcessRuleWindow.DataContext;
            ListAutoProcessRuleWindowViewModel.Initialize();

            ListAutoProcessRuleWindow.ShowDialog();

        });

        // メニューの「タグ編集」をクリックしたときの処理
        public SimpleDelegateCommand OpenTagWindowCommand => new((parameter) => {
            TagWindow tagWindow = new TagWindow();
            TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
            tagWindowViewModel.Initialize(null, () => { });
            tagWindow.ShowDialog();

        });
        // ステータスバーをクリックしたときの処理
        public SimpleDelegateCommand OpenStatusMessageWindowCommand => new((parameter) => {
            StatusMessageWindow statusMessageWindow = new StatusMessageWindow();
            StatusMessageWindowViewModel statusMessageWindowViewModel = (StatusMessageWindowViewModel)statusMessageWindow.DataContext;
            statusMessageWindowViewModel.Initialize();
            statusMessageWindow.ShowDialog();

        });

        // コンテキストメニューの「テキストを整形」の実行用コマンド
        public SimpleDelegateCommand FormatTextCommand => new((parameter) => {
            // 選択中のアイテムを取得
            ClipboardItemViewModel clipboardItemViewModel = SelectedItem!;
            if (clipboardItemViewModel == null) {
                return;
            }
            // 処理が終わるまでProgressIndicatorを表示
            try {
                IsIndeterminate = true;
                // テキストを取得
                string content = clipboardItemViewModel.ClipboardItem.Content;
                // テキストを整形
                content = ClipboardItem.FormatTextCommandExecute(content);
                // 整形したテキストをセット
                clipboardItemViewModel.ClipboardItem.Content = content;
                // 保存
                clipboardItemViewModel.ClipboardItem.Save();
                // 再描写
                ReloadClipboardItems();
            } finally {
                IsIndeterminate = false;
            }
        });
        // コンテキストメニューの「ファイルのパスを分割」の実行用コマンド
        public SimpleDelegateCommand SplitFilePathCommand => new((parameter) => {
            // 選択中のアイテムを取得
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            ClipboardItem clipboardItem = SelectedItem.ClipboardItem;
            // 処理が終わるまでProgressIndicatorを表示
            try {
                IsIndeterminate = true;
                // ファイルパスを分割
                ClipboardItem.SplitFilePathCommandExecute(clipboardItem);
                // 保存
                clipboardItem.Save();
                // 再描写
                ReloadClipboardItems();
            } finally {
                IsIndeterminate = false;
            }
        });

    }

}
