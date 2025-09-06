using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibMain.Model.VectorDB;
using LibMain.PythonIF.Request;

namespace LibMain.PythonIF.Response {
    public class VectorSearchResponse {


        public const string FOLDER_ID_KEY = EmbeddingRequest.FOLDER_ID_KEY;
        public const string FOLDER_PATH_KEY = EmbeddingRequest.FOLDER_PATH_KEY;
        public const string SOURCE_ID_KEY = EmbeddingRequest.SOURCE_ID_KEY;
        public const string SOURCE_TYPE_KEY = EmbeddingRequest.SOURCE_TYPE_KEY;
        public const string DESCRIPTION_KEY = EmbeddingRequest.DESCRIPTION_KEY;
        public const string SOURCE_PATH_KEY = EmbeddingRequest.SOURCE_PATH_KEY;

        public const string PAGE_CONTENT_KEY = "page_content";
        public const string DOC_ID_KEY = "doc_id";
        public const string SCORE_KEY = "score";
        public const string SUB_DOCS_KEY = "sub_docs";
        public const string METADATA_KEY = "metadata";

        public static readonly JsonSerializerOptions Options = new() {
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            IncludeFields = true,
            WriteIndented = true
        };
        public static VectorSearchResponse FromDict(Dictionary<string, object> dict) {
            VectorSearchResponse response = new();
            response.Content = dict.GetValueOrDefault(PAGE_CONTENT_KEY, "").ToString() ?? "";
            var metadata = dict.GetValueOrDefault(METADATA_KEY, new Dictionary<string, dynamic>());
            if (metadata is Dictionary<string, dynamic> metadataDict) {
                foreach (var kvp in metadataDict) {
                    if (kvp.Key == FOLDER_ID_KEY) {
                        response.FolderId = kvp.Value.ToString() ?? "";
                    } else if (kvp.Key == FOLDER_PATH_KEY) {
                        response.FolderPath = kvp.Value.ToString() ?? "";
                    } else if (kvp.Key == SOURCE_ID_KEY) {
                        response.SourceId = kvp.Value.ToString() ?? "";
                    } else if (kvp.Key == SOURCE_TYPE_KEY) {
                        response.SourceType = (VectorSourceType)Enum.Parse(typeof(VectorSourceType), kvp.Value.ToString() ?? "");
                    } else if (kvp.Key == DESCRIPTION_KEY) {
                        response.Description = kvp.Value.ToString() ?? "";
                    } else if (kvp.Key == SOURCE_PATH_KEY) {
                        response.SourcePath = kvp.Value.ToString() ?? "";
                    } else if (kvp.Key == DOC_ID_KEY) {
                        response.DocId = kvp.Value.ToString() ?? "";
                    } else if (kvp.Key == SCORE_KEY) {
                        response.Score = Convert.ToDouble(kvp.Value);
                    } else if (kvp.Key == "sub_docs") {
                        var type = kvp.Value.GetType().ToString();
                        if (kvp.Value is List<object> subDocs) {
                            foreach (var subDoc in subDocs) {
                                if (subDoc is Dictionary<string, object> subDocDict) {
                                    response.SubDocs.Add(VectorSearchResponse.FromDict(subDocDict));
                                }
                            }
                        }
                    }
                }
            }
            return response;
        }
        public string FolderId { get; set; } = "";

        public string FolderPath { get; set; } = "";

        public string SourceId { get; set; } = "";

        public VectorSourceType SourceType { get; set; } = VectorSourceType.None;

        public string Description { get; set; } = "";

        public string Content { get; set; } = "";
        public string SourcePath { get; set; } = "";
        public string DocId { get; set; } = string.Empty;

        public double Score { get; set; } = 0.0;

        // sub_docs
        public List<VectorSearchResponse> SubDocs { get; set; } = [];

        public VectorEmbeddingItem CreateVectorEmbeddingItem() {
            VectorEmbeddingItem embedding = new(SourceId, FolderPath);
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

        public static List<VectorSearchResponse> FromJson(string json) {
            JsonSerializerOptions options = Options;
            List<VectorSearchResponse>? result = JsonSerializer.Deserialize<List<VectorSearchResponse>>(json, options);
            return result ?? [];
        }

    }
}
