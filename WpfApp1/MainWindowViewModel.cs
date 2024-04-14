using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.AutoProcessRuleView;
using WpfApp1.View.ClipboardItemFolderView;
using WpfApp1.View.ClipboardItemView;
using WpfApp1.View.StatusMessageView;
using WpfApp1.View.TagView;


namespace WpfApp1 {

    public class MainWindowViewModel : ObservableObject {
        public static MainWindowViewModel? Instance = null;

        public MainWindowViewModel() {
            // データベースのチェックポイント処理
            ClipboardDatabaseController.GetClipboardDatabase().Checkpoint();

            // ロギング設定
            Tools.StatusText = StatusText;


            // クリップボードコントローラーの初期化
            // SearchConditionをLiteDBから取得
            SearchConditionRule? searchConditionRule = ClipboardDatabaseController.GetSearchConditionRule(ClipboardDatabaseController.SEARCH_CONDITION_APPLIED_CONDITION_NAME);
            if (searchConditionRule != null) {
                ClipboardItemFolder.GlobalSearchCondition = searchConditionRule;
            }

            // フォルダ階層を再描写する
            ReloadFolder();

            ClipboardController.Init(this);
            Instance = this;


            // バックアップ処理を実施
            BackupController.Init();

            // コンテキストメニューの初期化
            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("開く", OpenSelectedItemCommand, "Ctrl+O"));

            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("ファイルとして開く", OpenSelectedItemAsFileCommand, "Ctrl+Shit+O"));
            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("新規ファイルとして開く", OpenSelectedItemAsNewFileCommand, "Ctrl+Shit+Alt+O"));
            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("ピン止め", ChangePinCommand));

            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("コピー", CopyToClipboardCommand, "Ctrl+C"));
            ClipboardItemContextMenuItems.Add(new ClipboardAppMenuItem("削除", DeleteSelectedItemCommand, "Delete"));
                
            // サブメニュー設定
            ClipboardAppMenuItem pythonMenuItems = new ClipboardAppMenuItem("便利機能", SimpleDelegateCommand.EmptyCommand);
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("ファイルのパスを分割", SplitFilePathCommand));
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("テキストを抽出", ClipboardItemViewModel.ExtractTextCommand));
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("データをマスキング", ClipboardItemViewModel.MaskDataCommand));
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("データを整形", FormatTextCommand));
            pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem("OpenAIチャット", ClipboardItemViewModel.OpenAIChatCommand));
            // Pythonスクリプト(ユーザー定義)
            foreach (ScriptItem scriptItem in ClipboardItemViewModel.ScriptItems) {

                pythonMenuItems.SubMenuItems.Add(new ClipboardAppMenuItem(scriptItem.Description , new SimpleDelegateCommand((parameter) => {
                    ClipboardItemCommands.MenuItemRunPythonScriptCommandExecute(scriptItem);
                })
                ));
            }
            ClipboardItemContextMenuItems.Add(pythonMenuItems);

            // フォルダーのコンテキストメニュー
            ClipboardItemFolderContextMenuItems.Add(new ClipboardAppMenuItem("開く", ClipboardItemFolderViewModel.OpenFolderCommand));
            ClipboardItemFolderContextMenuItems.Add(new ClipboardAppMenuItem("編集", ClipboardItemFolderViewModel.EditFolderCommand));
            ClipboardItemFolderContextMenuItems.Add(new ClipboardAppMenuItem("新規作成", ClipboardItemFolderViewModel.CreateFolderCommand));
            ClipboardItemFolderContextMenuItems.Add(new ClipboardAppMenuItem("削除", ClipboardItemFolderViewModel.DeleteFolderCommand));

        }
        // フォルダ階層を再描写する
        public void ReloadFolder() {
            ClipboardItemFolders.Clear();
            ClipboardItemFolders.Add(new ClipboardItemFolderViewModel(ClipboardItemFolder.RootFolder));
            ClipboardItemFolders.Add(new ClipboardItemFolderViewModel(ClipboardItemFolder.SearchRootFolder));
            OnPropertyChanged("ClipboardItemFolders");
        }
        // ClipboardItemを再描写する
        public static void ReloadClipboardItems() {
            if (Instance == null) {
                return;
            }
            if (Instance.SelectedFolder == null) {
                return;
            }
            Instance.SelectedFolder.Load();
        }
        // ClipboardItemFolder

        public static ObservableCollection<ClipboardItemFolderViewModel> ClipboardItemFolders { get; set; } = new ObservableCollection<ClipboardItemFolderViewModel>();

        // Cutフラグ
        public bool CutFlag { get; set; } = false;
        // 選択中のアイテム(複数選択)
        public IList SelectedItems { get; set; } = new ObservableCollection<ClipboardItemViewModel>();

        // 選択中のアイテム
        private ClipboardItemViewModel? _selectedItem = null;
        public ClipboardItemViewModel? SelectedItem {
            get {
                return _selectedItem;
            }
            set {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        // 選択中のフォルダ
        private ClipboardItemFolderViewModel? _selectedFolder = new ClipboardItemFolderViewModel(ClipboardItemFolder.RootFolder);
        public ClipboardItemFolderViewModel? SelectedFolder {
            get {
                return _selectedFolder;
            }
            set {
                _selectedFolder = value;
                OnPropertyChanged("SelectedFolder");
            }
        }
        // Ctrl + C が押された時のClipboardItem
        public ClipboardItemViewModel? CopiedItem { get; set; } = null;

        // Ctrl + C が押された時のClipboardItemFolder
        public ClipboardItemFolderViewModel? CopiedItemFolder { get; set; } = null;
        // static 

        // ステータスバーのテキスト
        public static StatusText StatusText { get; } = new StatusText();

        // ステータスバーのテキスト更新と通知
        public void UpdateStatusText(string text) {
            StatusText.Text = text;
            OnPropertyChanged("StatusText");
        }
        //-----
        // ClipboardItemContextMenuItems
        //-----
        public ObservableCollection<ClipboardAppMenuItem> ClipboardItemContextMenuItems { get; set; } = new ObservableCollection<ClipboardAppMenuItem>();

        public ObservableCollection<ClipboardAppMenuItem> ClipboardItemFolderContextMenuItems { get; set; } = new ObservableCollection<ClipboardAppMenuItem>();

        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------

        // Ctrl + Q が押された時の処理
        public static SimpleDelegateCommand ExitCommand => new((parameter) => {
            // 終了確認ダイアログを表示。Yesならアプリケーションを終了
            MessageBoxResult result = MessageBox.Show("終了しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {

                System.Windows.Application.Current.Shutdown();
            }
            });

        // Ctrl + F が押された時の処理
        public static SimpleDelegateCommand SearchCommand => new SimpleDelegateCommand((parameter) => {
            ClipboardFolderCommands.SearchCommandExecute(parameter);
        });

        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public static SimpleDelegateCommand DeleteDisplayedItemCommand => new SimpleDelegateCommand(ClipboardFolderCommands.DeleteDisplayedItemCommandExecute);

        // Ctrl + R が押された時の処理
        public static SimpleDelegateCommand ReloadCommand => new SimpleDelegateCommand(ClipboardFolderCommands.ReloadCommandExecute);


        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public static SimpleDelegateCommand DeleteSelectedItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.DeleteSelectedItemCommandExecute);

        // Ctrl + N が押された時の処理
        public static SimpleDelegateCommand CreateItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.CreateItemCommandExecute);

        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand SettingCommand => new SimpleDelegateCommand(SettingCommandExecute);
        private static void SettingCommandExecute(object obj) {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }

        // ピン止めの切り替え処理
        public static SimpleDelegateCommand ChangePinCommand => new SimpleDelegateCommand(ClipboardItemCommands.ChangePinCommandExecute);
        // 選択中のアイテムを開く処理
        public static SimpleDelegateCommand OpenSelectedItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.OpenItemCommandExecute);
        // 選択したアイテムをテキストファイルとして開く処理
        public static SimpleDelegateCommand OpenSelectedItemAsFileCommand => new SimpleDelegateCommand(ClipboardItemCommands.OpenSelectedItemAsFileCommandExecute);
        // 選択したアイテムを新規として開く処理
        public static SimpleDelegateCommand OpenSelectedItemAsNewFileCommand => new SimpleDelegateCommand(ClipboardItemCommands.OpenSelectedItemAsNewFileCommandExecute);

        // Ctrl + X が押された時の処理
        public static SimpleDelegateCommand CutItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.CutItemCommandExecute);
        // Ctrl + C が押された時の処理
        public static SimpleDelegateCommand CopyToClipboardCommand => new SimpleDelegateCommand(ClipboardItemCommands.CopyToClipboardCommandExecute);
        // Ctrl + V が押された時の処理
        public static SimpleDelegateCommand PasteFromClipboardCommand => new SimpleDelegateCommand(ClipboardItemCommands.PasteFromClipboardCommandExecute);

        // Ctrl + M が押された時の処理
        public static SimpleDelegateCommand MergeItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.MergeItemCommandExecute);
        // Ctrl + Shift + M が押された時の処理
        public static SimpleDelegateCommand MergeItemWithHeaderCommand => new SimpleDelegateCommand(ClipboardItemCommands.MergeItemWithHeaderCommandExecute);

        // メニューの「Pythonスクリプト作成」をクリックしたときの処理
        public static SimpleDelegateCommand CreatePythonScriptCommand => new SimpleDelegateCommand(PythonCommands.CreatePythonScriptCommandExecute);
        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public static SimpleDelegateCommand EditPythonScriptCommand => new SimpleDelegateCommand(PythonCommands.EditPythonScriptCommandExecute);
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public static SimpleDelegateCommand OpenListAutoProcessRuleWindowCommand => new SimpleDelegateCommand(OpenListAutoProcessRuleWindowCommandExecute);
        private static void OpenListAutoProcessRuleWindowCommandExecute(object obj) {
            ListAutoProcessRuleWindow ListAutoProcessRuleWindow = new ListAutoProcessRuleWindow();
            ListAutoProcessRuleWindowViewModel ListAutoProcessRuleWindowViewModel = (ListAutoProcessRuleWindowViewModel)ListAutoProcessRuleWindow.DataContext;
            ListAutoProcessRuleWindowViewModel.Initialize();

            ListAutoProcessRuleWindow.ShowDialog();
        }
        // メニューの「タグ編集」をクリックしたときの処理
        public static SimpleDelegateCommand OpenTagWindowCommand => new SimpleDelegateCommand(OpenTagWindowCommandExecute);
        private static void OpenTagWindowCommandExecute(object obj) {
            TagWindow tagWindow = new TagWindow();

            tagWindow.ShowDialog();
        }
        // ステータスバーをクリックしたときの処理
        public static SimpleDelegateCommand OpenStatusMessageWindowCommand => new SimpleDelegateCommand(OpenStatusMessageWindowCommandExecute);
        private static void OpenStatusMessageWindowCommandExecute(object obj) {
            StatusMessageWindow statusMessageWindow = new StatusMessageWindow();
            StatusMessageWindowViewModel statusMessageWindowViewModel = (StatusMessageWindowViewModel)statusMessageWindow.DataContext;
            statusMessageWindowViewModel.Initialize();
            statusMessageWindow.ShowDialog();
        }

        // コンテキストメニューの「テキストを整形」の実行用コマンド
        public static SimpleDelegateCommand FormatTextCommand => new SimpleDelegateCommand(
            (parameter) => {
                // 選択中のアイテムを取得
                ClipboardItemViewModel clipboardItemViewModel = Instance!.SelectedItem!;
                if (clipboardItemViewModel == null) {
                    return;
                }
                string content = clipboardItemViewModel.ClipboardItem.Content;
                // テキストを整形
                content = AutoProcessCommand.FormatTextCommandExecute(content);
                // 整形したテキストをセット
                clipboardItemViewModel.ClipboardItem.Content = content;
                // LiteDBに保存
                ClipboardDatabaseController.UpsertItem(clipboardItemViewModel.ClipboardItem);
                // 再描写
                ReloadClipboardItems();
            });
        // コンテキストメニューの「ファイルのパスを分割」の実行用コマンド
        public static SimpleDelegateCommand SplitFilePathCommand => new SimpleDelegateCommand(
                       (parameter) => {
                // 選択中のアイテムを取得
                ClipboardItemViewModel clipboardItemViewModel = Instance!.SelectedItem!;
                if (clipboardItemViewModel == null) {
                    return;
                }
                ClipboardItem clipboardItem = clipboardItemViewModel.ClipboardItem;
                // ファイルパスを分割
                AutoProcessCommand.SplitFilePathCommandExecute(clipboardItem);
                // LiteDBに保存
                ClipboardDatabaseController.UpsertItem(clipboardItemViewModel.ClipboardItem);
                // 再描写
                ReloadClipboardItems();
            });

    }

}
