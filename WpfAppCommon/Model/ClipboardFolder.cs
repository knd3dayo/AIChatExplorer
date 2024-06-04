using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Nodes;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using WpfAppCommon.Factory;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public class ClipboardFolder : ObservableObject {

        // アプリ共通の検索条件
        public static SearchRule GlobalSearchCondition { get; set; } = new();

        //--------------------------------------------------------------------------------
        public static ClipboardFolder RootFolder {
            get {
                IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
                return ClipboardDatabaseController.GetRootFolder();
            }
        }
        public static ClipboardFolder SearchRootFolder {
            get {
                IClipboardDBController clipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
                return clipboardDatabaseController.GetSearchRootFolder();
            }
        }

        // プロパティ
        // LiteDBのID
        public ObjectId Id { get; set; } = ObjectId.Empty;
        // フォルダの表示名
        public string DisplayName { get; set; } = "";
        // フォルダの絶対パス LiteDB用
        public string CollectionName { get; set; } = "";
        // フォルダの絶対パス ファイルシステム用
        public string FolderPath { get; set; } = "";

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
        public ObservableCollection<ClipboardFolder> Children {
            get {
                ObservableCollection<ClipboardFolder> children = [];
                // CollectionNameが空の場合は空のリストを返す
                if (string.IsNullOrEmpty(CollectionName)) {
                    return children;
                }
                // LiteDBから自分が親となっているフォルダを取得
                IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
                var childrenNames = ClipboardDatabaseController.GetFolderRelations(CollectionName);
                foreach (var childName in childrenNames) {
                    var child = ClipboardDatabaseController.GetFolder(childName);
                    if (child != null) {
                        children.Add(child);
                    }
                }
                return children;
            }
        }
        public void DeleteChild(ClipboardFolder child) {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.DeleteFolder(child);
        }
        public ClipboardFolder CreateChild(string collectionName, string displayName) {
            ClipboardFolder child = new(this, collectionName, displayName);
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.UpsertFolderRelation(this, child);

            return child;
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
        // コンストラクタ
        public ClipboardFolder() {
        }

        public ClipboardFolder(ClipboardFolder? parent, string collectionName, string displayName) {
            if (parent == null) {
                // LiteDB用のパス
                CollectionName = collectionName;
                // ファイルシステム用のパス
                FolderPath = collectionName;

            } else {
                CollectionName = ConcatenateCollectionPath(parent.CollectionName, collectionName);
                FolderPath = ConcatenateFileSystemPath(parent.FolderPath, collectionName);
            }
            DisplayName = displayName;
            // クリップボードアイテムのロード
            Load();

        }
        public ClipboardFolder(string collectionName, string displayName) : this(null, collectionName, displayName) {

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
            item.CollectionName = CollectionName;
            // AbsoluteFolderPathを設定
            item.FolderPath = FolderPath;

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
            // CollectionNameが空の場合は空のリストを返す
            if (string.IsNullOrEmpty(CollectionName)) {
                return;
            }
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // フォルダに検索条件が設定されている場合
            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolderName(CollectionName);
            if (searchConditionRule != null) {
                // 検索対象フォルダパスの取得
                string? targetCollectionName = searchConditionRule.TargetFolder?.CollectionName;
                if (targetCollectionName != null) {
                    _items = [.. ClipboardDatabaseController.SearchItems(targetCollectionName, searchConditionRule.SearchCondition)];
                }
                // 検索対象フォルダパスがない場合は何もしない。
            } else {
                // 通常のフォルダの場合で、GlobalSearchConditionが設定されている場合
                if (GlobalSearchCondition.SearchCondition != null && GlobalSearchCondition.SearchCondition.IsEmpty() == false) {
                    _items = [.. ClipboardDatabaseController.SearchItems(CollectionName, GlobalSearchCondition.SearchCondition)];

                } else {
                    // 通常のフォルダの場合で、GlobalSearchConditionが設定されていない場合
                    _items = [.. ClipboardDatabaseController.GetItems(CollectionName)];
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
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + this.CollectionName + ".json";

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


    }
}
