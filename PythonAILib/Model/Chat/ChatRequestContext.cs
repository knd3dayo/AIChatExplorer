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
        public List<VectorSearchProperty> VectorSearchProperties { get; set; } = [];

        // AutoGenProperties
        [JsonPropertyName("autogen_props")]
        public AutoGenProperties AutoGenProperties { get; set; } = new AutoGenProperties();

        // OpenAIProperties
        [JsonPropertyName("openai_props")]
        public OpenAIProperties OpenAIProperties { get; set; } = new OpenAIProperties();

        // PromptTemplateText
        [JsonPropertyName("prompt_template_text")]
        public string PromptTemplateText { get; set; } = "";


        public OpenAIExecutionModeEnum ChatMode = OpenAIExecutionModeEnum.Normal;

        public SplitOnTokenLimitExceedModeEnum SplitMode = SplitOnTokenLimitExceedModeEnum.None;

        public string SummarizePromptText = PythonAILib.Resource.PromptStringResource.Instance.SummarizePromptText;

        public string RelatedInformationPromptText = PythonAILib.Resource.PromptStringResource.Instance.RelatedInformationByVectorSearch;

        // ToDictList
        public Dictionary<string, object> ToDict() {
            // RequestContext
            Dictionary<string, object> requestContext = new() {
                { "prompt_template_text", PromptTemplateText },
                { "chat_mode", ChatMode.ToString() },
                { "split_mode", SplitMode.ToString() },
                { "summarize_prompt_text", SummarizePromptText },
                { "related_information_prompt_text", RelatedInformationPromptText },
            };
            Dictionary<string, object> dict = new() {
                { "vector_db_items", VectorSearchProperty.ToDictList(VectorSearchProperties) },
                { "autogen_props", AutoGenProperties.ToDict() },
                { "openai_props", OpenAIProperties.ToDict() },
                { "request_context", requestContext },
            };
            return dict;
        }
        // ToJson
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }

       

        // CreateDefaultChatRequestContext 
        public static ChatRequestContext CreateDefaultChatRequestContext(
                OpenAIExecutionModeEnum chatMode, SplitOnTokenLimitExceedModeEnum splitMode ,List<VectorSearchProperty> vectorSearchProperties, AutoGenGroupChat? groupChat, string promptTemplateText
            ) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            AutoGenProperties autoGenProperties;

            if (groupChat != null) {
                autoGenProperties = new AutoGenProperties() {
                    WorkDir = libManager.ConfigParams.GetAutoGenWorkDir(),
                    AutoGenGroupChat = groupChat,
                };
            } else {
                autoGenProperties = new AutoGenProperties() {
                    WorkDir = libManager.ConfigParams.GetAutoGenWorkDir(),
                };
            }

            ChatRequestContext chatRequestContext = new() {
                VectorSearchProperties = vectorSearchProperties,
                OpenAIProperties = libManager.ConfigParams.GetOpenAIProperties(),
                AutoGenProperties = autoGenProperties,
                PromptTemplateText = promptTemplateText,
                ChatMode = chatMode,
                SplitMode = splitMode,
            };

            return chatRequestContext;
        }

    }
}
