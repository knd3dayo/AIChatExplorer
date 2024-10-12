using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonAILib.Model.Prompt {
    public class PromptChatResult() {

        public Dictionary<string, object> Results { get; set; } = [];

        public string GetTextContent(string promptName) {
            return Results.ContainsKey(promptName) ? (string)Results[promptName] : "";
        }
        public void SetTextContent(string promptName, string content) {
            Results[promptName] = content;
        }

        public List<string> GetListContent(string promptName) {
            return Results.ContainsKey(promptName) ? (List<string>)Results[promptName] : [];
        }
        public void SetListContent(string promptName, List<string> content) {
            Results[promptName] = content;
        }

        public dynamic? GetComplexContent(string promptName) {
            return Results.ContainsKey(promptName) ? Results[promptName]: null;
        }
        public void SetComplexContent(string promptName, dynamic content) {
            Results[promptName] = content;
        }


    }
}
