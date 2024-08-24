using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.PythonIF;

namespace PythonAILib.PythonIF {
    public class ContentInfo(VectorDBUpdateMode mode, string id, string content) {

        [JsonPropertyName("id")]
        public string Id { get; set; } = id;

        [JsonPropertyName("content")]
        public string Content { get; set; } = content;

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("mode")]
        public VectorDBUpdateMode Mode { get; set; } = mode;

        public string ToJson() {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, options);
        }
    }
}
