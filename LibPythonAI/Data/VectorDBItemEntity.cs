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
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = PythonAILibStringResources.Instance.VectorDBDescription;

        // ベクトルDBのURL
        public string VectorDBURL { get; set; } = "";

        // マルチベクトルリトリーバを使うかどうか
        public bool IsUseMultiVectorRetriever { get; set; } = false;

        // ドキュメントストアのURL マルチベクトルリトリーバを使う場合に指定する
        public string DocStoreURL { get; set; } = "";

        // ベクトルDBの種類を表す文字列
        public VectorDBTypeEnum VectorDBType { get; set; } = VectorDBTypeEnum.Chroma;

        // コレクション名
        public string CollectionName { get; set; } = DefaultCollectionName;


        // チャンクサイズ ベクトル生成時にドキュメントをこのサイズで分割してベクトルを生成する
        public int ChunkSize { get; set; } = 1024;

        // ベクトル検索時の検索結果上限
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
