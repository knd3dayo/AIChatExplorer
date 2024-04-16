using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.ClipboardItemView;

namespace WpfApp1.View.ClipboardItemFolderView
{
    public class ClipboardItemFolderViewModel
        (ClipboardItemFolder clipboardItemFolder) : ObservableObject
    {
        // ClipboardItemFolder
        public ClipboardItemFolder ClipboardItemFolder { get; set; } = clipboardItemFolder;

        // 子フォルダ
        public ObservableCollection<ClipboardItemFolderViewModel> Children
        {
            get
            {
                ObservableCollection<ClipboardItemFolderViewModel> children = new ObservableCollection<ClipboardItemFolderViewModel>();
                foreach (ClipboardItemFolder folder in ClipboardItemFolder.Children)
                {
                    children.Add(new ClipboardItemFolderViewModel(folder));
                }
                return children;
            }
            set
            {
                ClipboardItemFolder.Children.Clear();
                foreach (ClipboardItemFolderViewModel folder in value)
                {
                    ClipboardItemFolder.Children.Add(folder.ClipboardItemFolder);
                }
                OnPropertyChanged("Children");
            }
        }
        public string DisplayName
        {
            get
            {
                return ClipboardItemFolder.DisplayName;
            }
            set
            {
                ClipboardItemFolder.DisplayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public string AbsoluteCollectionName {
            get {
                return ClipboardItemFolder.AbsoluteCollectionName;
            }
            set {
                ClipboardItemFolder.AbsoluteCollectionName = value;
                OnPropertyChanged("AbsoluteCollectionName");
            }

        }
        public bool IsSelected
        {
            get
            {
                return ClipboardItemFolder.IsSelected;
            }
            set
            {
                ClipboardItemFolder.IsSelected = value;
                // MainWindowViewModelのSelectedFolderにも反映
                if (MainWindowViewModel.Instance != null) {
                    MainWindowViewModel.Instance.SelectedFolder = this;
                }
                OnPropertyChanged("IsSelected");
            }
        }
        // FolderSelectWindowで選択されたフォルダを適用する処理
        private bool _IsSelectedOnFolderSelectWindow;
        public bool IsSelectedOnFolderSelectWindow {
            get { return _IsSelectedOnFolderSelectWindow; }
            set {
                _IsSelectedOnFolderSelectWindow = value;
                OnPropertyChanged("_IsSelectedOnFolderSelectWindow");
            }
        }


        // Items
        public ObservableCollection<ClipboardItemViewModel> Items
        {
            get
            {
                ObservableCollection<ClipboardItemViewModel> items = new ObservableCollection<ClipboardItemViewModel>();
                foreach (ClipboardItem item in ClipboardItemFolder.Items)
                {
                    items.Add(new ClipboardItemViewModel(item));
                }
                return items;
            }
            set
            {
                ClipboardItemFolder.Items.Clear();
                foreach (ClipboardItemViewModel item in value)
                {
                    ClipboardItemFolder.Items.Add(item.ClipboardItem);
                }
                OnPropertyChanged("Items");
            }
        }
        // Load
        public void Load()
        {
            // Children.Item,SearchCondition,AutoProcessRule を更新

            foreach (ClipboardItem item in ClipboardItemFolder.Items)
            {
                Items.Add(new ClipboardItemViewModel(item));
            }
            OnPropertyChanged("Items");

            // Childrenを更新
            Children.Clear();
            foreach (ClipboardItemFolder folder in ClipboardItemFolder.Children)
            {
                Children.Add(new ClipboardItemFolderViewModel(folder));
            }
            OnPropertyChanged("Children");



        }

        // - コンテキストメニューの削除を表示するかどうか
        public bool IsDeleteVisible
        {
            get
            {
                // RootFolderは削除不可
                if (ClipboardItemFolder.AbsoluteCollectionName == ClipboardDatabaseController.CLIPBOARD_ROOT_FOLDER_NAME)
                {
                    return false;
                }
                // SearchRootFolderは削除不可
                if (ClipboardItemFolder.AbsoluteCollectionName == ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME)
                {
                    return false;
                }
                return true;
            }
        }
        // - コンテキストメニューの編集を表示するかどうか
        public bool IsEditVisible
        {
            get
            {
                // SearchRootFolderは編集不可
                if (ClipboardItemFolder.AbsoluteCollectionName == ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME)
                {
                    return false;
                }
                return true;
            }
        }
        // - コンテキストメニューの新規作成を表示するかどうか
        public bool IsCreateVisible
        {
            get
            {   // 検索フォルダの子フォルダは新規作成不可
                if (ClipboardItemFolder.IsSearchFolder)
                {
                    return false;
                }
                return true;
            }
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

        // FolderSelectWindowでFolderSelectWindowSelectFolderCommandが実行されたときの処理
        public static SimpleDelegateCommand FolderSelectWindowSelectFolderCommand => new SimpleDelegateCommand(FolderSelectWindowViewModel.FolderSelectWindowSelectFolderCommandExecute);

        // フォルダ内のアイテムをJSON形式でエクスポートする処理
        public static SimpleDelegateCommand ExportItemsFromFolderCommand => new SimpleDelegateCommand(ClipboardFolderCommands.ExportItemsFromFolderCommandExecute);
        // フォルダ内のアイテムをJSON形式でインポートする処理
        public static SimpleDelegateCommand ImportItemsToFolderCommand => new SimpleDelegateCommand(ClipboardFolderCommands.ImportItemsToFolderCommandExecute);

    }
}
