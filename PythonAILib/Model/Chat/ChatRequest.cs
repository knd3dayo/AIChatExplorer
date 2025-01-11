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

        [JsonPropertyName("model")]
        protected string Model {
            get {
                return OpenAIProperties.OpenAICompletionModel;
            }
        }
        // temperature
        [JsonPropertyName("temperature")]
        public double Temperature { get; set; } = 0.5;

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
        // max_tokens
        [JsonPropertyName("max_tokens")]
        protected int MaxTokens { get; set; } = 0;

        [JsonPropertyName("messages")]
        protected List<Dictionary<string, object>> Messages {
            get {
                return ChatMessage.ToDictList(ChatHistory);
            }
        }

        public OpenAIExecutionModeEnum ChatMode = OpenAIExecutionModeEnum.Normal;


        public List<ChatMessage> ChatHistory { get; set; } = [];

        public string PromptTemplateText { get; set; } = "";

        public string ContentText { get; set; } = "";

        // ImageのURLのリスト. data:image/{formatText};base64,{base64String}という形式の文字列のリスト
        public List<string> ImageURLs { get; set; } = [];

        public bool JsonMode { get; set; } = false;



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

        public void UpdateMessage() {
            // ChatHistoryのサイズが0か、最後のアイテムのRoleがAssistantRoleの場合は、ChatMessageを作成する.
            ChatMessage lastUserRoleMessage;
            if (ChatHistory.Count == 0 || ChatHistory.Last().Role == ChatMessage.AssistantRole) {
                lastUserRoleMessage = new ChatMessage(ChatMessage.UserRole, "");
                ChatHistory.Add(lastUserRoleMessage);
            } else {
                lastUserRoleMessage = ChatHistory.Last();
            }

            // PromptTextを作成
            string promptText = "";

            // PromptTemplateTextが空でない場合は、PromptTemplateTextを追加
            if (string.IsNullOrEmpty(PromptTemplateText) == false) {
                promptText = PromptTemplateText + PythonAILibStringResources.Instance.ContentHeader;
            }
            // ContentTextを追加
            promptText += ContentText;

            // 最後のユーザー発言のContentにPromptTextを追加
            lastUserRoleMessage.Content = promptText;
            // ImageURLsが空でない場合は、lastUserRoleMessageにImageURLsを追加
            if (ImageURLs.Count > 0) {
                lastUserRoleMessage.ImageURLs = ImageURLs;
            }
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

        // Chatを実行する
        public ChatResult? ExecuteChat(ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            if (this.ChatMode == OpenAIExecutionModeEnum.Normal) {
                // リクエストメッセージを最新化
                UpdateMessage();
                // 通常のChatを実行する。
                ChatResult? result = ChatUtil.ExecuteChatNormal(chatRequestContext, this);
                if (result == null) {
                    return null;
                }
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                ChatHistory.Add(new ChatMessage(ChatMessage.AssistantRole, result.Output, result.PageSourceList));
                return result;
            }
            if (this.ChatMode == OpenAIExecutionModeEnum.OpenAIRAG) {
                // リクエストメッセージを最新化
                UpdateMessage();
                // ContentTextの内容でベクトル検索して、コンテキスト情報を生成する
                ContentText += ChatUtil.GenerateVectorSearchResult(chatRequestContext, ContentText);
                // リクエストメッセージを最新化
                UpdateMessage();

                // ChatModeがOpenAIRAGの場合は、OpenAIRAGChatを実行する。
                ChatResult? result = ChatUtil.ExecuteChatOpenAIRAG(chatRequestContext, this);
                if (result == null) {
                    return null;
                }
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                ChatHistory.Add(new ChatMessage(ChatMessage.AssistantRole, result.Output, result.PageSourceList));
                return result;

            }

            if (this.ChatMode == OpenAIExecutionModeEnum.AutoGenGroupChat) {
                // AutoGenGroupChatを実行する
                return ExecuteAutoGenGroupChat(chatRequestContext, iterateAction);
            }
            if (this.ChatMode == OpenAIExecutionModeEnum.AutoGenNormalChat) {
                // AutoGenGroupChatを実行する
                return ExecuteAutoGenNormalChat(chatRequestContext, iterateAction);
            }
            if (this.ChatMode == OpenAIExecutionModeEnum.AutoGenNestedChat) {
                // AutoGenGroupChatを実行する
                return ExecuteAutoGenNestedChat(chatRequestContext, iterateAction);

            }
            return null;
        }

        // AutoGenNormalChatを実行する
        public ChatResult? ExecuteAutoGenNormalChat(ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            // リクエストメッセージを最新化
            UpdateMessage();
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
            UpdateMessage();
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
            UpdateMessage();
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
