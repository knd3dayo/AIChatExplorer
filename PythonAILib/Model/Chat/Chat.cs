using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resource;
using PythonAILib.Utils.Python;
using QAChat;

namespace PythonAILib.Model.Chat {
    /// <summary>
    /// ChatItemの履歴、
    /// </summary>
    public class Chat{

        public Chat() {
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


        public List<ContentItem> AdditionalItems { get; set; } = [];

        public List<VectorDBItem> VectorDBItems { get; set; } = [];


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

            // ContextItemsが空でない場合は、ContextItemsのContentを追加
            if (AdditionalItems.Any()) {
                promptText += PythonAILibStringResources.Instance.SourcesHeader;
                // ContextItemsのContentを追加
                foreach (ContentItem item in AdditionalItems) {
                    promptText += item.Content + "\n";
                }
            }
            return promptText;
        }

        public static string CreateImageURLFromFilePath(string filePath) {
            // filePathから画像のBase64文字列を作成
            byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
            string result = CreateImageURL(imageBytes);
            return result;
        }

        public static string CreateImageURL(string base64String) {

            ContentTypes.ImageType imageType = ContentTypes.GetImageTypeFromBase64(base64String);
            if (imageType == ContentTypes.ImageType.unknown) {
                return "";
            }
            string formatText = imageType.ToString();

            // Base64文字列から画像のURLを作成
            string result = $"data:image/{formatText};base64,{base64String}";
            return result;
        }

        private static string CreateImageURL(byte[] imageBytes) {
            // filePathから画像のBase64文字列を作成
            string base64String = Convert.ToBase64String(imageBytes);
            string result = CreateImageURL(base64String);
            return result;
        }

        private static List<Dictionary<string, object>> CreateOpenAIContentList(string content, List<string> imageURLs) {

            //OpenAIのリクエストパラメーターのContent部分のデータを作成
            List<Dictionary<string, object>> parameters = [];
            // Contentを作成
            var dc = new Dictionary<string, object> {
                ["type"] = "text",
                ["text"] = content
            };
            parameters.Add(dc);

            foreach (var imageURL in imageURLs) {
                // ImageURLプロパティを追加
                dc = new Dictionary<string, object> {
                    ["type"] = "image_url",
                    ["image_url"] = new Dictionary<string, object> {
                        ["url"] = imageURL
                    }
                };
                parameters.Add(dc);
            }
            return parameters;
        }

        private List<Dictionary<string, object>> CreateOpenAIMessagesList() {
            //OpenAIのリクエストパラメーターのMessage部分のデータを作成
            // Messages部分はRoleとContentからなるDictionaryのリスト
            List<Dictionary<string, object>> messages = [];
            foreach (var item in ChatHistory) {
                var itemDict = new Dictionary<string, object> {
                    ["role"] = item.Role,
                    ["content"] = CreateOpenAIContentList(item.Content, item.ImageURLs)
                };
                messages.Add(itemDict);
            }
            // このオブジェクトのプロパティを基にしたContentを作成
            // ImageURLとAdditionalImageURLsを結合したリストを作成
            List<string> additionalImageURLs = [];
            foreach (ContentItem item in AdditionalItems) {
                if (item.IsImage()) {
                    string? base64String = item.Base64String;
                    if (base64String == null) {
                        continue;
                    }
                    additionalImageURLs.Add(CreateImageURL(base64String));
                }
            }

            List<string> imageURLs = [.. ImageURLs, .. additionalImageURLs];

            var dc = new Dictionary<string, object> {
                ["role"] = ChatContentItem.UserRole,
                ["content"] = CreateOpenAIContentList(CreatePromptText(), imageURLs)
            };
            messages.Add(dc);
            return messages;
        }

        public string CreateOpenAIRequestJSON(OpenAIProperties openAIProperties) {
            // OpenAIのAPIに送信するJSONを作成

            // ClipboardAppConfigの設定を取得
            // ChatModel
            string model = openAIProperties.OpenAICompletionModel;

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


            //jsonに変換する
            string json = JsonSerializer.Serialize(dc, JsonSerializerOptions);
            return json;
        }

        // Chatを実行する
        public ChatResult? ExecuteChat(OpenAIProperties openAIProperties) {
            return ChatUtil.ExecuteChat(openAIProperties, this);
        }

    }
}
