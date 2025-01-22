using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Common;
using PythonAILib.Resource;
using PythonAILib.Utils.Python;

namespace PythonAILib.Model.Chat {
    /// <summary>
    /// ChatItemの履歴、
    /// </summary>
    public class ChatRequest {
        public ChatRequest() {
            OpenAIProperties = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties();
        }
        protected OpenAIProperties OpenAIProperties { get; private set; }

        // temperature
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.5;
        // max_tokens
        [JsonPropertyName("max_tokens")]
        protected int MaxTokens { get; set; } = 0;



        // SummaryOnMaxTokenExceed
        public bool SplitWhenMaxTokenReached { get; set; } = false;

        public List<ChatMessage> ChatHistory { get; set; } = [];

        public string ContentText { get; set; } = "";

        // ImageのURLのリスト. data:image/{formatText};base64,{base64String}という形式の文字列のリスト
        public List<string> ImageURLs { get; set; } = [];

        public bool JsonMode { get; set; } = false;



        // response_format
        [JsonPropertyName("response_format")]
        protected Dictionary<string, string> ResponseFormat {
            get {
                if (JsonMode) {
                    return new Dictionary<string, string> {
                        ["type"] = "json_object"
                    };
                }
                return new Dictionary<string, string>();
            }
        }

        [JsonPropertyName("model")]
        protected string Model {
            get {
                return OpenAIProperties.OpenAICompletionModel;
            }
        }

        [JsonPropertyName("messages")]
        protected List<Dictionary<string, object>> Messages {
            get {
                return ChatMessage.ToDictList(ChatHistory);
            }
        }

        public ChatRequest Copy() {
            ChatRequest chatRequest = new() {
                ContentText = ContentText,
                Temperature = Temperature,
                JsonMode = JsonMode,
                MaxTokens = MaxTokens,
                ChatHistory = ChatHistory.Select(x => x.Copy()).ToList(),
                ImageURLs = ImageURLs.ToList()
            };
            return chatRequest;
        }

        public ChatMessage? GetLastSendItem() {
            // ChatItemsのうち、ユーザー発言の最後のものを取得
            var lastUserChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatMessage.UserRole);
            return lastUserChatItem;
        }

