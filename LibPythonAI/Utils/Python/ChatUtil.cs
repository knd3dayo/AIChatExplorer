using System.Text;
using System.Text.Json;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.File;
using LibPythonAI.Model.Folder;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Utils.Python {
    public class ChatUtil {

        // JSON形式の結果をパースしてリストに変換
        private static readonly JsonSerializerOptions options = new() {
            PropertyNameCaseInsensitive = true
        };

        // Chatを実行して文字列の結果を取得する
        public static async Task<string> CreateTextChatResult(ChatRequestContext chatRequestContext, PromptItem promptText, string content) {
            ChatRequest chatRequest = new() {
                // NormalChat, OpenAI+RAG Chat, LangChainChatを実行
                ContentText = content,
            };

            chatRequestContext.PromptTemplateText = promptText.Prompt;
            chatRequestContext.SplitMode = promptText.SplitMode;

            ChatResponse? result = await ExecuteChat(promptText.ChatMode, chatRequest, chatRequestContext, (message) => { });
            if (result != null) {
                return result.Output;
            }
            return "";
        }

        // Chatを実行した結果を次の質問に渡すことを繰り返して文字列の結果を取得する
        public static async Task<string> CreateTextChatResult(OpenAIExecutionModeEnum chatMode, SplitModeEnum splitMode, ChatRequestContext chatRequestContext, List<VectorSearchItem> vectorDBProperties, List<string> promptList, string content) {
            string resultString = content;
            foreach (string prompt in promptList) {
                ChatRequest chatRequest = new() {
                    ContentText = resultString,
                };

                chatRequestContext.PromptTemplateText = prompt;


                ChatResponse? result = await ExecuteChat(chatMode, chatRequest, chatRequestContext, (message) => { });
                if (result != null) {
                    resultString = result.Output;
                }
            }
            return resultString;
        }

        // Chatを実行してリストの結果を取得する
        public static async Task<List<string>> CreateListChatResult(ChatRequestContext chatRequestContext, PromptItem promptItem, string content) {

            string promptText = PromptStringResourceJa.Instance.JsonStringListGenerationPrompt + "\n" + promptItem.Prompt;
            ChatRequest chatRequest = new() {
                // OpenAI+RAG Chatを実行
                ContentText = content,
                JsonMode = true
            };
            chatRequestContext.PromptTemplateText = promptText;
            chatRequestContext.SplitMode = promptItem.SplitMode;

            ChatResponse? result = await ExecuteChat(promptItem.ChatMode, chatRequest, chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {

                Dictionary<string, List<string>> jsonResult = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(result.Output, options) ?? [];
                if (jsonResult.TryGetValue("result", out List<string>? value)) {
                    return value;
                }
            }
            return [];
        }
        // CHatを実行してDictionary<string, object>の結果を取得する
        public static async Task<Dictionary<string, dynamic?>> CreateDictionaryChatResult(ChatRequestContext chatRequestContext, PromptItem promptItem, string content) {
            ChatRequest chatRequest = new() {
                // OpenAI+RAG Chatを実行
                ContentText = content,
                JsonMode = true
            };

            chatRequestContext.PromptTemplateText = promptItem.Prompt;
            chatRequestContext.SplitMode = promptItem.SplitMode;

            ChatResponse? result = await ExecuteChat(promptItem.ChatMode, chatRequest, chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }


        // Chatを実行して複雑な結果を取得する
        public static async Task<Dictionary<string, dynamic?>> CreateTableChatResult(ChatRequestContext chatRequestContext, PromptItem promptItem, string content) {
            ChatRequest chatRequest = new() {
                // OpenAI+RAG Chatを実行
                ContentText = content,
                JsonMode = true
            };
            chatRequestContext.PromptTemplateText = promptItem.Prompt;
            chatRequestContext.SplitMode = promptItem.SplitMode;

            ChatResponse? result = await ExecuteChat(promptItem.ChatMode, chatRequest, chatRequestContext, (message) => { });
            if (result != null && !string.IsNullOrEmpty(result.Output)) {
                // JSON文字列をDictionary<string, dynamic>型に変換
                return JsonUtil.ParseJson(result.Output);
            }
            return [];
        }

        // 画像からテキストを抽出する
        public static async Task<string> ExtractTextFromImage(ChatRequestContext chatRequestContext, List<string> ImageBase64List) {
            ChatRequest chatRequest = new();
            // Normal Chatを実行
            chatRequestContext.PromptTemplateText = PromptStringResourceJa.Instance.ExtractTextRequest;
            chatRequest.ContentText = "";
            chatRequest.ImageURLs = ImageBase64List.Select(CreateImageURL).ToList();
            if (chatRequest.ImageURLs.Count == 0) {
                return "";
            }

            ChatResponse? result = await ExecuteChat(OpenAIExecutionModeEnum.Normal, chatRequest, chatRequestContext, (message) => { });
            if (result != null) {
                return result.Output;
            }
            return "";
        }


        public static void CancelAutoGenChat(string sessionToken) {
            PythonExecutor.PythonAIFunctions?.CancelAutoGenChat(sessionToken);
        }

        public static async Task<ChatResponse?> ExecuteAutoGenGroupChat(ChatRequestContext chatRequestContext, ChatRequest chat, Action<string> iteration) {
            ChatResponse? result = await PythonExecutor.PythonAIFunctions.AutoGenGroupChatAsync(chatRequestContext, chat, iteration);
            return result;
        }

        public static async Task<ChatResponse?> ExecuteChatNormal(ChatRequestContext chatRequestContext, ChatRequest chat) {
            // Ensure PythonAIFunctions is not null before calling OpenAIChatAsync
            if (PythonExecutor.PythonAIFunctions == null) {
                return null;
            }

            ChatResponse? result = await PythonExecutor.PythonAIFunctions.OpenAIChatAsync(chatRequestContext, chat);
            if (result == null) {
                return null;
            }

            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            chat.ChatHistory.Add(new ChatMessage(ChatMessage.AssistantRole, result.Output, result.SourceDocuments));
            return result;
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

        public static List<(ContentItemTypes.ContentItemTypeEnum, string)> CreatePromptTextByRelatedItems(List<ContentItemWrapper> items, List<ContentItemDataDefinition> dataDefinitions) {
            // ContentItemWrapperのリストとContentItemDataDefinitionのリストを受け取り、PromptTextを作成する
            StringBuilder promptBuilder = new();
            // PythonNetの処理を呼び出す。
            List<(ContentItemTypes.ContentItemTypeEnum, string)> data = [];
            // ApplicationItemのリスト要素毎に処理を行う
            foreach (var applicationItem in items) {
                bool exportProperties = dataDefinitions.FirstOrDefault(x => x.Name == ContentItemDataDefinition.PropertiesName)?.IsChecked ?? false;
                if (exportProperties) {
                    // Propertiesを追加
                        data.Add((ContentItemTypes.ContentItemTypeEnum.Text, applicationItem.HeaderText));
                }
                bool exportTitle = dataDefinitions.FirstOrDefault(x => x.Name == ContentItemDataDefinition.TitleName)?.IsChecked ?? false;
                if (exportTitle) {
                    data.Add(( ContentItemTypes.ContentItemTypeEnum.Text, applicationItem.Description));
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
        public static async Task<ChatResponse?> ExecuteChat(OpenAIExecutionModeEnum chatMode, ChatRequest chatRequest, ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            // 通常のOpenAI Chatを実行する
            if (chatMode == OpenAIExecutionModeEnum.Normal) {

                // リクエストメッセージを最新化
                PrepareNormalRequest(chatRequestContext, chatRequest);
                // 通常のChatを実行する。
                return await ExecuteChatNormal(chatRequestContext, chatRequest);
            }
            // AutoGenGroupChatを実行する
            if (chatMode == OpenAIExecutionModeEnum.AutoGenGroupChat) {
                // AutoGenGroupChatを実行する
                return await ExecuteAutoGenGroupChat(chatRequest, chatRequestContext, iterateAction);
            }
            return null;
        }

        // AutoGenGroupChatを実行する
        public static async Task<ChatResponse?> ExecuteAutoGenGroupChat(ChatRequest chatRequest, ChatRequestContext chatRequestContext, Action<string> iterateAction) {
            // リクエストメッセージを最新化
            PrepareNormalRequest(chatRequestContext, chatRequest);

            // 結果
            ChatMessage result = new(ChatMessage.AssistantRole, "", []);
            chatRequest.ChatHistory.Add(result);
            // AutoGenGroupChatを実行する
            return await ExecuteAutoGenGroupChat(chatRequestContext, chatRequest, (message) => {
                result.Content += message + "\n";
                iterateAction(message);
            });
        }


    }
}
