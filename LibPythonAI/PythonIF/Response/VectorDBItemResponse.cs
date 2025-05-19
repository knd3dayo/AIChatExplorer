using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibPythonAI.Model.VectorDB;
using PythonAILib.Resources;

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

        public VectorDBItemResponse(Dictionary<string, object> dict) {
            Id = dict[ID_KEY]?.ToString() ?? "";
            Name = dict[NAME_KEY]?.ToString() ?? "";
            Description = dict[DESCRIPTION_KEY]?.ToString() ?? "";
            VectorDBURL = dict[VECTOR_DB_URL_KEY]?.ToString() ?? "";
            IsUseMultiVectorRetriever = Convert.ToBoolean(dict[IS_USE_MULTI_VECTOR_RETRIEVER_KEY]);
            DocStoreURL = dict[DOC_STORE_URL_KEY]?.ToString() ?? "";
            ChunkSize = Convert.ToInt32(dict[CHUNK_SIZE_KEY]);
            CollectionName = dict[COLLECTION_NAME_KEY]?.ToString() ?? "";
            IsEnabled = Convert.ToBoolean(dict[IS_ENABLED_KEY]);
            IsSystem = Convert.ToBoolean(dict[IS_SYSTEM_KEY]);
            Type = (VectorDBTypeEnum)int.Parse(dict[TYPE_KEY]?.ToString() ?? "0");
            DefaultSearchResultLimit = Convert.ToInt32(dict[DEFAULT_SEARCH_RESULT_LIMIT_KEY]);
            VectorDBTypeString = Type.ToString();
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
