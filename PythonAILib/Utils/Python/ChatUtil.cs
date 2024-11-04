using System.Text;
using System.Text.Json;
using PythonAILib.Model;
using PythonAILib.Model.Chat;
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
        public static string CreateTextChatResult(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, PromptItem promptText, string content) {
            Chat chatController = new() {
                // NormalChat, OpenAI+RAG Chat, LangChainChatを実行
                ChatMode = promptText.ChatType,
                PromptTemplateText = promptText.Prompt,
                ContentText = content,
                VectorDBItems = vectorDBItems,
            };

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null) {
                return result.Output;
            }
            return "";
        }

        // Chatを実行した結果を次の質問に渡すことを繰り返して文字列の結果を取得する
        public static string CreateTextChatResult(OpenAIExecutionModeEnum chatMode, OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, List<string> promptList, string content) {
            string resultString = content;
            foreach (string prompt in promptList) {
                Chat chatController = new() {
                    ChatMode = chatMode,
                    PromptTemplateText = prompt,
                    ContentText = resultString,
                    VectorDBItems = vectorDBItems
                };

                ChatResult? result = chatController.ExecuteChat(openAIProperties);
                if (result != null) {
                    resultString = result.Output;
                }
            }
            return resultString;
        }

        // Chatを実行してリストの結果を取得する
        public static List<string> CreateListChatResult(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, PromptItem promptItem, string content) {

            string promptText = PromptStringResource.Instance.JsonStringListGenerationPrompt + "\n" + promptItem.Prompt;
            Chat chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = promptItem.ChatType,
                PromptTemplateText = promptText,
                ContentText = content,
                VectorDBItems = vectorDBItems,
                JsonMode = true
            };

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null && !string.IsNullOrEmpty(result.Output)) {

                Dictionary<string, List<string>> jsonResult = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(result.Output, options) ?? [];
                if (jsonResult.TryGetValue("result", out List<string>? value)) {
                    return value;
                }
            }
            return [];
        }
        // CHatを実行してDictionary<string, object>の結果を取得する
        public static Dictionary<string, dynamic?> CreateDictionaryChatResult(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, PromptItem promptItem, string content) {
            Chat chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = promptItem.ChatType,
                PromptTemplateText = promptItem.Prompt,
                ContentText = content,
                VectorDBItems = vectorDBItems,
                JsonMode = true
            };

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }


        // Chatを実行して複雑な結果を取得する
        public static Dictionary<string, dynamic?> CreateTableChatResult(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, PromptItem promptItem, string content) {
            Chat chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = promptItem.ChatType,
                PromptTemplateText = promptItem.Prompt,
                ContentText = content,
                VectorDBItems = vectorDBItems,
                JsonMode = true
            };

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                // JSON文字列をDictionary<string, dynamic>型に変換
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }

        // 画像からテキストを抽出する
        public static string ExtractTextFromImage(OpenAIProperties openAIProperties, List<string> ImageBase64List) {
            Chat chatController = new();
            // Normal Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.Normal;
            chatController.PromptTemplateText = PromptStringResource.Instance.ExtractTextRequest;
            chatController.ContentText = "";
            chatController.ImageURLs = ImageBase64List.Select(Chat.CreateImageURL).ToList();
            if (chatController.ImageURLs.Count == 0) {
                return "";
            }

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null) {
                return result.Output;
            }
            return "";
        }
        // VectorSearchを実行してコンテキスト情報を生成する
        public static string GenerateVectorSearchResult(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, string query) {
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
            List<VectorSearchResult> results = PythonExecutor.PythonAIFunctions?.VectorSearch(openAIProperties, vectorDBItems, request) ?? [];
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
                return result;
            }
            return "";
        }
        // OpenAIChatを実行する。
        public static ChatResult? ExecuteChat(OpenAIProperties openAIProperties, Chat chat) {
            if (chat.ChatMode == OpenAIExecutionModeEnum.Normal) {
                // 通常のChatを実行する。
                return ExecuteChatNormal(openAIProperties, chat);
            }
            if (chat.ChatMode == OpenAIExecutionModeEnum.OpenAIRAG) {
                // ChatModeがOpenAIRAGの場合は、OpenAIRAGChatを実行する。
                return ExecuteChatOpenAIRAG(openAIProperties, chat);
            }
            if (chat.ChatMode == OpenAIExecutionModeEnum.LangChain) {
                // ChatModeがLangChainの場合は、LangChainChatを実行する。
                return ExecuteChatLangChain(openAIProperties, chat);
            }
            return null;
        }
        public static ChatResult? ExecuteChatLangChain(OpenAIProperties openAIProperties, Chat chat) {
            // ContentTextの内容でベクトル検索して、コンテキスト情報を生成する
            // GenerateVectorSearchResult(openAIProperties);

            ChatResult? result = PythonExecutor.PythonAIFunctions?.LangChainChat(openAIProperties, chat);
            if (result == null) {
                return null;
            }
            // リクエストをChatItemsに追加
            chat.ChatHistory.Add(new ChatContentItem(ChatContentItem.UserRole, chat.CreatePromptText()));
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            chat.ChatHistory.Add(new ChatContentItem(ChatContentItem.AssistantRole, result.Output, result.PageSourceList));
            return result;

        }

        public static ChatResult? ExecuteChatNormal(OpenAIProperties openAIProperties, Chat chat) {
            ChatResult? result = PythonExecutor.PythonAIFunctions?.OpenAIChat(openAIProperties, chat);
            // リクエストをChatItemsに追加
            if (result == null) {
                return null;
            }
            chat.ChatHistory.Add(new ChatContentItem(ChatContentItem.UserRole, chat.CreatePromptText()));
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            chat.ChatHistory.Add(new ChatContentItem(ChatContentItem.AssistantRole, result.Output, result.PageSourceList));

            return result;
        }

        public static ChatResult? ExecuteChatOpenAIRAG(OpenAIProperties openAIProperties, Chat chat) {
            // ContentTextの内容でベクトル検索して、コンテキスト情報を生成する
            chat.ContentText += ChatUtil.GenerateVectorSearchResult(openAIProperties, chat.VectorDBItems, chat.ContentText);
            chat.ChatHistory.Add(new ChatContentItem(ChatContentItem.UserRole, chat.ContentText));



            ChatResult? result = PythonExecutor.PythonAIFunctions?.OpenAIChat(openAIProperties, chat);
            // リクエストをChatItemsに追加
            if (result == null) {
                return null;
            }
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            chat.ChatHistory.Add(new ChatContentItem(ChatContentItem.AssistantRole, result.Output, result.PageSourceList));

            return result;

        }

    }
}
