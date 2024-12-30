using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace PythonAILib.Model.VectorDB {
    public class VectorDBEntry {

        public static readonly JsonSerializerOptions Options = new() {
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            IncludeFields = true,
            WriteIndented = true
        };

        public VectorDBEntry() { }

        public VectorDBEntry(string source_id) {
            SourceId = source_id;
        }

        [JsonPropertyName("source_id")]
        public string SourceId { get; set; } = "";

        [JsonPropertyName("source_type")]
        public VectorSourceType SourceType { get; set; } = VectorSourceType.None;

        [JsonPropertyName("description")]

        public string Description { get; set; } = "";
        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("source_path")]
        public string SourcePath { get; set; } = "";

        [JsonPropertyName("git_repository_url")]
        public string GitRepositoryUrl { get; set; } = "";

        [JsonPropertyName("git_relative_path")]
        public string GitRelativePath { get; set; } = "";

        [JsonPropertyName("image_url")]
        public string ImageURL { get; set; } = "";

        // doc_id
        [JsonPropertyName("doc_id")]
        public string DocId { get; set; } = string.Empty;

        // Score
        [JsonPropertyName("score")]
        public double Score { get; set; } = 0.0;

        // sub_docs
        [JsonPropertyName("sub_docs")]
        public List<VectorDBEntry> SubDocs { get; set; } = [];


        public void UpdateSourceInfo(string description, string content, VectorSourceType sourceType, string source_path, string git_repository_url, string git_relative_path, string image_url) {
            Description = description;
            Content = content;
            SourceType = sourceType;
            SourcePath = source_path;
            GitRepositoryUrl = git_repository_url;
            GitRelativePath = git_relative_path;
            ImageURL = image_url;

        }

        public string ToJson() {
            return JsonSerializer.Serialize(this, Options);
        }

        public static List<VectorDBEntry> FromJson(string json) {
            JsonSerializerOptions options = Options;
            List<VectorDBEntry>? result = System.Text.Json.JsonSerializer.Deserialize<List<VectorDBEntry>>(json, options);
            return result ?? [];
        }


    }
}
