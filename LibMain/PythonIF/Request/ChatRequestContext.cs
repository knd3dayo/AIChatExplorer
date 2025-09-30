using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using LibMain.Common;
using LibMain.Model.Chat;
using LibMain.Model.VectorDB;

namespace LibMain.PythonIF.Request {
    // リクエストと共に送信するコンテキスト情報
    public class ChatRequestContext {

        public ChatRequestContext(ChatSettings chatSettings) {
            ChatSettings = chatSettings;
        }
        public ChatSettings ChatSettings { get; private set; }


        public Dictionary<string, object> ToChatRequestContextDict() {
            Dictionary<string, object> requestContext = new() {
                { ChatSettings.SPLIT_MODE_KEY, ChatSettings.SplitMode.ToString() },
                { ChatSettings.RAG_MODE_KEY, ChatSettings.RAGMode.ToString() },
                { ChatSettings.PROMPT_TEMPLATE_TEXT_KEY, ChatSettings.PromptTemplateText }
            };
            
            if (ChatSettings.RAGMode != RAGModeEnum.None) {
                requestContext[ChatSettings.RAG_MODE_PROMPT_TEXT_KEY] = ChatSettings.RagModePromptText;
            }
            if (ChatSettings.SplitMode != SplitModeEnum.None) {
                requestContext[ChatSettings.PROMPT_TEMPLATE_TEXT_KEY] = ChatSettings.PromptTemplateText;
                requestContext[ChatSettings.SUMMARIZE_PROMPT_TEXT_KEY] = ChatSettings.SummarizePromptText;
                requestContext[ChatSettings.SPLIT_TOKEN_COUNT_KEY] = ChatSettings.SplitTokenCount;
                requestContext[ChatSettings.MAX_IMAGES_PER_REQUEST_KEY] = ChatSettings.MaxImagesPerRequest;
            }

            if (ChatSettings.VectorSearchRequests.Count > 0) {
                requestContext[ChatSettings.VECTOR_SEARCH_REQUESTS_KEY] = ChatSettings.ToDictVectorDBRequestDict();
            }
            return requestContext;

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
