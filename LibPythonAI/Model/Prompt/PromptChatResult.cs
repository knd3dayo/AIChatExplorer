using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Utils.Common;

namespace PythonAILib.Model.Prompt {
    public class PromptChatResult() {

        public Dictionary<string, object> Results { get; set; } = [];

        public string GetTextContent(string promptName) {
            return Results.TryGetValue(promptName, out object? value) ? (string)value : "";
        }
        public void SetTextContent(string promptName, string content) {
            Results[promptName] = content;
        }

        public List<string> GetListContent(string promptName) {
            return Results.TryGetValue(promptName, out object? value) ? (List<string>)value : [];
        }
        public void SetListContent(string promptName, List<string> content) {
            Results[promptName] = content;
        }

        public List<Dictionary<string, object>> GetTableContent(string promptName) {
            Results.TryGetValue(promptName, out object? values);
            if (values == null) {
                return [];
            }

            if (values is List<Dictionary<string, object>> list) {
                return list;
            }
            if (values is Object[] list2) {
                return list2.Select(x => (Dictionary<string, object>)x).ToList();

            }

            return [];
        }
        public void SetTableContent(string promptName, List<Dictionary<string, object>> content) {
            Results[promptName] = content;
        }

        public string ToJson() {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(this, options);
        }

        public static PromptChatResult? FromDict(Dictionary<string, dynamic?> dict) {
            PromptChatResult? result = new();
            // key = Results があるかどうか
            if (!dict.TryGetValue("Results", out object? value)) {
                return result;
            }
            // Resultsの中身を取り出す
            if (value is Dictionary<string, object> results) {
                result.Results = results;
            }
            return result;
        }

        public static PromptChatResult? FromJson(string json) {
            Dictionary<string, dynamic?> dict = JsonUtil.ParseJson(json);
            return FromDict(dict);
        }
    }
}
