using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;

namespace PythonAILib.Model.VectorDB {
    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class VectorDBItem {

        // システム共通のベクトルDBの名前
        public readonly static string SystemCommonVectorDBName = "default";
        // デフォルトのコレクション名
        public readonly static string DefaultCollectionName = "ai_app_default_collection";

        // システム共通のベクトルDB
        public static VectorDBItem GetDefaultVectorDB() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var item = libManager.DataFactory.GetVectorDBCollection<VectorDBItem>().FindOne(item => item.Name == SystemCommonVectorDBName);
            if (item == null) {
                string vectorDBPath = libManager.ConfigParams.GetSystemVectorDBPath();
                string docDBPath = libManager.ConfigParams.GetSystemDocDBPath();
                item = new VectorDBItem() {
                    Id = LiteDB.ObjectId.Empty,
                    Name = VectorDBItem.SystemCommonVectorDBName,
                    Description = PythonAILibStringResources.Instance.GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions,
                    Type = VectorDBTypeEnum.Chroma,
                    VectorDBURL = vectorDBPath,
                    DocStoreURL = $"sqlite:///{docDBPath}",
                    IsUseMultiVectorRetriever = true,
                    IsEnabled = true,
                    IsSystem = true,
                    CollectionName = DefaultCollectionName
                };
                item.Save();
            }
            // IsSystemフラグ導入前のバージョンへの対応
            item.IsSystem = true;
            return item;
        }

        public static string GetCatalogDBURL() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            return libManager.ConfigParams.GetCatalogDBURL();
        }

        private static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };


        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        // 名前
        [JsonPropertyName("VectorDBName")]
        public string Name { get; set; } = DefaultCollectionName;
        // 説明
        [JsonPropertyName("VectorDBDescription")]
        public string Description { get; set; } = PythonAILibStringResources.Instance.VectorDBDescription;

        // ベクトルDBのURL
        [JsonPropertyName("VectorDBURL")]
        public string VectorDBURL { get; set; } = "";

        // マルチベクトルリトリーバを使うかどうか
        [JsonPropertyName("IsUseMultiVectorRetriever")]
        public bool IsUseMultiVectorRetriever { get; set; } = false;

        // ドキュメントストアのURL マルチベクトルリトリーバを使う場合に指定する
        [JsonPropertyName("DocStoreURL")]
        public string DocStoreURL { get; set; } = "";

        // ベクトルDBの種類を表す列挙型
        [JsonIgnore]
        public VectorDBTypeEnum Type { get; set; } = VectorDBTypeEnum.Chroma;

        // ベクトルDBの種類を表す文字列
        [JsonPropertyName("VectorDBTypeString")]
        public string VectorDBTypeString {
            get {
                return Type.ToString();
            }
        }

        // FolderId
        [JsonPropertyName("folder_id")]
        public string FolderId { get; set; } = "";

        // コレクション名
        [JsonPropertyName("CollectionName")]
        public string? CollectionName { get; set; } = null;

        // カタログ用のDBURL
        [JsonPropertyName("CatalogDBURL")]
        public string CatalogDBURL { get; set; } = GetCatalogDBURL();


        // チャンクサイズ ベクトル生成時にドキュメントをこのサイズで分割してベクトルを生成する
        [JsonPropertyName("ChunkSize")]
        public int ChunkSize { get; set; } = 500;

        // ベクトル検索時の検索結果上限
        [JsonPropertyName("MaxSearchResults")]
        public int MaxSearchResults { get; set; } = 10;

        // 有効かどうか
        [JsonIgnore]
        public bool IsEnabled { get; set; } = true;

        // システム用のフラグ
        [JsonIgnore]
        public bool IsSystem { get; set; } = false;

        [BsonIgnore]
        public string DisplayText {
            get {
                if (string.IsNullOrEmpty(CollectionName)) {
                    return Name;
                }
                if (string.IsNullOrEmpty(FolderId)) {
                    return Name;
                }
                // ContentFolderを取得
                var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                ContentFolder? folder = collection.FindById(new ObjectId(FolderId));
                if (folder == null) {
                    return Name;
                }

                return $"{Name}:{folder.FolderName}";
            }
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

        // ToDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "VectorDBName", Name },
                { "VectorDBDescription", Description },
                { "VectorDBURL", VectorDBURL },
                { "IsUseMultiVectorRetriever", IsUseMultiVectorRetriever },
                { "DocStoreURL", DocStoreURL },
                { "VectorDBTypeString", VectorDBTypeString },
                { "CollectionName", CollectionName ?? ""},
                { "folder_id", FolderId ?? ""},
                { "ChunkSize", ChunkSize },
                { "MaxSearchResults", MaxSearchResults },
                { "IsEnabled", IsEnabled },
                { "IsSystem", IsSystem },
                { "CatalogDBURL", CatalogDBURL }
            };
            return dict;
        }
        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorDBItem> items) {
            return items.Select(item => item.ToDict()).ToList();
        }

        // Json文字列化する
        public static string ToJson(IEnumerable<VectorDBItem> items) {
            return System.Text.Json.JsonSerializer.Serialize(items, JsonSerializerOptions);
        }
        // Json文字列化する
        public static string ToJson(VectorDBItem item) {
            return System.Text.Json.JsonSerializer.Serialize(item, JsonSerializerOptions);
        }

        public static IEnumerable<VectorDBItem> GetItems() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            return libManager.DataFactory.GetVectorDBCollection<VectorDBItem>().FindAll();
        }
        // GetItemById
        public static VectorDBItem? GetItemById(LiteDB.ObjectId id) {
            return GetItems().FirstOrDefault(item => item.Id == id);
        }

        // Save
        public void Save() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetVectorDBCollection<VectorDBItem>();
            collection.Upsert(this);
        }

        // Delete
        public void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetVectorDBCollection<VectorDBItem>();
            collection.Delete(this.Id);
        }


        public static VectorDBItem GetFolderVectorDBItem() {
            VectorDBItem systemVectorItem = VectorDBItem.GetDefaultVectorDB();
            // NameとDescriptionとCollectionNameを設定する
            systemVectorItem.Name = SystemCommonVectorDBName;
            systemVectorItem.Description = SystemCommonVectorDBName;
            systemVectorItem.CollectionName = DefaultCollectionName;

            return systemVectorItem;
        }

        public static List<VectorDBItem> GetExternalVectorDBItems() {
            var collection = PythonAILibManager.Instance.DataFactory.GetVectorDBCollection<VectorDBItem>();
            var items = collection.Find(item => !item.IsSystem && item.Name != VectorDBItem.SystemCommonVectorDBName);
            if (items == null) {
                return [];
            }
            return new(items);
        }

        public static List<VectorDBItem> GetVectorDBItems() {
            var collection = PythonAILibManager.Instance.DataFactory.GetVectorDBCollection<VectorDBItem>();
            var items = collection.FindAll();
            if (items == null) {
                return [];
            }
            return new(items);
        }


    }
}
