using System.Text;
using System.Text.Json;
using LibPythonAI.Model.Prompt;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resources;
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
                ContentText = content,
            };

            chatRequestContext.PromptTemplateText = promptText.Prompt;
            chatRequestContext.ChatMode = promptText.ChatMode;
            chatRequestContext.SplitMode = promptText.SplitMode;

            ChatResult? result = ExecuteChat(chatRequest, chatRequestContext, (message) => { });
            if (result != null) {
                return result.Output;
            }
            return "";
        }

        // Chatを実行した結果を次の質問に渡すことを繰り返して文字列の結果を取得する
        public static string CreateTextChatResult(OpenAIExecutionModeEnum chatMode, SplitOnTokenLimitExceedModeEnum splitMode , ChatRequestContext chatRequestContext, List<string> promptList, string content) {
            string resultString = content;
            foreach (string prompt in promptList) {
                ChatRequest chatRequest = new() {
                    ContentText = resultString,
                };

                chatRequestContext.ChatMode = chatMode;
                chatRequestContext.PromptTemplateText = prompt;


                ChatResult? result = ExecuteChat(chatRequest, chatRequestContext, (message) => { });
                if (result != null) {
                    resultString = result.Output;
                }
            }
            return resultString;
        }

        // Chatを実行してリストの結果を取得する
        public static List<string> CreateListChatResult(ChatRequestContext chatRequestContext, PromptItem promptItem, string content) {

            string promptText = PromptStringResource.Instance.JsonStringListGenerationPrompt + "\n" + promptItem.Prompt;
            ChatRequest chatRequest = new() {
                // OpenAI+RAG Chatを実行
                ContentText = content,
                JsonMode = true
            };
            chatRequestContext.PromptTemplateText = promptText;
            chatRequestContext.ChatMode = promptItem.ChatMode;
            chatRequestContext.SplitMode = promptItem.SplitMode;

            ChatResult? result = ExecuteChat(chatRequest, chatRequestContext, (message) => { });
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
            ChatRequest chatRequest = new() {
                // OpenAI+RAG Chatを実行
                ContentText = content,
                JsonMode = true
            };
            chatRequestContext.ChatMode = promptItem.ChatMode;
            chatRequestContext.PromptTemplateText = promptItem.Prompt;
            chatRequestContext.SplitMode = promptItem.SplitMode;

            ChatResult? result = ExecuteChat(chatRequest, chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }


        // Chatを実行して複雑な結果を取得する
        public static Dictionary<string, dynamic?> CreateTableChatResult(ChatRequestContext chatRequestContext, PromptItem promptItem, string content) {
            ChatRequest chatRequest = new() {
                // OpenAI+RAG Chatを実行
                ContentText = content,
                JsonMode = true
            };
            chatRequestContext.ChatMode = promptItem.ChatMode;
            chatRequestContext.PromptTemplateText = promptItem.Prompt;
            chatRequestContext.SplitMode = promptItem.SplitMode;

            ChatResult? result = ExecuteChat(chatRequest, chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                // JSON文字列をDictionary<string, dynamic>型に変換
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }

        // 画像からテキストを抽出する
        public static string ExtractTextFromImage(ChatRequestContext chatRequestContext, List<string> ImageBase64List) {
            ChatRequest chatRequest = new();
            // Normal Chatを実行
            chatRequestContext.ChatMode = OpenAIExecutionModeEnum.Normal;
            chatRequestContext.PromptTemplateText = PromptStringResource.Instance.ExtractTextRequest;
            chatRequest.ContentText = "";
            chatRequest.ImageURLs = ImageBase64List.Select(CreateImageURL).ToList();
            if (chatRequest.ImageURLs.Count == 0) {
                return "";
            }

            ChatResult? result = ExecuteChat(chatRequest, chatRequestContext, (message) => { });
            if (result != null) {
                return result.Output;
            }
            return "";
        }
        // VectorSearchを実行してコンテキスト情報を生成する
        public static string GenerateVectorSearchResult(ChatRequestContext chatRequestContext, string query) {
            // ベクトル検索が存在するか否かのフラグ
            bool hasVectorSearch = false;
            StringBuilder sb = new();
            List<VectorMetadata> results = PythonExecutor.PythonAIFunctions?.VectorSearch(chatRequestContext, query) ?? [];
            sb.AppendLine();
            for (int i = 0; i < results.Count; i++) {
                VectorMetadata vectorSearchResult = results[i];
                sb.AppendLine($"## 参考情報:{i + 1} ##");
                sb.AppendLine("--------");
                sb.AppendLine(vectorSearchResult.Description);
                sb.AppendLine(vectorSearchResult.Content);
                hasVectorSearch = true;
            }

            if (hasVectorSearch) {
                // 結果をContentTextに追加
                string result = "\n" + PromptStringResource.Instance.RelatedInformation;
                result += sb.ToString();
                return result;
            }
            return "";
        }

        public static ChatResult? ExecuteAutoGenGroupChat(ChatRequestContext chatRequestContext, ChatRequest chat, Action<string> iteration) {
            ChatResult? result = PythonExecutor.PythonAIFunctions?.AutoGenGroupChat(chatRequestContext, chat, iteration);
            return result;
        }

        public static ChatResult? ExecuteChatNormal(ChatRequestContext chatRequestContext, ChatRequest chat) {
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

            ( bool isImage, ContentTypes.ImageType imageType) = ContentTypes.GetImageTypeFromBase64(base64String);
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
            string promptText = $"{chatRequestContext.PromptTemplateText}\n\n{chatRequest.ContentText}";

            // 最後のユーザー発言のContentにPromptTextを追加
            lastUserRoleMessage.Content = promptText;
            // ImageURLsが空でない場合は、lastUserRoleMessageにImageURLsを追加
            if (chatRequest.ImageURLs.Count > 0) {
                lastUserRoleMessage.ImageURLs = chatRequest.ImageURLs;
            }
        }

        // Chatを実行する
        public static ChatResult? ExecuteChat(ChatRequest chatRequest, ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            // 通常のOpenAI Chatを実行する
            if (chatRequestContext.ChatMode == OpenAIExecutionModeEnum.Normal) {

                // リクエストメッセージを最新化
                PrepareNormalRequest(chatRequestContext, chatRequest);
                // 通常のChatを実行する。
                ChatResult? result = ChatUtil.ExecuteChatNormal(chatRequestContext, chatRequest);
                if (result == null) {
                    return null;
                }
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                chatRequest.ChatHistory.Add(new ChatMessage(ChatMessage.AssistantRole, result.Output, result.PageSourceList));
                return result;
            }
            // AutoGenGroupChatを実行する
            if (chatRequestContext.ChatMode == OpenAIExecutionModeEnum.AutoGenGroupChat) {
                // AutoGenGroupChatを実行する
                return ExecuteAutoGenGroupChat(chatRequest, chatRequestContext, iterateAction);
            }
            return null;
        }

        // AutoGenGroupChatを実行する
        public static ChatResult? ExecuteAutoGenGroupChat(ChatRequest chatRequest, ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            // リクエストメッセージを最新化
            PrepareNormalRequest(chatRequestContext, chatRequest);
            // 結果
            ChatMessage result = new(ChatMessage.AssistantRole, "", []);
            chatRequest.ChatHistory.Add(result);

            // AutoGenGroupChatを実行する
            return ChatUtil.ExecuteAutoGenGroupChat(chatRequestContext, chatRequest, (message) => {
                result.Content += message + "\n";
                iterateAction(message);
            });
        }


    }
}
