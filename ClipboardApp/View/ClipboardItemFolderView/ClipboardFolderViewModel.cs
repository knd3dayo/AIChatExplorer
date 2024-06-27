using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemView;
using LibGit2Sharp;
using WpfAppCommon.Factory.Default;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemFolderView {
    public partial class ClipboardFolderViewModel(MainWindowViewModel mainWindowViewModel, ClipboardFolder clipboardItemFolder) : MyWindowViewModel {

        // ClipboardFolder
        public ClipboardFolder ClipboardItemFolder { get; } = clipboardItemFolder;
        // MainWindowViewModel
        protected MainWindowViewModel MainWindowViewModel { get; } = mainWindowViewModel;

        // Description
        public string Description {
            get {
                return ClipboardItemFolder.Description;
            }
            set {
                ClipboardItemFolder.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        // Items
        public ObservableCollection<ClipboardItemViewModel> Items { get; } = [];

        // 子フォルダ
        public ObservableCollection<ClipboardFolderViewModel> Children { get; } = [];

        public string FolderName {
            get {
                return ClipboardItemFolder.FolderName;
            }
            set {
                ClipboardItemFolder.FolderName = value;
                OnPropertyChanged(nameof(FolderName));
            }
        }
        public ClipboardFolderViewModel CreateChild(string folderName) {
            // 自身が標準フォルダの場合は、標準フォルダを作成
            if (ClipboardItemFolder.FolderType == ClipboardFolder.FolderTypeEnum.Normal) {
                ClipboardFolder childFolder = ClipboardItemFolder.CreateChild(folderName);
                childFolder.FolderType = ClipboardFolder.FolderTypeEnum.Normal;
                return new ClipboardFolderViewModel(MainWindowViewModel, childFolder);
            }else if (ClipboardItemFolder.FolderType == ClipboardFolder.FolderTypeEnum.Search) {
                // 自身が検索フォルダの場合は、検索フォルダを作成
                ClipboardFolder childFolder = ClipboardItemFolder.CreateChild(folderName);
                childFolder.FolderType = ClipboardFolder.FolderTypeEnum.Search;
                return new SearchFolderViewModel(MainWindowViewModel, childFolder);
            }
            throw new Exception("Invalid FolderType");
        }

        public string FolderPath {
            get {
                return ClipboardItemFolder.FolderPath;
            }
        }

        // Delete
        public void Delete() {
            ClipboardItemFolder.Delete();
        }
        // LoadChildren
        public virtual void LoadChildren() {
            Children.Clear();
            foreach (var child in ClipboardItemFolder.Children) {
                if (child == null) {
                    continue;
                }
                Children.Add(new ClipboardFolderViewModel(MainWindowViewModel, child));
            }

        }
        // LoadItems
        public virtual void LoadItems() {
            Items.Clear();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(item));
            }
        }

        // Load
        public void Load() {
            MainWindowViewModel.IsIndeterminate = true;
            try {

                LoadChildren();
                LoadItems();

                UpdateStatusText();
            } finally {
                MainWindowViewModel.IsIndeterminate = false;
            }

        }
        // AddItem
        public void AddItem(ClipboardItemViewModel item) {
            ClipboardItemFolder.AddItem(item.ClipboardItem);

        }
        public void DeleteItem(ClipboardItemViewModel item) {
            item.Delete();
            Items.Remove(item);

        }
        // Save
        public void Save() {
            ClipboardItemFolder.Save();
        }

        // - コンテキストメニューの削除を表示するかどうか
        public bool IsDeleteVisible {
            get {
                // RootFolderは削除不可
                return ClipboardItemFolder.IsRootFolder == false;
            }
        }
        // - コンテキストメニューの編集を表示するかどうか xamlで使う
        public bool IsEditVisible {
            get {
                // RootFolderは編集不可
                return ClipboardItemFolder.IsRootFolder == false;
            }
        }

        public virtual ObservableCollection<MenuItem> MenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                // 新規作成
                MenuItem createMenuItem = new();
                createMenuItem.Header = StringResources.Create;
                createMenuItem.Command = CreateFolderCommand;
                createMenuItem.CommandParameter = this;
                menuItems.Add(createMenuItem);

                // 編集
                MenuItem editMenuItem = new();
                editMenuItem.Header = StringResources.Edit;
                editMenuItem.Command = EditFolderCommand;
                editMenuItem.IsEnabled = IsEditVisible;
                editMenuItem.CommandParameter = this;
                menuItems.Add(editMenuItem);

                // 削除
                MenuItem deleteMenuItem = new();
                deleteMenuItem.Header = StringResources.Delete;
                deleteMenuItem.Command = DeleteFolderCommand;
                deleteMenuItem.IsEnabled = IsDeleteVisible;
                deleteMenuItem.CommandParameter = this;
                menuItems.Add(deleteMenuItem);

                // インポート    
                MenuItem importMenuItem = new();
                importMenuItem.Header = StringResources.Import;
                importMenuItem.Command = ImportItemsToFolderCommand;
                importMenuItem.CommandParameter = this;
                menuItems.Add(importMenuItem);

                // エクスポート
                MenuItem exportMenuItem = new();
                exportMenuItem.Header = StringResources.Export;
                exportMenuItem.Command = ExportItemsFromFolderCommand;
                exportMenuItem.CommandParameter = this;
                menuItems.Add(exportMenuItem);

                return menuItems;

            }


        }


        private void UpdateStatusText() {
            string message = $"フォルダ[{FolderName}]";
            // AutoProcessRuleが設定されている場合
            var rules = AutoProcessRuleController.GetAutoProcessRules(ClipboardItemFolder);
            if (rules.Count > 0) {
                message += " 自動処理が設定されています[";
                foreach (AutoProcessRule item in rules) {
                    message += item.RuleName + " ";
                }
                message += "]";
            }

            // folderが検索フォルダの場合
            SearchRule? searchConditionRule = ClipboardFolder.GlobalSearchCondition;
            if (ClipboardItemFolder.FolderType == ClipboardFolder.FolderTypeEnum.Search) {
                searchConditionRule = SearchRuleController.GetSearchRuleByFolder(ClipboardItemFolder);
            }
            SearchCondition? searchCondition = searchConditionRule?.SearchCondition;
            // SearchConditionがNullでなく、 Emptyでもない場合
            if (searchCondition != null && !searchCondition.IsEmpty()) {
                message += " 検索条件[";
                message += searchCondition.ToStringSearchCondition();
                message += "]";
            }
            Tools.StatusText.ReadyText = message;
            Tools.StatusText.Text = message;

        }
        
    }
}