        public ChatMessage? GetLastResponseItem() {
            // ChatItemsのうち、アシスタント発言の最後のものを取得
            var lastAssistantChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatMessage.AssistantRole);
            return lastAssistantChatItem;
        }

        public static void PrepareNormalRequest(ChatRequestContext chatRequestContext, ChatRequest chatRequest) {
            // SplitWhenMaxTokenReachedがFalseの場合は、ChatHistoryをクリアする
            if (chatRequest.SplitWhenMaxTokenReached == false) {
                chatRequest.ChatHistory.Clear();
            }
            // ChatHistoryのサイズが0か、最後のアイテムのRoleがAssistantRoleの場合は、ChatMessageを作成する.
            ChatMessage lastUserRoleMessage;
            if (chatRequest.ChatHistory.Count == 0 || chatRequest.ChatHistory.Last().Role == ChatMessage.AssistantRole) {
                lastUserRoleMessage = new ChatMessage(ChatMessage.UserRole, "");
                chatRequest.ChatHistory.Add(lastUserRoleMessage);
            } else {
                lastUserRoleMessage = chatRequest.ChatHistory.Last();
            }

            // PromptTextを作成
            string promptText = "";

            // PromptTemplateTextが空でない場合は、PromptTemplateTextを追加
            if (string.IsNullOrEmpty(chatRequestContext.PromptTemplateText) == false) {
                promptText = chatRequestContext.PromptTemplateText + PythonAILibStringResources.Instance.ContentHeader;
            }
            // ContentTextを追加
            promptText += chatRequest.ContentText;
            
            // ModeがRAGの場合は、ベクトル検索の結果を追加
            if (chatRequestContext.ChatMode == OpenAIExecutionModeEnum.OpenAIRAG) {
                promptText += ChatUtil.GenerateVectorSearchResult(chatRequestContext, chatRequest.ContentText);
            }
            // 最後のユーザー発言のContentにPromptTextを追加
            lastUserRoleMessage.Content = promptText;
            // ImageURLsが空でない場合は、lastUserRoleMessageにImageURLsを追加
            if (chatRequest.ImageURLs.Count > 0) {
                lastUserRoleMessage.ImageURLs = chatRequest.ImageURLs;
            }
        }

        public string GetMessages(ChatRequestContext chatRequestContext) {
            PrepareNormalRequest(chatRequestContext, this);
            // ChatHistoryのContentの文字列を連結して返す
            return string.Join("", ChatHistory.Select(x => x.Content));
        }

        public Dictionary<string, object> ToDict() {
            // OpenAIのAPIに送信するJSONを作成

            // ClipboardAppConfigの設定を取得
            // ChatModel
            string model = OpenAIProperties.OpenAICompletionModel;

            // model, messages, temperature, response_format, max_tokensを設定する.
            var dc = new Dictionary<string, object> {
                ["model"] = model,
                ["messages"] = ChatMessage.ToDictList(ChatHistory),
                ["temperature"] = Temperature
            };
            // jsonModeがTrueの場合は、response_formatを json_objectに設定する
            if (JsonMode) {
                Dictionary<string, string> responseFormat = new() {
                    ["type"] = "json_object"
                };
                dc["response_format"] = responseFormat;

            }
            // maxTokensが0より大きい場合は、max_tokensを設定する
            if (MaxTokens > 0) {
                dc["max_tokens"] = MaxTokens;
            }

            return dc;
        }

        public static List<Dictionary<string, object>> ToDictList(List<ChatRequest> requests) {
            return requests.Select(x => x.ToDict()).ToList();
        }

        // Chatを実行する
        public ChatResult? ExecuteChat(ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            if (chatRequestContext.ChatMode == OpenAIExecutionModeEnum.Normal || chatRequestContext.ChatMode == OpenAIExecutionModeEnum.OpenAIRAG) {
                // リクエストメッセージを最新化
                PrepareNormalRequest(chatRequestContext, this);
                // 通常のChatを実行する。
                ChatResult? result = ChatUtil.ExecuteChatNormal(chatRequestContext, this);
                if (result == null) {
                    return null;
                }
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                ChatHistory.Add(new ChatMessage(ChatMessage.AssistantRole, result.Output, result.PageSourceList));
                return result;
            }

            if (chatRequestContext.ChatMode == OpenAIExecutionModeEnum.AutoGenGroupChat) {
                // AutoGenGroupChatを実行する
                return ExecuteAutoGenGroupChat(chatRequestContext, iterateAction);
            }
            if (chatRequestContext.ChatMode == OpenAIExecutionModeEnum.AutoGenNormalChat) {
                // AutoGenGroupChatを実行する
                return ExecuteAutoGenNormalChat(chatRequestContext, iterateAction);
            }
            if (chatRequestContext.ChatMode == OpenAIExecutionModeEnum.AutoGenNestedChat) {
                // AutoGenGroupChatを実行する
                return ExecuteAutoGenNestedChat(chatRequestContext, iterateAction);

            }
            return null;
        }

        // AutoGenNormalChatを実行する
        public ChatResult? ExecuteAutoGenNormalChat(ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            // リクエストメッセージを最新化
            PrepareNormalRequest(chatRequestContext, this);
            // 結果
            ChatMessage result = new(ChatMessage.AssistantRole, "", []);
            ChatHistory.Add(result);

            // AutoGenGroupChatを実行する
            return ChatUtil.ExecuteAutoGenNormalChat(chatRequestContext, this, (message) => {
                result.Content += message;
                iterateAction(message);
            });
        }
        // AutoGenGroupChatを実行する
        public ChatResult? ExecuteAutoGenGroupChat(ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            // リクエストメッセージを最新化
            PrepareNormalRequest(chatRequestContext, this);
            // 結果
            ChatMessage result = new(ChatMessage.AssistantRole, "", []);
            ChatHistory.Add(result);

            // AutoGenGroupChatを実行する
            return ChatUtil.ExecuteAutoGenGroupChat(chatRequestContext, this, (message) => {
                result.Content += message;
                iterateAction(message);
            });
        }

        // AutoGenNestedChatを実行する
        public ChatResult? ExecuteAutoGenNestedChat(ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            // リクエストメッセージを最新化
            PrepareNormalRequest(chatRequestContext, this);
            // 結果
            ChatMessage result = new(ChatMessage.AssistantRole, "", []);
            ChatHistory.Add(result);

            // AutoGenGroupChatを実行する
            return ChatUtil.ExecuteAutoGenNestedChat(chatRequestContext, this, (message) => {
                result.Content += message;
                iterateAction(message);
            });
        }
    }
}
