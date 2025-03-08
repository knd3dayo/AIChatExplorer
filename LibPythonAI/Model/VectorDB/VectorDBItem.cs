using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibPythonAI.Data;
using PythonAILib.Common;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resources;

namespace LibPythonAI.Model.VectorDB {
    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class VectorDBItem {

        public VectorDBItemEntity Entity { get; set; }

        public VectorDBItem(VectorDBItemEntity entity) {
            Entity = entity;
        }

        public string Id { get => Entity.Id; }

        // システム共通のベクトルDBの名前
        public readonly static string SystemCommonVectorDBName = "default";
        // デフォルトのコレクション名
        public readonly static string DefaultCollectionName = "ai_app_default_collection";

        public static void Init() {
            var item = GetItemByName(SystemCommonVectorDBName);
            if (item == null) {
                PythonAILibManager libManager = PythonAILibManager.Instance;
                string vectorDBPath = libManager.ConfigParams.GetSystemVectorDBPath();
                string docDBPath = libManager.ConfigParams.GetSystemDocDBPath();
                var entity = new VectorDBItemEntity() {
                    Name = SystemCommonVectorDBName,
                    Description = PythonAILibStringResources.Instance.GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions,
                    VectorDBType = VectorDBTypeEnum.Chroma,
                    VectorDBURL = vectorDBPath,
                    DocStoreURL = $"sqlite:///{docDBPath}",
                    IsUseMultiVectorRetriever = true,
                    IsEnabled = true,
                    IsSystem = true,
                    CollectionName = DefaultCollectionName
                };
                item = new(entity);
                item.Save();
            }
        }
        // システム共通のベクトルDB
        public static VectorDBItem GetDefaultVectorDB() {
            var item = GetItemByName(SystemCommonVectorDBName);
            if (item == null) {
                Init();
                item = GetItemByName(SystemCommonVectorDBName);
            }
            return item!;
        }

        private static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };


        // 名前
        [JsonPropertyName("vector_db_name")]
        public string Name {
            get => Entity.Name;
            set => Entity.Name = value;
        }
        // 説明
        [JsonPropertyName("vector_db_description")]
        public string Description {
            get => Entity.Description;
            set => Entity.Description = value;
        }

        // ベクトルDBのURL
        [JsonPropertyName("vector_db_url")]
        public string VectorDBURL {
            get => Entity.VectorDBURL;
            set => Entity.VectorDBURL = value;
        }

        // マルチベクトルリトリーバを使うかどうか
        [JsonPropertyName("is_use_multi_vector_retriever")]
        public bool IsUseMultiVectorRetriever {
            get => Entity.IsUseMultiVectorRetriever;
            set => Entity.IsUseMultiVectorRetriever = value;
        }

        // ドキュメントストアのURL マルチベクトルリトリーバを使う場合に指定する
        [JsonPropertyName("doc_store_url")]
        public string DocStoreURL {
            get => Entity.DocStoreURL;
            set => Entity.DocStoreURL = value;
        }

        // ベクトルDBの種類を表す列挙型
        [JsonIgnore]
        public VectorDBTypeEnum Type {
            get => Entity.VectorDBType;
            set => Entity.VectorDBType = value;
        }

        // ベクトルDBの種類を表す文字列
        [JsonPropertyName("vector_db_type_string")]
        public string VectorDBTypeString {
            get {
                return Type.ToString();
            }
        }

        // コレクション名
        [JsonPropertyName("collection_name")]
        public string CollectionName {
            get => Entity.CollectionName;
            set => Entity.CollectionName = value;
        }

        // チャンクサイズ ベクトル生成時にドキュメントをこのサイズで分割してベクトルを生成する
        [JsonPropertyName("chunk_size")]
        public int ChunkSize {
            get => Entity.ChunkSize;
            set => Entity.ChunkSize = value;
        }

