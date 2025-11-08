using System.Text;
using System.Text.Json;
using LibMain.Model.Chat;
using LibMain.Model.Content;
using LibMain.Model.Prompt;
using LibMain.Model.VectorDB;
using LibMain.PythonIF.Request;
using LibMain.PythonIF.Response;
using LibMain.Resources;
using LibMain.Utils.Common;

namespace LibMain.Utils.Python {
    public class ChatUtil {

        // JSON形式の結果をパースしてリストに変換
        private static readonly JsonSerializerOptions options = new() {
            PropertyNameCaseInsensitive = true
        };

        // Chatを実行して文字列の結果を取得する
        public static async Task<string> CreateTextChatResult(ChatSettings chatSettings, PromptItem promptText, string content) {
            ChatRequest chatRequest = new() {
                // NormalChat, OpenAI+RAG Chat, LangChainChatを実行
                ContentText = content,
            };

            if (promptText.RAGMode != RAGModeEnum.None) {
                await chatRequest.ApplyVectorSearchResults(new VectorSearchItem(new VectorDBItem()) {
                    InputText = content,
                    TopK = 3,
                    ScoreThreshold = 0.0f,
                });
            }

            chatSettings.PromptTemplateText = promptText.Prompt;
            chatSettings.SplitMode = promptText.SplitMode;
            ChatRequestContext chatRequestContext = new(chatSettings);

            ChatResponse? result = await chatRequest.ExecuteChat(promptText.ChatMode, chatRequestContext, (message) => { });
            if (result != null) {
                return result.Output;
            }
            return "";
        }


