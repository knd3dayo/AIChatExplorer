using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resource;

namespace PythonAILib.Model.Chat {
    /// <summary>
    /// ChatItemの履歴、
    /// </summary>
    public class Chat {


        public OpenAIExecutionModeEnum ChatMode = OpenAIExecutionModeEnum.Normal;

        public List<ChatHistoryItem> ChatHistory { get; set; } = [];

        public string PromptTemplateText { get; set; } = "";
        public string ContentText { get; set; } = "";

        public List<string> ImageURLs { get; set; } = [];

        // リクエスト時の調整用のパラメーター
        public double Temperature { get; set; } = 0.5;
        public bool JsonMode { get; set; } = false;
        public int MaxTokens { get; set; } = 0;


        public List<ContentItem> AdditionalItems { get; set; } = [];

        public List<VectorDBItem> VectorDBItems { get; set; } = [];


        public ChatHistoryItem? GetLastSendItem() {
            // ChatItemsのうち、ユーザー発言の最後のものを取得
            var lastUserChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatHistoryItem.UserRole);
            return lastUserChatItem;
        }

        public ChatHistoryItem? GetLastResponseItem() {
            // ChatItemsのうち、アシスタント発言の最後のものを取得
            var lastAssistantChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatHistoryItem.AssistantRole);
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
                ["role"] = ChatHistoryItem.UserRole,
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

            //Encode設定
            var op = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            //jsonに変換する
            string json = JsonSerializer.Serialize(dc, op);
            return json;
        }

        // OpenAIChatを実行する。
        public ChatResult? ExecuteChat(OpenAIProperties openAIProperties) {
            if (ChatMode == OpenAIExecutionModeEnum.Normal) {
                // 通常のChatを実行する。
                return ExecuteChatNormal(openAIProperties);
            }
            if (ChatMode == OpenAIExecutionModeEnum.OpenAIRAG) {
                // ChatModeがOpenAIRAGの場合は、OpenAIRAGChatを実行する。
                return ExecuteChatOpenAIRAG(openAIProperties);
            }
            if (ChatMode == OpenAIExecutionModeEnum.LangChain) {
                // ChatModeがLangChainの場合は、LangChainChatを実行する。
                return ExecuteChatLangChain(openAIProperties);
            }
            return null;
        }
        private ChatResult? ExecuteChatLangChain(OpenAIProperties openAIProperties) {
            // ContentTextの内容でベクトル検索して、コンテキスト情報を生成する
            // GenerateVectorSearchResult(openAIProperties);

            openAIProperties.VectorDBItems.AddRange(VectorDBItems);
            ChatResult? result = PythonExecutor.PythonAIFunctions?.LangChainChat(openAIProperties, this);
            if (result == null) {
                return null;
            }
            // リクエストをChatItemsに追加
            ChatHistory.Add(new ChatHistoryItem(ChatHistoryItem.UserRole, CreatePromptText()));
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            ChatHistory.Add(new ChatHistoryItem(ChatHistoryItem.AssistantRole, result.Output, result.PageSourceList));
            return result;

        }

        private ChatResult? ExecuteChatNormal(OpenAIProperties openAIProperties) {
            ChatResult? result = PythonExecutor.PythonAIFunctions?.OpenAIChat(openAIProperties, this);
            // リクエストをChatItemsに追加
            if (result == null) {
                return null;
            }
            ChatHistory.Add(new ChatHistoryItem(ChatHistoryItem.UserRole, CreatePromptText()));
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            ChatHistory.Add(new ChatHistoryItem(ChatHistoryItem.AssistantRole, result.Output, result.PageSourceList));

            return result;
        }

        private ChatResult? ExecuteChatOpenAIRAG(OpenAIProperties openAIProperties) {
            // ContentTextの内容でベクトル検索して、コンテキスト情報を生成する
            GenerateVectorSearchResult(openAIProperties);

            ChatResult? result = PythonExecutor.PythonAIFunctions?.OpenAIChat(openAIProperties, this);
            // リクエストをChatItemsに追加
            if (result == null) {
                return null;
            }
            ChatHistory.Add(new ChatHistoryItem(ChatHistoryItem.UserRole, CreatePromptText()));
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            ChatHistory.Add(new ChatHistoryItem(ChatHistoryItem.AssistantRole, result.Output, result.PageSourceList));

            return result;

        }
        // VectorSearchを実行してコンテキスト情報を生成する
        private void GenerateVectorSearchResult(OpenAIProperties openAIProperties) {
            // ベクトル検索が存在するか否かのフラグ
            bool hasVectorSearch = false;
            // VectorSearchRequestを作成. テスト用にFilterを設定
            VectorSearchRequest request = new() {
                Query = ContentText,
                SearchKWArgs = new Dictionary<string, object> {
                    ["k"] = 4,
                    // filter
                    ["filter"] = new Dictionary<string, object> {
                        ["content_type"] = "text"
                    }
                }
            };

            StringBuilder sb = new();
            List<VectorSearchResult> results = PythonExecutor.PythonAIFunctions?.VectorSearch(openAIProperties, VectorDBItems, request) ?? [];
            sb.AppendLine();
            for (int i = 0; i < results.Count; i++) {
                VectorSearchResult vectorSearchResult = results[i];
                sb.AppendLine($"## 参考情報:{i + 1} ##");
                sb.AppendLine("--------");
                sb.AppendLine(vectorSearchResult.Description);
                sb.AppendLine($"** {PromptStringResource.Instance.DocumentReliability}: {vectorSearchResult.Reliability}% **");
                sb.AppendLine(vectorSearchResult.Content);
                hasVectorSearch = true;
            }

            if (hasVectorSearch) {
                // 結果をContentTextに追加
                string result = PromptStringResource.Instance.RelatedInformation;
                result += sb.ToString();
                // 結果をContentTextに追加
                ContentText += result;
            }
        }

    }
}
