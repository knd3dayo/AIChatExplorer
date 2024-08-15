using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Model;

namespace PythonAILib.PythonIF {
    public class ImageInfo {
        // = ChatRequest.CreateImageURL(base64String);

        public ImageInfo(VectorDBUpdateMode mode, string id) {
            Mode = mode;
            Id = id;
        }
        public ImageInfo(VectorDBUpdateMode mode, string id, string base64String) {
            Mode = mode;
            Id = id;
            ImageURL = base64String;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; } 

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("image_url")]
        public string ImageURL { get; set; } = "";

        [JsonPropertyName("mode")]
        public VectorDBUpdateMode Mode { get; set; }

        public string ToJson() {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return JsonSerializer.Serialize(this, options);
        }
    }
}
