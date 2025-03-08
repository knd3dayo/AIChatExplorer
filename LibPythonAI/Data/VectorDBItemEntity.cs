using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resources;

namespace LibPythonAI.Data {
    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class VectorDBItemEntity {

        // システム共通のベクトルDBの名前
        public readonly static string SystemCommonVectorDBName = "default";
        // デフォルトのコレクション名
        public readonly static string DefaultCollectionName = "ai_app_default_collection";

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // 名前
        [JsonPropertyName("vector_db_name")]
        public string Name { get; set; } = "";
        // 説明
        [JsonPropertyName("vector_db_description")]
        public string Description { get; set; } = PythonAILibStringResources.Instance.VectorDBDescription;

        // ベクトルDBのURL
        [JsonPropertyName("vector_db_url")]
        public string VectorDBURL { get; set; } = "";

        // マルチベクトルリトリーバを使うかどうか
        [JsonPropertyName("is_use_multi_vector_retriever")]
        public bool IsUseMultiVectorRetriever { get; set; } = false;

        // ドキュメントストアのURL マルチベクトルリトリーバを使う場合に指定する
        [JsonPropertyName("doc_store_url")]
        public string DocStoreURL { get; set; } = "";

        // ベクトルDBの種類を表す文字列
        [JsonPropertyName("vector_db_type_string")]
        public VectorDBTypeEnum VectorDBType { get; set; } = VectorDBTypeEnum.Chroma;

        // コレクション名
        [JsonPropertyName("collection_name")]
        public string CollectionName { get; set; } = DefaultCollectionName;


        // チャンクサイズ ベクトル生成時にドキュメントをこのサイズで分割してベクトルを生成する
        [JsonPropertyName("chunk_size")]
        public int ChunkSize { get; set; } = 1024;

        // ベクトル検索時の検索結果上限
        [JsonPropertyName("default_search_result_limit")]
        public int DefaultSearchResultLimit { get; set; } = 10;

        // 有効かどうか
        [JsonIgnore]
        public bool IsEnabled { get; set; } = true;

        // システム用のフラグ
        [JsonIgnore]
        public bool IsSystem { get; set; } = false;

        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            VectorDBItemEntity item = (VectorDBItemEntity)obj;
            return Id == item.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

    }
}
