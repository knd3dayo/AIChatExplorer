using System.Text.Json;
using System.Text.Json.Serialization;
using LibPythonAI.PythonIF;

namespace LibPythonAI.Model.AutoGen {
    public class AutoGenGroupChat {

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All),
            WriteIndented = true
        };

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("llm_config_name")]
        public string LLMConfigName { get; set; } = "";


        [JsonPropertyName("agent_names")]
        public List<string> AgentNames { get; set; } = [];

        // CreateEntriesDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "name", Name },
                { "description", Description },
                { "llm_config_name", LLMConfigName },
                { "agent_names", AgentNames },
            };
            return dict;
        }

        // SaveAsync
        public void SaveAsync() {
            // APIを呼び出して、設定を保存する
             PythonExecutor.PythonAIFunctions.UpdateAutoGenGroupChatAsync(this);
        }

        // DeleteAsync
        public void DeleteAsync() {
            // APIを呼び出して、設定を削除する
             PythonExecutor.PythonAIFunctions.DeleteAutoGenGroupChatAsync(this);
        }

        // GetAutoGenChatListAsync
        public static async Task<List<AutoGenGroupChat>> GetAutoGenChatListAsync() {
            // APIを呼び出して、設定を取得する
            List<AutoGenGroupChat> list = await PythonExecutor.PythonAIFunctions.GetAutoGenGroupChatListAsync();
            return list;
        }


        // FromDict
        public static AutoGenGroupChat FromDict(Dictionary<string, object> dict) {
            List<string> agentNames = [];
            if (dict.ContainsKey("agent_names")) {
                var items = (List<object>)dict["agent_names"];
                foreach (var item in items) {
                    if (item.ToString() is string name) {
                        agentNames.Add(name);
                    }
                }
            }
            AutoGenGroupChat chat = new() {
                Name = dict["name"].ToString() ?? "",
                Description = dict["description"].ToString() ?? "",
                LLMConfigName = dict["llm_config_name"].ToString() ?? "",
                AgentNames = agentNames
            };
            return chat;
        }
    }


}
