using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WpfApp1
{
    public class FolderCreateWindowViewModel : ObservableObject
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
        public ClipboardItemFolder? ParentFolder { get; set; }

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
        private void CreateCommandExecute(object parameter)
        {

            if (ParentFolder == null)
            {
                Tools.ShowMessage("親フォルダが指定されていません");
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
            if (!System.Text.RegularExpressions.Regex.IsMatch(CollectionName, "^[a-zA-Z0-9_]+$"))
            {
                Tools.ShowMessage("フォルダ名は英文字で入力してください");
                return;
            }

            // フォルダを作成
            ClipboardItemFolder child = new ClipboardItemFolder(CollectionName, DisplayName);
            // 検索条件を常時適用するかどうか
            child.AlwaysApplySearchCondition = AlwaysApplySearchCondition;

            // 自動処理を追加
            if (SelectedAutoProcessItem1 != null)
            {
                child.AutoProcessItems.Add(SelectedAutoProcessItem1);
            }
            if (SelectedAutoProcessItem2 != null)
            {
                child.AutoProcessItems.Add(SelectedAutoProcessItem2);
            }
            if (SelectedAutoProcessItem3 != null)
            {
                child.AutoProcessItems.Add(SelectedAutoProcessItem3);
            }
            ClipboardDatabaseController.UpsertFolder(child);
            ClipboardDatabaseController.UpsertFolderRelation(ParentFolder, child);

            // RootFolderを再読み込み
            ParentFolder.Load();

            FolderCreateWindow.Current?.Close();
        }

        public SimpleDelegateCommand CancelCommand => new SimpleDelegateCommand(CancelCommandExecute);
        private void CancelCommandExecute(object parameter)
        {
            FolderCreateWindow.Current?.Close();

        }
    }

}
