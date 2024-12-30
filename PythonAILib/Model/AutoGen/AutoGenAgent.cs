using System.Text.Json.Serialization;
using PythonAILib.Common;
using PythonAILib.Model.VectorDB;

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
        [JsonPropertyName("type_value")]
        public string TypeValue { get; set; } = "";

        [JsonPropertyName("tool_names_for_execution")]
        public List<string> ToolNamesForExecution { get; set; } = new List<string>();

        // tool_names_for_llm
        [JsonPropertyName("tool_names_for_llm")]
        public List<string> ToolNamesForLlm { get; set; } = new List<string>();


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
        [JsonPropertyName("llm_execution")]
        public bool Llm { get; set; } = false;

        // List(VectorDBItem)
        [JsonPropertyName("vector_db_items")]
        public List<VectorDBItem> VectorDBItems { get; set; } = [];


        // ToDictList
        public static Dictionary<string, object> ToDict(AutoGenAgent data) {
            // Create a dictionary
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "name", data.Name },
                { "description", data.Description },
                { "system_message", data.SystemMessage },
                { "type_value", data.TypeValue },
                { "tool_names_for_execution", data.ToolNamesForExecution },
                { "tool_names_for_llm", data.ToolNamesForLlm },
                { "human_input_mode", data.HumanInputMode },
                { "termination_msg", data.TerminationMsg },
                { "code_execution", data.CodeExecution },
                { "llm_execution", data.Llm },
                { "vector_db_items", VectorDBItem.ToDictList(data.VectorDBItems) }

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
        public void Save(bool allow_override = true) {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenAgentCollection<AutoGenAgent>();
            var items = collection.Find(x => x.Name == Name);
            if (items.Count() > 0 && !allow_override) {
                return;
            }
            foreach (var item in items) {
                collection.Delete(item.Id);
            }
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
