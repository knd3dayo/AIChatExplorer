using ClipboardApp.Factory;
using ClipboardApp.Model.Item;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Search;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folder {
    public partial class SearchFolder : ClipboardFolder {

        public override void Save() {
            Save<SearchFolder, ClipboardItem>();
        }
        // 削除
        public override void Delete() {
            DeleteFolder<SearchFolder, ClipboardItem>(this);
        }
        // 親フォルダ
        public override SearchFolder? GetParent() {
            return GetParent<SearchFolder>();
        }


        //--------------------------------------------------------------------------------
        // コンストラクタ
        public SearchFolder() {
            IsAutoProcessEnabled = false;
            FolderType = FolderTypeEnum.Search;
        }

        protected SearchFolder(SearchFolder? parent, string folderName) : base(parent, folderName) {

            IsAutoProcessEnabled = false;
            FolderType = FolderTypeEnum.Search;
            ParentId = parent?.Id ?? ObjectId.Empty;
            FolderName = folderName;

        }

        // フォルダの絶対パス
        public override string FolderPath {
            get {
                SearchFolder? parent = (SearchFolder?)ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<SearchFolder>().FindById(ParentId);
                if (parent == null) {
                    return FolderName;
                }
                return $"{parent.FolderPath}/{FolderName}";
            }
        }

        // アイテム LiteDBには保存しない。
        [BsonIgnore]
        public override List<T> GetItems<T>() {
            List<ContentItem> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // フォルダに検索条件が設定されている場合
            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(this);
            if (searchConditionRule != null && searchConditionRule.TargetFolder != null) {
                // 検索対象フォルダのアイテムを検索する。
                _items = [.. searchConditionRule.TargetFolder.SearchItems(searchConditionRule.SearchCondition).OrderByDescending(x => x.UpdatedAt)];

            }
            // 検索対象フォルダパスがない場合は何もしない。
            return _items.Cast<T>().ToList();
        }

        // 子フォルダ
        public override List<T> GetChildren<T>() {
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<SearchFolder>();
            IEnumerable<SearchFolder> folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);

            return folders.Cast<T>().ToList();

        }


        // 子フォルダ BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public virtual void DeleteChild(SearchFolder child) {
            DeleteFolder<SearchFolder, ClipboardItem>(child);
        }

        public override SearchFolder CreateChild(string folderName) {
            SearchFolder child = new(this, folderName);
            return child;
        }

        // アイテムを追加する処理
        public override void AddItem(ContentItem item) {
            // 何もしない
        }

        // ClipboardItemを削除
        public virtual void DeleteItem(ContentItem item) {
            // 何もしない
        }

        #region システムのクリップボードへ貼り付けられたアイテムに関連する処理
        public override void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ContentItem> _afterClipboardChanged) {
            // 何もしない
        }
        #endregion
    }
}

