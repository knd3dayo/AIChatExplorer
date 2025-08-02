using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folders;
using LibPythonAI.Model.Search;
using LibPythonAI.Resources;

namespace AIChatExplorer.Model.Folders.Search {
    public partial class SearchFolder : ContentFolderWrapper {

        // コンストラクタ
        public SearchFolder() : base() {
            IsAutoProcessEnabled = true;
            FolderTypeString = FolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
        }

        protected SearchFolder(SearchFolder? parent, string folderName) : base(parent, folderName) {

            FolderTypeString = FolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
            Parent = parent;
            FolderName = folderName;
            IsAutoProcessEnabled = false;

        }

        public async Task<SearchRule?> GetSearchRule() {
            return await SearchRule.GetItemBySearchFolder(this);
        }


        // アイテム LiteDBには保存しない。
        public async Task<List<ContentItemWrapper>> GetItems(bool isSync = true) {
            List<ContentItemWrapper> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            // フォルダに検索条件が設定されている場合
            SearchRule? searchConditionRule = await SearchRule.GetItemBySearchFolder(this);
            if (searchConditionRule != null) {
                _items = await searchConditionRule.SearchItems();

            }
            return _items;
        }

        // 子フォルダ


        public override SearchFolder CreateChild(string folderName) {
            ContentFolderEntity childFolder = new() {
                ParentId = Id,
                FolderName = folderName,
                FolderTypeString = FolderTypeString
            };
            SearchFolder child = new() { Entity = childFolder };
            return child;
        }

        // アイテムを追加する処理
        public override async Task AddItemAsync(ContentItemWrapper item, bool applyGlobalAutoAction = false, Action<ContentItemWrapper>? afterUpdate = null) {
            // 何もしない
            await Task.CompletedTask;
        }

        // ApplicationItemを削除
        public virtual void DeleteItem(ContentItemWrapper item) {
            // 何もしない
        }

        public override async Task Delete() {
            // SearchRuleを削除
            SearchRule? searchConditionRule = await SearchRule.GetItemBySearchFolder(this);
            searchConditionRule?.Delete();
            await base.Delete();
        }


        public override async Task<string> GetStatusText() {
            string message = $"{PythonAILibStringResources.Instance.Folder}[{FolderName}]";
            // folderが検索フォルダの場合
            SearchRule? searchConditionRule = await SearchRule.GetItemBySearchFolder(this);
            SearchCondition? searchCondition = searchConditionRule?.SearchCondition;
            // SearchConditionがNullでなく、 Emptyでもない場合
            if (searchCondition != null && !searchCondition.IsEmpty()) {
                message += $" {PythonAILibStringResources.Instance.SearchCondition}[";
                message += searchCondition.ToStringSearchCondition();
                message += "]";
            }
            return message;
        }
    }
}

