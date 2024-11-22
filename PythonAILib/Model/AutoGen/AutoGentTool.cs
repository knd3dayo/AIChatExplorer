using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PythonAILib.Model.AutoGen {
    public class AutoGentTool {

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        // ToDict
        public static Dictionary<string, object> ToDict(AutoGentTool data) {
            // Create a dictionary
            Dictionary<string, object> dict = new Dictionary<string, object> {
                { "name", data.Name },
                { "description", data.Description },
                { "content", data.Content },
            };
            return dict;
        }
        // ToDict
        public static List<Dictionary<string, object>> ToDict(List<AutoGentTool> data) {
            List<Dictionary<string, object>> dictList = new List<Dictionary<string, object>>();
            foreach (AutoGentTool item in data) {
                dictList.Add(ToDict(item));
            }
            return dictList;
        }
    }
}
