using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.PythonIF;

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

        // UseSystemAgent
        [JsonPropertyName("use_system_agent")]
        public bool UseSystemAgent { get; set; } = false;

        // ToDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "work_dir", WorkDir },
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

        private static bool Initialized { get; set; } = false;
        // デフォルトの設定を取得
        public static void Init(OpenAIProperties openAIProperties) {
            // Initが実行済みの場合は処理をスキップ
            if (Initialized) {
                return;
            }

            ChatRequestContext chatRequestContent = new() {
                OpenAIProperties = openAIProperties,
            };
            Dictionary<string, dynamic?> defaultSettings = PythonExecutor.PythonAIFunctions.GetAutoGenDefaultSettings(chatRequestContent);

            // defaultSettings から tools を取得
            if (defaultSettings.TryGetValue("tools", out dynamic? toolsData)) {
                if (toolsData != null) {
                    foreach (var toolData in toolsData) {
                        AutoGenTool tool = new AutoGenTool {
                            Name = toolData["name"],
                            Description = toolData["description"],
                            Content = toolData["content"],
                        };
                        tool.Save();
                    }
                }
            }
            // defaultSettings から agents を取得
            if (defaultSettings.TryGetValue("agents", out dynamic? agentsData)) {
                if (agentsData != null) {
                    foreach (var agentData in agentsData) {
                        string toolNames = agentData["tools"];
                        List<string> strings = toolNames.Split(",").ToList();

                        AutoGenAgent agent = new AutoGenAgent {
                            Name = agentData["name"],
                            Description = agentData["description"],
                            HumanInputMode = agentData["human_input_mode"],
                            TerminationMsg = agentData["is_termination_msg"],
                            CodeExecution = agentData["code_execution_config"],
                            Llm = agentData["llm_config"],
                            Type = agentData["type"],
                            ToolNamesList = strings,
                        };
                        agent.Save();
                    }
                }
            }
            Initialized = true;

        }
    }
}
