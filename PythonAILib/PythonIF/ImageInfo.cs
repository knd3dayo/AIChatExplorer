using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Model;

namespace PythonAILib.PythonIF {
    public partial interface IPythonFunctions {
        public class ImageInfo(VectorDBUpdateMode mode, string id, byte[] base64bytes) {
            [JsonPropertyName("Id")]
            public string Id { get; set; } = id;
            [JsonPropertyName("image_url")]
            public string ImageURL { get; set; } = ChatRequest.CreateImageURL(base64bytes);
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
