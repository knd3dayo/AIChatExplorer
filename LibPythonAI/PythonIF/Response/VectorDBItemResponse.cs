using System.Text.Json.Serialization;
using LibPythonAI.Model.VectorDB;

namespace LibPythonAI.PythonIF.Request {
    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class VectorDBItemResponse {


        public const string ID_KEY = "id";
        public const string NAME_KEY = "name";
        public const string DESCRIPTION_KEY = "description";
        public const string VECTOR_DB_URL_KEY = "vector_db_url";
        public const string IS_USE_MULTI_VECTOR_RETRIEVER_KEY = "is_use_multi_vector_retriever";
        public const string DOC_STORE_URL_KEY = "doc_store_url";
        public const string CHUNK_SIZE_KEY = "chunk_size";
        public const string COLLECTION_NAME_KEY = "collection_name";
        public const string IS_ENABLED_KEY = "is_enabled";
        public const string IS_SYSTEM_KEY = "is_system";
        public const string TYPE_KEY = "vector_db_type";
        public const string DEFAULT_SEARCH_RESULT_LIMIT_KEY = "default_search_result_limit";
        public const string DEFAULT_SCORE_THREASHOLD_KEY = "default_score_threshold";

        public static VectorDBItemResponse FromDict(Dictionary<string, object> dict) {
            // VectorDBItemResponseを作成する
            VectorDBItemResponse item = new() {
                Id = dict[ID_KEY]?.ToString() ?? "",
                Name = dict[NAME_KEY]?.ToString() ?? "",
                Description = dict[DESCRIPTION_KEY]?.ToString() ?? "",
                VectorDBURL = dict[VECTOR_DB_URL_KEY]?.ToString() ?? "",
                IsUseMultiVectorRetriever = Convert.ToBoolean(dict[IS_USE_MULTI_VECTOR_RETRIEVER_KEY]),
                DocStoreURL = dict[DOC_STORE_URL_KEY]?.ToString() ?? "",
                ChunkSize = Convert.ToInt32(dict[CHUNK_SIZE_KEY]),
                CollectionName = dict[COLLECTION_NAME_KEY]?.ToString() ?? "",
                IsEnabled = Convert.ToBoolean(dict[IS_ENABLED_KEY]),
                IsSystem = Convert.ToBoolean(dict[IS_SYSTEM_KEY]),
                Type = (VectorDBTypeEnum)int.Parse(dict[TYPE_KEY]?.ToString() ?? "0"),
                DefaultSearchResultLimit = Convert.ToInt32(dict[DEFAULT_SEARCH_RESULT_LIMIT_KEY])
            };
            return item;
        }

        public string Id { get; set; } = string.Empty;
        // 名前
        public string Name { get; set; } = string.Empty;
        // 説明
        public string Description { get; set; } = string.Empty;

        // ベクトルDBのURL
        public string VectorDBURL { get; set; } = string.Empty;

        // マルチベクトルリトリーバを使うかどうか
        public bool IsUseMultiVectorRetriever { get; set; } = false;

        // ドキュメントストアのURL マルチベクトルリトリーバを使う場合に指定する
        public string DocStoreURL { get; set; } = string.Empty;

        // ベクトルDBの種類を表す列挙型
        [JsonIgnore]
        public VectorDBTypeEnum Type { get; set; } = VectorDBTypeEnum.Chroma;

        // ベクトルDBの種類を表す文字列
        public string VectorDBTypeString { get; set; } = string.Empty;

        // コレクション名
        public string CollectionName { get; set; } = string.Empty;

        // チャンクサイズ ベクトル生成時にドキュメントをこのサイズで分割してベクトルを生成する
        public int ChunkSize { get; set; } = 1024; // デフォルト値

        // ベクトル検索時の検索結果上限
        public int DefaultSearchResultLimit { get; set; } = 10; // デフォルト値
        // スコアの閾値
        public float DefaultScoreThreshold { get; set; } = 0.5f;

        public bool IsEnabled { get; set; }

        public bool IsSystem { get; set; }

        public VectorDBItem CreateVectorDBItem() {
            // VectorDBItemを作成する
            VectorDBItem item = new() {
                Id = Id,
                Name = Name,
                Description = Description,
                VectorDBURL = VectorDBURL,
                IsUseMultiVectorRetriever = IsUseMultiVectorRetriever,
                DocStoreURL = DocStoreURL,
                ChunkSize = ChunkSize,
                CollectionName = CollectionName,
                IsEnabled = IsEnabled,
                IsSystem = IsSystem,
                Type = Type,
                DefaultSearchResultLimit = DefaultSearchResultLimit
            };
            return item;
        }
    }
}
