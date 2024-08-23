using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.PythonIF;

namespace PythonAILib.Model {
    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public abstract class VectorDBItem {

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
            return System.Text.Json.JsonSerializer.Serialize(items, options);
        }
        // Json文字列化する
        public static string ToJson(VectorDBItem item) {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return System.Text.Json.JsonSerializer.Serialize(item, options);
        }


        // Save
        public abstract void Save();

        // Delete
        public abstract void Delete();
        public abstract void UpdateIndex(ContentInfo clipboard);
        public abstract void DeleteIndex(ContentInfo clipboard);

        public abstract void UpdateIndex(ImageInfo imageInfo);

        public abstract void DeleteIndex(ImageInfo imageInfo);

    }
}
