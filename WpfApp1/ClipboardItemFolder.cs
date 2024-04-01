using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WpfApp1
{
    public  class ClipboardItemFolder: ObservableObject
    {

        // プロパティ
        // LiteDBのID
        public int Id { get; set; }
        // フォルダの表示名
        public string DisplayName { get; set; } = "";
        // フォルダの絶対パス
        public string AbsoluteCollectionName { get; set; } = "";
        // 検索条件
        public SearchCondition SearchCondition { get; set;  } = new SearchCondition();

        // 常に検索条件を適用する。
        public bool AlwaysApplySearchCondition { get; set; } = false;
        // 現在、検索条件を適用中かどうか
        public bool IsApplyingSearchCondition { get; set; } = false;

        // 子フォルダ
        public ObservableCollection<ClipboardItemFolder> Children { get; set; } = new ObservableCollection<ClipboardItemFolder>();

        public ObservableCollection<ClipboardItem> Items { get; set; } = new ObservableCollection<ClipboardItem>();

        //AutoProcessItem
        public ObservableCollection<AutoProcessItem> AutoProcessItems { get; set; } = new ObservableCollection<AutoProcessItem>();

        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ClipboardItemFolder()
        {
        }

        public ClipboardItemFolder(ClipboardItemFolder? parent, string collectionName, string displayName)
        {
            if (parent == null)
            {
                AbsoluteCollectionName = collectionName;
            }
            else
            { 
                AbsoluteCollectionName = ClipboardDatabaseController.ConcatenatePath(parent.AbsoluteCollectionName, collectionName);
            }
            DisplayName = displayName;

        }
        public ClipboardItemFolder(string collectionName, string displayName) : this(null, collectionName, displayName)
        {

        }
        //--------------------------------------------------------------------------------

        // ClipboardItemを作成
        public ClipboardItem CreateItem()
        {
            ClipboardItem item = new ClipboardItem();
            item.CollectionName = AbsoluteCollectionName;
            // Itemsに追加
            Items.Add(item);
            // 追加したアイテムをSelectedItemに設定
            if (MainWindowViewModel.Instance != null)
            {
                MainWindowViewModel.Instance.SelectedItem = item;
            }
            ClipboardDatabaseController.UpsertFolder(this);
            return item;
        }
        // ClipboardItemを追加
        public void UpsertItem(ClipboardItem item)
        {
            Items.Add(item);
            // 追加したアイテムをSelectedItemに設定
            if (MainWindowViewModel.Instance != null)
            {
                    MainWindowViewModel.Instance.SelectedItem = item;
            }

            // LiteDBに保存
            ClipboardController.UpsertItem(item);
        }
        // ClipboardItemを削除
        public void DeleteItem(ClipboardItem item)
        {
            Items.Remove(item);
            // LiteDBに保存
            ClipboardController.DeleteItem(item);
        }

        public List<ClipboardItem> GetDuplicateList(ClipboardItem item)
        {
            List<ClipboardItem> dupList = new List<ClipboardItem>();
            // prevCountで指定された数だけ過去のアイテムと比較する
            for (int i = 0; i < Items.Count; i++)
            {
                ClipboardItem prev = Items[i];
                if (item.IsDuplicate(prev))
                {
                    dupList.Add(prev);
                }
            }
            return dupList;
        }

        public void RemoveDuplicateItems(ClipboardItem item)
        {
            List<ClipboardItem> dupList = GetDuplicateList(item);
            foreach (var dupItem in dupList)
            {
                DeleteItem(dupItem);
            }
        }
        public bool Load()
        {
            bool result = ClipboardDatabaseController.Load(this);
            OnPropertyChanged("Children");
            return result;
        }

        public void DeleteItems()
        {
            ClipboardDatabaseController.DeleteItems(this);

        }

        public void Filter(SearchCondition searchCondition)
        {
            ClipboardDatabaseController.Filter(this, searchCondition);
        }

        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        // 自動処理を適用する処理
        public ClipboardItem ApplyAutoProcess(ClipboardItem clipboardItem)
        {
            ClipboardItem result = clipboardItem;
            foreach (var autoProcessItem in AutoProcessItems)
            {
                MainWindowViewModel.Instance?.UpdateStatusText ($"自動処理:{autoProcessItem.Name}を実行します");
                result = autoProcessItem.Execute(result);
            }
            return result;
        }

        //--------------------------------------------------------------------------------
        //--コマンド
        //--------------------------------------------------------------------------------

        // OpenItemCommandの処理
        public static SimpleDelegateCommand OpenFolderCommand => new SimpleDelegateCommand(ClipboardFolderCommands.OpenFolderCommandExecute);

        // 新規フォルダ作成コマンド
        public static SimpleDelegateCommand CreateFolderCommand => new SimpleDelegateCommand(ClipboardFolderCommands.CreateFolderCommandExecute);
        // フォルダ編集コマンド
        public static SimpleDelegateCommand EditFolderCommand => new SimpleDelegateCommand(ClipboardFolderCommands.EditFolderCommandExecute);
        // フォルダ削除コマンド
        public static SimpleDelegateCommand DeleteFolderCommand => new SimpleDelegateCommand(ClipboardFolderCommands.DeleteFolderCommandExecute);


    }
}
