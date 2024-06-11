using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Nodes;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using WpfAppCommon.Factory;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public class ClipboardFolder  {

        public class RootFolderInfo {

            public string FolderName { get; set; } = "";
            public LiteDB.ObjectId Id { get; set; } = ObjectId.Empty;

            public LiteDB.ObjectId FolderId { get; set; } = ObjectId.Empty;

        }

        public static readonly string CLIPBOARD_ROOT_FOLDER_NAME = "クリップボード";
        public static readonly string SEARCH_ROOT_FOLDER_NAME = "検索フォルダ";


        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ClipboardFolder() {
        }

        private ClipboardFolder(ClipboardFolder? parent, string folderName) {

            ParentId = parent?.Id ?? ObjectId.Empty;
            FolderName = folderName;

            // クリップボードアイテムのロード
            Load();

        }

        // プロパティ
        // LiteDBのID
        public ObjectId Id { get; set; } = ObjectId.Empty;

        // 親フォルダのID
        public ObjectId ParentId { get; set; } = ObjectId.Empty;

        // フォルダの絶対パス ファイルシステム用
        public string FolderPath {
            get {
                ClipboardFolder? parent = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(ParentId);
                if (parent == null) {
                    return FolderName;
                }
                return ConcatenateFileSystemPath(parent.FolderPath, FolderName);
            }
        }

        //　フォルダ名
        public string FolderName { get; set; } = "";


        // Description
        public string Description { get; set; } = "";

        // AutoProcessRuleのIdのリスト
        public List<ObjectId> AutoProcessRuleIds { get; set; } = [];

        // AutoProcessRuleのリスト
        public ObservableCollection<AutoProcessRule> AutoProcessRules {
            get {
                ObservableCollection<AutoProcessRule> autoProcessRules = [.. AutoProcessRuleController.GetAutoProcessRules(this)];
                return autoProcessRules;
            }
        }
        // AddAutoProcessRule
        public void AddAutoProcessRule(AutoProcessRule rule) {
            AutoProcessRuleIds.Add(rule.Id);
            Save();
        }

        // 検索フォルダかどうか
        public bool IsSearchFolder { get; set; } = false;

        // 現在、検索条件を適用中かどうか
        public bool IsApplyingSearchCondition { get; set; } = false;

        // 子フォルダ BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public void DeleteChild(ClipboardFolder child) {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.DeleteFolder(child);
        }
        public ClipboardFolder CreateChild(string folderName) {
            ClipboardFolder child = new(this, folderName);
            return child;
        }

        public List<ClipboardFolder> Children {
            get {
                // DBからParentIDが自分のIDのものを取得
                return ClipboardAppFactory.Instance.GetClipboardDBController().GetFoldersByParentId(Id);
            }
        }

        private List<ClipboardItem> _items = [];
        // アイテム BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public List<ClipboardItem> Items {
            get {
                return _items;
            }
        }
        //------------
        // 親フォルダのパスと子フォルダ名を連結する。LiteDB用
        private string ConcatenateCollectionPath(string parentPath, string childPath) {
            if (string.IsNullOrEmpty(parentPath))
                return childPath;
            if (string.IsNullOrEmpty(childPath))
                return parentPath;
            return parentPath + "_" + childPath;
        }
        // 親フォルダのパスと子フォルダ名を連結する。ファイルシステム用
        private string ConcatenateFileSystemPath(string parentPath, string childPath) {
            if (string.IsNullOrEmpty(parentPath))
                return childPath;
            if (string.IsNullOrEmpty(childPath))
                return parentPath;
            return Path.Combine(parentPath, childPath);
        }
        //--------------------------------------------------------------------------------
        // 自分自身を保存
        public void Save() {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.UpsertFolder(this);
        }
        // アイテムを追加する処理
        public ClipboardItem AddItem(ClipboardItem item) {
            // CollectionNameを設定
            item.FolderObjectId = this.Id;

            // 自動処理を適用
            ClipboardItem? result = ApplyAutoProcess(item);

            if (result == null) {
                // 自動処理で削除または移動された場合は何もしない
                Tools.Info("自動処理でアイテムが削除または移動されました");
                return item;
            }
            // 保存
            result.Save();
            // Itemsに追加
            Items.Add(result);
            // 通知
            Tools.Info("アイテムを追加しました");
            return item;
        }
        // ClipboardItemを削除
        public void DeleteItem(ClipboardItem item) {
            // LiteDBに保存
            item.Delete();
        }
        // Delete
        public void Delete() {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.DeleteFolder(this);
        }
        public void Load() {

            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // フォルダに検索条件が設定されている場合
            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(this);
            if (searchConditionRule != null) {
                _items = [.. ClipboardDatabaseController.SearchItems(this, searchConditionRule.SearchCondition)];

                // 検索対象フォルダパスがない場合は何もしない。
            } else {
                // 通常のフォルダの場合で、GlobalSearchConditionが設定されている場合
                if (GlobalSearchCondition.SearchCondition != null && GlobalSearchCondition.SearchCondition.IsEmpty() == false) {
                    _items = [.. ClipboardDatabaseController.SearchItems(this, GlobalSearchCondition.SearchCondition)];

                } else {
                    // 通常のフォルダの場合で、GlobalSearchConditionが設定されていない場合
                    _items = [.. ClipboardDatabaseController.GetItems(this)];
                }
            }
        }

        // 自動処理を適用する処理
        public ClipboardItem? ApplyAutoProcess(ClipboardItem clipboardItem) {
            ClipboardItem? result = clipboardItem;
            // AutoProcessRulesを取得
            var AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(this);
            foreach (var rule in AutoProcessRules) {
                Tools.Info("自動処理を適用します " + rule.GetDescriptionString());
                result = rule.RunAction(result);
                // resultがNullの場合は処理を中断
                if (result == null) {
                    Tools.Info("自動処理でアイテムが削除されました");
                    return null;
                }
            }
            return result;
        }   

        // フォルダ内のアイテムをJSON形式でExport
        public void ExportItemsToJson(string directoryPath) {
            JsonArray jsonArray = [];
            foreach (ClipboardItem item in Items) {
                jsonArray.Add(ClipboardItem.ToJson(item));
            }
            string jsonString = jsonArray.ToString();
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + this.Id.ToString() + ".json";

            File.WriteAllText(Path.Combine(directoryPath, fileName), jsonString);

        }

        //exportしたJSONファイルをインポート
        public void ImportItemsFromJson(string json, Action<ActionMessage> action) {
            JsonNode? node = JsonNode.Parse(json);
            if (node == null) {
                action(ActionMessage.Error("JSON文字列をパースできませんでした"));
                return;
            }
            JsonArray? jsonArray = node as JsonArray;
            if (jsonArray == null) {
                action(ActionMessage.Error("JSON文字列をパースできませんでした"));
                return;
            }

            // Itemsをクリア
            Items.Clear();

            foreach (JsonValue? jsonValue in jsonArray.Cast<JsonValue?>()) {
                if (jsonValue == null) {
                    continue;
                }
                string jsonString = jsonValue.ToString();
                ClipboardItem? item = ClipboardItem.FromJson(jsonString, action);
                if (item == null) {
                    continue;
                }
                // Itemsに追加
                Items.Add(item);
                //保存
                item.Save();
            }

        }
        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public void MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItem newItem) {
            // SourceApplicationNameが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationName)) {
                return;
            }
            // NewItemのSourceApplicationTitleが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationTitle)) {
                return;
            }
            if (Items.Count == 0) {
                return;
            }
            List<ClipboardItem> sameTitleItems = [];
            // マージ先のアイテムのうち、SourceApplicationTitleとSourceApplicationNameが一致するアイテムを取得
            foreach (var item in Items) {
                if (newItem.SourceApplicationTitle == item.SourceApplicationTitle
                    && newItem.SourceApplicationName == item.SourceApplicationName ) {
                    // TypeがTextのアイテムのみマージ
                    if (item.ContentType == ClipboardContentTypes.Text) {
                        sameTitleItems.Add(item);
                    }
                }
            }
            // mergeFromItemsが空の場合は、newItemをそのまま返す。
            if (sameTitleItems.Count == 0) {
                return;
            }
            // マージ元のアイテムをマージ先(更新時間が一番古いもの)のアイテムにマージ
            ClipboardItem mergeToItem = Items.Last();
            // sameTitleItemsの1から最後までをマージ元のアイテムとする
            sameTitleItems.RemoveAt(sameTitleItems.Count - 1);

            // sameTitleItemsに、newItemを追加
            sameTitleItems.Insert(0, newItem);
            // マージ元のアイテムをマージ先のアイテムにマージ

            mergeToItem.MergeItems(sameTitleItems, false, Tools.DefaultAction);
            // newItemにマージしたアイテムをコピー
            mergeToItem.CopyTo(newItem);
            // マージしたアイテムを削除
            foreach (var mergedItem in sameTitleItems) {
                DeleteItem(mergedItem);
            }
            // mergedItemを削除
            DeleteItem(mergeToItem);
        }
        // 指定されたフォルダの全アイテムをマージするコマンド
        public void MergeItemsCommandExecute(ClipboardItem item) {
            if (Items.Count == 0) {
                return;
            }

            // マージ元のアイテム
            List<ClipboardItem> mergedFromItems = [];
            for (int i = Items.Count - 1; i > 0; i--) {
                // TypeがTextのアイテムのみマージ
                if (Items[i].ContentType == ClipboardContentTypes.Text) {
                    mergedFromItems.Add(Items[i]);
                }
            }
            // 先頭に引数のアイテムを追加
            mergedFromItems.Insert(0, item);
            // mergeToItemを取得(更新時間が一番古いアイテム)
            ClipboardItem mergeToItem = mergedFromItems.Last();
            // mergedFromItemsから、mergeToItemを削除
            mergedFromItems.RemoveAt(mergedFromItems.Count - 1);

            // マージ元のアイテムをマージ先のアイテムにマージ
            mergeToItem.MergeItems(mergedFromItems, false, Tools.DefaultAction);

            // マージ先アイテムを、newItemにコピー
            mergeToItem.CopyTo(item);

            // マージしたアイテムを削除
            foreach (var mergedItem in mergedFromItems) {
                DeleteItem(mergedItem);
            }
            // マージ先アイテムを削除
            DeleteItem(mergeToItem);

        }


        // アプリ共通の検索条件
        public static SearchRule GlobalSearchCondition { get; set; } = new();

        //--------------------------------------------------------------------------------
        public static ClipboardFolder RootFolder {
            get {
                ClipboardFolder? rootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolder(CLIPBOARD_ROOT_FOLDER_NAME);
                if (rootFolder == null) {
                    rootFolder = new();
                    rootFolder.FolderName = CLIPBOARD_ROOT_FOLDER_NAME;
                    rootFolder.Save();
                }
                return rootFolder;
            }
        }
        public static ClipboardFolder SearchRootFolder {
            get {
                ClipboardFolder? searchRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolder(SEARCH_ROOT_FOLDER_NAME);
                if (searchRootFolder == null) {
                    searchRootFolder = new();
                    searchRootFolder.FolderName = SEARCH_ROOT_FOLDER_NAME;
                    searchRootFolder.IsSearchFolder = true;
                    searchRootFolder.Save();
                }
                return searchRootFolder;
            }
        }

        // ObjectIdからフォルダを取得
        public static ClipboardFolder? GetFolderById(ObjectId id) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(id);

        }
    }
}
