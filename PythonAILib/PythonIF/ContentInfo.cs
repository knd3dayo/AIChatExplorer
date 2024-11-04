using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace PythonAILib.PythonIF {
    public class ContentInfo(VectorDBUpdateMode mode, string id, string content, string description, int reliability) : UpdateVectorDBInfo(mode, id, description, reliability) {

        [JsonPropertyName("content")]
        public string Content { get; set; } = content;
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
