using System.Collections.ObjectModel;
using ClipboardApp.Factory;
using ClipboardApp.Factory.Default;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemView;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipboardApp.View.ClipboardItemFolderView
{
    public class ClipboardItemFolderViewModel
        (ClipboardItemFolder clipboardItemFolder) : ObservableObject {
        // ClipboardItemFolder
        public ClipboardItemFolder ClipboardItemFolder { get; set; } = clipboardItemFolder;


        // Items
        public ObservableCollection<ClipboardItemViewModel> Items {
            get {
                ObservableCollection<ClipboardItemViewModel> items = [];
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
                OnPropertyChanged(nameof(Items));
            }
        }
        // 子フォルダ
        public ObservableCollection<ClipboardItemFolderViewModel> Children {
            get {
                ObservableCollection<ClipboardItemFolderViewModel> children = [];
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
                OnPropertyChanged(nameof(Children));
            }
        }
        public string DisplayName {
            get {
                return ClipboardItemFolder.DisplayName;
            }
            set {
                ClipboardItemFolder.DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
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

        // Load
        public void Load() {
            // Children.Item,SearchCondition,AutoProcessRule を更新
            OnPropertyChanged(nameof(Items));
            // Childrenを更新
            OnPropertyChanged(nameof(Children));
            // StatusTextを更新
            UpdateStatusText();

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
            StatusText? StatusText = MainWindowViewModel.StatusText;
            if (StatusText == null) {
                return;
            }
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
            SearchRule? searchConditionRule = ClipboardItemFolder.GlobalSearchCondition;
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
            StatusText.InitText = message;
            StatusText.Text = message;

        }

        //--------------------------------------------------------------------------------
        //--コマンド
        //--------------------------------------------------------------------------------

        // 新規フォルダ作成コマンド
        public static SimpleDelegateCommand CreateFolderCommand => new((parameter) => {
            // parameterがClipboardItemFolderViewModelではない場合はエラーメッセージを表示
            if (parameter is not ClipboardItemFolderViewModel folderViewModel) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardFolderCommands.CreateFolderCommandExecute(folderViewModel, () => {
                folderViewModel.Load();
            });
        });
        // フォルダ編集コマンド
        public static SimpleDelegateCommand EditFolderCommand => new (ClipboardFolderCommands.EditFolderCommandExecute);
        // フォルダ削除コマンド
        public static SimpleDelegateCommand DeleteFolderCommand => new (ClipboardFolderCommands.DeleteFolderCommandExecute);

        // FolderSelectWindowでFolderSelectWindowSelectFolderCommandが実行されたときの処理
        public static SimpleDelegateCommand FolderSelectWindowSelectFolderCommand => new (FolderSelectWindowViewModel.FolderSelectWindowSelectFolderCommandExecute);

        // フォルダ内のアイテムをJSON形式でエクスポートする処理
        public static SimpleDelegateCommand ExportItemsFromFolderCommand => new (ClipboardFolderCommands.ExportItemsFromFolderCommandExecute);
        // フォルダ内のアイテムをJSON形式でインポートする処理
        public static SimpleDelegateCommand ImportItemsToFolderCommand => new (ClipboardFolderCommands.ImportItemsToFolderCommandExecute);

    }
}
