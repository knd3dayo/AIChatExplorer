using System.Text.Json.Serialization;

namespace ClipboardApp.Model {
    public class JSONChatItem {
        [JsonPropertyName("role")]
        public string Role { get; set; }
        [JsonPropertyName("content")]
        public string Content { get; set; }
        public JSONChatItem(string role, string content) {
            Role = role;
            Content = content;
        }
    }
}
