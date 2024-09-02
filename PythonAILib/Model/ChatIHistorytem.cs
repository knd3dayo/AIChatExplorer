using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace PythonAILib.Model {
    public class ChatIHistorytem {

        public static string SystemRole { get; } = "system";
        public static string AssistantRole { get; } = "assistant";
        public static string UserRole { get; } = "user";

        [JsonPropertyName("role")]
        public string Role { get; set; } = SystemRole;

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        // JSON化しないプロパティ
        [JsonIgnore]
        public List<string> Sources { get; set; } = [];

        [JsonIgnore]
        public string SourceDocumentURL { get; set; } = "";

        // ImageのURLのリスト
        [JsonIgnore]
        public List<string> ImageURLs { get; set; } = [];

        [JsonIgnore]
        // Content + Sourcesを返す
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

        public ChatIHistorytem(string role, string text) {
            Role = role;
            Content = text;
        }
        public ChatIHistorytem(string role , string text , List<string> sources)  {
            Role = role;
            Content = text;
            Sources = sources;
        }
        // ChatItemsをJSON文字列に変換する
        public static string ToJson(IEnumerable<ChatIHistorytem> items) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(items, options);
        }


    }
}
