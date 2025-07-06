using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.File {
    public class CommonDataTable(List<List<string>> rows) {

        [JsonPropertyName("rows")]
        public List<List<string>> Rows { get; set; } = rows;

        public static string ToJson(CommonDataTable dataTable) {
            JsonArray jsonArray = [];
            foreach (List<string> row in dataTable.Rows) {
                JsonArray jsonRow = [.. row];
                jsonArray.Add(jsonRow);
            }
            return JsonSerializer.Serialize(dataTable, JsonUtil.JsonSerializerOptions);
        }

        public static CommonDataTable FromJson(string json) {
            List<List<string>> result = JsonSerializer.Deserialize<List<List<string>>>(json, JsonUtil.JsonSerializerOptions) ?? [];
            return new CommonDataTable(result);

        }

    }
}