        // Chatを実行してリストの結果を取得する
        public static async Task<List<string>> CreateListChatResult(ChatSettings chatSettings, PromptItem promptItem, string content) {

            string promptText = PromptStringResourceJa.Instance.JsonStringListGenerationPrompt + "\n" + promptItem.Prompt;
            ChatRequest chatRequest = new() {
                // OpenAI+RAG Chatを実行
                ContentText = content,
                JsonMode = true
            };

            if (promptItem.RAGMode != RAGModeEnum.None) {
                await chatRequest.ApplyVectorSearchResults(new VectorSearchItem(new VectorDBItem()) {
                    InputText = content,
                    TopK = 3,
                    ScoreThreshold = 0.0f,
                });
            }

            chatSettings.PromptTemplateText = promptText;
            chatSettings.SplitMode = promptItem.SplitMode;

            ChatRequestContext chatRequestContext = new(chatSettings);

            ChatResponse? result = await chatRequest.ExecuteChat(promptItem.ChatMode, chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {

                Dictionary<string, List<string>> jsonResult = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(result.Output, options) ?? [];
                if (jsonResult.TryGetValue("result", out List<string>? value)) {
                    return value;
                }
            }
            return [];
        }
        // CHatを実行してDictionary<string, object>の結果を取得する
        public static async Task<Dictionary<string, dynamic?>> CreateDictionaryChatResult(ChatSettings chatSettings, PromptItem promptItem, string content) {
            ChatRequest chatRequest = new() {
                // OpenAI+RAG Chatを実行
                ContentText = content,
                JsonMode = true
            };

            chatSettings.PromptTemplateText = promptItem.Prompt;
            chatSettings.SplitMode = promptItem.SplitMode;
            ChatRequestContext chatRequestContext = new(chatSettings);

            if (promptItem.RAGMode != RAGModeEnum.None) {
                await chatRequest.ApplyVectorSearchResults(new VectorSearchItem(new VectorDBItem()) {
                    InputText = content,
                    TopK = 3,
                    ScoreThreshold = 0.0f,
                });
            }

            ChatResponse? result = await chatRequest.ExecuteChat(promptItem.ChatMode, chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }


        // Chatを実行して複雑な結果を取得する
        public static async Task<Dictionary<string, dynamic?>> CreateTableChatResult(ChatSettings chatSettings, PromptItem promptItem, string content) {
            ChatRequest chatRequest = new() {
                // OpenAI+RAG Chatを実行
                ContentText = content,
                JsonMode = true
            };
            chatSettings.PromptTemplateText = promptItem.Prompt;
            chatSettings.SplitMode = promptItem.SplitMode;

            ChatRequestContext chatRequestContext = new(chatSettings);

            if (promptItem.RAGMode != RAGModeEnum.None) {
                await chatRequest.ApplyVectorSearchResults(new VectorSearchItem(new VectorDBItem()) {
                    InputText = content,
                    TopK = 3,
                    ScoreThreshold = 0.0f,
                });
            }

            ChatResponse? result = await chatRequest.ExecuteChat(promptItem.ChatMode, chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                // JSON文字列をDictionary<string, dynamic>型に変換
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }

        // 画像からテキストを抽出する
        public static async Task<string> ExtractTextFromImage(ChatSettings chatSettings, List<string> ImageBase64List) {
            ChatRequest chatRequest = new();
            // Normal Chatを実行

            chatRequest.ContentText = "";
            chatRequest.ImageURLs = ImageBase64List.Select(CreateImageURL).ToList();
            if (chatRequest.ImageURLs.Count == 0) {
                return "";
            }

            ChatRequestContext chatRequestContext = new(chatSettings);

            ChatResponse? result = await chatRequest.ExecuteChat(OpenAIExecutionModeEnum.Normal, chatRequestContext, (message) => { });
            if (result != null) {
                return result.Output;
            }
            return "";
        }


        public static string CreateImageURLFromFilePath(string filePath) {
            // filePathから画像のBase64文字列を作成
            byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
            string result = CreateImageURLFromBytes(imageBytes);
            return result;
        }

        public static string CreateImageURL(string base64String) {

            (bool isImage, ContentItemTypes.ImageType imageType) = ContentItemTypes.GetImageTypeFromBase64(base64String);
            if (imageType == ContentItemTypes.ImageType.unknown) {
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

        public static async Task<List<(ContentItemTypes.ContentItemTypeEnum, string)>> CreatePromptTextByRelatedItems(ChatRelatedItems relatedItems) {
            List<ContentItem> items = relatedItems.ContentItems;
            List<ContentItemDataDefinition> dataDefinitions = relatedItems.DataDefinitions;
            // ContentItemのリストとContentItemDataDefinitionのリストを受け取り、PromptTextを作成する
            StringBuilder promptBuilder = new();
            // PythonNetの処理を呼び出す。
            List<(ContentItemTypes.ContentItemTypeEnum, string)> data = [];
            // ApplicationItemのリスト要素毎に処理を行う
            foreach (var applicationItem in items) {
                bool exportProperties = dataDefinitions.FirstOrDefault(x => x.Name == ContentItemDataDefinition.PropertiesName)?.IsChecked ?? false;
                if (exportProperties) {
                    // Propertiesを追加
                    var headerText = await applicationItem.GetHeaderTextAsync();
                    data.Add((ContentItemTypes.ContentItemTypeEnum.Text, headerText));
                }
                bool exportTitle = dataDefinitions.FirstOrDefault(x => x.Name == ContentItemDataDefinition.TitleName)?.IsChecked ?? false;
                if (exportTitle) {
                    data.Add((ContentItemTypes.ContentItemTypeEnum.Text, applicationItem.Description));
                }
                // Path
                bool exportSourcePath = dataDefinitions.FirstOrDefault(x => x.Name == ContentItemDataDefinition.SourcePathName)?.IsChecked ?? false;
                if (exportSourcePath) {
                    data.Add((ContentItemTypes.ContentItemTypeEnum.Text, applicationItem.SourcePath));
                }
                // Text
                bool exportText = dataDefinitions.FirstOrDefault(x => x.Name == ContentItemDataDefinition.TextName)?.IsChecked ?? false;
                if (exportText) {
                    data.Add((ContentItemTypes.ContentItemTypeEnum.Text, applicationItem.Content));
                }
                // Image
                bool exportImage = dataDefinitions.FirstOrDefault(x => x.Name == ContentItemDataDefinition.ImageName)?.IsChecked ?? false;
                if (exportImage) {
                    var imageURL = CreateImageURL(applicationItem.Base64Image);
                    data.Add((ContentItemTypes.ContentItemTypeEnum.Image, imageURL));
                }

                // PromptItemのリスト要素毎に処理を行う
                foreach (var promptItem in dataDefinitions.Where(x => x.IsPromptItem)) {
                    if (promptItem.IsChecked) {
                        string promptResult = applicationItem.PromptChatResult.GetTextContent(promptItem.Name);
                        data.Add((ContentItemTypes.ContentItemTypeEnum.Text, promptResult));
                    }
                }

            }
            return data;
        }

    }
}
