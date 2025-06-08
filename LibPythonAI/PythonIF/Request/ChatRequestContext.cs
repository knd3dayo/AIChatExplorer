using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using LibPythonAI.Common;
using LibPythonAI.Model.AutoGen;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Resources;

namespace LibPythonAI.PythonIF.Request {
    // リクエストと共に送信するコンテキスト情報
    public class ChatRequestContext {

        public const string VECTOR_SEARCH_REQUESTS_KEY = "vector_search_requests";
        public const string AUTOGEN_PROPS_KEY = "autogen_props";
        public const string SPLIT_TOKEN_COUNT_KEY = "split_token_count";
        public const string PROMPT_TEMPLATE_TEXT_KEY = "prompt_template_text";
        public const string SPLIT_MODE_KEY = "split_mode";
        public const string SUMMARIZE_PROMPT_TEXT_KEY = "summarize_prompt_text";
        public const string RELATED_INFORMATION_PROMPT_TEXT_KEY = "related_information_prompt_text";
        public const string RAG_MODE_KEY = "rag_mode";
        public const string RAG_MODE_PROMPT_TEXT_KEY = "rag_mode_prompt_text";


        // ベクトル検索

        [JsonPropertyName(VECTOR_SEARCH_REQUESTS_KEY)]
        public List<VectorSearchRequest> VectorSearchRequests { get; set; } = [];

        // AutoGenPropsRequest
        [JsonPropertyName(AUTOGEN_PROPS_KEY)]
        public AutoGenPropsRequest? AutoGenPropsRequest { get; set; }

        // リクエストを分割するトークン数
        [JsonPropertyName(SPLIT_TOKEN_COUNT_KEY)]
        public int SplitTokenCount { get; set; } = 8000;

        // PromptTemplateText
        [JsonPropertyName(PROMPT_TEMPLATE_TEXT_KEY)]
        public string PromptTemplateText { get; set; } = "";

        // RAGを使用するかどうか
        [JsonPropertyName(RAG_MODE_KEY)]
        public RAGModeEnum RAGMode { get; set; } = RAGModeEnum.None;

        // RAGを使用する場合のプロンプト
        [JsonPropertyName(RAG_MODE_PROMPT_TEXT_KEY)]
        public string RagModePromptText { get; set; } = "";


        public SplitModeEnum SplitMode = SplitModeEnum.None;

        public string SummarizePromptText = PromptStringResource.Instance.SummarizePromptText;

        public Dictionary<string, object> ToChatRequestContextDict() {
            Dictionary<string, object> requestContext = new() {
                { SPLIT_MODE_KEY, SplitMode.ToString() },
                { RAG_MODE_KEY, RAGMode.ToString() },
            };
            if (RAGMode != RAGModeEnum.None) {
                requestContext[RAG_MODE_PROMPT_TEXT_KEY] = RagModePromptText;
            }
            if (SplitMode != SplitModeEnum.None) {
                requestContext[PROMPT_TEMPLATE_TEXT_KEY] = PromptTemplateText;
                requestContext[SUMMARIZE_PROMPT_TEXT_KEY] = SummarizePromptText;
                requestContext[SPLIT_TOKEN_COUNT_KEY] = SplitTokenCount;
            }
            return requestContext;

        }

        // CreateEntriesDictList
        public List<Dictionary<string, object>> ToDictVectorDBRequestDict() {
            return RAGMode != RAGModeEnum.None ? VectorSearchRequest.ToDictList(VectorSearchRequests) : [];
        }



        // CreateDefaultChatRequestContext 
        public static ChatRequestContext CreateDefaultChatRequestContext(
                OpenAIExecutionModeEnum chatMode, SplitModeEnum splitMode, int split_token_count, RAGModeEnum ragModeEnum,
                ObservableCollection<VectorSearchItem> vectorSearchItems, AutoGenProperties? autoGenProperties, string promptTemplateText
            ) {
            PythonAILibManager libManager = PythonAILibManager.Instance;

            ChatRequestContext chatRequestContext = new() {
                VectorSearchRequests = vectorSearchItems.Select(x => new VectorSearchRequest(x)).ToList(),
                PromptTemplateText = promptTemplateText,
                RAGMode = ragModeEnum,
                SplitMode = splitMode,
                SplitTokenCount = split_token_count,
            };
            if (autoGenProperties != null) {
                chatRequestContext.AutoGenPropsRequest = new AutoGenPropsRequest(autoGenProperties);
            }

            return chatRequestContext;
        }

    }
}
