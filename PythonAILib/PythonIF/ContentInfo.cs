using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace PythonAILib.PythonIF {
    public partial interface IPythonFunctions {
        public class ContentInfo(VectorDBUpdateMode mode, string id, string content) {

            [JsonPropertyName("Id")]
            public string Id { get; set; } = id;
            [JsonPropertyName("Content")]
            public string Content { get; set; } = content;
            [JsonPropertyName("Mode")]
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
}
