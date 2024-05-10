using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemView;
using WpfAppCommon.Factory.Default;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemFolderView {
    public partial class ClipboardFolderViewModel(MainWindowViewModel mainWindowViewModel, ClipboardFolder clipboardItemFolder) : MyWindowViewModel {

        // ClipboardFolder
        private ClipboardFolder ClipboardItemFolder { get; } = clipboardItemFolder;
        // MainWindowViewModel
        public MainWindowViewModel MainWindowViewModel { get; } = mainWindowViewModel;

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

        public string DisplayName {
            get {
                return ClipboardItemFolder.DisplayName;
            }
            set {
                ClipboardItemFolder.DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }
        public ClipboardFolderViewModel CreateChild(string collectionName, string displayName) {
            ClipboardFolder childFolder = ClipboardItemFolder.CreateChild(collectionName, displayName);
            ClipboardItemFolder.Children.Add(childFolder);
            return new ClipboardFolderViewModel(MainWindowViewModel, childFolder);
        }

        public string AbsoluteCollectionName {
            get {
                return ClipboardItemFolder.AbsoluteCollectionName;
            }
            set {
                ClipboardItemFolder.AbsoluteCollectionName = value;
                OnPropertyChanged(nameof(AbsoluteCollectionName));
            }

        }

        // Delete
        public void Delete() {
            ClipboardItemFolder.Delete();
        }
        // LoadChildren
        public void LoadChildren() {
            Children.Clear();
            foreach (ClipboardFolder folder in ClipboardItemFolder.Children) {
                Children.Add(new ClipboardFolderViewModel(MainWindowViewModel, folder));
            }
        }
        // LoadItems
        public void LoadItems() {
            Items.Clear();
            // DBから読み込み
            ClipboardItemFolder.Load();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(this, item));
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
        public ClipboardItemViewModel AddItem(ClipboardItemViewModel item) {
            return ClipboardItemViewModel.AddItem(this.ClipboardItemFolder, item);
        }
        public void DeleteItem(ClipboardItemViewModel item) {
            item.Delete();
            Items.Remove(item);

        }
        // - コンテキストメニューの削除を表示するかどうか
        public bool IsDeleteVisible {
            get {
                // RootFolderは削除不可
                if (ClipboardItemFolder.AbsoluteCollectionName == DefaultClipboardDBController.CLIPBOARD_ROOT_FOLDER_NAME) {
                    return false;
                }
                // SearchRootFolderは削除不可
                if (ClipboardItemFolder.AbsoluteCollectionName == DefaultClipboardDBController.SEARCH_ROOT_FOLDER_NAME) {
                    return false;
                }
                return true;
            }
        }
        // - コンテキストメニューの編集を表示するかどうか xamlで使う
        public bool IsEditVisible {
            get {
                // SearchRootFolderは編集不可
                if (ClipboardItemFolder.AbsoluteCollectionName == DefaultClipboardDBController.SEARCH_ROOT_FOLDER_NAME) {
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
            string message = $"フォルダ[{DisplayName}]";
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
                searchConditionRule = SearchRuleController.GetSearchRuleByFolderName(ClipboardItemFolder.AbsoluteCollectionName);
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
        // AutoProcessRuleのTargetFolderを設定
        public void SetAutoProcessRuleTargetFolder(AutoProcessRule autoProcessRule) {
            autoProcessRule.TargetFolder = ClipboardItemFolder;
        }
        // AutoProcessRuleのDestinationFolderを設定
        public void SetAutoProcessRuleDestinationFolder(AutoProcessRule autoProcessRule) {
            autoProcessRule.DestinationFolder = ClipboardItemFolder;
        }
        // AutoProcessRuleを取得
        public ObservableCollection<AutoProcessRule> GetAutoProcessRules() {
            return AutoProcessRuleController.GetAutoProcessRules(ClipboardItemFolder);
        }
        // AutoProcessRuleを追加
        public void AddAutoProcessRule(AutoProcessRule rule) {
            ClipboardItemFolder.AddAutoProcessRule(rule);
        }

        // AddChild
        public void AddChild(ClipboardFolderViewModel child) {
            ClipboardItemFolder.Children.Add(child.ClipboardItemFolder);
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
        public static SimpleDelegateCommand CreateFolderCommand => new((parameter) => {
            // parameterがClipboardItemFolderViewModelではない場合はエラーメッセージを表示
            if (parameter is not ClipboardFolderViewModel folderViewModel) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardFolderViewModel.CreateFolderCommandExecute(folderViewModel, () => {
                folderViewModel.Load();
            });
        });
        // フォルダ編集コマンド
        public SimpleDelegateCommand EditFolderCommand => new((parameter) => {
            ClipboardFolderViewModel.EditFolderCommandExecute(parameter, () => {
                Load();
                Tools.Info("フォルダを編集しました");
            });
        });

        // フォルダ削除コマンド
        public SimpleDelegateCommand DeleteFolderCommand => new(ClipboardFolderViewModel.DeleteFolderCommandExecute);

        // FolderSelectWindowでFolderSelectWindowSelectFolderCommandが実行されたときの処理
        public static SimpleDelegateCommand FolderSelectWindowSelectFolderCommand => new(FolderSelectWindowViewModel.FolderSelectWindowSelectFolderCommandExecute);

        // フォルダ内のアイテムをJSON形式でエクスポートする処理
        public SimpleDelegateCommand ExportItemsFromFolderCommand => new(
            (parameter) => {
                ClipboardFolderViewModel.ExportItemsFromFolderCommandExecute(this);
            });

        // フォルダ内のアイテムをJSON形式でインポートする処理
        public SimpleDelegateCommand ImportItemsToFolderCommand => new((parameter) => {
            ClipboardFolderViewModel.ImportItemsToFolderCommandExecute(this);
        });

    }
}
