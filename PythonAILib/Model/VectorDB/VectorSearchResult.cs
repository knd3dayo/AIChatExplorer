using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace PythonAILib.Model.VectorDB {
    public class VectorSearchResult {

        // Content
        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        // Source
        [JsonPropertyName("source")]
        public string Source { get; set; } = string.Empty;

        // SourceUrl
        [JsonPropertyName("source_url")]
        public string SourceUrl { get; set; } = string.Empty;

        public string ToJson() {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(this, options);
        }

        public static List<VectorSearchResult> FromJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            List<VectorSearchResult>? result = JsonSerializer.Deserialize<List<VectorSearchResult>>(json, options);
            return result ?? [];
        }

    }
}
