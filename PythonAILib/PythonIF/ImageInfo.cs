using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Model;

namespace PythonAILib.PythonIF {
    public class ImageInfo(VectorDBUpdateMode mode, string id, string content, string base64String, string description, int reliability) 
        : UpdateVectorDBInfo(mode, id, description, reliability) {
        // = ChatRequest.CreateImageURL(base64String);

        [JsonPropertyName("content")]
        public string Content { get; set; } = content;

        [JsonPropertyName("image_url")]
        public string ImageURL { get; set; } = base64String;

    }
}
