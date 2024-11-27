using System.Text.Json.Serialization;
using PythonAILib.Common;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenAgent {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        // system_message
        [JsonPropertyName("system_message")]
        public string SystemMessage { get; set; } = "";

        // type
        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        public List<string> ToolNamesList { get; set; } = new List<string>();

        // tools
        [JsonPropertyName("tool_names")]
        public string ToolNames {
            get {
                return string.Join(",", ToolNamesList);
            }
        }

        // human_input_mode
        [JsonPropertyName("human_input_mode")]
        public string HumanInputMode { get; set; } = "";

        // termination_msg
        [JsonPropertyName("termination_msg")]
        public string TerminationMsg { get; set; } = "";

        // code_execution 
        [JsonPropertyName("code_execution")]
        public bool CodeExecution { get; set; } = false;

        // llm
        [JsonPropertyName("llm")]
        public bool Llm { get; set; } = false;

        // ToDictList
        public static Dictionary<string, object> ToDict(AutoGenAgent data) {
            // Create a dictionary
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "name", data.Name },
                { "description", data.Description },
                { "system_message", data.SystemMessage },
                { "type", data.Type },
                { "tool_names", data.ToolNames },
                { "human_input_mode", data.HumanInputMode },
                { "termination_msg", data.TerminationMsg },
                { "code_execution", data.CodeExecution },
                { "llm", data.Llm }
            };
            return dict;
        }
        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<AutoGenAgent> data) {
            // Create a list of dictionaries
            List<Dictionary<string, object>> dictList = [];
            foreach (AutoGenAgent item in data) {
                dictList.Add(ToDict(item));
            }
            return dictList;
        }

        // Save
        public void Save() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenAgentCollection<AutoGenAgent>();
            collection.Upsert(this);
        }
        // Delete
        public void Delete() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenAgentCollection<AutoGenAgent>();
            collection.Delete(this.Id);
        }

        // FindAll
        public static List<AutoGenAgent> FindAll() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenAgentCollection<AutoGenAgent>();
            return collection.FindAll().ToList();
        }
    }
}
