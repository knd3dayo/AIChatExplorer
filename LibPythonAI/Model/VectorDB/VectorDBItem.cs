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
                { "id", Id },
                { "vector_db_type", (int)Type },
                { "is_enabled", IsEnabled },
                { "is_system", IsSystem },
                { "name", Name },
                { "description", Description },
                { "system_message", PromptStringResource.Instance.VectorDBSystemMessage(Description) },
                { "vector_db_url", VectorDBURL },
                { "is_use_multi_vector_retriever", IsUseMultiVectorRetriever },
                { "doc_store_url", DocStoreURL },
                { "vector_db_type_string", VectorDBTypeString },
                { "collection_name", CollectionName ?? ""},
                { "chunk_size", ChunkSize },
            };
            return dict;
        }


        // SaveAsync
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
            item.Id = dict["id"]?.ToString() ?? "";
            item.Name = dict["name"]?.ToString() ?? "";
            item.Description = dict["description"]?.ToString() ?? "";
            item.VectorDBURL = dict["vector_db_url"]?.ToString() ?? "";
            item.IsUseMultiVectorRetriever = Convert.ToBoolean(dict["is_use_multi_vector_retriever"]);
            item.DocStoreURL = dict["doc_store_url"]?.ToString() ?? "";
            item.ChunkSize = Convert.ToInt32(dict["chunk_size"]);
            item.CollectionName = dict["collection_name"]?.ToString() ?? "";
            item.IsEnabled = Convert.ToBoolean(dict["is_enabled"]);
            item.IsSystem = Convert.ToBoolean(dict["is_system"]);
            item.Type = (VectorDBTypeEnum)Int32.Parse(dict["vector_db_type"]?.ToString() ?? "0");
            item.DefaultSearchResultLimit = Convert.ToInt32(dict["default_search_result_limit"]);

            return item;

        }
    }
}
