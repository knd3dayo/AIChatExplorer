using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace PythonAILib.Model.AutoGen {
    public class AutoGenGroupChat {

        public static JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        // work_dir
        [JsonPropertyName("work_dir")]
        public string WorkDir { get; set; } = "";

        public List<AutoGenAgent> AutoGenAgents { get; set; } = new List<AutoGenAgent>();

        public List<AutoGentTool> AutoGentTools { get; set; } = new List<AutoGentTool>();

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "name", Name },
                { "description", Description },
                { "work_dir", WorkDir },
                { "agents", AutoGenAgent.ToDict(AutoGenAgents) },
                { "tools", AutoGentTool.ToDict(AutoGentTools) },
            };
            return dict;
        }

        // ToJson
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }
    }
}
