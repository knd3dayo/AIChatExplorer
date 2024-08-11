using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Model;

namespace PythonAILib.PythonIF {
    public class ImageInfo(VectorDBUpdateMode mode, string id, byte[] base64bytes) {
        [JsonPropertyName("id")]
        public string Id { get; set; } = id;

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("image_url")]
        public string ImageURL { get; set; } = ChatRequest.CreateImageURL(base64bytes);
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
