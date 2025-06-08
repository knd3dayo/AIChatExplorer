using System.Text.Json;
using System.Text.Json.Serialization;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;

namespace LibPythonAI.Model.AutoGen {
    public class AutoGenAgent {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
            WriteIndented = true
        };

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        // system_message
        [JsonPropertyName("system_message")]
        public string SystemMessage { get; set; } = "";

        // type
        [JsonPropertyName("type_value")]
        public string TypeValue { get; set; } = "";

        [JsonPropertyName("tool_names")]
        public List<string> ToolNames { get; set; } = new List<string>();

        // code_execution 
        [JsonPropertyName("code_execution")]
        public bool CodeExecution { get; set; } = false;

        // llm
        [JsonPropertyName("llm_config_name")]
        public string LLMConfigName { get; set; } = "";

        // List(VectorDBItem)
        [JsonPropertyName("vector_db_items")]
        public List<VectorDBItem> VectorDBItems { get; set; } = [];


        // VectorDBSearchAgent
        [JsonPropertyName("vector_db_search_agent")]
        public bool VectorDBSearchAgent { get; set; } = false;

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            // Create a dictionary
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "name", Name },
                { "description", Description },
                { "system_message", SystemMessage },
                { "type_value", TypeValue },
                { "tool_names", ToolNames },
                { "code_execution", CodeExecution },
                { "llm_config_name", LLMConfigName },
                { "vector_db_items", VectorDBItemRequest.ToDictList(VectorDBItems.Select(x => new VectorDBItemRequest(x))) },
                { "vector_db_search_agent", VectorDBSearchAgent },

            };
            return dict;
        }


        // CreateEntriesDictList
        public static List<Dictionary<string, object>> ToDictList(List<AutoGenAgent> data) {
            // Create a list of dictionaries
            List<Dictionary<string, object>> dictList = [];
            foreach (AutoGenAgent item in data) {
                dictList.Add(item.ToDict());
            }
            return dictList;
        }

        // SaveAsync
        public async Task SaveAsync() {
            // APIを呼び出して、設定を保存する
            await PythonExecutor.PythonAIFunctions.UpdateAutoGenAgentAsync(this);
        }
        // DeleteAsync
        public async Task DeleteAsync() {
            // APIを呼び出して、設定を削除する
            await PythonExecutor.PythonAIFunctions.DeleteAutoGenAgentAsync(this);

        }

        // ToJson
        public string ToJson() {
            // Serialize the object to JSON
            string jsonString = JsonSerializer.Serialize(this, jsonSerializerOptions);
            return jsonString;
        }
        // FromDict
        public static AutoGenAgent FromDict(Dictionary<string, object> dict) {
            // Deserialize the dictionary to an object
            string jsonString = JsonSerializer.Serialize(dict, jsonSerializerOptions);
            AutoGenAgent data = JsonSerializer.Deserialize<AutoGenAgent>(jsonString, jsonSerializerOptions)!;
            return data;
        }

        // GetAutoGenAgentList
        public static async Task<List<AutoGenAgent>> GetAutoGenAgentList() {
            // APIを呼び出して、設定を取得する
            List<AutoGenAgent> agents = await PythonExecutor.PythonAIFunctions.GetAutoGenAgentListAsync();
            return agents;
        }

    }
}
