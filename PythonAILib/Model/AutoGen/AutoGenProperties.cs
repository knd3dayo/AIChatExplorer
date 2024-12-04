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

        // ToDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "work_dir", WorkDir },
                { "group_chat", AutoGenGroupChat.ToDict() },
                { "agents", AutoGenAgent.ToDictList(AutoGenAgents) },
                { "tools", AutoGenTool.ToDictList(AutoGenTools) },
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
            // defaultSettings から group_chat を取得
            if (defaultSettings.TryGetValue("group_chat", out dynamic? groupChatData)) {
                if (groupChatData != null) {
                    AutoGenGroupChat groupChat = new() {
                        Name = groupChatData["name"],
                        Description = groupChatData["description"],
                        InitAgentName = groupChatData["init_agent_name"],
                        AgentNames = [],
                    };
                    foreach (object item in groupChatData["agent_names"]) {
                        var value = item.ToString();
                        if (value != null) {
                            groupChat.AgentNames.Add(value);
                        }
                    }
                    groupChat.Save();
                }
            }
            // defaultSettings から tools を取得
            if (defaultSettings.TryGetValue("tools", out dynamic? toolsData)) {
                if (toolsData != null) {
                    foreach (var toolData in toolsData) {
                        AutoGenTool tool = new AutoGenTool {
                            Name = toolData["name"],
                            Description = toolData["description"],
                            SourcePath = toolData["source_path"],
                        };
                        tool.Save();
                    }
                }
            }
            // defaultSettings から agents を取得
            if (defaultSettings.TryGetValue("agents", out dynamic? agentsDataList)) {
                if (agentsDataList != null) {
                    foreach (var agentData in agentsDataList) {
                        List<string> toolNamesForExecution = [];
                        foreach( object item in agentData["tool_names_for_execution"]) {
                            var value = item.ToString();
                            if (value != null) {
                                toolNamesForExecution.Add(value);
                            }
                        }
                        List<string> toolNamesForLlm = [];
                        foreach (object item in agentData["tool_names_for_llm"]) {
                            var value = item.ToString();
                            if (value != null) {
                                toolNamesForLlm.Add(value);
                            }
                        }

                        // List<string>に変換

                        AutoGenAgent agent = new() {
                            Name = agentData["name"],
                            Description = agentData["description"],
                            HumanInputMode = agentData["human_input_mode"],
                            TerminationMsg = agentData["termination_msg"],
                            CodeExecution = agentData["code_execution"],
                            Llm = agentData["llm_execution"],
                            TypeValue = agentData["type_value"],
                            ToolNamesForExecution = toolNamesForExecution,
                            ToolNamesForLlm = toolNamesForLlm,
                        };
                        agent.Save();
                    }
                }
            }

            Initialized = true;

        }
    }
}
