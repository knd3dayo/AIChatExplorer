using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WpfApp1
{
    public class FolderEditWindowViewModel : ObservableObject
    {
        // 編集モードか新規子フォルダ作成モードか
        public enum Mode
        {
            Edit,
            CreateChild
        }
        // Windowの名前
        public string WindowName
        {
            get
            {
                return CurrentMode == Mode.Edit ? "フォルダ編集" : "新規フォルダ作成";
            }
        }
        public Mode CurrentMode { get; set; }
        private string _collectionName = "";
        public string CollectionName
        {
            get
            {
                return _collectionName;
            }
            set
            {
                _collectionName = value;
                // プロパティが変更されたことを通知
                OnPropertyChanged("CollectionName");
            }
        }
        private string _displayName = "";
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
                // プロパティが変更されたことを通知
                OnPropertyChanged("DisplayName");
            }
        }
        public ClipboardItemFolder? Folder { get; set; }


        // 検索条件を常時適用するかどうか
        private bool _alwaysApplySearchCondition = false;
        public bool AlwaysApplySearchCondition
        {
            get
            {
                return _alwaysApplySearchCondition;
            }
            set
            {
                _alwaysApplySearchCondition = value;
                OnPropertyChanged("AlwaysApplySearchCondition");
            }
        }

        // CollectionNameが編集可能かどうか
        public bool IsCollectionNameEditable
        {
            get
            {
                return CurrentMode == Mode.CreateChild;
            }
        }

        public SimpleDelegateCommand CreateCommand => new SimpleDelegateCommand(CreateCommandExecute);

        // 起動時の処理
        public void Init(ClipboardItemFolder clipboardItemFolder, Mode mode)
        {
            CurrentMode = mode;
            OnPropertyChanged("IsCollectionNameEditable");

            Folder = clipboardItemFolder;
            // 編集モードの場合
            if (CurrentMode == Mode.Edit)
            {
                // CollectionNameを設定
                // AbsoluteCollectionNameを_で分割して最後の要素をCollectionNameに設定
                CollectionName = clipboardItemFolder.AbsoluteCollectionName.Split('_').Last();

                // 常時検索条件を適用するかどうか
                AlwaysApplySearchCondition = clipboardItemFolder.AlwaysApplySearchCondition;


                DisplayName = clipboardItemFolder.DisplayName;
            }
            // 新規子フォルダ作成モードの場合
            else if (CurrentMode == Mode.CreateChild)
            {
                // CollectionNameを設定
                CollectionName = "";
                DisplayName = "";
            }

        }
        private void CreateCommandExecute(object parameter)
        {

            if (Folder == null)
            {
                Tools.ShowMessage("フォルダが指定されていません");
                return;
            }
            // CollectionNameが空の場合はエラー
            if (CollectionName == "")
            {
                Tools.ShowMessage("フォルダ名を入力してください");
                return;
            }
            // DisplayNameが空の場合はエラー
            if (DisplayName == "")
            {
                Tools.ShowMessage("表示名を入力してください");
                return;
            }
            // CollectionNameが[a-Z$_]以外の場合はエラー
            if (!System.Text.RegularExpressions.Regex.IsMatch(CollectionName, "^[a-zA-Z0-9]+$"))
            {
                Tools.ShowMessage("フォルダ名は英文字で入力してください");
                return;
            }

            // 編集モードの場合
            if (CurrentMode == Mode.Edit)
            {
                // DisplayNameを設定
                Folder.DisplayName = DisplayName;
                // 検索条件を常時適用するかどうか
                Folder.AlwaysApplySearchCondition = AlwaysApplySearchCondition;

                ClipboardDatabaseController.UpsertFolder(Folder);
            }
            // 新規子フォルダ作成モードの場合
            else if (CurrentMode == Mode.CreateChild)
            {
                // フォルダを作成
                ClipboardItemFolder child = new ClipboardItemFolder(Folder, CollectionName, DisplayName);
                // 検索条件を常時適用するかどうか
                child.AlwaysApplySearchCondition = AlwaysApplySearchCondition;

                ClipboardDatabaseController.UpsertFolder(child);
                // 親フォルダと子フォルダの関係を登録
                ClipboardDatabaseController.UpsertFolderRelation(Folder, child);

            }

            // RootFolderを再読み込み
            ClipboardController.RootFolder.Load();

            FolderEditWindow.Current?.Close();
        }

        public SimpleDelegateCommand CancelCommand => new SimpleDelegateCommand(CancelCommandExecute);
        private void CancelCommandExecute(object parameter)
        {
            FolderEditWindow.Current?.Close();

        }

        // OpenListAutoProcessRuleWindowCommand
        public SimpleDelegateCommand OpenListAutoProcessRuleWindowCommand => new SimpleDelegateCommand(OpenListAutoProcessRuleWindowCommandExecute);
        private void OpenListAutoProcessRuleWindowCommandExecute(object parameter)
        {
            if (Folder == null)
            {
                Tools.Error("フォルダが指定されていません");
                return;
            }
            ListAutoProcessRuleWindow ListAutoProcessRuleWindow = new ListAutoProcessRuleWindow();
            ListAutoProcessRuleWindowViewModel ListAutoProcessRuleWindowViewModel = ((ListAutoProcessRuleWindowViewModel)ListAutoProcessRuleWindow.DataContext);
            ListAutoProcessRuleWindowViewModel.Initialize(Folder);

            ListAutoProcessRuleWindow.ShowDialog();
        }

    }

}
