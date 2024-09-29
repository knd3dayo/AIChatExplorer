using System.Text.Json;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resource;
using QAChat;

namespace PythonAILib.Model.Chat {
    public class ChatUtil {

        // 生成AIチャットの結果を取得する
        public static List<string> CreateSystemPromptChatResult(string content, PromptItem.SystemDefinedPromptNames systemDefinedPrompt, List<VectorDBItem> vectorDBItems) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

            // システム定義のPromptItemを取得
            PromptItem? promptItem = libManager.DataFactory.GetPromptTemplateByName(systemDefinedPrompt.ToString());

            if (promptItem == null) {
                throw new Exception("PromptItem not found");
            }
            // promptItemのResultTypeがTextの場合
            if (promptItem.PromptResultType == PromptItem.PromptResultTypeEnum.Text) {
                // promptItemのChatTypeがNormalの場合、ChatRequest.CreateChatを実行
                if (promptItem.ChatType == PromptItem.ChatTypeEnum.Normal) {
                    string result = ChatUtil.CreateNormalChatResultText(openAIProperties, content, promptItem.Prompt);
                    if (string.IsNullOrEmpty(result) == false) {
                        return [result];
                    }
                }
                // promptItemのChatTypeがRAGの場合、ChatRequest.CreateRAGChatを実行
                if (promptItem.ChatType == PromptItem.ChatTypeEnum.RAG) {
                    string result = ChatUtil.CreateRAGChatResultText(openAIProperties, vectorDBItems, content, promptItem.Prompt);
                    if (string.IsNullOrEmpty(result) == false) {
                        return [result];
                    }
                }
            }
            // promptItemのResultTypeがListの場合
            if (promptItem.PromptResultType == PromptItem.PromptResultTypeEnum.List) {
                // promptItemのChatTypeがNormalの場合、ChatRequest.CreateChatを実行
                if (promptItem.ChatType == PromptItem.ChatTypeEnum.Normal) {
                    List<string> result = ChatUtil.CreateNormalResultBulletedList(openAIProperties, content, promptItem.Prompt);
                    return result;
                }
                // promptItemのChatTypeがRAGの場合、ChatRequest.CreateRAGChatを実行
                if (promptItem.ChatType == PromptItem.ChatTypeEnum.RAG) {
                    List<string> result = ChatUtil.CreateRAGResultBulletedList(openAIProperties, vectorDBItems, content, promptItem.Prompt);
                    return result;
                }
            }
            return [];
        }



        // Normal Chatを実行して文字列の結果を取得する
        public static string CreateNormalChatResultText(OpenAIProperties openAIProperties, string content, string promptText) {
            Chat chatController = new() {
                // Normal Chatを実行
                ChatMode = OpenAIExecutionModeEnum.Normal,
                PromptTemplateText = promptText,
                ContentText = content
            };
            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null) {
                return result.Response;
            }
            return "";
        }

        // RAG Chatを実行して文字列の結果を取得する
        public static string CreateRAGChatResultText(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, string content, string promptText) {
            Chat chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = OpenAIExecutionModeEnum.OpenAIRAG,
                PromptTemplateText = promptText,
                ContentText = content,
                VectorDBItems = vectorDBItems
            };

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null) {
                return result.Response;
            }
            return "";
        }

        // RAG Chatを実行した結果を次の質問に渡すことを繰り返して文字列の結果を取得する
        public static string CreateRAGChatResultText(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, string content, List<string> promptList) {
            string resultString = content;
            foreach (string prompt in promptList) {
                Chat chatController = new() {
                    // OpenAI+RAG Chatを実行
                    ChatMode = OpenAIExecutionModeEnum.OpenAIRAG,
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

        // Normal Chatを実行してリストの結果を取得する
        public static List<string> CreateNormalResultBulletedList(OpenAIProperties openAIProperties, string content, string promptText) {
            Chat chatController = new() {
                // Normal Chatを実行
                ChatMode = OpenAIExecutionModeEnum.Normal,
                PromptTemplateText = promptText,
                ContentText = content
            };

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null && !string.IsNullOrEmpty(result.Response)) {
                // JSON形式の結果をパースしてリストに変換
                JsonSerializerOptions options = new() {
                    PropertyNameCaseInsensitive = true
                };

                Dictionary<string, List<string>> jsonResult = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(result.Response, options) ?? [];
                if (jsonResult.TryGetValue("result", out List<string>? value)) {
                    return value;
                }
            }
            return [];
        }

        // RAT Chatを実行してリストの結果を取得する
        public static List<string> CreateRAGResultBulletedList(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, string content, string promptText) {
            // JSON形式の出力を指示するプロンプトを先頭に追加
            promptText = PromptStringResource.Instance.JsonStringListGenerationPrompt + promptText;

            Chat chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = OpenAIExecutionModeEnum.OpenAIRAG,
                PromptTemplateText = promptText,
                ContentText = content,
                VectorDBItems = vectorDBItems,
                JsonMode = true
            };

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null && !string.IsNullOrEmpty(result.Response)) {
                // JSON形式の結果をパースしてリストに変換
                JsonSerializerOptions options = new() {
                    PropertyNameCaseInsensitive = true
                };

                Dictionary<string, List<string>> jsonResult = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(result.Response, options) ?? [];
                if (jsonResult.TryGetValue("result", out List<string>? value)) {
                    return value;
                }
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
