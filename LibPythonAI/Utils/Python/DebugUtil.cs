using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Common;
using PythonAILib.Model.Chat;

namespace PythonAILib.Utils.Python {
    public class DebugUtil {

        private static readonly JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };

        public static string DebugRequestParametersFile {
            get {
                return Path.Combine(PythonAILibManager.Instance.ConfigParams.GetAppDataPath(), "debug_request_parameters.json");
            }
        }

        // ChatRequestの内容からPythonスクリプトを実行するコマンド文字列を生成する。
        public static List<string> GetPythonScriptCommand(string pythonScriptName, string pythonScriptArgs, string beforeExecScriptCommands = "", string afterExecScriptCommands = "") {
            List<string> cmdLines = [];
            // python_libのディレクトリ
            string pythonAILibDir = PythonAILibManager.Instance.ConfigParams.GetPythonLibPath();
            // debug用のScriptのディレクトリ
            string debugScriptDir = Path.Combine(pythonAILibDir, "ai_chat_explorer", "debug_tool");
            // Scriptのフルパス
            string pythonScriptPath = Path.Combine(debugScriptDir, pythonScriptName);
            // 文字コードをUTF-8に設定
            cmdLines.Add("chcp 65001");
            // venvが有効な場合は、activate.batを実行
            string venvPath = PythonAILibManager.Instance.ConfigParams.GetPathToVirtualEnv();
            if (!string.IsNullOrEmpty(venvPath)) {
                string venvActivateScript = Path.Combine(venvPath, "Scripts", "activate");
                cmdLines.Add($"call {venvActivateScript}");
            }
            // 事前処理
            cmdLines.Add(beforeExecScriptCommands);
            cmdLines.Add($"set PYTHONPATH={pythonAILibDir}");
            // APP_DB_PATH
            cmdLines.Add($"set APP_DATA_PATH={PythonAILibManager.Instance.ConfigParams.GetAppDataPath()}");
            cmdLines.Add($"python {pythonScriptPath} {pythonScriptArgs}");
            // 事後処理
            cmdLines.Add(afterExecScriptCommands);
            cmdLines.Add("\n");

            return cmdLines;
        }

        // ChatRequestの内容とList<VectorDBItem>からパラメーターファイルを作成する。
        public static string CreateParameterJsonFile(OpenAIExecutionModeEnum chatMode, ChatRequestContext chatRequestContext, ChatRequest? chatRequest) {
            // JSONファイルに保存
            string parametersJson = CreateParameterJson(chatMode, chatRequestContext, chatRequest);
            string parametersJsonFile = Path.GetTempFileName();
            File.WriteAllText(parametersJsonFile, parametersJson);

            return parametersJsonFile;
        }

        // ChatRequestの内容とList<VectorDBItem>からパラメーターJsonを作成する。
        public static string CreateParameterJson(OpenAIExecutionModeEnum chatMode, ChatRequestContext chatRequestContext, ChatRequest? chatRequest) {
            Dictionary<string, object> parametersDict = [];
            parametersDict["chat_request_context"] = chatRequestContext.ToChatRequestContextDict();
            parametersDict["openai_props"] = chatRequestContext.OpenAIProperties.ToDict();
            parametersDict["autogen_props"] = chatRequestContext.AutoGenProperties.ToDict();
            parametersDict["vector_db_props"] = chatRequestContext.ToDictVectorDBItemsDict();

            // ChatRequestをDictionaryに保存
            if (chatRequest != null) {
                if (chatMode == OpenAIExecutionModeEnum.Normal) {
                    parametersDict["chat_request"] = chatRequest.ToDict();
                }
                if (chatMode == OpenAIExecutionModeEnum.AutoGenGroupChat) {
                    parametersDict["autogen_request"] = chatRequest.ToDict();
                }
            }

            string parametersJson = JsonSerializer.Serialize(parametersDict, options);
            return parametersJson;

        }

