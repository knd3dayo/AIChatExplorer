using System.Text.Json;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resource;
using PythonAILib.Utils;
using QAChat;

namespace PythonAILib.Model.Chat {
    public class ChatUtil {

        // JSON形式の結果をパースしてリストに変換
        private static JsonSerializerOptions options = new() {
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
                return result.Response;
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
                    resultString = result.Response;
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
            if (result != null && !string.IsNullOrEmpty(result.Response)) {

                Dictionary<string, List<string>> jsonResult = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(result.Response, options) ?? [];
                if (jsonResult.TryGetValue("result", out List<string>? value)) {
                    return value;
                }
            }
            return [];
        }
        // CHatを実行してDictionary<string, string>の結果を取得する
        public static Dictionary<string, string> CreateDictionaryChatResult(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, PromptItem promptItem, string content) {
            Chat chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = promptItem.ChatType,
                PromptTemplateText = promptItem.Prompt,
                ContentText = content,
                VectorDBItems = vectorDBItems,
                JsonMode = true
            };

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null && !string.IsNullOrEmpty(result.Response)) {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(result.Response, options) ?? [];
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
            if (result != null && !string.IsNullOrEmpty(result.Response)) {
                // JSON文字列をDictionary<string, dynamic>型に変換
                return JsonUtil.ParseJson(result.Response);
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
                return result.Response;
            }
            return "";
        }

    }
}
