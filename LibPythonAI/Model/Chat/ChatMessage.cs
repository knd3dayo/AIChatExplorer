using System.Text.Json.Serialization;
using System.Windows.Documents;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using PythonAILib.Resources;
using PythonAILib.Utils.Common;

namespace LibPythonAI.Model.Chat {
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
        public List<VectorDBEmbedding> SourceDocuments { get; set; } = [];

        // ImageのURLのリスト
        [JsonIgnore]
        public List<string> ImageURLs { get; set; } = [];

        [JsonIgnore]
        // SourcePath + Sourcesを返す
        public string ContentWithSources {
            get {
                if (SourceDocuments.Count == 0) {
                    return Content;
                }
                return Content + "\n\n" + PythonAILibStringResources.Instance.ReferenceDocument + "\n" + string.Join("  \n", SourceDocuments.Select(x => x.SourcePath));
            }
        }
        [JsonIgnore]
        public FlowDocument? MarkdownContentWithSources {
            get {
                return  Tools.CreateFlowDocument(ContentWithSources);
            }
        }

        public ChatMessage(string role, string text) {
            Role = role;
            Content = text;
        }
        public ChatMessage(string role, string text, List<VectorDBEmbedding> sources) {
            Role = role;
            Content = text;
            SourceDocuments = sources;
        }
        // Source

        // Copy
        public ChatMessage Copy() {
            ChatMessage result = new(Role, Content, SourceDocuments) {
                ImageURLs = ImageURLs,
                SourceDocuments = SourceDocuments
            };
            return result;
        }
        // SourcesString
        public string SourcesString {
            get => string.Join("\n", SourceDocuments.Select(x => x.SourcePath));
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

        public static ChatMessage FromDict(Dictionary<string, dynamic?> dict) {
            string role = SystemRole;
            if (dict.TryGetValue("role", out object? value)) {
                if (value != null) {
                    role = (string)value;
                }
            }

            List<Dictionary<string, string>> sources = [];
            if (dict.TryGetValue("sources", out object? value1)) {
                if (value1 != null) {
                    sources = (List<Dictionary<string, string>>)value1;
                }
            }
            List<string> imageURLs = [];
            if (dict.TryGetValue("image_urls", out object? value2)) {
                if (value2 != null) {
                    imageURLs = (List<string>)value2;
                }
            }
            // content
            string content = "";
            if (dict.TryGetValue("content", out object? value3)) {
                if (value3 != null) {
                    content = (string)value3;
                }
            }
            ChatMessage message = new(role, content) {
                ImageURLs = imageURLs
            };
            return message;

        }

        public static List<ChatMessage> FromDictList(List<Dictionary<string, dynamic?>> dictList) {
            List<ChatMessage> messages = [];
            foreach (var dict in dictList) {
                messages.Add(FromDict(dict));
            }
            return messages;
        }
        public static List<ChatMessage> FromListJson(string jsonString) {
            List<Dictionary<string, dynamic?>> dictList = JsonUtil.ParseJsonArray(jsonString);
            return FromDictList(dictList);
        }
    }
}
