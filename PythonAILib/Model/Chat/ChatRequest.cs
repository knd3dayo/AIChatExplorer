using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
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

        //Encode設定
        private static JsonSerializerOptions JsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        [JsonPropertyName("model")]
        protected string Model {
            get {
                return OpenAIProperties.OpenAICompletionModel;
            }
        }
        [JsonPropertyName("messages")]
        protected List<Dictionary<string, object>> Messages {
            get {
                return CreateOpenAIMessagesList();
            }
        }
        // temperature
        [JsonPropertyName("temperature")]
        protected double Temperature { get; set; } = 0.5;
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


        public OpenAIExecutionModeEnum ChatMode = OpenAIExecutionModeEnum.Normal;


        public List<ChatContentItem> ChatHistory { get; set; } = [];

        public string PromptTemplateText { get; set; } = "";
        public string ContentText { get; set; } = "";

        public List<string> ImageURLs { get; set; } = [];

        public bool JsonMode { get; set; } = false;



        public ChatContentItem? GetLastSendItem() {
            // ChatItemsのうち、ユーザー発言の最後のものを取得
            var lastUserChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatContentItem.UserRole);
            return lastUserChatItem;
        }

        public ChatContentItem? GetLastResponseItem() {
            // ChatItemsのうち、アシスタント発言の最後のものを取得
            var lastAssistantChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatContentItem.AssistantRole);
            return lastAssistantChatItem;
        }
        public string CreatePromptText() {
            // PromptTextを作成
            string promptText = "";

            // PromptTemplateTextが空でない場合は、PromptTemplateTextを追加
            if (string.IsNullOrEmpty(PromptTemplateText) == false) {
                promptText = PromptTemplateText + PythonAILibStringResources.Instance.ContentHeader;
            }
            // ContentTextを追加
            promptText += ContentText;

            return promptText;
        }
        public Dictionary<string, object> ToDict() {
            // OpenAIのAPIに送信するJSONを作成

            // ClipboardAppConfigの設定を取得
            // ChatModel
            string model = OpenAIProperties.OpenAICompletionModel;

            // model, messages, temperature, response_format, max_tokensを設定する.
            var dc = new Dictionary<string, object> {
                ["model"] = model,
                ["messages"] = CreateOpenAIMessagesList(),
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
        public ChatResult? ExecuteChat(ChatRequestContext chatRequestContext, Action<string> iterateAction ) {
            if (this.ChatMode == OpenAIExecutionModeEnum.Normal) {
                // リクエストをChatItemsに追加

                ChatHistory.Add(new ChatContentItem(ChatContentItem.UserRole, CreatePromptText()));
                // 通常のChatを実行する。
                ChatResult? result = ChatUtil.ExecuteChatNormal(chatRequestContext, this);
                if (result == null) {
                    return null;
                }
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                ChatHistory.Add(new ChatContentItem(ChatContentItem.AssistantRole, result.Output, result.PageSourceList));
                return result;
            }
            if (this.ChatMode == OpenAIExecutionModeEnum.OpenAIRAG) {
                // ContentTextの内容でベクトル検索して、コンテキスト情報を生成する
                ContentText += ChatUtil.GenerateVectorSearchResult(chatRequestContext, ContentText);
                ChatHistory.Add(new ChatContentItem(ChatContentItem.UserRole, ContentText));

                // ChatModeがOpenAIRAGの場合は、OpenAIRAGChatを実行する。
                ChatResult? result = ChatUtil.ExecuteChatOpenAIRAG(chatRequestContext, this);
                if (result == null) {
                    return null;
                }
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                ChatHistory.Add(new ChatContentItem(ChatContentItem.AssistantRole, result.Output, result.PageSourceList));
                return result;

            }
            if (this.ChatMode == OpenAIExecutionModeEnum.LangChain) {
                ChatHistory.Add(new ChatContentItem(ChatContentItem.UserRole, CreatePromptText()));

                // ChatModeがLangChainの場合は、LangChainChatを実行する。
                ChatResult? result = ChatUtil.ExecuteChatLangChain(chatRequestContext, this);
                if (result == null) {
                    return null;
                }
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                ChatHistory.Add(new ChatContentItem(ChatContentItem.AssistantRole, result.Output, result.PageSourceList));
                return result;
            }
            if (this.ChatMode == OpenAIExecutionModeEnum.AutoGenChatGroup) {
                // AutoGenGroupChatを実行する
                return ExecuteAutoGenGroupChat(chatRequestContext, iterateAction);
            }
            return null;
        }
        // AutoGenGroupChatを実行する
        public ChatResult? ExecuteAutoGenGroupChat(ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            ChatHistory.Add(new ChatContentItem(ChatContentItem.UserRole, CreatePromptText()));
            // 結果
            ChatContentItem result = new(ChatContentItem.AssistantRole, "", []);
            ChatHistory.Add(result);

            // AutoGenGroupChatを実行する
            return ChatUtil.ExecuteAutoGenGroupChat(chatRequestContext, this, (message) => {
                result.Content += message;
                iterateAction(message);
            });
        }

        private List<Dictionary<string, object>> CreateOpenAIMessagesList() {
            //OpenAIのリクエストパラメーターのMessage部分のデータを作成
            // Messages部分はRoleとContentからなるDictionaryのリスト
            List<Dictionary<string, object>> messages = [];
            foreach (var item in ChatHistory) {
                var itemDict = new Dictionary<string, object> {
                    ["role"] = item.Role,
                    ["content"] = ChatUtil.CreateOpenAIContentList(item.Content, item.ImageURLs)
                };
                messages.Add(itemDict);
            }
            // このオブジェクトのプロパティを基にしたContentを作成
            List<string> imageURLs = [.. ImageURLs];

            var dc = new Dictionary<string, object> {
                ["role"] = ChatContentItem.UserRole,
                ["content"] = ChatUtil.CreateOpenAIContentList(CreatePromptText(), imageURLs)
            };
            messages.Add(dc);
            return messages;
        }
    }
}
