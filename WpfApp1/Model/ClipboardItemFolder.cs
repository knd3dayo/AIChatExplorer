using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Nodes;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemView;

namespace ClipboardApp.Model {
    public class ClipboardItemFolder : ObservableObject {

        // アプリ共通の検索条件
        public static SearchConditionRule GlobalSearchCondition = new SearchConditionRule();

        //--------------------------------------------------------------------------------
        public static ClipboardItemFolder RootFolder {
            get {
                var folder = ClipboardDatabaseController.GetClipboardItemFolder(ClipboardDatabaseController.CLIPBOARD_ROOT_FOLDER_NAME);
                if (folder == null) {
                    folder = new(ClipboardDatabaseController.CLIPBOARD_ROOT_FOLDER_NAME, "クリップボード");
                    ClipboardDatabaseController.UpsertFolder(folder);
                }
                return folder;
            }
        }
        public static ClipboardItemFolder SearchRootFolder {
            get {
                var folder = ClipboardDatabaseController.GetClipboardItemFolder(ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME);
                if (folder == null) {
                    folder = new(ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME, "検索フォルダ");
                    ClipboardDatabaseController.UpsertFolder(folder);
                }
                return folder;
            }
        }

        // プロパティ
        // LiteDBのID
        public ObjectId? Id { get; set; }
        // フォルダの表示名
        public string DisplayName { get; set; } = "";
        // フォルダの絶対パス
        public string AbsoluteCollectionName { get; set; } = "";

        // 検索フォルダかどうか
        public bool IsSearchFolder { get; set; } = false;

        // 現在、検索条件を適用中かどうか
        public bool IsApplyingSearchCondition { get; set; } = false;

        // 子フォルダ BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public ObservableCollection<ClipboardItemFolder> Children {
            get {
                ObservableCollection<ClipboardItemFolder> children = new ObservableCollection<ClipboardItemFolder>();
                // AbsoluteCollectionNameが空の場合は空のリストを返す
                if (string.IsNullOrEmpty(AbsoluteCollectionName)) {
                    return children;
                }
                // LiteDBから自分が親となっているフォルダを取得
                var childrenNames = ClipboardDatabaseController.GetClipboardItemFolderChildRelations(AbsoluteCollectionName);
                foreach (var childName in childrenNames) {
                    var child = ClipboardDatabaseController.GetClipboardItemFolder(childName);
                    if (child != null) {
                        children.Add(child);
                    }
                }
                return children;
            }
        }

