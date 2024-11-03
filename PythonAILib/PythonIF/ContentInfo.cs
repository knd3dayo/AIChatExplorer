using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.PythonIF;

namespace PythonAILib.PythonIF {
    public class ContentInfo(VectorDBUpdateMode mode, string id, string content, string description, int reliability) {

        [JsonPropertyName("id")]
        public string Id { get; set; } = id;

        [JsonPropertyName("content")]
        public string Content { get; set; } = content;

        [JsonPropertyName("description")]
        public string Description { get; set; } = description;

        [JsonPropertyName("mode")]
        public VectorDBUpdateMode Mode { get; set; } = mode;

        // 文書の信頼度
        [JsonPropertyName("reliability")]
        public int Reliability { get; set; } = reliability;

        public string ToJson() {
            var options = new JsonSerializerOptions {
                Converters = { new JsonStringEnumConverter() },
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, options);
        }
    }
}
