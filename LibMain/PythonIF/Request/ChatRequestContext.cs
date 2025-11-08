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
                { ChatSettings.PROMPT_TEMPLATE_TEXT_KEY, ChatSettings.PromptTemplateText }
            };

            if (ChatSettings.SplitMode != SplitModeEnum.None) {
                requestContext[ChatSettings.PROMPT_TEMPLATE_TEXT_KEY] = ChatSettings.PromptTemplateText;
                requestContext[ChatSettings.SUMMARIZE_PROMPT_TEXT_KEY] = ChatSettings.SummarizePromptText;
                requestContext[ChatSettings.SPLIT_TOKEN_COUNT_KEY] = ChatSettings.SplitTokenCount;
                requestContext[ChatSettings.MAX_IMAGES_PER_REQUEST_KEY] = ChatSettings.MaxImagesPerRequest;
            }

            return requestContext;

        }


        // CreateDefaultChatRequestContext 
        public static ChatRequestContext CreateDefaultChatRequestContext(
                OpenAIExecutionModeEnum chatMode, SplitModeEnum splitMode, int split_token_count, RAGModeEnum ragModeEnum,
                VectorSearchItem? vectorSearchItem, string promptTemplateText
            ) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            ChatSettings chatSettings = new() {
                PromptTemplateText = promptTemplateText,
                SplitMode = splitMode,
                SplitTokenCount = split_token_count,
            };

            ChatRequestContext chatRequestContext = new(chatSettings);
            return chatRequestContext;
        }


    }
}
