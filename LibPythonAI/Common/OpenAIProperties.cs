using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace PythonAILib.Common {
    public class OpenAIProperties {

        [JsonPropertyName("OpenAIKey")]
        public string OpenAIKey { get; set; } = "";
        [JsonPropertyName("OpenAICompletionModel")]
        public string OpenAICompletionModel { get; set; } = "";

        [JsonPropertyName("OpenAIEmbeddingModel")]
        public string OpenAIEmbeddingModel { get; set; } = "";

        [JsonPropertyName("AzureOpenAI")]
        public bool AzureOpenAI { get; set; } = false;

        [JsonPropertyName("AzureOpenAIEndpoint")]
        public string AzureOpenAIEndpoint { get; set; } = "";

        [JsonPropertyName("OpenAIBaseURL")]
        public string OpenAIBaseURL { get; set; } = "";

        // AzureOpenAIAPIVersion
        [JsonPropertyName("AzureOpenAIAPIVersion")]
        public string AzureOpenAIAPIVersion { get; set; } = "";

        // ToDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "OpenAIKey", OpenAIKey },
                { "OpenAICompletionModel", OpenAICompletionModel },
                { "OpenAIEmbeddingModel", OpenAIEmbeddingModel },
                { "AzureOpenAI", AzureOpenAI },
                { "AzureOpenAIEndpoint", AzureOpenAIEndpoint },
                { "OpenAIBaseURL", OpenAIBaseURL },
                { "AzureOpenAIAPIVersion", AzureOpenAIAPIVersion }
            };
            return dict;
        }

        public string ToJson() {
            // option
            var options = new JsonSerializerOptions {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Serialize(this, options);
        }
    }
}
