using System.Text.Json;
using System.Text.Json.Serialization;
using LibMain.Model.VectorDB;
using LibMain.Resources;
using LibMain.Utils.Common;

namespace LibMain.PythonIF.Request {
    public class VectorSearchRequest {

        public const string VECTOR_DB_NAME_KEY = "vector_db_name";
        public const string QUERY_KEY = "query";

        public const string TOP_K_KEY = "k";
        public const string FILTER_KEY = "filter";

        public const string FOLDER_ID_KEY = "folder_id";
        public const string FOLDER_PATH_KEY = "folder_path";
        public const string CONTENT_TYPE_KEY = "content_type";

        public const string SCORE_THRESHOLD_KEY = "score_threshold";


        public VectorSearchRequest(VectorSearchItem vectorSearchItem) {
            VectorDBName = vectorSearchItem.VectorDBItemName;
            Query = vectorSearchItem.InputText;
            TopK = vectorSearchItem.TopK;
            FolderPath = vectorSearchItem.FolderPath;
            ScoreThreshold = vectorSearchItem.ScoreThreshold;
        }

        [JsonConstructor]
        public VectorSearchRequest(string? name, string? model, string? query, int topK, float scoreThreshold, string? folderPath, string contentType) {
            VectorDBName = name;
            Query = query;
            TopK = topK;
            ScoreThreshold = scoreThreshold;
            FolderPath = folderPath;
            ContentType = contentType;
        }

        [JsonPropertyName(VECTOR_DB_NAME_KEY)]
        public string? VectorDBName { init; get; } = null;

        [JsonPropertyName(QUERY_KEY)]
        public string? Query { get; set; } = null;

        //TopK
        [JsonPropertyName(TOP_K_KEY)]
        public int TopK { get; set; } = 5; // デフォルト値

        // score_threshold
        [JsonPropertyName(SCORE_THRESHOLD_KEY)]
        public float ScoreThreshold { get; set; } = 0.5f;

        // FolderPath
        [JsonPropertyName(FOLDER_PATH_KEY)]
        public string? FolderPath { get; set; } = null;

        [JsonPropertyName(CONTENT_TYPE_KEY)]
        public string ContentType { init; get; } = string.Empty;

        public Dictionary<string, object> ToDict() {
            if (string.IsNullOrEmpty(VectorDBName)) {
                throw new Exception(PythonAILibStringResourcesJa.Instance.PropertyNotSet(VECTOR_DB_NAME_KEY));
            }

            Dictionary<string, object> dict = [];
            dict[VECTOR_DB_NAME_KEY] = VectorDBName;
            if (!string.IsNullOrEmpty(Query)) {
                dict[QUERY_KEY] = Query;
            }
            dict[TOP_K_KEY] = TopK;
            Dictionary<string, object> filter = [];
            if (FolderPath != null) {
                filter[FOLDER_PATH_KEY] = FolderPath;
            }
            dict[FILTER_KEY] = filter;

            return dict;
        }

        public string ToJson() {
            return JsonSerializer.Serialize(this, JsonUtil.JsonSerializerOptions);
        }

        public static VectorSearchRequest? FromJson(string json) {

            return JsonSerializer.Deserialize<VectorSearchRequest>(json, JsonUtil.JsonSerializerOptions);
        }


    }
}
