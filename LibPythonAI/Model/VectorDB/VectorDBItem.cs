using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Resources;

namespace LibPythonAI.Model.VectorDB {
    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class VectorDBItem {


        // システム共通のベクトルDBの名前
        public readonly static string SystemCommonVectorDBName = "default";
        // デフォルトのコレクション名 
        public readonly static string DefaultCollectionName = "ai_app_default_collection";
        // フォルダーカタログのコレクション名 
        public readonly static string FolderCatalogCollectionName = "ai_app_folder_catalog_collection";


        public string Id { get; set; } = Guid.NewGuid().ToString();

        // システム共通のベクトルDB
        public static VectorDBItem GetDefaultVectorDB() {
            var item = GetItemByName(SystemCommonVectorDBName);
            if (item == null) {
                throw new Exception(PythonAILibStringResources.Instance.VectorDBNotFound(SystemCommonVectorDBName));
            }
            return item!;
        }

        private static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };


        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // ベクトルDBのURL
        public string VectorDBURL { get; set; } = "";

        // マルチベクトルリトリーバを使うかどうか
        public bool IsUseMultiVectorRetriever { get; set; } = false;

        // ドキュメントストアのURL マルチベクトルリトリーバを使う場合に指定する
        public string DocStoreURL { get; set; } = "";

        // ベクトルDBの種類を表す列挙型
        [JsonIgnore]
        public VectorDBTypeEnum Type { get; set; } = VectorDBTypeEnum.Chroma;

        // ベクトルDBの種類を表す文字列
        public string VectorDBTypeString { get => Type.ToString(); }

        // コレクション名
        public string CollectionName { get; set; } = DefaultCollectionName;

        // チャンクサイズ ベクトル生成時にドキュメントをこのサイズで分割してベクトルを生成する
        public int ChunkSize { get; set; } = 1024;

        // ベクトル検索時の検索結果上限
        public int DefaultSearchResultLimit { get; set; } = 10;

        // 有効かどうか
        [JsonIgnore]
        public bool IsEnabled { get; set; } = false;

        // システム用のフラグ
        [JsonIgnore]
        public bool IsSystem { get; set; } = false;

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "Id", Id },
                { "VectorDBType", (int)Type },
                { "IsEnabled", IsEnabled },
                { "IsSystem", IsSystem },
                { "Name", Name },
                { "Description", Description },
                { "SystemMessage", PromptStringResource.Instance.VectorDBSystemMessage(Description) },
                { "VectorDBURL", VectorDBURL },
                { "IsUseMultiVectorRetriever", IsUseMultiVectorRetriever },
                { "DocStoreURL", DocStoreURL },
                { "VectorDBTypeString", VectorDBTypeString },
                { "CollectionName", CollectionName ?? ""},
                { "ChunkSize", ChunkSize },
            };
            return dict;
        }


        // Save
        public async Task SaveAsync() {
            await Task.Run(() => PythonAILib.PythonIF.PythonExecutor.PythonAIFunctions.UpdateVectorDBItem(this));
        }

        // DeleteAsync
        public async Task DeleteAsync() {
            await Task.Run(() => PythonAILib.PythonIF.PythonExecutor.PythonAIFunctions.DeleteVectorDBItem(this));
        }


        // CreateEntriesDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorDBItem> items) {
            return items.Select(item => item.ToDict()).ToList();
        }

        // Json文字列化する
        public static string ToJson(IEnumerable<VectorDBItem> items) {
            return JsonSerializer.Serialize(items, JsonSerializerOptions);
        }
        // Json文字列化する
        public static string ToJson(VectorDBItem item) {
            return JsonSerializer.Serialize(item, JsonSerializerOptions);
        }

        private static List<VectorDBItem> _items = new(); // 修正: 空のリストを初期化
        public static async Task LoadItemsAsync() {
            // 修正: 非同期メソッドで 'await' を使用
            _items = await Task.Run(() => PythonAILib.PythonIF.PythonExecutor.PythonAIFunctions.GetVectorDBItemsAsync());
        }

        public static List<VectorDBItem> GetItems() {
            return _items;
        }

        public static List<VectorDBItem> GetVectorDBItems(bool includeDefaultVectorDB) {
            List<VectorDBItem> items = GetItems();
            if (includeDefaultVectorDB) {
                return items;
            }
            // システム共通のベクトルDBは除外する
            return items.Where(item => !item.IsSystem && item.Name != SystemCommonVectorDBName).ToList();

        }

        // GetItemById
        public static VectorDBItem? GetItemById(string? id) {
            List<VectorDBItem> items = GetItems();
            // IDが一致するアイテムを取得
            VectorDBItem? item = items.FirstOrDefault(i => i.Id == id);

            return item;
        }

        // GetItemByName
        public static VectorDBItem? GetItemByName(string? name) {
            List<VectorDBItem> items = GetItems();
            // 名前が一致するアイテムを取得
            VectorDBItem? item = items.FirstOrDefault(i => i.Name == name);
            return item;
        }

        public static List<VectorDBItem> GetExternalVectorDBItems() {
            List<VectorDBItem> allItems = GetItems();
            List<VectorDBItem> result = [];
            // システム共通のベクトルDBは除外する
            result = allItems.Where(item => !item.IsSystem && item.Name != SystemCommonVectorDBName).ToList();
            return result;

        }

        public static VectorDBItem FromDict(Dictionary<string, object> dict) {
            VectorDBItem item = new();
            item.Id = dict["Id"]?.ToString() ?? "";
            item.Name = dict["Name"]?.ToString() ?? "";
            item.Description = dict["Description"]?.ToString() ?? "";
            item.VectorDBURL = dict["VectorDBURL"]?.ToString() ?? "";
            item.IsUseMultiVectorRetriever = Convert.ToBoolean(dict["IsUseMultiVectorRetriever"]);
            item.DocStoreURL = dict["DocStoreURL"]?.ToString() ?? "";
            item.ChunkSize = Convert.ToInt32(dict["ChunkSize"]);
            item.CollectionName = dict["CollectionName"]?.ToString() ?? "";
            item.IsEnabled = Convert.ToBoolean(dict["IsEnabled"]);
            item.IsSystem = Convert.ToBoolean(dict["IsSystem"]);
            item.Type = (VectorDBTypeEnum)Int32.Parse(dict["VectorDBType"]?.ToString() ?? "0");
            item.DefaultSearchResultLimit = Convert.ToInt32(dict["DefaultSearchResultLimit"]);

            return item;

        }
    }
}
