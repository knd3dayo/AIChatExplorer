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
    public class VectorDBItem(VectorDBItemEntity entity) {

        public VectorDBItemEntity Entity { get; set; } = entity;

        public string Id { get => Entity.Id; }

        // システム共通のベクトルDBの名前
        public readonly static string SystemCommonVectorDBName = "default";
        // デフォルトのコレクション名 
        public readonly static string DefaultCollectionName = "ai_app_default_collection";
        // フォルダーカタログのコレクション名 
        public readonly static string FolderCatalogCollectionName = "ai_app_folder_catalog_collection";

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
        public string Name {
            get => Entity.Name;
            set => Entity.Name = value;
        }
        // 説明
        public string Description {
            get => Entity.Description;
            set => Entity.Description = value;
        }

        // ベクトルDBのURL
        public string VectorDBURL {
            get => Entity.VectorDBURL;
            set => Entity.VectorDBURL = value;
        }

        // マルチベクトルリトリーバを使うかどうか
        public bool IsUseMultiVectorRetriever {
            get => Entity.IsUseMultiVectorRetriever;
            set => Entity.IsUseMultiVectorRetriever = value;
        }

        // ドキュメントストアのURL マルチベクトルリトリーバを使う場合に指定する
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
        public string VectorDBTypeString {
            get {
                return Type.ToString();
            }
        }

        // コレクション名
        public string CollectionName {
            get => Entity.CollectionName;
            set => Entity.CollectionName = value;
        }

        // チャンクサイズ ベクトル生成時にドキュメントをこのサイズで分割してベクトルを生成する
        public int ChunkSize {
            get => Entity.ChunkSize;
            set => Entity.ChunkSize = value;
        }

        // ベクトル検索時の検索結果上限
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

        // GetItemByName
        public static VectorDBItem? GetItemByName(string? name) {
            using PythonAILibDBContext db = new();
            var item = db.VectorDBItems.FirstOrDefault(item => item.Name == name);
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
    }
}
