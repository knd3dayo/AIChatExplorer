using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace PythonAILib.Model {
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
        public string? AzureOpenAIEndpoint { get; set; } = null;

        [JsonPropertyName("OpenAICompletionBaseURL")]
        public string? OpenAICompletionBaseURL { get; set; } = null;

        [JsonPropertyName("OpenAIEmbeddingBaseURL")]
        public string? OpenAIEmbeddingBaseURL { get; set; } = null;

        [JsonPropertyName("VectorDBItems")]
        public List<VectorDBItemBase> VectorDBItems { get; set; } = [];


        public string ToJson() {
            // option
            var options = new JsonSerializerOptions {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Serialize(this);
        }
    }
}
