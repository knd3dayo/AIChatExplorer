using WpfAppCommon.Factory;

namespace WpfAppCommon.Model {
    public class SearchFolder : ClipboardFolder {

        //--------------------------------------------------------------------------------
        // コンストラクタ
        public SearchFolder() : base() { }

        protected SearchFolder(ClipboardFolder? parent, string folderName) : base(parent, folderName) { }

        // アイテムを追加する処理
        public override ClipboardItem AddItem(ClipboardItem item) {
            // 何もしない
            return item;
        }
        // ClipboardItemを削除
        public override void DeleteItem(ClipboardItem item) {
            // 何もしない
        }

        public override void Load() {

            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // フォルダに検索条件が設定されている場合
            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(this);
            if (searchConditionRule != null) {
                _items = [.. ClipboardDatabaseController.SearchItems(this, searchConditionRule.SearchCondition)];

            }
            // 検索対象フォルダパスがない場合は何もしない。
        }
        public override List<ClipboardFolder> Children {
            get {
                // DBからParentIDが自分のIDのものを取得
                return [.. ClipboardAppFactory.Instance.GetClipboardDBController().GetSearchFoldersByParentId(Id)];
            }
        }


        // 自動処理を適用する処理
        public override ClipboardItem? ApplyAutoProcess(ClipboardItem clipboardItem) {
            // 何もしない
            return clipboardItem;
        }

        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public override void MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItem newItem) {
            // 何もしない
        }
        // 指定されたフォルダの全アイテムをマージするコマンド
        public override void MergeItemsCommandExecute(ClipboardItem item) {
            // 何もしない
        }


    }
}

