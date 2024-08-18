using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace PythonAILib.Model {
    public  class VectorSearchRequest {

        // Query
        [JsonPropertyName("query")]
        public string Query { get; set; } = string.Empty;

        // SearchKWArgs
        [JsonPropertyName("search_kwargs")]
        public Dictionary<string, object> SearchKWArgs { get; set; } = [];

    public string ToJson() {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(this, options);
        }
    }
}
