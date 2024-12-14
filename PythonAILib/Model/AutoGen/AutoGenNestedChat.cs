using System.Text.Json.Serialization;
using PythonAILib.Common;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenNestedChat {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        [JsonPropertyName("name")]
        public string Name { get; set; } = "default";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("init_agent_name")]
        public string InitAgentName { get; set; } = "user_proxy";

        [JsonPropertyName("group_chat_list")]
        public List<AutoGenGroupChat> AutoGenGroupChatList { get; set; } = [];

        [JsonPropertyName("normal_chat_list")]
        public List<AutoGenGroupChat> AutoGenNormalChatList { get; set; } = [];

        public List<AutoGenAgent> AutoGenAgents { get; set; } = [];

        public List<AutoGenTool> AutoGentTools { get; set; } = [];

        // ToDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "name", Name },
                { "description", Description },
                { "init_agent_name", InitAgentName },
                { "group_chat_list", AutoGenGroupChatList },
                { "normal_chat_list", AutoGenNormalChatList },
            };
            return dict;
        }

        // Save
        public void Save(bool allow_override = true) {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenNestedChatCollection<AutoGenNestedChat>();
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
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenNestedChatCollection<AutoGenNestedChat>();
            collection.Delete(this.Id);
        }
        // FindAll
        public static List<AutoGenNestedChat> FindAll() {
            var collection = PythonAILibManager.Instance.DataFactory.GetAutoGenNestedChatCollection<AutoGenNestedChat>();
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

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}
