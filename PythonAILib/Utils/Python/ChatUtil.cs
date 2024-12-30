using System.Text;
using System.Text.Json;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;

namespace PythonAILib.Utils.Python {
    public class ChatUtil {

        // JSON形式の結果をパースしてリストに変換
        private static readonly JsonSerializerOptions options = new() {
            PropertyNameCaseInsensitive = true
        };

        // Chatを実行して文字列の結果を取得する
        public static string CreateTextChatResult(ChatRequestContext chatRequestContext, PromptItem promptText, string content) {
            ChatRequest chatRequest = new() {
                // NormalChat, OpenAI+RAG Chat, LangChainChatを実行
                ChatMode = promptText.ChatType,
                PromptTemplateText = promptText.Prompt,
                ContentText = content,
            };

            ChatResult? result = chatRequest.ExecuteChat(chatRequestContext, (message) => { });
            if (result != null) {
                return result.Output;
            }
            return "";
        }

        // Chatを実行した結果を次の質問に渡すことを繰り返して文字列の結果を取得する
        public static string CreateTextChatResult(OpenAIExecutionModeEnum chatMode, ChatRequestContext chatRequestContext, List<string> promptList, string content) {
            string resultString = content;
            foreach (string prompt in promptList) {
                ChatRequest chatController = new() {
                    ChatMode = chatMode,
                    PromptTemplateText = prompt,
                    ContentText = resultString,
                };

                ChatResult? result = chatController.ExecuteChat(chatRequestContext, (message) => { });
                if (result != null) {
                    resultString = result.Output;
                }
            }
            return resultString;
        }

        // Chatを実行してリストの結果を取得する
        public static List<string> CreateListChatResult(ChatRequestContext chatRequestContext, PromptItem promptItem, string content) {

            string promptText = PromptStringResource.Instance.JsonStringListGenerationPrompt + "\n" + promptItem.Prompt;
            ChatRequest chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = promptItem.ChatType,
                PromptTemplateText = promptText,
                ContentText = content,
                JsonMode = true
            };

            ChatResult? result = chatController.ExecuteChat(chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {

                Dictionary<string, List<string>> jsonResult = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(result.Output, options) ?? [];
                if (jsonResult.TryGetValue("result", out List<string>? value)) {
                    return value;
                }
            }
            return [];
        }
        // CHatを実行してDictionary<string, object>の結果を取得する
        public static Dictionary<string, dynamic?> CreateDictionaryChatResult(ChatRequestContext chatRequestContext, PromptItem promptItem, string content) {
            ChatRequest chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = promptItem.ChatType,
                PromptTemplateText = promptItem.Prompt,
                ContentText = content,
                JsonMode = true
            };

            ChatResult? result = chatController.ExecuteChat(chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }


        // Chatを実行して複雑な結果を取得する
        public static Dictionary<string, dynamic?> CreateTableChatResult(ChatRequestContext chatRequestContext, PromptItem promptItem, string content) {
            ChatRequest chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = promptItem.ChatType,
                PromptTemplateText = promptItem.Prompt,
                ContentText = content,
                JsonMode = true
            };

            ChatResult? result = chatController.ExecuteChat(chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                // JSON文字列をDictionary<string, dynamic>型に変換
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }

        // 画像からテキストを抽出する
        public static string ExtractTextFromImage(ChatRequestContext chatRequestContext, List<string> ImageBase64List) {
            ChatRequest chatController = new();
            // Normal Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.Normal;
            chatController.PromptTemplateText = PromptStringResource.Instance.ExtractTextRequest;
            chatController.ContentText = "";
            chatController.ImageURLs = ImageBase64List.Select(CreateImageURL).ToList();
            if (chatController.ImageURLs.Count == 0) {
                return "";
            }

            ChatResult? result = chatController.ExecuteChat(chatRequestContext, (message) => { });
            if (result != null) {
                return result.Output;
            }
            return "";
        }
        // VectorSearchを実行してコンテキスト情報を生成する
        public static string GenerateVectorSearchResult(ChatRequestContext chatRequestContext, string query) {
            // ベクトル検索が存在するか否かのフラグ
            bool hasVectorSearch = false;
            // VectorSearchRequestを作成. テスト用にFilterを設定
            VectorSearchRequest request = new() {
                Query = query,
                SearchKWArgs = new Dictionary<string, object> {
                    ["k"] = 4,
                    // filter
                    ["filter"] = new Dictionary<string, object> {
                        ["content_type"] = "text"
                    }
                }
            };

            StringBuilder sb = new();
            List<VectorDBEntry> results = PythonExecutor.PythonAIFunctions?.VectorSearch(chatRequestContext, request) ?? [];
            sb.AppendLine();
            for (int i = 0; i < results.Count; i++) {
                VectorDBEntry vectorSearchResult = results[i];
                sb.AppendLine($"## 参考情報:{i + 1} ##");
                sb.AppendLine("--------");
                sb.AppendLine(vectorSearchResult.Description);
                sb.AppendLine(vectorSearchResult.Content);
                hasVectorSearch = true;
            }

            if (hasVectorSearch) {
                // 結果をContentTextに追加
                string result = PromptStringResource.Instance.RelatedInformation;
                result += sb.ToString();
                return result;
            }
            return "";
        }

        public static ChatResult? ExecuteAutoGenGroupChat(ChatRequestContext chatRequestContext, ChatRequest chat, Action<string> iteration) {
            ChatResult? result = PythonExecutor.PythonAIFunctions?.AutoGenGroupChat(chatRequestContext, chat, iteration);
            return result;
        }

        // AutoGenNormalChatを実行
        public static ChatResult? ExecuteAutoGenNormalChat(ChatRequestContext chatRequestContext, ChatRequest chat, Action<string> iteration) {
            ChatResult? result = PythonExecutor.PythonAIFunctions?.AutoGenNormalChat(chatRequestContext, chat, iteration);
            return result;
        }

        // AutoGenNestedChatを実行
        public static ChatResult? ExecuteAutoGenNestedChat(ChatRequestContext chatRequestContext, ChatRequest chat, Action<string> iteration) {
            ChatResult? result = PythonExecutor.PythonAIFunctions?.AutoGenNestedChat(chatRequestContext, chat, iteration);
            return result;
        }

        public static ChatResult? ExecuteChatLangChain(ChatRequestContext chatRequestContext, ChatRequest chat) {
            // ContentTextの内容でベクトル検索して、コンテキスト情報を生成する
            // GenerateVectorSearchResult(openAIProperties);
            ChatResult? result = PythonExecutor.PythonAIFunctions?.LangChainChat(chatRequestContext, chat);
            return result;
        }

        public static ChatResult? ExecuteChatNormal(ChatRequestContext chatRequestContext, ChatRequest chat) {
            ChatResult? result = PythonExecutor.PythonAIFunctions?.OpenAIChat(chatRequestContext, chat);
            return result;
        }

        public static ChatResult? ExecuteChatOpenAIRAG(ChatRequestContext chatRequestContext, ChatRequest chat) {

            ChatResult? result = PythonExecutor.PythonAIFunctions?.OpenAIChat(chatRequestContext, chat);
            return result;

        }



        public static string CreateImageURLFromFilePath(string filePath) {
            // filePathから画像のBase64文字列を作成
            byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
            string result = CreateImageURLFromBytes(imageBytes);
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

        public static string CreateImageURLFromBytes(byte[] imageBytes) {
            // filePathから画像のBase64文字列を作成
            string base64String = Convert.ToBase64String(imageBytes);
            string result = CreateImageURL(base64String);
            return result;
        }
    }
}
