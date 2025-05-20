using LibPythonAI.Model.VectorDB;
using PythonAILib.Resources;

namespace LibPythonAI.PythonIF.Request {
    public class VectorSearchRequest {

        public const string NAME_KEY = "name";
        public const string MODEL_KEY = "model";
        public const string FOLDER_ID_KEY = "folder_id";
        public const string FOLDER_PATH_KEY = "folder_path";
        public const string CONTENT_TYPE_KEY = "content_type";
        public const string QUERY_KEY = "query";
        public const string SEARCH_KWARGS_KEY = "search_kwargs";
        public const string FILTER_KEY = "filter";
        public const string TOP_K_KEY = "k";
        public const string SCORE_THRESHOLD_KEY = "score_threshold";


        public VectorSearchRequest(VectorSearchItem vectorSearchItem) {
            Name = vectorSearchItem.VectorDBItemName;
            Model = vectorSearchItem.Model;
            Query = vectorSearchItem.InputText;
            TopK = vectorSearchItem.TopK;
            FolderId = vectorSearchItem.FolderId;
            FolderPath = vectorSearchItem.FolderPath;
            ScoreThreshold = vectorSearchItem.ScoreThreshold;
        }
        public string? Name { init; get; } = null;

        public string? Model { get; set; } = null;

        public string? Query { get; set; } = null;

        //TopK
        public int TopK { get; set; } = 5; // デフォルト値

        // score_threshold
        public float ScoreThreshold { get; set; } = 0.5f;

        // FolderId
        public string? FolderId { get; set; } = null;

        // FolderPath
        public string? FolderPath { get; set; } = null;

        public string ContentType { init; get; } = string.Empty;

        // SearchKWargs
        private Dictionary<string, object> GetSearchKwargs() {
            Dictionary<string, object> dict = new() {
                [TOP_K_KEY] = TopK,
                [SCORE_THRESHOLD_KEY] = ScoreThreshold,
            };
            // filter 
            Dictionary<string, object> filter = new();
            // folder_idが指定されている場合
            if (FolderId != null) {
                filter[FOLDER_ID_KEY] = FolderId;
            }
            // folder_pathが指定されている場合
            if (FolderPath != null) {
                filter[FOLDER_PATH_KEY] = FolderPath;
            }
            // content_typeが指定されている場合
            if (ContentType != string.Empty) {
                filter[CONTENT_TYPE_KEY] = ContentType;
            }
            // filterが指定されている場合
            if (filter.Count > 0) {
                dict[FILTER_KEY] = filter;
            }

            return dict;
        }

        public Dictionary<string, object> ToDict() {
            if (string.IsNullOrEmpty(Name)) {
                throw new Exception(PythonAILibStringResources.Instance.PropertyNotSet(NAME_KEY));
            }
            if (string.IsNullOrEmpty(Model)) {
                throw new Exception(PythonAILibStringResources.Instance.PropertyNotSet(MODEL_KEY));
            }

            Dictionary<string, object> dict = [];
            dict[NAME_KEY] = Name;
            dict[MODEL_KEY] = Model;
            var search_kwargs = GetSearchKwargs();
            if (search_kwargs.Count > 0) {
                dict[SEARCH_KWARGS_KEY] = search_kwargs;
            }
            if (!string.IsNullOrEmpty(Query)) {
                dict[QUERY_KEY] = Query;
            }
            if (!string.IsNullOrEmpty(FolderId)) {
                dict[FOLDER_ID_KEY] = FolderId;
            }
            return dict;
        }

        // CreateEntriesDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorSearchRequest> items) {
            return items.Select(item => item.ToDict()).ToList();
        }

    }
}
