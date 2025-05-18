using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.PythonIF;

namespace LibPythonAI.Model.AutoGen {
    public class AutoGenLLMConfig {

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };


        public static readonly string API_TYPE_AZURE = "azure";
        public static readonly string API_TYPE_OPENAI = "openai";

        // name コンフィグ名
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";
        // api_type api_type (azure, openaiなど)
        [JsonPropertyName("api_type")]
        public string ApiType { get; set; } = "";
        // api_version apiのバージョン
        [JsonPropertyName("api_version")]
        public string ApiVersion { get; set; } = "";
        // model llmのモデル
        [JsonPropertyName("model")]
        public string Model { get; set; } = "";
        // api_key apiキー
        [JsonPropertyName("api_key")]
        public string ApiKey { get; set; } = "";
        // base_url base_url
        [JsonPropertyName("base_url")]
        public string BaseURL { get; set; } = "";

        public async Task SaveAsync() {
            // APIを呼び出して、設定を保存する
            await PythonExecutor.PythonAIFunctions.UpdateAutogenLLMConfigAsync(this);
        }

        public async Task DeleteAsync() {
            // APIを呼び出して、設定を削除する
            await PythonExecutor.PythonAIFunctions.DeleteAutogenLLMConfigAsync(this);
        }

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                ["name"] = Name,
                ["api_type"] = ApiType,
                ["api_version"] = ApiVersion,
                ["model"] = Model,
                ["api_key"] = ApiKey,
                ["base_url"] = BaseURL
            };
            return dict;
        }

        public static AutoGenLLMConfig FromDict(Dictionary<string, object> dict) {
            AutoGenLLMConfig config = new() {
                Name = dict["name"]?.ToString() ?? "",
                ApiType = dict["api_type"]?.ToString() ?? "",
                ApiVersion = dict["api_version"]?.ToString() ?? "",
                Model = dict["model"]?.ToString() ?? "",
                ApiKey = dict["api_key"]?.ToString() ?? "",
                BaseURL = dict["base_url"]?.ToString() ?? "",
            };
            return config;
        }

        public static async Task<List<AutoGenLLMConfig>> GetAutoGenLLMConfigListAsync() {
            // APIを呼び出して、設定を取得する
            List<AutoGenLLMConfig> list = await PythonExecutor.PythonAIFunctions.GetAutoGenLLMConfigListAsync();
            return list;
        }


    }
}
