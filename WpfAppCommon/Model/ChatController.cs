using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using MS.WindowsAPICodePack.Internal;
using QAChat.Model;
using WpfAppCommon.PythonIF;

namespace WpfAppCommon.Model {
    /// <summary>
    /// ChatItemの履歴、
    /// </summary>
    public class ChatController {

        public OpenAIExecutionModeEnum ChatMode = OpenAIExecutionModeEnum.RAG;

        public List<ChatItem> ChatItems = [];

        public ChatItem? LastSendItem {
            get {
                // ChatItemsのうち、ユーザー発言の最後のものを取得
                var lastUserChatItem = ChatItems.LastOrDefault(x => x.Role == ChatItem.UserRole);
                return lastUserChatItem;
            }
        }
        public ChatItem? LastResponseItem {
            get {
                // ChatItemsのうち、アシスタント発言の最後のものを取得
                var lastAssistantChatItem = ChatItems.LastOrDefault(x => x.Role == ChatItem.AssistantRole);
                return lastAssistantChatItem;
            }
        }

        public string PromptTemplateText { get; set; } = "";
        public string ContentText { get; set; } = "";

        public List<string> ImageBase64Strings = [];

        public List<ClipboardItem> ContextItems = [];

        public IEnumerable<VectorDBItem> VectorDBItems = [];

        public string CreatePromptText() {
            // PromptTextを作成
            string promptText = "";

            // PromptTemplateTextが空でない場合は、PromptTemplateTextを追加
            if (string.IsNullOrEmpty(PromptTemplateText) == false) {
                promptText = PromptTemplateText + "\n---------以下は本文です------\n";
            }
            // ContentTextを追加
            promptText += ContentText;

            // ContextItemsが空でない場合は、ContextItemsのContentを追加
            if (ContextItems.Any()) {
                promptText += "\n---------以下は関連情報です------\n";
                // ContextItemsのContentを追加
                foreach (var item in ContextItems) {
                    promptText += item.Content + "\n";
                }
            }
            return promptText;
        }

        public static string CreateImageURLFromFilePath(string filePath) {
            // filePathから画像のBase64文字列を作成
            byte[] imageBytes = File.ReadAllBytes(filePath);
            string base64String = Convert.ToBase64String(imageBytes);
            string result = CreateImageURLFromBase64String(base64String);
            return result;
        }

        public static string CreateImageURLFromBase64String(string base64String) {
            // Base64文字列から画像のURLを作成
            string result = $"data:application/octet-stream;base64,{base64String}";
            return result;
        }


        public List<Dictionary<string, string>> CreateOpenAIContentList(string content, List<string> imageBase64String) {

            //OpenAIのリクエストパラメーターのContent部分のデータを作成
            List<Dictionary<string, string>> parameters = [];
            // Contentを作成
            var dc = new Dictionary<string, string> {
                ["type"] = "text",
                ["text"] = content
            };
            parameters.Add(dc);

            foreach (var base64string in imageBase64String) {
                // ImageURLプロパティを追加
                dc = new Dictionary<string, string> {
                    ["type"] = "image_url",
                    ["image_url"] = CreateImageURLFromBase64String(base64string)
                };
                parameters.Add(dc);
            }
            return parameters;
        }
        public List<Dictionary<string, object>> CreateOpenAIMessagesList() {
            //OpenAIのリクエストパラメーターのMessage部分のデータを作成
            // Messages部分はRoleとContentからなるDictionaryのリスト
            List<Dictionary<string, object>> messages = [];
            foreach (var item in ChatItems) {
                var itemDict = new Dictionary<string, object> {
                    ["role"] = item.Role,
                    ["content"] = CreateOpenAIContentList(item.Content, item.ImageBase64Strings)
                };
                messages.Add(itemDict);
            }
            // このオブジェクトのプロパティを基にしたContentを作成
            var dc = new Dictionary<string, object> {
                ["role"] = ChatItem.UserRole,
                ["content"] = CreateOpenAIContentList(CreatePromptText(), ImageBase64Strings)
            };
            messages.Add(dc);

            return messages;

        }

        public string CreateOpenAIRequestJSON(double temperature = 0.5, bool jsonMode = false, int maxTokens = 0) {
            // OpenAIのAPIに送信するJSONを作成

            // ClipboardAppConfigの設定を取得
            // ChatModel
            string model = ClipboardAppConfig.OpenAICompletionModel;

            // model, messages, temperature, response_format, max_tokensを設定する.
            var dc = new Dictionary<string, object> {
                ["model"] = model,
                ["messages"] = CreateOpenAIMessagesList(),
                ["temperature"] = temperature
            };
            // jsonModeがTrueの場合は、response_formatを json_objectに設定する
            if (jsonMode) {
                dc["response_format"] = "type:json_object";
            }
            // maxTokensが0より大きい場合は、max_tokensを設定する
            if (maxTokens > 0) {
                dc["max_tokens"] = maxTokens;
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
            string prompt = CreatePromptText();
            string requestJson = CreateOpenAIRequestJSON();
            // ChatModeがRAGの場合は、RAGChatを実行する。
            if (ChatMode == OpenAIExecutionModeEnum.RAG) {
                ChatResult? result = PythonExecutor.PythonFunctions?.LangChainChat(ClipboardAppConfig.CreateOpenAIProperties(), VectorDBItems, requestJson);
                if (result == null) {
                    return null;
                }
                // リクエストをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatItem.UserRole, prompt));
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                ChatItems.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));

                return result;

            }
            if (ChatMode == OpenAIExecutionModeEnum.Normal) {
                ChatResult? result = PythonExecutor.PythonFunctions?.OpenAIChat(ClipboardAppConfig.CreateOpenAIProperties(), requestJson);
                // リクエストをChatItemsに追加
                if (result == null) {
                    return null;
                }
                ChatItems.Add(new ChatItem(ChatItem.UserRole, prompt));
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                ChatItems.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));

                return result;
            }
            return null;
        }

        public void SetChatItems(ClipboardItem clipboardItem) {
            // ClipboardItemのChatItemsを設定
            clipboardItem.ChatItems.Clear();
            foreach (var item in ChatItems) {
                clipboardItem.ChatItems.Add(item);
            }
        }

    }
}
