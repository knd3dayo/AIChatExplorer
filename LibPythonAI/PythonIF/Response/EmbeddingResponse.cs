using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using PythonAILib.Model.VectorDB;

namespace LibPythonAI.PythonIF.Response {
    public class EmbeddingResponse {

        public const string NAME_KEY = EmbeddingRequest.NAME_KEY;
        public const string MODEL_KEY = EmbeddingRequest.MODEL_KEY;
        public const string FOLDER_ID_KEY = EmbeddingRequest.FOLDER_ID_KEY;
        public const string FOLDER_PATH_KEY = EmbeddingRequest.FOLDER_PATH_KEY;
        public const string SOURCE_ID_KEY = EmbeddingRequest.SOURCE_ID_KEY;
        public const string SOURCE_TYPE_KEY = EmbeddingRequest.SOURCE_TYPE_KEY;
        public const string DESCRIPTION_KEY = EmbeddingRequest.DESCRIPTION_KEY;
        public const string CONTENT_KEY = EmbeddingRequest.CONTENT_KEY;
        public const string SOURCE_PATH_KEY = EmbeddingRequest.SOURCE_PATH_KEY;
        public const string DOC_ID_KEY = EmbeddingRequest.DOC_ID_KEY;
        public const string SCORE_KEY = EmbeddingRequest.SCORE_KEY;
        public const string SUB_DOCS_KEY = EmbeddingRequest.SUB_DOCS_KEY;

        public static readonly JsonSerializerOptions Options = new() {
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            IncludeFields = true,
            WriteIndented = true
        };
        public EmbeddingResponse(Dictionary<string, object> dict) {
            SourceId = dict[SOURCE_ID_KEY].ToString() ?? "";
            SourceType = (VectorSourceType)Enum.Parse(typeof(VectorSourceType), dict[SOURCE_TYPE_KEY].ToString() ?? "");
            Description = dict[DESCRIPTION_KEY].ToString() ?? "";
            Content = dict[CONTENT_KEY].ToString() ?? "";
            SourcePath = dict[SOURCE_PATH_KEY].ToString() ?? "";
            DocId = dict[DOC_ID_KEY].ToString() ?? "";
            Score = Convert.ToDouble(dict[SCORE_KEY]);
            if (dict.ContainsKey(FOLDER_ID_KEY)) {
                FolderId = dict[FOLDER_ID_KEY].ToString() ?? "";
            }
            if (dict.ContainsKey(FOLDER_PATH_KEY)) {
                FolderPath = dict[FOLDER_PATH_KEY].ToString() ?? "";
            }
            if (dict.ContainsKey(SUB_DOCS_KEY)) {
                foreach (var subDoc in (List<object>)dict[SUB_DOCS_KEY]) {
                    SubDocs.Add(new((Dictionary<string, object>)subDoc));
                }
            }
        }

        [JsonPropertyName(FOLDER_ID_KEY)]
        public string FolderId { get; set; } = "";

        [JsonPropertyName(FOLDER_PATH_KEY)]
        public string? FolderPath { get; set; } = null;

        [JsonPropertyName(SOURCE_ID_KEY)]
        public string SourceId { get; set; } = "";

        [JsonPropertyName(SOURCE_TYPE_KEY)]
        public VectorSourceType SourceType { get; set; } = VectorSourceType.None;

        [JsonPropertyName(DESCRIPTION_KEY)]
        public string Description { get; set; } = "";

        [JsonPropertyName(CONTENT_KEY)]
        public string Content { get; set; } = "";

        [JsonPropertyName(SOURCE_PATH_KEY)]
        public string SourcePath { get; set; } = "";

        [JsonPropertyName(DOC_ID_KEY)]
        public string DocId { get; set; } = string.Empty;

        [JsonPropertyName(SCORE_KEY)]
        public double Score { get; set; } = 0.0;

        // sub_docs
        [JsonPropertyName(SUB_DOCS_KEY)]
        public List<EmbeddingResponse> SubDocs { get; set; } = [];

        public VectorEmbeddingItem CreateVectorEmbeddingItem() {
            VectorEmbeddingItem embedding = new(SourceId, FolderId);
            embedding.SourceType = SourceType;
            embedding.Description = Description;
            embedding.Content = Content;
            embedding.SourcePath = SourcePath;
            embedding.FolderPath = FolderPath;
            embedding.DocId = DocId;
            embedding.Score = Score;
            if (SubDocs.Count > 0) {
                foreach (var subDoc in SubDocs) {
                    embedding.SubDocs.Add(subDoc.CreateVectorEmbeddingItem());
                }
            }
            return embedding;
        }

        public static List<EmbeddingResponse> FromJson(string json) {
            JsonSerializerOptions options = Options;
            List<EmbeddingResponse>? result = JsonSerializer.Deserialize<List<EmbeddingResponse>>(json, options);
            return result ?? [];
        }

    }
}
