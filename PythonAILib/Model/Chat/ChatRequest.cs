using System.Text.Json.Serialization;
using PythonAILib.Common;
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
            // ChatHistoryのサイズが0か、最後のアイテムのRoleがAssistantRoleの場合は、ChatMessageを作成する.
            ChatMessage lastUserRoleMessage;
            if (chatRequest.ChatHistory.Count == 0 || chatRequest.ChatHistory.Last().Role == ChatMessage.AssistantRole) {
                lastUserRoleMessage = new ChatMessage(ChatMessage.UserRole, "");
                chatRequest.ChatHistory.Add(lastUserRoleMessage);
            } else {
                lastUserRoleMessage = chatRequest.ChatHistory.Last();
            }

            // PromptTextを作成
            string promptText = chatRequest.ContentText;

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
            // 通常のOpenAI Chatを実行する
            if (chatRequestContext.ChatMode == OpenAIExecutionModeEnum.Normal) {

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
            // AutoGenGroupChatを実行する
            if (chatRequestContext.ChatMode == OpenAIExecutionModeEnum.AutoGenGroupChat) {
                // AutoGenGroupChatを実行する
                return ExecuteAutoGenGroupChat(chatRequestContext, iterateAction);
            }
            return null;
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
                result.Content += message + "\n";
                iterateAction(message);
            });
        }
    }
}
