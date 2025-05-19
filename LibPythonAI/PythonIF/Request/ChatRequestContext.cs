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

        public const string OPENAI_PROPS_KEY = "openai_props";
        public const string VECTOR_SEARCH_REQUESTS_KEY = "vector_search_requests";
        public const string AUTOGEN_PROPS_KEY = "autogen_props";
        public const string SPLIT_TOKEN_COUNT_KEY = "split_token_count";
        public const string PROMPT_TEMPLATE_TEXT_KEY = "prompt_template_text";
        public const string SPLIT_MODE_KEY = "split_mode";
        public const string SUMMARIZE_PROMPT_TEXT_KEY = "summarize_prompt_text";
        public const string RELATED_INFORMATION_PROMPT_TEXT_KEY = "related_information_prompt_text";
        public const string USE_VECTOR_DB_KEY = "use_vector_db";

        
        // OpenAIPropsRequest
        [JsonPropertyName(OPENAI_PROPS_KEY)]
        public OpenAIPropsRequest? OpenAIPropsRequest { get; set; } 

        // ベクトル検索

        [JsonPropertyName(VECTOR_SEARCH_REQUESTS_KEY)]
        public List<VectorSearchRequest> VectorSearchRequests { get; set; } = [];

        // AutoGenProperties
        [JsonPropertyName(AUTOGEN_PROPS_KEY)]
        public AutoGenProperties? AutoGenProperties { get; set; }

        // リクエストを分割するトークン数
        [JsonPropertyName(SPLIT_TOKEN_COUNT_KEY)]
        public int SplitTokenCount { get; set; } = 8000;

        // PromptTemplateText
        [JsonPropertyName(PROMPT_TEMPLATE_TEXT_KEY)]
        public string PromptTemplateText { get; set; } = "";

        // ベクトルDBを使用するかどうか
        [JsonIgnore]
        public bool UseVectorDB { get; set; } = false;


        public SplitOnTokenLimitExceedModeEnum SplitMode = SplitOnTokenLimitExceedModeEnum.None;

        public string SummarizePromptText = PythonAILib.Resources.PromptStringResource.Instance.SummarizePromptText;

        public string RelatedInformationPromptText = PythonAILib.Resources.PromptStringResource.Instance.RelatedInformationByVectorSearch;

        public Dictionary<string, object> ToChatRequestContextDict() {
            Dictionary<string, object> requestContext = new() {
                { SPLIT_MODE_KEY, SplitMode.ToString() },
            };

            if (SplitMode != SplitOnTokenLimitExceedModeEnum.None) {
                requestContext[PROMPT_TEMPLATE_TEXT_KEY] = PromptTemplateText;
                requestContext[SUMMARIZE_PROMPT_TEXT_KEY] = SummarizePromptText;
                requestContext[RELATED_INFORMATION_PROMPT_TEXT_KEY] = RelatedInformationPromptText;
                requestContext[SPLIT_TOKEN_COUNT_KEY] = SplitTokenCount;
            }
            return requestContext;

        }

        // CreateEntriesDictList
        public List<Dictionary<string, object>> ToDictVectorDBRequestDict() {
            return UseVectorDB ? VectorSearchRequest.ToDictList(VectorSearchRequests) : [];
        }

       

        // CreateDefaultChatRequestContext 
        public static ChatRequestContext CreateDefaultChatRequestContext(
                OpenAIExecutionModeEnum chatMode, SplitOnTokenLimitExceedModeEnum splitMode , int split_token_count, bool userVectorDB,
                ObservableCollection<VectorSearchItem> vectorSearchItems, AutoGenProperties? autoGenProperties, string promptTemplateText
            ) {
            PythonAILibManager libManager = PythonAILibManager.Instance;

            ChatRequestContext chatRequestContext = new() {
                VectorSearchRequests = vectorSearchItems.Select(x => new VectorSearchRequest(x)).ToList(),
                OpenAIPropsRequest = new OpenAIPropsRequest(libManager.ConfigParams.GetOpenAIProperties()),
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
