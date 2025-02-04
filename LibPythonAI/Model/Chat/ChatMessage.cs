using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Resources;

namespace PythonAILib.Model.Chat {
    public class ChatMessage {

        public static string SystemRole { get; } = "system";
        public static string AssistantRole { get; } = "assistant";
        public static string UserRole { get; } = "user";

        [JsonPropertyName("role")]
        public string Role { get; set; } = SystemRole;

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        // JSON化しないプロパティ
        [JsonIgnore]
        public List<Dictionary<string, string>> Sources { get; set; } = [];

        [JsonIgnore]
        public string SourceDocumentURL { get; set; } = "";

        // ImageのURLのリスト
        [JsonIgnore]
        public List<string> ImageURLs { get; set; } = [];

        [JsonIgnore]
        // SourcePath + Sourcesを返す
        public string ContentWithSources {
            get {
                if (Sources.Count == 0) {
                    return Content;
                }
                // sourceDocumentURLが空の場合は<参照元ドキュメントルート>とする。
                if (string.IsNullOrEmpty(SourceDocumentURL)) {
                    SourceDocumentURL = PythonAILibStringResources.Instance.SourceDocumentRoot;
                }
                // Sourcesの各要素にSourceDocumentURLを付加する。
                List<string> SourcesWithLink = Sources.ConvertAll(x => SourceDocumentURL + x);
                return Content + "\n" + string.Join("\n", SourcesWithLink);
            }
        }

        public ChatMessage(string role, string text) {
            Role = role;
            Content = text;
        }
        public ChatMessage(string role, string text, List<Dictionary<string,string>> sources) {
            Role = role;
            Content = text;
            Sources = sources;
        }
        // Source

        // Copy
        public ChatMessage Copy() {
            ChatMessage result = new(Role, Content, Sources) {
                ImageURLs = ImageURLs,
                SourceDocumentURL = SourceDocumentURL
            };
            return result;
        }

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            // type: textと、image_urlの2種類を格納
            List<Dictionary<string, object>> contentItems = [];

            Dictionary<string, object> dc = new() {
                ["type"] = "text",
                ["text"] = Content
            };
            contentItems.Add(dc);
            foreach (var imageURL in ImageURLs) {
                // ImageURLプロパティを追加
                dc = new Dictionary<string, object> {
                    ["type"] = "image_url",
                    ["image_url"] = new Dictionary<string, object> {
                        ["url"] = imageURL
                    }
                };
                contentItems.Add(dc);
            }
 
            Dictionary<string, object> message = new() {
                { "role", Role },
                { "content", contentItems }
            };
            return message;
        }

        public static List<Dictionary<string, object>> ToDictList(List<ChatMessage> messagesList) {
            List<Dictionary<string, object>> messageValues = [];
            foreach (var message in messagesList) {
                messageValues.Add(message.ToDict());
            }
            return messageValues;
        }
    }
}
