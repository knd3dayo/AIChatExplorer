using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Model.Chat {
    // リクエストと共に送信するコンテキスト情報
    public class ChatRequestContext {

        public static JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };
        // ベクトルDBアイテム

        [JsonPropertyName("vector_db_items")]
        public List<VectorDBProperty> VectorDBProperties { get; set; } = [];

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

        public OpenAIExecutionModeEnum ChatMode = OpenAIExecutionModeEnum.Normal;

        public SplitOnTokenLimitExceedModeEnum SplitMode = SplitOnTokenLimitExceedModeEnum.None;

        public string SummarizePromptText = PythonAILib.Resources.PromptStringResource.Instance.SummarizePromptText;

        public string RelatedInformationPromptText = PythonAILib.Resources.PromptStringResource.Instance.RelatedInformationByVectorSearch;

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            // RequestContext
            Dictionary<string, object> requestContext = new() {
                { "prompt_template_text", PromptTemplateText },
                { "chat_mode", ChatMode.ToString() },
                { "split_mode", SplitMode.ToString() },
                { "summarize_prompt_text", SummarizePromptText },
                { "related_information_prompt_text", RelatedInformationPromptText },
                { "split_token_count", SplitTokenCount },

            };
            Dictionary<string, object> dict = new() {
                // VectorSearchProperty UseVectorDBがTrueの場合は追加、Falseの場合は空リスト
                { "vector_db_items", UseVectorDB ? VectorDBProperty.ToDictList(VectorDBProperties) : [] },
                { "autogen_props", AutoGenProperties.ToDict() },
                { "openai_props", OpenAIProperties.ToDict() },
                { "chat_request_context", requestContext },
            };
            return dict;
        }
        // ToJson
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }

       

        // CreateDefaultChatRequestContext 
        public static ChatRequestContext CreateDefaultChatRequestContext(
                OpenAIExecutionModeEnum chatMode, SplitOnTokenLimitExceedModeEnum splitMode , int split_token_count, bool userVectorDB,  List<VectorDBProperty> vectorSearchProperties, AutoGenProperties? autoGenProperties, string promptTemplateText
            ) {
            PythonAILibManager libManager = PythonAILibManager.Instance;

            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = vectorSearchProperties,
                OpenAIProperties = libManager.ConfigParams.GetOpenAIProperties(),
                PromptTemplateText = promptTemplateText,
                ChatMode = chatMode,
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
