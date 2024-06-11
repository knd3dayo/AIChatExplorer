using System.Collections.ObjectModel;
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
        public MainWindowViewModel MainWindowViewModel { get; } = mainWindowViewModel;

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

        // 検索フォルダかどうか
        public bool IsSearchFolder {
            get {
                return ClipboardItemFolder.IsSearchFolder;
            }
            set {
                ClipboardItemFolder.IsSearchFolder = value;
                OnPropertyChanged(nameof(IsSearchFolder));
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
            ClipboardFolder childFolder = ClipboardItemFolder.CreateChild(folderName);

            return new ClipboardFolderViewModel(MainWindowViewModel, childFolder);
        }

        /**
        public string CollectionName {
            get {
                return ClipboardItemFolder.CollectionName;
            }

        }
        **/

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
        public void LoadChildren() {
            Children.Clear();
            foreach (var child in ClipboardItemFolder.Children) {
                if (child == null) {
                    continue;
                }
                Children.Add(new ClipboardFolderViewModel(MainWindowViewModel, child));
            }
        }
        // LoadItems
        public void LoadItems() {
            Items.Clear();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(item));
            }
        }

        // Load
        public void Load() {
            MainWindowViewModel.IsIndeterminate = true;
            try {
                // DBから読み込み
                ClipboardItemFolder.Load();

                UpdateStatusText();
            } finally {
                MainWindowViewModel.IsIndeterminate = false;
            }
            LoadChildren();
            LoadItems();

        }
        // AddItem
        public void AddItem(ClipboardItemViewModel item) {
            ClipboardItemFolder.AddItem(item.ClipboardItem);

        }
        public void DeleteItem(ClipboardItemViewModel item) {
            item.Delete();
            Items.Remove(item);

        }
        // - コンテキストメニューの削除を表示するかどうか
        public bool IsDeleteVisible {
            get {
                // RootFolderは削除不可
                if (ClipboardItemFolder.Id == ClipboardFolder.RootFolder.Id) {
                    return false;
                }
                // SearchRootFolderは削除不可
                if (ClipboardItemFolder.Id == ClipboardFolder.SearchRootFolder.Id) {
                    return false;
                }
                return true;
            }
        }
        // - コンテキストメニューの編集を表示するかどうか xamlで使う
        public bool IsEditVisible {
            get {
                // SearchRootFolderは編集不可
                if (ClipboardItemFolder.Id == ClipboardFolder.SearchRootFolder.Id) {
                    return false;
                }
                return true;
            }
        }
        // - コンテキストメニューの新規作成を表示するかどうか　xamlで使う
        public bool IsCreateVisible {
            get {   // 検索フォルダの子フォルダは新規作成不可
                if (ClipboardItemFolder.IsSearchFolder) {
                    return false;
                }
                return true;
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
            if (ClipboardItemFolder.IsSearchFolder) {
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
        public void SetSearchFolder(SearchRule searchRule) {
            searchRule.SearchFolder = ClipboardItemFolder;
        }
        public void SetSearchTargetFolder(SearchRule searchRule) {
            searchRule.TargetFolder = ClipboardItemFolder;
        }

        // AutoProcessRuleを取得
        public ObservableCollection<AutoProcessRule> GetAutoProcessRules() {
            return AutoProcessRuleController.GetAutoProcessRules(ClipboardItemFolder);
        }
        // AutoProcessRuleを追加
        public void AddAutoProcessRule(AutoProcessRule rule) {
            ClipboardItemFolder.AddAutoProcessRule(rule);
        }


        // Save
        public void Save() {
            ClipboardItemFolder.Save();
        }

        // フォルダ内のアイテムをJSON形式でExport
        public void ExportItemsToJson(string directoryPath) {
            this.ClipboardItemFolder.ExportItemsToJson(directoryPath);
        }

        //exportしたJSONファイルをインポート
        public void ImportItemsFromJson(string json, Action<ActionMessage> action) {
            this.ClipboardItemFolder.ImportItemsFromJson(json, action);
        }

        //--------------------------------------------------------------------------------
        //--コマンド
        //--------------------------------------------------------------------------------

        // 新規フォルダ作成コマンド
        public static SimpleDelegateCommand<ClipboardFolderViewModel> CreateFolderCommand => new((folderViewModel) => {
            
            CreateFolderCommandExecute(folderViewModel, () => {
                // 親フォルダを保存
                folderViewModel.Save();
                folderViewModel.Load();
            });
        });
        // フォルダ編集コマンド
        public SimpleDelegateCommand<ClipboardFolderViewModel> EditFolderCommand => new((parameter) => {
            
            EditFolderCommandExecute(parameter, () => {
                Load();
                Tools.Info("フォルダを編集しました");
            });
        });

        // フォルダ削除コマンド
        public SimpleDelegateCommand<object> DeleteFolderCommand => new(ClipboardFolderViewModel.DeleteFolderCommandExecute);

        // FolderSelectWindowでFolderSelectWindowSelectFolderCommandが実行されたときの処理
        public static SimpleDelegateCommand<object> FolderSelectWindowSelectFolderCommand => new(FolderSelectWindowViewModel.FolderSelectWindowSelectFolderCommandExecute);

        // フォルダ内のアイテムをJSON形式でエクスポートする処理
        public SimpleDelegateCommand<object> ExportItemsFromFolderCommand => new(
            (parameter) => {
                ClipboardFolderViewModel.ExportItemsFromFolderCommandExecute(this);
            });

        // フォルダ内のアイテムをJSON形式でインポートする処理
        public SimpleDelegateCommand<object> ImportItemsToFolderCommand => new((parameter) => {
            ClipboardFolderViewModel.ImportItemsToFolderCommandExecute(this);
        });

    }
}
