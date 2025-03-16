using AIChatExplorer.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Search;
using LibUIPythonAI.Resource;

namespace AIChatExplorer.Model.Folders.Search {
    public partial class SearchFolder : ContentFolderWrapper {

        // コンストラクタ
        public SearchFolder(ContentFolderEntity folder) : base(folder) {
            IsAutoProcessEnabled = true;
            FolderTypeString = AIChatExplorerFolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
        }

        protected SearchFolder(SearchFolder? parent, string folderName) : base(parent, folderName) {

            FolderTypeString = AIChatExplorerFolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
            Parent = parent;
            FolderName = folderName;
            IsAutoProcessEnabled = false;

        }

        public SearchRule? SearchRule {
            get {
                return SearchRule.GetItemBySearchFolder(this);
            }
        }


        // アイテム LiteDBには保存しない。
        public override List<T> GetItems<T>() {
            List<T> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            // フォルダに検索条件が設定されている場合
            SearchRule? searchConditionRule = SearchRule.GetItemBySearchFolder(this);
            if (searchConditionRule != null && searchConditionRule.TargetFolder != null) {
                _items = searchConditionRule.SearchItems().Select(x => (T)x).ToList();

            }
            return _items;
        }

        // 子フォルダ


        public override SearchFolder CreateChild(string folderName) {
            ContentFolderEntity childFolder = new() {
                ParentId = Id,
                FolderName = folderName,
            };
            SearchFolder child = new(childFolder);
            return child;
        }

        // アイテムを追加する処理
        public override void AddItem(ContentItemWrapper item, bool applyGlobalAutoAction = false, Action<ContentItemWrapper>? afterUpdate = null) {
            // 何もしない
        }

        // ClipboardItemを削除
        public virtual void DeleteItem(ContentItemWrapper item) {
            // 何もしない
        }

        public override void Delete() {
            SearchRule? searchConditionRule = SearchRule.GetItemBySearchFolder(this);
            if (searchConditionRule != null) {
                searchConditionRule.Delete();
            }

            base.Delete();
        }


        public override string GetStatusText() {
            string message = $"{CommonStringResources.Instance.Folder}[{FolderName}]";
            // folderが検索フォルダの場合
            SearchRule? searchConditionRule = SearchRule.GetItemBySearchFolder(this);
            SearchCondition? searchCondition = searchConditionRule?.SearchCondition;
            // SearchConditionがNullでなく、 Emptyでもない場合
            if (searchCondition != null && !searchCondition.IsEmpty()) {
                message += $" {CommonStringResources.Instance.SearchCondition}[";
                message += searchCondition.ToStringSearchCondition();
                message += "]";
            }
            return message;
        }
    }
}

