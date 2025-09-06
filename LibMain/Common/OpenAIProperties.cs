using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace LibMain.Common {
    public class OpenAIProperties {

 
        public string OpenAIKey { get; set; } = "";

        public string OpenAICompletionModel { get; set; } = "";

        public string OpenAIEmbeddingModel { get; set; } = "";

        public bool AzureOpenAI { get; set; } = false;

        public string AzureOpenAIEndpoint { get; set; } = "";

        public string OpenAIBaseURL { get; set; } = "";

        public string AzureOpenAIAPIVersion { get; set; } = "";


    }
}
