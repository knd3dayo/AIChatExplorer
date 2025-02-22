using ClipboardApp.Factory;
using ClipboardApp.Model.Item;
using LibUIPythonAI.Resource;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Search;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folders.Search {
    public partial class SearchFolder : ContentFolderWrapper {

        // コンストラクタ
        public SearchFolder(ContentFolder folder) : base(folder) {
            IsAutoProcessEnabled = true;
            FolderType = FolderTypeEnum.Search;
        }

        protected SearchFolder(SearchFolder? parent, string folderName) : base(parent, folderName) {

            FolderType = FolderTypeEnum.Search;
            ParentId = parent?.Id ?? ObjectId.Empty;
            FolderName = folderName;
            IsAutoProcessEnabled = false;

        }
        // 親フォルダ
        public override SearchFolder? GetParent() {
            var parentFolder = ContentFolderInstance.GetParent();
            if (parentFolder == null) {
                return null;
            }
            return new SearchFolder(parentFolder);
        }

        // アイテム LiteDBには保存しない。
        [BsonIgnore]
        public override List<ContentItemWrapper> GetItems() {
            List<ContentItemWrapper> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            ClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
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
            var children = ContentFolderInstance.GetChildren<ContentFolder>();
            List<ContentFolderWrapper> result = [];
            foreach (var child in children) {
                result.Add(new SearchFolder(child));
            }
            return result;

        }

        public override SearchFolder CreateChild(string folderName) {
            SearchFolder child = new(this, folderName);
            return child;
        }

        // アイテムを追加する処理
        public override void AddItem(ContentItemWrapper item, bool applyGlobalAutoAction = false, Action<ContentItemWrapper>? afterUpdate = null) {
            // 何もしない
        }

        // ClipboardItemを削除
        public virtual void DeleteItem(ContentItem item) {
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

