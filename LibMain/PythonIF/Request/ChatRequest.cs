using System.Text;
using LibMain.Common;
using LibMain.Model.Chat;
using LibMain.Model.Content;
using LibMain.Resources;
using LibMain.Utils.Python;

namespace LibMain.PythonIF.Request {
    /// <summary>
    /// ChatItemの履歴、
    /// </summary>
    public class ChatRequest {

        public const string MODEL_KEY = "model";
        public const string TEMPERATURE_KEY = "temperature";
        public const string MAX_TOKENS_KEY = "max_tokens";
        public const string TYPE_KEY = "type";
        public const string CONTENT_KEY = "content";
        public const string IMAGE_URL_KEY = "image_url";
        public const string JSON_MODE_KEY = "json_mode";
        public const string MESSAGES_KEY = "messages";
        public const string RESPONSE_FORMAT_KEY = "response_format";

        public ChatRequest() {
            var props = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties();
            Model = props.OpenAICompletionModel;
        }

        public string Model { get; private set; }

        public double Temperature { get; set; } = 0.5;

        protected int MaxTokens { get; set; } = 0;

        public List<ChatMessage> ChatHistory { get; set; } = [];

        public string ContentText { get; set; } = "";

        // ImageのURLのリスト. data:image/{formatText};base64,{base64String}という形式の文字列のリスト
        public List<string> ImageURLs { get; set; } = [];

        public bool JsonMode { get; set; } = false;


        public ChatMessage? GetLastSendItem() {
            // ChatItemsのうち、ユーザー発言の最後のものを取得
            var lastUserChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatMessage.UserRole);
            return lastUserChatItem;
        }

        public ChatMessage? GetLastResponseItem() {
            // ChatItemsのうち、アシスタント発言の最後のものを取得
            var lastAssistantChatItem = ChatHistory.LastOrDefault(x => x.Role == ChatMessage.AssistantRole);
            return lastAssistantChatItem;
        }

        public async Task ApplyReletedItems(ChatRelatedItems relatedItems) {
            // relaedItemsのSendRelatedItemsOnlyFirstRequestがtrueの場合かつChatHistoryが1より大きい場合は、関連アイテムを送信しない
            if (relatedItems.SendRelatedItemsOnlyFirstRequest && ChatHistory.Count > 1) {
                return;
            }
            // ContentItemのリストとContentItemDataDefinitionのリストを受け取り、ChatHistoryに追加する
            List<(ContentItemTypes.ContentItemTypeEnum, string)> values = await ChatUtil.CreatePromptTextByRelatedItems(relatedItems);
            StringBuilder sb = new();
            foreach (var value in values) {
                // ContentItemTypes.ContentItemTypeEnumの値に応じて、ChatMessageを作成
                if (value.Item1 == ContentItemTypes.ContentItemTypeEnum.Text) {
                    sb.AppendLine(value.Item2);
                } else if (value.Item1 == ContentItemTypes.ContentItemTypeEnum.Image) {
                    if (string.IsNullOrEmpty(value.Item2) == false) {
                        // 画像の場合は、ImageURLを設定する
                        ImageURLs.Add(value.Item2);
                    }
                }
            }
            // ContentTextに +\n---参考情報---+\n + sbの内容を追加
            if (sb.Length > 0) {
                ContentText += $"\n---{PythonAILibStringResources.Instance.ReferenceInformation}---\n" + sb.ToString();
            }

        }

        public Dictionary<string, object> ToDict() {
            // OpenAIのAPIに送信するJSONを作成


            // model, messages, temperature, response_format, max_tokensを設定する.
            var dc = new Dictionary<string, object> {
                [MODEL_KEY] = Model,
                [MESSAGES_KEY] = ChatMessage.ToDictList(ChatHistory),
            };
            // modelがo*以外の場合は、temperatureを設定する
            if (Model.StartsWith("o") == false) {
                dc[TEMPERATURE_KEY] = Temperature;
            }
            // jsonModeがTrueの場合は、response_formatを json_objectに設定する
            if (JsonMode) {
                Dictionary<string, string> responseFormat = new() {
                    [TYPE_KEY] = "json_object"
                };
                dc[RESPONSE_FORMAT_KEY] = responseFormat;

            }
            // maxTokensが0より大きい場合は、max_tokensを設定する
            if (MaxTokens > 0) {
                dc[MAX_TOKENS_KEY] = MaxTokens;
            }

            return dc;
        }

        public ChatRequest Copy() {
            // ChatRequestのコピーを作成
            ChatRequest result = new() {
                Model = Model,
                Temperature = Temperature,
                MaxTokens = MaxTokens,
                ContentText = ContentText,
                JsonMode = JsonMode,
                ImageURLs = [.. ImageURLs],
                ChatHistory = [.. ChatHistory]
            };
            return result;
        }

    }
}
