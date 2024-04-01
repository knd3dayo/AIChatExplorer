using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WpfApp1
{
    public class FolderEditWindowViewModel : ObservableObject
    {
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

        // 自動処理1のItemSource
        public ObservableCollection<AutoProcessItem> AutoProcessItems { get; set; } = AutoProcessItem.AutoProcessItems;
        // 自動処理1の選択中のItem
        public AutoProcessItem? SelectedAutoProcessItem1 { get; set; }
        // 自動処理2の選択中のItem
        public AutoProcessItem? SelectedAutoProcessItem2 { get; set; }
        // 自動処理3の選択中のItem
        public AutoProcessItem? SelectedAutoProcessItem3 { get; set; }

        public SimpleDelegateCommand CreateCommand => new SimpleDelegateCommand(CreateCommandExecute);

        // 起動時の処理
        public void Init(ClipboardItemFolder clipboardItemFolder)
        {
            Folder = clipboardItemFolder;
            // AbsoluteCollectionNameを_で分割して最後の要素をCollectionNameに設定
            CollectionName =　clipboardItemFolder.AbsoluteCollectionName.Split('_').Last();

            // 常時検索条件を適用するかどうか
            AlwaysApplySearchCondition = clipboardItemFolder.AlwaysApplySearchCondition;

            DisplayName = clipboardItemFolder.DisplayName;
            if (clipboardItemFolder.AutoProcessItems.Count > 0)
            {
                SelectedAutoProcessItem1 = clipboardItemFolder.AutoProcessItems[0];
            }
            if (clipboardItemFolder.AutoProcessItems.Count > 1)
            {
                SelectedAutoProcessItem2 = clipboardItemFolder.AutoProcessItems[1];
            }
            if (clipboardItemFolder.AutoProcessItems.Count > 2)
            {
                SelectedAutoProcessItem3 = clipboardItemFolder.AutoProcessItems[2];
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
            // DisplayNameを設定
            Folder.DisplayName = DisplayName;
            // 検索条件を常時適用するかどうか
            Folder.AlwaysApplySearchCondition = AlwaysApplySearchCondition;

            // 自動処理を追加
            if (SelectedAutoProcessItem1 != null)
            {
                Folder.AutoProcessItems.Add(SelectedAutoProcessItem1);
            }
            if (SelectedAutoProcessItem2 != null)
            {
                Folder.AutoProcessItems.Add(SelectedAutoProcessItem2);
            }
            if (SelectedAutoProcessItem3 != null)
            {
                Folder.AutoProcessItems.Add(SelectedAutoProcessItem3);
            }
            ClipboardDatabaseController.UpsertFolder(Folder);

            // RootFolderを再読み込み
            ClipboardController.RootFolder.Load();

            FolderEditWindow.Current?.Close();
        }

        public SimpleDelegateCommand CancelCommand => new SimpleDelegateCommand(CancelCommandExecute);
        private void CancelCommandExecute(object parameter)
        {
            FolderEditWindow.Current?.Close();

        }
    }

}
