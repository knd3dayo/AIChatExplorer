using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils;
using QAChat;

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
                PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
                return libManager.DataFactory.GetSystemVectorDBItem();
            }
        }

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

        // マルチベクトルリトリーバを使う場合のドキュメントのチャンクサイズ
        [JsonPropertyName("MultiVectorDocChunkSize")]
        public int MultiVectorDocChunkSize { get; set; } = 10000;

        // ベクトル検索時の検索結果上限
        [JsonPropertyName("MaxSearchResults")]
        public int MaxSearchResults { get; set; } = 10;

        // 有効かどうか
        [JsonIgnore]
        public bool IsEnabled { get; set; } = true;

        // システム用のフラグ
        [JsonIgnore]
        public bool IsSystem { get; set; } = false;

        // Json文字列化する
        public static string ToJson(IEnumerable<VectorDBItem> items) {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return JsonSerializer.Serialize(items, options);
        }
        // Json文字列化する
        public static string ToJson(VectorDBItem item) {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return JsonSerializer.Serialize(item, options);
        }

        public static IEnumerable<VectorDBItem> GetItems() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            return libManager.DataFactory.GetVectorDBItems();
        }
        // GetItemById
        public static VectorDBItem? GetItemById(LiteDB.ObjectId id) {
            return GetItems().FirstOrDefault(item => item.Id == id);
        }

        // Save
        public void Save() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.UpsertVectorDBItem(this);
        }

        // Delete
        public void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.DeleteVectorDBItem(this);
        }

        public void UpdateIndex(ContentInfo contentInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            UpdateIndex(contentInfo, openAIProperties);
        }

        public void DeleteIndex(ContentInfo contentInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            DeleteIndex(contentInfo, openAIProperties);

        }

        public void UpdateIndex(ImageInfo imageInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            UpdateIndex(imageInfo, openAIProperties);
        }

        public void DeleteIndex(ImageInfo imageInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            DeleteIndex(imageInfo, openAIProperties);
        }

        public void UpdateIndex(ContentInfo contentInfo, OpenAIProperties openAIProperties) {
            LogWrapper.Info(PythonAILibStringResources.Instance.SaveEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(openAIProperties, contentInfo, this);
            LogWrapper.Info(PythonAILibStringResources.Instance.SavedEmbedding);
        }
        public void DeleteIndex(ContentInfo contentInfo, OpenAIProperties openAIProperties) {
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(openAIProperties, contentInfo, this);
            LogWrapper.Info(PythonAILibStringResources.Instance.DeletedEmbedding);
        }
        public void UpdateIndex(ImageInfo imageInfo, OpenAIProperties openAIProperties) {
            LogWrapper.Info(PythonAILibStringResources.Instance.SaveTextEmbeddingFromImage);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(openAIProperties, imageInfo, this);
            LogWrapper.Info(PythonAILibStringResources.Instance.SavedTextEmbeddingFromImage);

        }
        public void DeleteIndex(ImageInfo imageInfo, OpenAIProperties openAIProperties) {
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteTextEmbeddingFromImage);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(openAIProperties, imageInfo, this);
            LogWrapper.Info(PythonAILibStringResources.Instance.DeletedTextEmbeddingFromImage);
        }

    }
}