        // アイテム BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public ObservableCollection<ClipboardItem> Items {
            get {
                // AbsoluteCollectionNameが空の場合は空のリストを返す
                if (string.IsNullOrEmpty(AbsoluteCollectionName)) {
                    return new ObservableCollection<ClipboardItem>();
                }
                IEnumerable<ClipboardItem> items = new List<ClipboardItem>();
                // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
                // 検索フォルダの場合は、SearchConditionを適用して取得
                if (IsSearchFolder) {
                    // 検索フォルダの場合
                    SearchConditionRule? searchConditionRule = ClipboardDatabaseController.GetSearchConditionRuleByCollectionName(AbsoluteCollectionName);
                    if (searchConditionRule != null) {
                        items = ClipboardDatabaseController.SearchClipboardItems(searchConditionRule);
                    }
                } else {
                    // 通常のフォルダの場合
                    items = ClipboardDatabaseController.GetClipboardItems(AbsoluteCollectionName, GlobalSearchCondition.SearchCondition);
                }
                
                ObservableCollection<ClipboardItem> result = [.. items];
                return result;
            }
        }
        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ClipboardItemFolder() {
        }

        public ClipboardItemFolder(ClipboardItemFolder? parent, string collectionName, string displayName) {
            if (parent == null) {
                AbsoluteCollectionName = collectionName;
            } else {
                AbsoluteCollectionName = ClipboardDatabaseController.ConcatenatePath(parent.AbsoluteCollectionName, collectionName);
            }
            DisplayName = displayName;

        }
        public ClipboardItemFolder(string collectionName, string displayName) : this(null, collectionName, displayName) {

        }
        //--------------------------------------------------------------------------------

        // ClipboardItemを作成
        public ClipboardItem CreateItem() {
            ClipboardItem item = new ClipboardItem();
            item.CollectionName = AbsoluteCollectionName;
            // Itemsに追加
            Items.Add(item);
            // 追加したアイテムをSelectedItemに設定
            if (MainWindowViewModel.Instance != null) {
                MainWindowViewModel.Instance.SelectedItem = new ClipboardItemViewModel(item);
            }
            ClipboardDatabaseController.UpsertFolder(this);
            return item;
        }
        // ClipboardItemを追加
        public void UpsertItem(ClipboardItem item) {
            Items.Add(item);
            // 追加したアイテムをSelectedItemに設定
            if (MainWindowViewModel.Instance != null) {
                MainWindowViewModel.Instance.SelectedItem = new ClipboardItemViewModel(item);
            }

            // LiteDBに保存
            ClipboardDatabaseController.UpsertItem(item);
        }
        // ClipboardItemを削除
        public void DeleteItem(ClipboardItem item) {
            // LiteDBに保存
            ClipboardDatabaseController.DeleteItem(item);
        }


        public bool IsSelected { get; set; } = false;

        // FolderSelectWindowで選択されたフォルダを適用する処理
        public bool IsSelectedOnFolderSelectWindow { get; set; } = false;

        // 自動処理を適用する処理
        public ClipboardItem? ApplyAutoProcess(ClipboardItem clipboardItem) {
            ClipboardItem? result = clipboardItem;
            // AutoProcessRulesを取得
            var AutoProcessRules = ClipboardDatabaseController.GetAutoProcessRules(this);
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

        // アイテムを追加する処理
        public ClipboardItem AddItem(ClipboardItem item) {
            // AbsoluteCollectionNameを設定
            item.CollectionName = AbsoluteCollectionName;

            // 自動処理を適用
            ClipboardItem? result = ApplyAutoProcess(item);

            if (result == null) {
                // 自動処理で削除または移動された場合は何もしない
                Tools.Info("自動処理でアイテムが削除または移動されました");
                return item;
            }

            // LiteDBに保存
            ClipboardDatabaseController.UpsertItem(result);
            // Itemsに追加
            Items.Add(result);

            return item;

        }
        // フォルダ内のアイテムをJSON形式でExport
        public void ExportItemsToJson(string directoryPath) {
            JsonArray jsonArray = new JsonArray();
            foreach (ClipboardItem item in Items) {
                jsonArray.Add(ClipboardItem.ToJson(item));
            }
            string jsonString = jsonArray.ToString();
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + this.AbsoluteCollectionName + ".json";

            File.WriteAllText(Path.Combine(directoryPath, fileName), jsonString);

        }

        //exportしたJSONファイルをインポート
        public void ImportItemsFromJson(string json) {
            JsonNode? node = JsonNode.Parse(json);
            if (node == null) {
                throw new ThisApplicationException("JSON文字列をパースできませんでした");
            }
            JsonArray? jsonArray = node as JsonArray;
            if (jsonArray == null) {
                throw new ThisApplicationException("JSON文字列をパースできませんでした");
            }

            // Itemsをクリア
            Items.Clear();

            foreach (JsonValue? jsonValue in jsonArray) {
                if (jsonValue == null) {
                    continue;
                }
                string jsonString = jsonValue.ToString();
                ClipboardItem? item = ClipboardItem.FromJson(jsonString);
                if (item == null) {
                    continue;
                }
                // Itemsに追加
                Items.Add(item);
                // LiteDBに保存
                ClipboardDatabaseController.UpsertItem(item);
            }

        }
    }
}
