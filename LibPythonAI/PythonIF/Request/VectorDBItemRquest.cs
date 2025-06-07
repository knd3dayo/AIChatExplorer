using System.Text.Json.Serialization;
using LibPythonAI.Model.VectorDB;

namespace LibPythonAI.PythonIF.Request {
    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class VectorDBItemRequest {

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



        public VectorDBItemRequest(VectorDBItem item) {
            Id = item.Id;
            Name = item.Name;
            Description = item.Description;
            VectorDBURL = item.VectorDBURL;
            IsUseMultiVectorRetriever = item.IsUseMultiVectorRetriever;
            DocStoreURL = item.DocStoreURL;
            ChunkSize = item.ChunkSize;
            CollectionName = item.CollectionName ?? "";
            IsEnabled = item.IsEnabled;
            IsSystem = item.IsSystem;
            Type = item.Type;
            DefaultSearchResultLimit = item.DefaultSearchResultLimit;
            VectorDBTypeString = item.VectorDBTypeString;
            DefaultScoreThreshold = item.DefaultScoreThreshold;

        }
        public string Id { get; set; }
        // 名前
        public string Name { get; set; }
        // 説明
        public string Description { get; set; }

        // ベクトルDBのURL
        public string VectorDBURL { get; set; }

        // マルチベクトルリトリーバを使うかどうか
        public bool IsUseMultiVectorRetriever { get; set; }

        // ドキュメントストアのURL マルチベクトルリトリーバを使う場合に指定する
        public string DocStoreURL { get; set; }

        // ベクトルDBの種類を表す列挙型
        [JsonIgnore]
        public VectorDBTypeEnum Type { get; set; }

        // ベクトルDBの種類を表す文字列
        public string VectorDBTypeString { get; set; }

        // コレクション名
        public string CollectionName { get; set; }

        // チャンクサイズ ベクトル生成時にドキュメントをこのサイズで分割してベクトルを生成する
        public int ChunkSize { get; set; }

        // ベクトル検索時の検索結果上限
        public int DefaultSearchResultLimit { get; set; }

        // スコアの閾値
        public float DefaultScoreThreshold { get; set; }

        public bool IsEnabled { get; set; }

        public bool IsSystem { get; set; }

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { ID_KEY, Id },
                { NAME_KEY, Name },
                { DESCRIPTION_KEY, Description },
                { VECTOR_DB_URL_KEY, VectorDBURL },
                { IS_USE_MULTI_VECTOR_RETRIEVER_KEY, IsUseMultiVectorRetriever },
                { DOC_STORE_URL_KEY, DocStoreURL },
                { CHUNK_SIZE_KEY, ChunkSize },
                { COLLECTION_NAME_KEY, CollectionName },
                { IS_ENABLED_KEY, IsEnabled },
                { IS_SYSTEM_KEY, IsSystem },
                { TYPE_KEY, (int)Type },
                { DEFAULT_SEARCH_RESULT_LIMIT_KEY, DefaultSearchResultLimit},
                { DEFAULT_SCORE_THREASHOLD_KEY, DefaultScoreThreshold }
            }
            ;
            return dict;
        }



        // CreateEntriesDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorDBItemRequest> items) {
            return items.Select(item => item.ToDict()).ToList();
        }


    }
}
