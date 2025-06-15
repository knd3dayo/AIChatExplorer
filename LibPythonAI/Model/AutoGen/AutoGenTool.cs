using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibPythonAI.PythonIF;

namespace LibPythonAI.Model.AutoGen {
    public class AutoGenTool {

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("path")]
        public string Path { get; set; } = "";

        // ToJson
        public string ToJson() {
            // Serialize the object to JSON
            string jsonString = JsonSerializer.Serialize(this, jsonSerializerOptions);
            return jsonString;
        }

        // FromDict
        public static AutoGenTool FromDict(Dictionary<string, object> dict) {
            AutoGenTool config = new() {
                Name = dict["name"]?.ToString() ?? "",
                Description = dict["description"]?.ToString() ?? "",
                Path = dict["path"]?.ToString() ?? "",
            };
            return config;
        }

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            // Create a dictionary
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "name", Name },
                { "description", Description },
                { "path", Path },
            };
            return dict;
        }
        // CreateEntriesDictList
        public static List<Dictionary<string, object>> ToDictList(List<AutoGenTool> data) {
            List<Dictionary<string, object>> dictList = new List<Dictionary<string, object>>();
            foreach (AutoGenTool item in data) {
                dictList.Add(item.ToDict());
            }
            return dictList;
        }

        public void SaveAsync() {
            // APIを呼び出して、設定を保存する
             PythonExecutor.PythonAIFunctions.UpdateAutoGenToolAsync(this);
        }

        public void DeleteAsync() {
            // APIを呼び出して、設定を削除する
             PythonExecutor.PythonAIFunctions.DeleteAutoGenToolAsync(this);
        }

        // GetAutoGenToolListAsync
        public static async Task<List<AutoGenTool>> GetAutoGenToolListAsync() {
            // APIを呼び出して、設定を取得する
            List<AutoGenTool> autoGenTools = await PythonExecutor.PythonAIFunctions.GetAutoGenToolListAsync();
            return autoGenTools;

        }

    }
}
