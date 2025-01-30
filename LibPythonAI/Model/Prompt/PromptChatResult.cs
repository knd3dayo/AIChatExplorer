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


    }
}
