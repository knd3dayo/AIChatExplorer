using System.Text.Json.Serialization;
using PythonAILib.Common;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenGroupChat {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        [JsonPropertyName("name")]
        public string Name { get; set; } = "default";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("init_agent_name")]
        public string InitAgentName { get; set; } = "user_proxy";

        [JsonPropertyName("agent_names")]
        public List<string> AgentNames { get; set; } = [];
        public List<AutoGenAgent> AutoGenAgents { get; set; } = [];

        public List<AutoGenTool> AutoGentTools { get; set; } = [];

        // ToDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "name", Name },
                { "description", Description },
                { "init_agent_name", InitAgentName },
                { "agent_names", AgentNames },
            };
            return dict;
        }

        // Save
        public void Save(bool allow_override = true) {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenGroupChatCollection<AutoGenGroupChat>();
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
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenGroupChatCollection<AutoGenGroupChat>();
            collection.Delete(this.Id);
        }
        // FindAll
        public static List<AutoGenGroupChat> FindAll() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenGroupChatCollection<AutoGenGroupChat>();
            return collection.FindAll().ToList();
        }

        // Equals
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            AutoGenGroupChat other = (AutoGenGroupChat)obj;
            return Name == other.Name;
        }
    }
}
