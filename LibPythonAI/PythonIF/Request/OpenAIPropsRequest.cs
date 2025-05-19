using System.Text.Json.Serialization;
using PythonAILib.Common;

namespace LibPythonAI.PythonIF.Request {
    public class OpenAIPropsRequest {
        public const string OPEN_AI_KEY_KEY = "OpenAIKey";
        public const string OPEN_AI_COMPLETION_MODEL_KEY = "OpenAICompletionModel";
        public const string OPEN_AI_EMBEDDING_MODEL_KEY = "OpenAIEmbeddingModel";
        public const string AZURE_OPENAI_KEY = "AzureOpenAI";
        public const string AZURE_OPENAI_ENDPOINT_KEY = "AzureOpenAIEndpoint";
        public const string OPENAI_BASE_URL_KEY = "OpenAIBaseURL";
        public const string AZURE_OPENAI_API_VERSION_KEY = "AzureOpenAIAPIVersion";

        public OpenAIPropsRequest(OpenAIProperties openAIProperties) {
            OpenAIKey = openAIProperties.OpenAIKey;
            AzureOpenAI = openAIProperties.AzureOpenAI;
            AzureOpenAIEndpoint = openAIProperties.AzureOpenAIEndpoint;
            OpenAIBaseURL = openAIProperties.OpenAIBaseURL;
            AzureOpenAIAPIVersion = openAIProperties.AzureOpenAIAPIVersion;

        }

        [JsonPropertyName(OPEN_AI_KEY_KEY)]
        public string OpenAIKey { get; set; } = "";

        [JsonPropertyName(AZURE_OPENAI_KEY)]
        public bool AzureOpenAI { get; set; } = false;

        [JsonPropertyName(AZURE_OPENAI_ENDPOINT_KEY)]
        public string AzureOpenAIEndpoint { get; set; } = "";

        [JsonPropertyName(OPENAI_BASE_URL_KEY)]
        public string OpenAIBaseURL { get; set; } = "";

        [JsonPropertyName(AZURE_OPENAI_API_VERSION_KEY)]
        public string AzureOpenAIAPIVersion { get; set; } = "";

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { OPEN_AI_KEY_KEY, OpenAIKey },
                { AZURE_OPENAI_KEY, AzureOpenAI },
                { AZURE_OPENAI_ENDPOINT_KEY, AzureOpenAIEndpoint },
                { OPENAI_BASE_URL_KEY, OpenAIBaseURL },
                { AZURE_OPENAI_API_VERSION_KEY, AzureOpenAIAPIVersion }
            };
            return dict;
        }

    }
}
