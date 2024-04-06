using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;


namespace WpfApp1
{

    public class MainWindowViewModel: ObservableObject
    {
        public static MainWindowViewModel? Instance = null;

        public MainWindowViewModel()
        {
            // ロギング設定
            Tools.StatusText = StatusText;

            // クリップボードコントローラーの初期化
            ClipboardController.Init(this);
            Instance = this;


            ClipboardItemFolders.Add(ClipboardController.RootFolder);

            OnPropertyChanged("ClipboardItemFolders");

        }

        // ClipboardItemFolder

        public static ObservableCollection<ClipboardItemFolder> ClipboardItemFolders { get; set; } = new ObservableCollection<ClipboardItemFolder>();

        // Cutフラグ
        public bool CutFlag { get; set; } = false;

        // 選択中のアイテム
        private ClipboardItem? _selectedItem = null;
        public ClipboardItem? SelectedItem { 
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
        // 選択中のフォルダ
        private  ClipboardItemFolder? _selectedFolder = ClipboardController.RootFolder;
        public   ClipboardItemFolder? SelectedFolder {
            get
            {
                return _selectedFolder;
            }
            set
            {
                _selectedFolder = value;
                OnPropertyChanged("SelectedFolder");
            }
        }
        // Ctrl + C が押された時のClipboardItem
        public ClipboardItem? CopiedItem { get; set; } = null;

        // Ctrl + C が押された時のClipboardItemFolder
        public ClipboardItemFolder? CopiedItemFolder { get; set; } = null;
        // static 

        // ステータスバーのテキスト
        public static StatusText StatusText { get; } = new StatusText();

        // ステータスバーのテキスト更新と通知
        public void UpdateStatusText(string text)
        {
            StatusText.Text = text;
            OnPropertyChanged("StatusText");
        }
        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------

        // Ctrl + Q が押された時の処理
        public static SimpleDelegateCommand ExitCommand => new(ExitCommandExecute);

        private static void ExitCommandExecute(Object obj)
        {
            // System.Windows.MessageBox.Show("Exit");
            System.Windows.Application.Current.Shutdown();
        }

        // Ctrl + F が押された時の処理
        public static SimpleDelegateCommand searchCommand => new SimpleDelegateCommand(ClipboardFolderCommands.SearchCommandExecute);

        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public static SimpleDelegateCommand DeleteDisplayedItemCommand => new SimpleDelegateCommand(ClipboardFolderCommands.DeleteDisplayedItemCommandExecute);

        // Ctrl + R が押された時の処理
        public static SimpleDelegateCommand ReloadCommand => new SimpleDelegateCommand(ClipboardFolderCommands.ReloadCommandExecute);


        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public static SimpleDelegateCommand DeleteSelectedItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.DeleteSelectedItemCommandExecute);

        // 選択中のアイテムを開く処理
        public static SimpleDelegateCommand OpenSelectedItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.OpenSelectedItemCommandExecute);
        // 選択したアイテムを新規として開く処理
        public static SimpleDelegateCommand OpenSelectedItemAsNewCommand => new SimpleDelegateCommand(ClipboardItemCommands.OpenSelectedItemAsNewCommandExecute);

        // Ctrl + N が押された時の処理
        public static SimpleDelegateCommand CreateItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.CreateItemCommandExecute);

        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand SettingCommand => new SimpleDelegateCommand(SettingCommandExecute);
        private static void SettingCommandExecute(object obj)
        {
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }
        

        // Ctrl + X が押された時の処理
        public static SimpleDelegateCommand CutItemCommand => new SimpleDelegateCommand(ClipboardItemCommands.CutItemCommandExecute);
        // Ctrl + C が押された時の処理
        public static SimpleDelegateCommand CopyToClipboardCommand => new SimpleDelegateCommand(ClipboardItemCommands.CopyToClipboardCommandExecute);
        // Ctrl + V が押された時の処理
        public static SimpleDelegateCommand PasteFromClipboardCommand => new SimpleDelegateCommand(ClipboardItemCommands.PasteFromClipboardCommandExecute);

        // メニューの「Pythonスクリプト作成」をクリックしたときの処理
        public static SimpleDelegateCommand CreatePythonScriptCommand => new SimpleDelegateCommand(PythonCommands.CreatePythonScriptCommandExecute);
        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public static SimpleDelegateCommand EditPythonScriptCommand => new SimpleDelegateCommand(PythonCommands.EditPythonScriptCommandExecute);
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public static SimpleDelegateCommand OpenListAutoProcessRuleWindowCommand => new SimpleDelegateCommand(OpenListAutoProcessRuleWindowCommandExecute);
        private static void OpenListAutoProcessRuleWindowCommandExecute(object obj)
        {
            ListAutoProcessRuleWindow ListAutoProcessRuleWindow = new ListAutoProcessRuleWindow();
            ListAutoProcessRuleWindowViewModel ListAutoProcessRuleWindowViewModel = ((ListAutoProcessRuleWindowViewModel)ListAutoProcessRuleWindow.DataContext);
            ListAutoProcessRuleWindowViewModel.Initialize();

            ListAutoProcessRuleWindow.ShowDialog();
        }
        
        // スクリプトを削除するコマンド


        // OpenAIChatを実行したときの処理

    }

}
