using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
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
        public static string SystemCommonVectorDBName = "SystemCommonVectorDB";

        // システム共通のベクトルDB
        public static VectorDBItem SystemCommonVectorDB {
            get {
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
                        IsSystem = true
                    };
                    item.Save();
                }
                // IsSystemフラグ導入前のバージョンへの対応
                item.IsSystem = true;
                return item;

            }
        }
        private static JsonSerializerOptions JsonSerializerOptions { get; } = new JsonSerializerOptions {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };


        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        // 名前
        [JsonPropertyName("VectorDBName")]
        public string Name { get; set; } = "";
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

        // コレクション名
        [JsonPropertyName("CollectionName")]
        public string? CollectionName { get; set; } = null;

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
                { "ChunkSize", ChunkSize },
                { "MaxSearchResults", MaxSearchResults },
                { "IsEnabled", IsEnabled },
                { "IsSystem", IsSystem }
            };
            return dict;
        }
        // ToDictList
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

        public void UpdateIndex(ContentInfo contentInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = [this],
                OpenAIProperties = openAIProperties
            };

            UpdateIndex(chatRequestContext, contentInfo);
        }

        public void DeleteIndex(ContentInfo contentInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = [this],
                OpenAIProperties = openAIProperties
            };
            DeleteIndex(chatRequestContext, contentInfo);
        }

        public void UpdateIndex(ImageInfo imageInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = [this],
                OpenAIProperties = openAIProperties
            };
            UpdateIndex(chatRequestContext, imageInfo);
        }

        public void DeleteIndex(ImageInfo imageInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = [this],
                OpenAIProperties = openAIProperties
            };
            DeleteIndex(chatRequestContext, imageInfo);
        }

        public void UpdateIndex(ChatRequestContext chatRequestContext, ContentInfo contentInfo ) {
            LogWrapper.Info(PythonAILibStringResources.Instance.SaveEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(chatRequestContext, contentInfo);
            LogWrapper.Info(PythonAILibStringResources.Instance.SavedEmbedding);
        }
        public void DeleteIndex(ChatRequestContext chatRequestContext, ContentInfo contentInfo) {
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(chatRequestContext, contentInfo);
            LogWrapper.Info(PythonAILibStringResources.Instance.DeletedEmbedding);
        }
        public void UpdateIndex(ChatRequestContext chatRequestContext, ImageInfo imageInfo) {
            LogWrapper.Info(PythonAILibStringResources.Instance.SaveTextEmbeddingFromImage);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(chatRequestContext, imageInfo);
            LogWrapper.Info(PythonAILibStringResources.Instance.SavedTextEmbeddingFromImage);

        }
        public void DeleteIndex(ChatRequestContext chatRequestContext, ImageInfo imageInfo) {
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteTextEmbeddingFromImage);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(chatRequestContext, imageInfo);
            LogWrapper.Info(PythonAILibStringResources.Instance.DeletedTextEmbeddingFromImage);
        }
        public static VectorDBItem GetFolderVectorDBItem(ContentFolder folder) {
            VectorDBItem systemVectorItem = VectorDBItem.SystemCommonVectorDB;
            // NameとDescriptionとCollectionNameを設定する
            systemVectorItem.Name = folder.FolderName;
            systemVectorItem.Description = folder.Description;
            systemVectorItem.CollectionName = folder.Id.ToString();

            return systemVectorItem;
        }
    }
}
