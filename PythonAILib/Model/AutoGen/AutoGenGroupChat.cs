using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenGroupChat {

        [JsonPropertyName("name")]
        public string Name { get; set; } = "default";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("init_agent_name")]
        public string InitAgentName { get; set; } = "user_proxy";

        public List<AutoGenAgent> AutoGenAgents { get; set; } = [];

        public List<AutoGentTool> AutoGentTools { get; set; } = [];

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "name", Name },
                { "description", Description },
                { "init_agent_name", InitAgentName },
                { "agents", AutoGenAgent.ToDict(AutoGenAgents) },
                { "tools", AutoGentTool.ToDict(AutoGentTools) },
            };
            return dict;
        }
    }
}
