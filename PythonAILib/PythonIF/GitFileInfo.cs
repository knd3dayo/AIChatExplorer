using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace PythonAILib.PythonIF {
    public class GitFileInfo(VectorDBUpdateMode mode, string relativePath, string workDirectory, string repositoryURL, string description , int reliability) {
        [JsonPropertyName("mode")]
        public VectorDBUpdateMode Mode { get; set; } = mode;

        [JsonPropertyName("RelativePath")]
        public string RelativePath { get; set; } = relativePath;
        [JsonPropertyName("WorkDirectory")]
        public string WorkDirectory { get; set; } = workDirectory;
        [JsonPropertyName("RepositoryURL")]
        public string RepositoryURL { get; set; } = repositoryURL;
        // description
        [JsonPropertyName("description")]
        public string Description { get; set; } = description;
        // 文書の信頼度
        [JsonPropertyName("reliability")]
        public int Reliability { get; set; } = reliability;

        // 絶対パス
        public string AbsolutePath {
            get {
                return System.IO.Path.Combine(WorkDirectory, RelativePath);
            }
        }

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
