using ClipboardApp.Model.Folders.Browser;
using ClipboardApp.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Search;
using LibUIPythonAI.Resource;
using PythonAILib.Model.Search;

namespace ClipboardApp.Model.Folders.Search {
    public partial class SearchFolder : ContentFolderWrapper {

        // コンストラクタ
        public SearchFolder(ContentFolderEntity folder) : base(folder) {
            IsAutoProcessEnabled = true;
            FolderTypeString = FolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
        }

        protected SearchFolder(SearchFolder? parent, string folderName) : base(parent, folderName) {

            FolderTypeString = FolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
            Parent = parent;
            FolderName = folderName;
            IsAutoProcessEnabled = false;

        }
        // 親フォルダ
        public override SearchFolder? GetParent() {
            using PythonAILibDBContext db = new();
            var parentFolder = db.ContentFolders.FirstOrDefault(x => x.Id == Entity.ParentId);
            if (parentFolder == null) {
                return null;
            }
            return new SearchFolder(parentFolder);
        }

        // アイテム LiteDBには保存しない。
        public override List<ContentItemWrapper> GetItems() {
            List<ContentItemWrapper> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            // フォルダに検索条件が設定されている場合
            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(this);
            if (searchConditionRule != null && searchConditionRule.TargetFolder != null) {
                // 検索対象フォルダのアイテムを検索する。
                _items = [.. searchConditionRule.TargetFolder.SearchItems(searchConditionRule.SearchCondition).OrderByDescending(x => x.UpdatedAt)];

            }
            return _items;
        }

        // 子フォルダ
        public override List<ContentFolderWrapper> GetChildren() {
            using PythonAILibDBContext db = new();
            var children = db.ContentFolders.Where(x => x.ParentId == Entity.Id).Select(x => new ContentFolderWrapper(x)).ToList();
            return children;

        }

        public override SearchFolder CreateChild(string folderName) {
            ContentFolderEntity childFolder = new() {
                ParentId = Entity.Id,
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

        public override string GetStatusText() {
            string message = $"{CommonStringResources.Instance.Folder}[{FolderName}]";
            // folderが検索フォルダの場合
            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(this);
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