        // ベクトル検索時の検索結果上限
        [JsonPropertyName("default_search_result_limit")]
        public int DefaultSearchResultLimit {
            get => Entity.DefaultSearchResultLimit;
            set => Entity.DefaultSearchResultLimit = value;
        }

        // 有効かどうか
        [JsonIgnore]
        public bool IsEnabled {
            get => Entity.IsEnabled;
            set => Entity.IsEnabled = value;
        }

        // システム用のフラグ
        [JsonIgnore]
        public bool IsSystem {
            get => Entity.IsSystem;
            set => Entity.IsSystem = value;
        }

        // Equals
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            VectorDBItem other = (VectorDBItem)obj;
            return VectorDBURL == other.VectorDBURL;
        }
        public override int GetHashCode() {
            return VectorDBURL.GetHashCode();
        }

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "vector_db_name", Name },
                { "vector_db_description", Description },
                { "vector_db_system_message", PromptStringResource.Instance.VectorDBSystemMessage(Description) },
                { "vector_db_url", VectorDBURL },
                { "is_use_multi_vector_retriever", IsUseMultiVectorRetriever },
                { "doc_store_url", DocStoreURL },
                { "vector_db_type_string", VectorDBTypeString },
                { "collection_name", CollectionName ?? ""},
                { "chunk_size", ChunkSize },
            };
            return dict;
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

        public static List<VectorDBItem> GetItems() {
            List<VectorDBItem> result = [];
            using PythonAILibDBContext db = new();
            var items = db.VectorDBItems;
            foreach (var item in items) {
                result.Add(new VectorDBItem(item));
            }
            return result;
        }

        // GetItemById
        public static VectorDBItem? GetItemById(string? id) {
            using PythonAILibDBContext db = new();
            var item = db.VectorDBItems.Find(id);
            if (item == null) {
                return null;
            }
            return new VectorDBItem(item);
        }

        // Save
        public void Save() {
            using PythonAILibDBContext db = new();
            var item = db.VectorDBItems.Find(Id);
            if (item == null) {
                db.VectorDBItems.Add(Entity);
            } else {
                db.VectorDBItems.Entry(item).CurrentValues.SetValues(Entity);
            }
            db.SaveChanges();
        }

        // Delete
        public void Delete() {
            using PythonAILibDBContext db = new();
            var item = db.VectorDBItems.Find(Id);
            if (item != null) {
                db.VectorDBItems.Remove(item);
            }
            db.SaveChanges();
        }


        public static VectorDBItem GetFolderVectorDBItem() {
            VectorDBItem systemVectorItem = GetDefaultVectorDB();
            // NameとDescriptionとCollectionNameを設定する
            systemVectorItem.Name = SystemCommonVectorDBName;
            systemVectorItem.Description = SystemCommonVectorDBName;
            systemVectorItem.CollectionName = DefaultCollectionName;

            return systemVectorItem;
        }

        public static List<VectorDBItem> GetExternalVectorDBItems() {
            List<VectorDBItem> result = [];
            using PythonAILibDBContext db = new();
            var items = db.VectorDBItems.Where(item => !item.IsSystem && item.Name != SystemCommonVectorDBName);
            if (items == null) {
                return result;
            }
            foreach (var item in items) {
                result.Add(new VectorDBItem(item));
            }
            return result;
        }

        public static List<VectorDBItem> GetVectorDBItems(bool includeDefaultVectorDB) {
            List<VectorDBItem> result = [];
            if (includeDefaultVectorDB) {
                result.Add(GetDefaultVectorDB());
            }
            result.AddRange(GetExternalVectorDBItems());
            return result;
        }

        // GetItemByName
        public static VectorDBItem? GetItemByName(string? name) {
            using PythonAILibDBContext db = new();
            var item = db.VectorDBItems.FirstOrDefault(item => item.Name == name);
            if (item == null) {
                return null;
            }
            return new VectorDBItem(item);


        }


    }
}
