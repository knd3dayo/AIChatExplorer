using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace QAChat.Model {
    public class ChatItem {

        public static string SystemRole = "system";
        public static string AssistantRole = "assistant";
        public static string UserRole = "user";

        [JsonPropertyName("role")]
        public string Role { get; set; } = SystemRole;

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        // JSON化しないプロパティ
        [JsonIgnore]
        public List<string> Sources { get; set; } = new List<string>();

        [JsonIgnore]
        // Content + Sourcesを返す
        public string ContentWithSources {
            get {
                if (Sources.Count == 0) {
                    return Content;
                }
                string sourceDocumentURL = Properties.Settings.Default.SourceDocumentURL;
                // sourceDocumentURLが空の場合は<参照元ドキュメントルート>とする。
                if (string.IsNullOrEmpty(sourceDocumentURL)) {
                    sourceDocumentURL = "<参照元ドキュメントルート>";
                }
                // Sourcesの各要素にSourceDocumentURLを付加する。
                List<string> SourcesWithLink = Sources.ConvertAll(x => sourceDocumentURL + x);
                return Content + "\n" + string.Join("\n", SourcesWithLink);
            }
        }

        public ChatItem(string role, string text) {
            Role = role;
            Content = text;
        }
        public ChatItem(string role , string text , List<string> sources)  {
            Role = role;
            Content = text;
            Sources = sources;
        }
        // ChatItemsをJSON文字列に変換する
        public static string ToJson(IEnumerable<ChatItem> items) {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return System.Text.Json.JsonSerializer.Serialize(items, options);
        }


    }
    public class ChatResult {
        
        public List<string> ReferencedContents { get; set; } = new List<string>();
        public List<string> ReferencedFilePath { get; set; } = new List<string>();

        public string Response { get; set; } = "";

        public string Verbose { get; set; } = "";

        public ChatResult() {}

    }
}
