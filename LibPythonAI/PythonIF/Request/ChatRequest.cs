using System.Text.Json.Serialization;
using LibPythonAI.Model.Chat;
using LibPythonAI.Utils.Python;
using PythonAILib.Common;

namespace LibPythonAI.PythonIF.Request {
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

        public string GetMessages(ChatRequestContext chatRequestContext) {
            ChatUtil.PrepareNormalRequest(chatRequestContext, this);
            // ChatHistoryのContentの文字列を連結して返す
            return string.Join("", ChatHistory.Select(x => x.Content));
        }

        public Dictionary<string, object> ToDict() {
            // OpenAIのAPIに送信するJSONを作成

            // ChatModel
            string model = OpenAIProperties.OpenAICompletionModel;

            // model, messages, temperature, response_format, max_tokensを設定する.
            var dc = new Dictionary<string, object> {
                ["model"] = model,
                ["messages"] = ChatMessage.ToDictList(ChatHistory),
            };
            // modelがo*以外の場合は、temperatureを設定する
            if (model.StartsWith("o") == false) {
                dc["temperature"] = Temperature;
            }
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

    }
}
