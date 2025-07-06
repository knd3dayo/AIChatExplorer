using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LibPythonAI.Utils.Common {
    public class JsonUtil {

        public static JsonSerializerOptions JsonSerializerOptions {
            
            get {
                JsonSerializerOptions jsonSerializerOptions = new() {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true,
                };
                return jsonSerializerOptions;
            }
        }

        // JSON文字列をList<Dictionary<string, dynamic>>型に変換するメソッド
        public static List<Dictionary<string, dynamic?>> ParseJsonArray(string json) {
            var list = JsonSerializer.Deserialize<List<JsonElement>>(json);
            if (list == null) {
                return [];
            }
            // JsonElementから値を取り出して、dynamic型に入れてDictionary<string, dynamic>型で返す
            var result = list.Select(d => d.EnumerateObject().ToDictionary(a => a.Name, a => ParseJsonElement(a.Value)))
                .ToList();
            return result;
        }

        // JSON文字列をDictionary<string, dynamic>型に変換するメソッド
        public static Dictionary<string, dynamic?> ParseJson(string json) {

            var dic = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);
            if (dic == null) {
                return [];
            }
            // JsonElementから値を取り出して、Dynamic型に入れてDictionary<string, dynamic>型で返す
            var result = dic.Select(d => new { key = d.Key, value = ParseJsonElement(d.Value) })
                .ToDictionary(a => a.key, a => a.value);
            return result;

        }

        // JsonElementの型を調べて変換するメソッド
        private static dynamic? ParseJsonElement(JsonElement elem) {
            // データの種類によって値を取得する処理を変える
            return elem.ValueKind switch {
                JsonValueKind.String => elem.GetString(),
                JsonValueKind.Number => elem.GetDecimal(),
                JsonValueKind.False => false,
                JsonValueKind.True => true,
                JsonValueKind.Array => elem.EnumerateArray().Select(e => ParseJsonElement(e)).ToList(),
                JsonValueKind.Null => null,
                JsonValueKind.Object => ParseJson(elem.GetRawText()),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
