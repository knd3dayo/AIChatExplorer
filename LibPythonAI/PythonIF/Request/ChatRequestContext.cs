using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibPythonAI.Model.VectorDB;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.Chat;

namespace LibPythonAI.PythonIF.Request {
    // リクエストと共に送信するコンテキスト情報
    public class ChatRequestContext {

        private static readonly JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };

        // ベクトル検索

        [JsonPropertyName("vector_search_requests")]
        public ObservableCollection<VectorSearchProperty> VectorSearchRequests { get; set; } = [];

        // AutoGenProperties
        [JsonPropertyName("autogen_props")]
        public AutoGenProperties AutoGenProperties { get; set; } = new AutoGenProperties();

        // OpenAIProperties
        [JsonPropertyName("openai_props")]
        public OpenAIProperties OpenAIProperties { get; set; } = new OpenAIProperties();

        // リクエストを分割するトークン数
        [JsonPropertyName("split_token_count")]
        public int SplitTokenCount { get; set; } = 8000;

        // PromptTemplateText
        [JsonPropertyName("prompt_template_text")]
        public string PromptTemplateText { get; set; } = "";

        // ベクトルDBを使用するかどうか
        [JsonIgnore]
        public bool UseVectorDB { get; set; } = false;


        public SplitOnTokenLimitExceedModeEnum SplitMode = SplitOnTokenLimitExceedModeEnum.None;

        public string SummarizePromptText = PythonAILib.Resources.PromptStringResource.Instance.SummarizePromptText;

        public string RelatedInformationPromptText = PythonAILib.Resources.PromptStringResource.Instance.RelatedInformationByVectorSearch;

        public Dictionary<string, object> ToChatRequestContextDict() {
            Dictionary<string, object> requestContext = new() {
                { "split_mode", SplitMode.ToString() },
            };

            if (SplitMode != SplitOnTokenLimitExceedModeEnum.None) {
                requestContext["prompt_template_text"] = PromptTemplateText;
                requestContext["summarize_prompt_text"] = SummarizePromptText;
                requestContext["related_information_prompt_text"] = RelatedInformationPromptText;
                requestContext["split_token_count"] = SplitTokenCount;
            }
            return requestContext;

        }

        // CreateEntriesDictList
        public List<Dictionary<string, object>> ToDictVectorDBItemsDict() {
            return UseVectorDB ? VectorSearchProperty.ToDictList(VectorSearchRequests) : [];
        }

       

        // CreateDefaultChatRequestContext 
        public static ChatRequestContext CreateDefaultChatRequestContext(
                OpenAIExecutionModeEnum chatMode, SplitOnTokenLimitExceedModeEnum splitMode , int split_token_count, bool userVectorDB,
                ObservableCollection<VectorSearchProperty> vectorSearchRequests, AutoGenProperties? autoGenProperties, string promptTemplateText
            ) {
            PythonAILibManager libManager = PythonAILibManager.Instance;

            ChatRequestContext chatRequestContext = new() {
                VectorSearchRequests = vectorSearchRequests,
                OpenAIProperties = libManager.ConfigParams.GetOpenAIProperties(),
                PromptTemplateText = promptTemplateText,
                UseVectorDB = userVectorDB,
                SplitMode = splitMode,
                SplitTokenCount = split_token_count,
            };
            if (autoGenProperties != null) {
                chatRequestContext.AutoGenProperties = autoGenProperties;
            }

            return chatRequestContext;
        }

    }
}
