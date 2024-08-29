using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.PythonIF;

namespace PythonAILib.Model {
    /// <summary>
    /// ChatItemの履歴、
    /// </summary>
    public class ChatRequest {

        public ChatRequest(OpenAIProperties openAIProperties) {
            OpenAIProperties = openAIProperties;
        }

        public OpenAIExecutionModeEnum ChatMode = OpenAIExecutionModeEnum.Normal;

        public List<ChatItem> ChatHistory { get; set; } = [];

        public ChatItem? LastSendItem {
            get {
                // ChatItemsのうち、ユーザー発言の最後のものを取得
                var lastUserChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatItem.UserRole);
                return lastUserChatItem;
            }
        }
        public ChatItem? LastResponseItem {
            get {
                // ChatItemsのうち、アシスタント発言の最後のものを取得
                var lastAssistantChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatItem.AssistantRole);
                return lastAssistantChatItem;
            }
        }
        public OpenAIProperties OpenAIProperties { get; set; }

        public string PromptTemplateText { get; set; } = "";
        public string ContentText { get; set; } = "";

        public List<string> ImageURLs { get; set; } = [];

        // リクエスト時の調整用のパラメーター
        public double Temperature { get; set; } = 0.5;
        public bool JsonMode { get; set; } = false;
        public int MaxTokens { get; set; } = 0;


        public List<string> AdditionalTextItems { get; set; } = [];

        public List<string> AdditionalImageURLs { get; set; } = [];

        public List<VectorDBItemBase> VectorDBItems { get; set; } = [];

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
            if (AdditionalTextItems.Any()) {
                promptText += PythonAILibStringResources.Instance.SourcesHeader;
                // ContextItemsのContentを追加
                foreach (var item in AdditionalTextItems) {
                    promptText += item + "\n";
                }
            }
            return promptText;
        }

        public static string CreateImageURLFromFilePath(string filePath) {
            // filePathから画像のBase64文字列を作成
            byte[] imageBytes = File.ReadAllBytes(filePath);
            string result = CreateImageURL(imageBytes);
            return result;
        }

        public static string CreateImageURL(byte[] imageBytes) {
            // filePathから画像のBase64文字列を作成
            string base64String = Convert.ToBase64String(imageBytes);
            string result = CreateImageURL(base64String);
            return result;
        }

        public static string CreateImageURL(string base64String) {
            string base64Header = base64String.Substring(0, 5);
            // 先頭の文字列からイメージのフォーマットを判別
            // PNG  iVBOR
            // gif  R0lGO
            //jpeg  /9j/4
            // となる
            string formatText;
            if (base64Header == "iVBOR") {
                formatText = "png";
            } else if (base64Header == "R0lGO") {
                formatText = "gif";
            } else if (base64Header == "/9j/4") {
                formatText = "jpeg";
            } else {
                // エラー
                throw new Exception(PythonAILibStringResources.Instance.UnknownImageFormat);
            }

            // Base64文字列から画像のURLを作成
            string result = $"data:image/{formatText};base64,{base64String}";
            return result;
        }

        public static List<Dictionary<string, object>> CreateOpenAIContentList(string content, List<string> imageURLs) {

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
        public List<Dictionary<string, object>> CreateOpenAIMessagesList() {
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
            List<string> imageUrls = ImageURLs.Concat(AdditionalImageURLs).ToList();

            var dc = new Dictionary<string, object> {
                ["role"] = ChatItem.UserRole,
                ["content"] = CreateOpenAIContentList(CreatePromptText(), imageUrls)
            };
            messages.Add(dc);
            return messages;
        }

        public string CreateOpenAIRequestJSON() {
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
        public ChatResult? ExecuteChat() {
            if (ChatMode == OpenAIExecutionModeEnum.Normal) {
                // 通常のChatを実行する。
                return ExecuteChatNormal();
            }
            if (ChatMode == OpenAIExecutionModeEnum.OpenAIRAG) {
                // ChatModeがOpenAIRAGの場合は、OpenAIRAGChatを実行する。
                return ExecuteChatOpenAIRAG();
            }
            if (ChatMode == OpenAIExecutionModeEnum.LangChain) {
                // ChatModeがLangChainの場合は、LangChainChatを実行する。
                return ExecuteChatLangChain();
            }
            if (ChatMode == OpenAIExecutionModeEnum.AnalyzeAndDictionarize) {
                return ExecuteChatAnalyzeAndDictionarize();
            }

            return null;
        }
        private ChatResult? ExecuteChatLangChain() {
            OpenAIProperties.VectorDBItems.AddRange(VectorDBItems);
            ChatResult? result = PythonExecutor.PythonAIFunctions?.LangChainChat(this);
            if (result == null) {
                return null;
            }
            // リクエストをChatItemsに追加
            ChatHistory.Add(new ChatItem(ChatItem.UserRole, CreatePromptText()));
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            ChatHistory.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));
            return result;

        }

        private ChatResult? ExecuteChatNormal() {
            ChatResult? result = PythonExecutor.PythonAIFunctions?.OpenAIChat(this);
            // リクエストをChatItemsに追加
            if (result == null) {
                return null;
            }
            ChatHistory.Add(new ChatItem(ChatItem.UserRole, CreatePromptText()));
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            ChatHistory.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));

            return result;
        }
        private ChatResult? ExecuteChatOpenAIRAG() {
            // ContentTextの内容をベクトル検索する。
            StringBuilder sb = new();
            sb.AppendLine(PythonAILibStringResources.Instance.UnknownContent);
            sb.AppendLine("----------------------------------------------------");
            // ベクトル検索が存在するか否かのフラグ
            bool hasVectorSearch = false;
            // VectorSearchRequestを作成. テスト用にFilterを設定
            VectorSearchRequest request = new() {
                Query = ContentText,
                SearchKWArgs = new Dictionary<string, object> {
                    ["k"] = 10,
                    // filter
                    ["filter"] = new Dictionary<string, object> {
                        ["content_type"] = "text"
                    }
                }
            };
            foreach (var vectorDBItem in VectorDBItems) {
                List<VectorSearchResult> results = PythonExecutor.PythonAIFunctions?.VectorSearch(OpenAIProperties, vectorDBItem, request) ?? [];
                foreach (var vectorSearchResult in results) {
                    sb.AppendLine(vectorSearchResult.Content);
                    hasVectorSearch = true;
                }
            }
            if (hasVectorSearch) {
                // 結果をContentTextに追加
                ContentText += "\n" + sb.ToString();
            }

            ChatResult? result = PythonExecutor.PythonAIFunctions?.OpenAIChat(this);
            // リクエストをChatItemsに追加
            if (result == null) {
                return null;
            }
            ChatHistory.Add(new ChatItem(ChatItem.UserRole, CreatePromptText()));
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            ChatHistory.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));

            return result;

        }
        private ChatResult? ExecuteChatAnalyzeAndDictionarize() {
            // 実験的機能1(文章解析+辞書生成+RAG)
            // 新規のChatRequestを作成.ContentTextにはこのChatRequestのContentTextを設定する.
            // PromptTemplateTextは、定義が不明なものや「それはなんであるか？」が不明なものを含む文章をJSON形式で返す指示を設定する。
            string newRequestPrompt = PythonAILibStringResources.Instance.AnalyzeAndDictionarizeRequest;

            ChatRequest newRequest = new(OpenAIProperties) {
                ContentText = ContentText,
                PromptTemplateText = newRequestPrompt,
                JsonMode = true,
            };

            ChatResult? result = PythonExecutor.PythonAIFunctions?.OpenAIChat(newRequest);
            // リクエストをChatItemsに追加
            if (result == null) {
                throw new Exception(PythonAILibStringResources.Instance.ChatResultNull);
            }
            // レスポンスからJsonSerializerでDictionary<string,List<Dictionary<string, object>>>を取得
            Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(result.Response);
            if (resultDict == null) {
                throw new Exception(PythonAILibStringResources.Instance.ChatResultResponseInvalid);
            }
            // documents を取得
            JsonElement? documentsObject = (JsonElement)resultDict["result"];
            if (documentsObject == null) {
                throw new Exception(PythonAILibStringResources.Instance.ChatResultResponseResultNotFound);
            }
            string documents = documentsObject.ToString() ?? "[]";
            // documentsをList<Dictionary<string, object>>に変換
            List<Dictionary<string, string>> jsonList = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(documents) ?? [];

            // リストの要素毎にVectorSearchを実行
            // 結果用のStringBuilderを作成
            StringBuilder sb = new();
            sb.AppendLine(PythonAILibStringResources.Instance.UnknownContentDescription);
            sb.AppendLine("----------------------------------------------------");
            // ベクトル検索が存在するか否かのフラグ
            bool hasVectorSearch = false;
            foreach (var item in jsonList) {
                string sentence = item["sentence"] as string ?? "";
                if (string.IsNullOrEmpty(sentence)) {
                    continue;
                }
                sb.AppendLine($"### {sentence} ###");
                // VectorSearchRequestを作成. テスト用にFilterを設定
                VectorSearchRequest request = new() {
                    Query = sentence,
                    SearchKWArgs = new Dictionary<string, object> {
                        ["k"] = 10,
                        // filter
                        ["filter"] = new Dictionary<string, object> {
                            ["content_type"] = "text"
                        }
                    }
                };
                // VectorSearchを実行
                foreach (var vectorDBItem in VectorDBItems) {
                    List<VectorSearchResult> vectorSearchResults = PythonExecutor.PythonAIFunctions?.VectorSearch(OpenAIProperties, vectorDBItem, request) ?? [];
                    foreach (var vectorSearchResult in vectorSearchResults) {
                        sb.AppendLine($"{vectorSearchResult.Content}");
                        hasVectorSearch = true;
                    }
                }
            }
            if (hasVectorSearch) {
                // 結果を元のContentTextに追加
                ContentText += "\n" + sb.ToString();
            }
            // NormalChatを実行
            return ExecuteChatNormal();

        }

        public static string CreateSummary(OpenAIProperties openAIProperties, string content) {
            ChatRequest chatController = new(openAIProperties);
            // Normal Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.Normal;
            chatController.PromptTemplateText = PythonAILibStringResources.Instance.SummarizeRequest;
            chatController.ContentText = content;

            ChatResult? result = chatController.ExecuteChat();
            if (result != null) {
                return result.Response;
            }
            return "";
        }

        // 背景情報を作成する
        public static string CreateBackgroundInfo(OpenAIProperties openAIProperties, List<VectorDBItemBase> vectorDBItems, string content) {
            ChatRequest chatController = new(openAIProperties);
            // OpenAI+RAG Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.OpenAIRAG;
            chatController.PromptTemplateText = PythonAILibStringResources.Instance.BackgroundInfoRequest;
            chatController.ContentText = content;

            chatController.VectorDBItems = vectorDBItems;

            ChatResult? result = chatController.ExecuteChat();
            if (result != null) {
                return result.Response;
            }
            return "";
        }
        // 日本語文章を解析する
        public static string AnalyzeJapaneseSentence(OpenAIProperties openAIProperties, List<VectorDBItemBase> vectorDBItems, string content) {
            ChatRequest chatController = new(openAIProperties);
            // OpenAI+RAG Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.OpenAIRAG;
            chatController.PromptTemplateText = PythonAILibStringResources.Instance.AnalyzeJapaneseSentenceRequest;
            chatController.ContentText = content;

            ChatResult? result = chatController.ExecuteChat();
            if (result != null) {
                return result.Response;
            }
            return "";
        }
        // 自動QAを生成する
        public static string GenerateQA(OpenAIProperties openAIProperties, List<VectorDBItemBase> vectorDBItems, string content) {
            ChatRequest chatController = new(openAIProperties);
            // OpenAI+RAG Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.OpenAIRAG;
            chatController.PromptTemplateText = PythonAILibStringResources.Instance.GenerateQuestionRequest;
            chatController.ContentText = content;

            chatController.VectorDBItems = vectorDBItems;

            ChatResult? result = chatController.ExecuteChat();
            if (result == null) {
                return "";
            }
            // 生成した質問をAIに問い合わせる
            string question = result.Response;
            chatController = new ChatRequest(openAIProperties);
            // OpenAI+RAG Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.OpenAIRAG;
            chatController.PromptTemplateText = PythonAILibStringResources.Instance.AnswerRequest;
            chatController.ContentText = question;

            chatController.VectorDBItems = vectorDBItems;

            result = chatController.ExecuteChat();
            if (result != null) {
                return result.Response;
            }
            return "";
        }

        // タイトルを作成する
        public static string CreateTitle(OpenAIProperties openAIProperties, string content) {
            ChatRequest chatController = new(openAIProperties);
            // Normal Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.Normal;
            chatController.PromptTemplateText = PythonAILibStringResources.Instance.TitleRequest;
            chatController.ContentText = content;

            ChatResult? result = chatController.ExecuteChat();
            if (result != null) {
                return result.Response;
            }
            return "";
        }
        // 画像からテキストを抽出する
        public static string ExtractTextFromImage(OpenAIProperties openAIProperties, List<string> ImageBase64List) {
            ChatRequest chatController = new(openAIProperties);
            // Normal Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.Normal;
            chatController.PromptTemplateText = PythonAILibStringResources.Instance.ExtractTextRequest;
            chatController.ContentText = "";
            chatController.ImageURLs = ImageBase64List.Select(ChatRequest.CreateImageURL).ToList();
            if (chatController.ImageURLs.Count == 0) {
                return "";
            }

            ChatResult? result = chatController.ExecuteChat();
            if (result != null) {
                return result.Response;
            }
            return "";
        }
    }
}
