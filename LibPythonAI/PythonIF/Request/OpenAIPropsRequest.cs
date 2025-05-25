using System.Text.Json.Serialization;
using PythonAILib.Common;

namespace LibPythonAI.PythonIF.Request {
    public class OpenAIPropsRequest {
        public const string OPEN_AI_KEY_KEY = "openai_key";
        public const string OPEN_AI_COMPLETION_MODEL_KEY = "openai_completion_model";
        public const string OPEN_AI_EMBEDDING_MODEL_KEY = "openai_embedding_model";
        public const string AZURE_OPENAI_KEY = "azure_openai";
        public const string AZURE_OPENAI_ENDPOINT_KEY = "azure_openai_endpoint";
        public const string OPENAI_BASE_URL_KEY = "openai_base_url";
        public const string AZURE_OPENAI_API_VERSION_KEY = "azure_openai_api_version";

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
