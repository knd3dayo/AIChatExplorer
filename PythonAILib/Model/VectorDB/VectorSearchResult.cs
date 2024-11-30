using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LiteDB;

namespace PythonAILib.Model.VectorDB {
    public class VectorSearchResult {
        // doc_id
        [JsonPropertyName("doc_id")]
        public string DocId { get; set; } = string.Empty;
        // FolderId
        [JsonPropertyName("folder_id")]
        public string FolderId { get; set; } = string.Empty;
        // SourcePath
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        // Source
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        // SourceUrl
        [JsonPropertyName("source_url")]
        public string SourceUrl { get; set; } = string.Empty;

        // Score
        [JsonPropertyName("score")]
        public double Score { get; set; } = 0.0;

        // Description
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        // Reliability
        [JsonPropertyName("reliability")]
        public int Reliability { get; set; } = 0;

        // sub_docs
        [JsonPropertyName("sub_docs")]
        public List<VectorSearchResult> SubDocs { get; set; } = [];

 
        public string ToJson() {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(this, options);
        }

        public static List<VectorSearchResult> FromJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            List<VectorSearchResult>? result = System.Text.Json.JsonSerializer.Deserialize<List<VectorSearchResult>>(json, options);
            return result ?? [];
        }

    }
}
