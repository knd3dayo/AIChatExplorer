using System.Text.Json.Serialization;

namespace PythonAILib.PythonIF {
    public class ContentInfo(VectorDBUpdateMode mode, string id, string content, string description, int reliability) : UpdateVectorDBInfo(mode, id, description, reliability) {

        [JsonPropertyName("content")]
        public string Content { get; set; } = content;

    }
}
