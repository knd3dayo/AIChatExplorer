using System.Text.Json;
using PythonAILib.Model.Abstract;
using PythonAILib.Resource;

namespace PythonAILib.Model.Chat {
    public class ChatUtil {

        // タイトルを作成する
        public static string CreateTitle(OpenAIProperties openAIProperties, string content, string promptText = "") {


            // contentの文字数が4096文字を超える場合は4096文字までに制限
            string contentText = content.Length > 4096 ? content[..4096] : content;

            Chat chatController = new() {
                // Normal Chatを実行
                ChatMode = OpenAIExecutionModeEnum.Normal,
                PromptTemplateText = promptText,
                ContentText = contentText
            };

            if (string.IsNullOrEmpty(promptText)) {
                chatController.PromptTemplateText = PromptStringResource.Instance.TitleGenerationPrompt;
            }

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null) {
                return result.Response;
            }
            return "";
        }
        // 背景情報を作成する
        public static string CreateBackgroundInfo(OpenAIProperties openAIProperties, List<VectorDBItemBase> vectorDBItems, string content, string promptText = "") {
            Chat chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = OpenAIExecutionModeEnum.OpenAIRAG,
                PromptTemplateText = promptText,
                ContentText = content,
                VectorDBItems = vectorDBItems
            };
            if (string.IsNullOrEmpty(promptText)) {
                chatController.PromptTemplateText = PromptStringResource.Instance.BackgroundInformationGenerationPrompt;
            }

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null) {
                return result.Response;
            }
            return "";
        }

        // サマリーを作成する
        public static string CreateSummary(OpenAIProperties openAIProperties, string content, string promptText = "") {
            Chat chatController = new() {
                // Normal Chatを実行
                ChatMode = OpenAIExecutionModeEnum.Normal,
                PromptTemplateText = promptText,
                ContentText = content
            };
            if (string.IsNullOrEmpty(promptText)) {
                chatController.PromptTemplateText = PromptStringResource.Instance.SummaryGenerationPrompt;
            }

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null) {
                return result.Response;
            }
            return "";
        }


        // 日本語文章を解析する
        public static string AnalyzeJapaneseSentence(OpenAIProperties openAIProperties, List<VectorDBItemBase> vectorDBItems, string content) {
            Chat chatController = new();
            // OpenAI+RAG Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.OpenAIRAG;
            chatController.PromptTemplateText = PromptStringResource.Instance.AnalyzeJapaneseSentenceRequest;
            chatController.ContentText = content;

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null) {
                return result.Response;
            }
            return "";
        }
        // 自動QAを生成する
        public static string GenerateQA(OpenAIProperties openAIProperties, List<VectorDBItemBase> vectorDBItems, string content) {
            Chat chatController = new();
            // OpenAI+RAG Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.OpenAIRAG;
            chatController.PromptTemplateText = PromptStringResource.Instance.GenerateQuestionRequest;
            chatController.ContentText = content;

            chatController.VectorDBItems = vectorDBItems;

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result == null) {
                return "";
            }
            // 生成した質問をAIに問い合わせる
            string question = result.Response;
            chatController = new Chat();
            // OpenAI+RAG Chatを実行
            chatController.ChatMode = OpenAIExecutionModeEnum.OpenAIRAG;
            chatController.PromptTemplateText = PromptStringResource.Instance.AnswerRequest;
            chatController.ContentText = question;

            chatController.VectorDBItems = vectorDBItems;

            result = chatController.ExecuteChat(openAIProperties);
            if (result != null) {
                return result.Response;
            }
            return "";
        }
        // 課題リストを生成する
        public static List<string> CreateIssues(OpenAIProperties openAIProperties, List<VectorDBItemBase> vectorDBItems, string content, string promptText = "") {
            Chat chatController = new() {
                // OpenAI+RAG Chatを実行
                ChatMode = OpenAIExecutionModeEnum.OpenAIRAG,
                PromptTemplateText = promptText,
                ContentText = content,
                VectorDBItems = vectorDBItems,
                JsonMode = true
            };
            if (string.IsNullOrEmpty(promptText)) {
                chatController.PromptTemplateText = PromptStringResource.Instance.IssuesGenerationPrompt;
            }

            ChatResult? result = chatController.ExecuteChat(openAIProperties);
            if (result != null &&  !string.IsNullOrEmpty(result.Response)) {
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
