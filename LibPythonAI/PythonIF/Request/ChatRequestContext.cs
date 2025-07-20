using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using LibPythonAI.Common;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.VectorDB;

namespace LibPythonAI.PythonIF.Request {
    // リクエストと共に送信するコンテキスト情報
    public class ChatRequestContext {

        public ChatRequestContext(ChatSettings chatSettings) {
            ChatSettings = chatSettings;
        }
        public ChatSettings ChatSettings { get; private set; }

        // ベクトル検索

        [JsonPropertyName(ChatSettings.VECTOR_SEARCH_REQUESTS_KEY)]
        public List<VectorSearchRequest> VectorSearchRequests { get => ChatSettings.VectorSearchRequests; }

        // リクエストを分割するトークン数
        [JsonPropertyName(ChatSettings.SPLIT_TOKEN_COUNT_KEY)]
        public int SplitTokenCount { get => ChatSettings.SplitTokenCount; }

        // PromptTemplateText
        [JsonPropertyName(ChatSettings.PROMPT_TEMPLATE_TEXT_KEY)]
        public string PromptTemplateText { get => ChatSettings.PromptTemplateText; }

        // RAGを使用するかどうか
        [JsonPropertyName(ChatSettings.RAG_MODE_KEY)]
        public RAGModeEnum RAGMode { get => ChatSettings.RAGMode; }

        // RAGを使用する場合のプロンプト
        [JsonPropertyName(ChatSettings.RAG_MODE_PROMPT_TEXT_KEY)]
        public string RagModePromptText { get => ChatSettings.RagModePromptText; }


        public SplitModeEnum SplitMode { get => ChatSettings.SplitMode; }

        public string SummarizePromptText { get => ChatSettings.SummarizePromptText; }

        public Dictionary<string, object> ToChatRequestContextDict() {
            Dictionary<string, object> requestContext = new() {
                { ChatSettings.SPLIT_MODE_KEY, SplitMode.ToString() },
                { ChatSettings.RAG_MODE_KEY, RAGMode.ToString() },
            };
            if (RAGMode != RAGModeEnum.None) {
                requestContext[ChatSettings.RAG_MODE_PROMPT_TEXT_KEY] = RagModePromptText;
            }
            if (SplitMode != SplitModeEnum.None) {
                requestContext[ChatSettings.PROMPT_TEMPLATE_TEXT_KEY] = PromptTemplateText;
                requestContext[ChatSettings.SUMMARIZE_PROMPT_TEXT_KEY] = SummarizePromptText;
                requestContext[ChatSettings.SPLIT_TOKEN_COUNT_KEY] = SplitTokenCount;
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
                ObservableCollection<VectorSearchItem> vectorSearchItems, string promptTemplateText
            ) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            ChatSettings chatSettings = new() {
                PromptTemplateText = promptTemplateText,
                RAGMode = ragModeEnum,
                SplitMode = splitMode,
                SplitTokenCount = split_token_count,
                VectorSearchRequests = vectorSearchItems.Select(x => new VectorSearchRequest(x)).ToList(),
            };

            ChatRequestContext chatRequestContext = new(chatSettings);
            return chatRequestContext;
        }

    }
}