        // AutoGenGroupChatのテスト1を実行するコマンド文字列を生成する。
        public static List<string> CreateAutoGenGroupChatTest1CommandLine(string parametersJsonFile, string? outputFile) {

            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            string beforeExecScriptCommands = "notepad " + parametersJsonFile;
            // 事後コマンド pauseで一時停止
            string afterExecScriptCommands = "pause";
            string options = $"-p {parametersJsonFile}";
            List<string> cmdLines = DebugUtil.GetPythonScriptCommand("autogen_group_chat_debug_01.py", $"{options}",
                beforeExecScriptCommands, afterExecScriptCommands);

            return cmdLines;
        }
        // コマンド文字列を実行する
        public static void ExecuteDebugCommand(List<string> commandLines) {
            string command = string.Join(" & ", commandLines);
            ProcessStartInfo psi = new() {
                FileName = "cmd.exe",
                Arguments = $"/k {command}",
                WorkingDirectory =PythonAILibManager.Instance.ConfigParams.GetAppDataPath(),
                UseShellExecute = true,
            };
            Process.Start(psi);
        }

        // Chatを実行するコマンド文字列を生成する。
        public static List<string> CreateChatCommandLine(OpenAIExecutionModeEnum chatMode, ChatRequestContext chatRequestContext, ChatRequest chatRequest) {
            // ModeがNormalまたはOpenAIRAGの場合は、OpenAIChatを実行するコマンドを返す
            if (chatMode == OpenAIExecutionModeEnum.Normal) {
                // パラメーターファイルを作成
                string parametersJson = DebugUtil.CreateParameterJson(chatMode, chatRequestContext, chatRequest);
                File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);
                return DebugUtil.CreateOpenAIChatCommandLine(DebugUtil.DebugRequestParametersFile);
            }
            // ModeがAutoGenの場合は、AutoGenのGroupChatを実行するコマンドを返す
            if (chatMode == OpenAIExecutionModeEnum.AutoGenGroupChat) {
                // パラメーターファイルを作成
                string parametersJson = DebugUtil.CreateParameterJson(chatMode, chatRequestContext, chatRequest);
                File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);

                return DebugUtil.CreateAutoGenGroupChatTest1CommandLine(DebugUtil.DebugRequestParametersFile, null);
            }
            return [];
        }

        // OpenAIチャットを実行するコマンド文字列を生成する。
        public static List<string> CreateOpenAIChatCommandLine(string parametersJsonFile) {
            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            string beforeExecScriptCommands = "notepad " + parametersJsonFile;
            // 事後コマンド pauseで一時停止
            string afterExecScriptCommands = "pause";
            string options = $"-p {parametersJsonFile}";
            List<string> cmdLines = DebugUtil.GetPythonScriptCommand("ai_app_open_ai_chat_debug_01.py", $"{options}", beforeExecScriptCommands, afterExecScriptCommands);


            return cmdLines;
        }
        // LangChainチャットを実行するコマンド文字列を生成する。
        public static List<string> CreateLangChainChatCommandLine(string parametersJsonFile) {
            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            string beforeExecScriptCommands = "notepad " + parametersJsonFile;
            // 事後コマンド pauseで一時停止
            string afterExecScriptCommands = "pause";
            string options = $"-p {parametersJsonFile}";
            List<string> cmdLines = DebugUtil.GetPythonScriptCommand("ai_app_langchain_chat_debug_01.py", $"{options}", beforeExecScriptCommands, afterExecScriptCommands);

            return cmdLines;
        }

        public static string ProcessAutoGenGroupChatResult(string responseJson) {
            StringBuilder sb = new();
            // responseJsonはJsonElementのリスト
            List<JsonElement> jsonElements = JsonSerializer.Deserialize<List<JsonElement>>(responseJson) ?? [];
            foreach (var jsonElement in jsonElements) {
                Dictionary<string, dynamic?>? dic = Common.JsonUtil.ParseJson(jsonElement.ToString());
                // role, name , contentを取得
                string role = dic?["role"] ?? "";
                string name = dic?["name"] ?? "";
                string content = dic?["content"] ?? "";
                // roleが[user]または[assistant]の場合は、role, name, contentをStringBuilderに追加
                if (role == "user" || role == "assistant") {
                    sb.Append($"{role} {name}:\n{content}\n");
                }
            }
            return sb.ToString();
        }
    }
}
