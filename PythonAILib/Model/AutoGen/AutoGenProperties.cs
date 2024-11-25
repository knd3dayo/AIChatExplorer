using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Common;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenProperties {

        public static JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };

        // work_dir
        [JsonPropertyName("work_dir")]
        public string WorkDir { get; set; } = "";

        // AutoGenGroupChat
        [JsonPropertyName("group_chat")]
        public AutoGenGroupChat AutoGenGroupChat { get; set; } = new AutoGenGroupChat();

        // AutoGenAgent
        [JsonPropertyName("agents")]
        public List<AutoGenAgent> AutoGenAgents { get; set; } = new List<AutoGenAgent>();

        // AutoGenTools
        [JsonPropertyName("tools")]
        public List<AutoGenTool> AutoGenTools { get; set; } = new List<AutoGenTool>();

        // OpenAIProperties
        [JsonPropertyName("open_ai_props")]
        public OpenAIProperties OpenAIProperties { get; set; } = new OpenAIProperties();

        // UseSystemAgent
        [JsonPropertyName("use_system_agent")]
        public bool UseSystemAgent { get; set; } = false;

        // ToDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "work_dir", WorkDir },
                { "open_ai_props", OpenAIProperties.ToDict() },
                { "group_chat", AutoGenGroupChat.ToDict() },
                { "agents", AutoGenAgent.ToDictList(AutoGenAgents) },
                { "tools", AutoGenTool.ToDictList(AutoGenTools) },
                { "use_default", UseSystemAgent },
            };
            return dict;
        }

        // ToJson
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }

    }
}
