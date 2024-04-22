using System.Collections.ObjectModel;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipboardApp.View.ClipboardItemFolderView {
    public class ClipboardItemFolderViewModel
        (ClipboardItemFolder clipboardItemFolder) : ObservableObject {
        // ClipboardItemFolder
        public ClipboardItemFolder ClipboardItemFolder { get; set; } = clipboardItemFolder;

        // 子フォルダ
        public ObservableCollection<ClipboardItemFolderViewModel> Children {
            get {
                ObservableCollection<ClipboardItemFolderViewModel> children = new ObservableCollection<ClipboardItemFolderViewModel>();
                foreach (ClipboardItemFolder folder in ClipboardItemFolder.Children) {
                    children.Add(new ClipboardItemFolderViewModel(folder));
                }
                return children;
            }
            set {
                ClipboardItemFolder.Children.Clear();
                foreach (ClipboardItemFolderViewModel folder in value) {
                    ClipboardItemFolder.Children.Add(folder.ClipboardItemFolder);
                }
                OnPropertyChanged("Children");
            }
        }
        public string DisplayName {
            get {
                return ClipboardItemFolder.DisplayName;
            }
            set {
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
        public bool IsSelected {
            get {
                return ClipboardItemFolder.IsSelected;
            }
            set {
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
        public ObservableCollection<ClipboardItemViewModel> Items {
            get {
                ObservableCollection<ClipboardItemViewModel> items = new ObservableCollection<ClipboardItemViewModel>();
                foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                    items.Add(new ClipboardItemViewModel(item));
                }
                return items;
            }
            set {
                ClipboardItemFolder.Items.Clear();
                foreach (ClipboardItemViewModel item in value) {
                    ClipboardItemFolder.Items.Add(item.ClipboardItem);
                }
                OnPropertyChanged("Items");
            }
        }
        // Load
        public void Load() {
            // Children.Item,SearchCondition,AutoProcessRule を更新

            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(item));
            }
            OnPropertyChanged("Items");

            // Childrenを更新
            Children.Clear();
            foreach (ClipboardItemFolder folder in ClipboardItemFolder.Children) {
                Children.Add(new ClipboardItemFolderViewModel(folder));
            }
            OnPropertyChanged("Children");

            // StatusTextを更新
            UpdateStatusText(this);

        }

        // - コンテキストメニューの削除を表示するかどうか
        public bool IsDeleteVisible {
            get {
                // RootFolderは削除不可
                if (ClipboardItemFolder.AbsoluteCollectionName == ClipboardDatabaseController.CLIPBOARD_ROOT_FOLDER_NAME) {
                    return false;
                }
                // SearchRootFolderは削除不可
                if (ClipboardItemFolder.AbsoluteCollectionName == ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME) {
                    return false;
                }
                return true;
            }
        }
        // - コンテキストメニューの編集を表示するかどうか
        public bool IsEditVisible {
            get {
                // SearchRootFolderは編集不可
                if (ClipboardItemFolder.AbsoluteCollectionName == ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME) {
                    return false;
                }
                return true;
            }
        }
        // - コンテキストメニューの新規作成を表示するかどうか
        public bool IsCreateVisible {
            get {   // 検索フォルダの子フォルダは新規作成不可
                if (ClipboardItemFolder.IsSearchFolder) {
                    return false;
                }
                return true;
            }
        }

        private static void UpdateStatusText(ClipboardItemFolderViewModel folderViewModel) {
            StatusText? StatusText = MainWindowViewModel.StatusText;
            if (StatusText == null) {
                return;
            }
            string message = $"フォルダ[{folderViewModel.DisplayName}]";
            // AutoProcessRuleが設定されている場合
            if (ClipboardDatabaseController.GetAutoProcessRules(folderViewModel.ClipboardItemFolder).Count() > 0) {
                message += " 自動処理が設定されています[";
                foreach (AutoProcessRule item in ClipboardDatabaseController.GetAutoProcessRules(folderViewModel.ClipboardItemFolder)) {
                    message += item.RuleName + " ";
                }
                message += "]";
            }

            // folderが検索フォルダの場合
            SearchConditionRule? searchConditionRule = ClipboardItemFolder.GlobalSearchCondition;
            if (folderViewModel.ClipboardItemFolder.IsSearchFolder) {
                searchConditionRule = ClipboardDatabaseController.GetSearchConditionRuleByCollectionName(folderViewModel.ClipboardItemFolder.AbsoluteCollectionName);
            }
            SearchCondition? searchCondition = searchConditionRule?.SearchCondition;
            // SearchConditionがNullでなく、 Emptyでもない場合
            if (searchCondition != null && ! searchCondition.IsEmpty()) {
                message += " 検索条件[";
                message += searchCondition.ToStringSearchCondition();
                message += "]";
            }
            StatusText.Text = message;
            StatusText.InitText = message;

        }

        //--------------------------------------------------------------------------------
        //--コマンド
        //--------------------------------------------------------------------------------

        // OpenItemCommandの処理
        // public static SimpleDelegateCommand OpenFolderCommand => new SimpleDelegateCommand(ClipboardFolderCommands.OpenFolderCommandExecute);

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
