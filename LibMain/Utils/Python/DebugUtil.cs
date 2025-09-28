using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using LibMain.Common;
using LibMain.Model.Chat;
using LibMain.PythonIF.Request;
using LibMain.Utils.Common;

namespace LibMain.Utils.Python {
    public class DebugUtil {


        public static string DebugRequestParametersFile {
            get {
                return Path.Combine(PythonAILibManager.Instance.ConfigParams.GetAppDataPath(), "debug_request_parameters.json");
            }
        }

        // ChatRequestの内容からPythonスクリプトを実行するコマンド文字列を生成する。
        public static List<string> GetPythonScriptCommand(List<string> mainCmmands, List<string> beforeExecScriptCommands, List<string> afterExecScriptCommands) {
            List<string> cmdLines = [];

            // 文字コードをUTF-8に設定
            cmdLines.Add("chcp 65001");
            // venvが有効な場合は、activate.batを実行
            string venvPath = PythonAILibManager.Instance.ConfigParams.GetPathToVirtualEnv();
            if (!string.IsNullOrEmpty(venvPath)) {
                string venvActivateScript = Path.Combine(venvPath, "Scripts", "activate");
                cmdLines.Add($"call {venvActivateScript}");
            }
            // 事前処理
            foreach (string cmd in beforeExecScriptCommands) {
                cmdLines.Add(cmd);
            }
            cmdLines.Add($"set PYTHONPATH={PythonAILibManager.Instance.ConfigParams.GetPythonLibPath()}");
            // APP_DB_PATH
            cmdLines.Add($"set APP_DATA_PATH={PythonAILibManager.Instance.ConfigParams.GetAppDataPath()}");

            // メインコマンドを実行
            foreach (string cmd in mainCmmands) {
                cmdLines.Add(cmd);
            }
            // 事後処理
            foreach (string cmd in afterExecScriptCommands) {
                cmdLines.Add(cmd);
            }
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
            RequestContainer requestContainer = new() {
                RequestContextInstance = chatRequestContext,
                ChatRequestInstance = chatRequest
            };
            string parametersJson = requestContainer.ToJson();
            return parametersJson;

        }

        // AutoGenGroupChatのテスト1を実行するコマンド文字列を生成する。
        public static List<string> CreateAutoGenGroupChatTest1CommandLine(string parametersJsonFile, string? outputFile) {

            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            List<string> beforeExecScriptCommands = [
                "notepad " + parametersJsonFile,
            ];

            // 事後コマンド pauseで一時停止
            List<string> afterExecScriptCommands = [
                "pause"
            ];

            string options = $"-f {parametersJsonFile}";
            // app_data_pathのディレクトリ
            string app_data_path = PythonAILibManager.Instance.ConfigParams.GetAppDataPath();

            // api用のモジュールパス
            string api_module_path = "ai_chat_lib.cmd_tools.autogen_group_chat_api";
            // local用のモジュールパス
            string local_script_path = "ai_chat_lib.cmd_tools.autogen_group_chat_local";

            List<string> scripytCmdLines = [
                "REM When using a api server, set the server URL and request JSON file in the command line.",
                $"python -m {api_module_path} {options}",
                "REM When using a local environment with a request JSON file, set the application data directory and set the request JSON file in the command line.",
                $"REM python -m {local_script_path} {options} -d {app_data_path}",
            ];

            List<string> cmdLines = GetPythonScriptCommand(scripytCmdLines, beforeExecScriptCommands, afterExecScriptCommands);

            return cmdLines;
        }
        // コマンド文字列を実行する
        public static void ExecuteDebugCommand(List<string> commandLines) {
            string command = string.Join(" & ", commandLines);
            ProcessStartInfo psi = new() {
                FileName = "cmd.exe",
                Arguments = $"/k {command}",
                WorkingDirectory = PythonAILibManager.Instance.ConfigParams.GetAppDataPath(),
                UseShellExecute = true,
            };
            Process.Start(psi);
        }

        // Chatを実行するコマンド文字列を生成する。
        public static List<string> CreateChatCommandLine(OpenAIExecutionModeEnum chatMode, ChatRequestContext chatRequestContext, ChatRequest chatRequest) {
            // ModeがNormalまたはOpenAIRAGの場合は、OpenAIChatを実行するコマンドを返す
            if (chatMode == OpenAIExecutionModeEnum.Normal) {
                // パラメーターファイルを作成
                string parametersJson = CreateParameterJson(chatMode, chatRequestContext, chatRequest);
                File.WriteAllText(DebugRequestParametersFile, parametersJson);
                return CreateOpenAIChatCommandLine(DebugRequestParametersFile);
            }
            // ModeがAutoGenの場合は、AutoGenのGroupChatを実行するコマンドを返す
            if (chatMode == OpenAIExecutionModeEnum.AutoGenGroupChat) {
                // パラメーターファイルを作成
                string parametersJson = CreateParameterJson(chatMode, chatRequestContext, chatRequest);
                File.WriteAllText(DebugRequestParametersFile, parametersJson);

                return CreateAutoGenGroupChatTest1CommandLine(DebugRequestParametersFile, null);
            }
            return [];
        }

        // OpenAIチャットを実行するコマンド文字列を生成する。
        public static List<string> CreateOpenAIChatCommandLine(string parametersJsonFile) {
            string apiUrl = PythonAILibManager.Instance.ConfigParams.GetAPIServerURL();
            string appDataPath = PythonAILibManager.Instance.ConfigParams.GetAppDataPath();
            string pythonLibPath = PythonAILibManager.Instance.ConfigParams.GetPythonLibPath();
            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            List<string> beforeExecScriptCommands = ["notepad " + parametersJsonFile];
            // 事後コマンド pauseで一時停止
            List<string> afterExecScriptCommands = ["pause"];

            string api_module_path = "ai_chat_lib.cmd_tools.normal_chat_api";
            string local_script_path = "ai_chat_lib.cmd_tools.normal_chat_local";
            // メインコマンド
            List<string> mainCmmands = [
                "REM When using a api server, set the server URL and request JSON file in the command line.",
                $"python -m {api_module_path} -f {parametersJsonFile}  -s {apiUrl}",
                "REM When using a local environment with a request JSON file, set the application data directory and set the request JSON file in the command line.",
                $"REM  python -m {local_script_path} -f {parametersJsonFile}  -d {appDataPath}",
                "REM When using a local environment with environment variables, set the application data directory and set the parameters in the environment variables.",
                $"REM  python -m  {local_script_path} -d {appDataPath}",

            ];

            List<string> cmdLines = GetPythonScriptCommand(mainCmmands, beforeExecScriptCommands, afterExecScriptCommands);


            return cmdLines;
        }
        // ベクトル検索を実行するコマンド文字列を生成する。
        public static List<string> CreateLVearchCommandLine(string parametersJsonFile) {
            string apiUrl = PythonAILibManager.Instance.ConfigParams.GetAPIServerURL();
            string appDataPath = PythonAILibManager.Instance.ConfigParams.GetAppDataPath();

            // 事前コマンド デバッグ用に、notepadでパラメーターファイルを開く
            List<string> beforeExecScriptCommands = ["notepad " + parametersJsonFile];
            // 事後コマンド pauseで一時停止
            List<string> afterExecScriptCommands = ["pause"];

            // メインコマンド
            List<string> mainCmmands = [
                "REM When using a api server, set the server URL and request JSON file in the command line.",
                $"python -m ai_chat_lib.cmd_tools.vector_search_api.py -f {parametersJsonFile} -s {apiUrl}",
                "REM When using a local environment with a request JSON file, set the application data directory and set the request JSON file in the command line.",
                $"REM python  -m ai_chat_lib.cmd_tools.vector_search_local.py -f {parametersJsonFile}  -d {appDataPath}",
                "REM When using a local environment with environment variables, set the application data directory and set the parameters in the environment variables.",
                $"REM python -m ai_chat_lib.cmd_tools.vector_search_local.py -d {appDataPath}",
            ];
            List<string> cmdLines = GetPythonScriptCommand(mainCmmands, beforeExecScriptCommands, afterExecScriptCommands);

            return cmdLines;
        }

        public static string ProcessAutoGenGroupChatResult(string responseJson) {
            StringBuilder sb = new();
            // responseJsonはJsonElementのリスト
            List<JsonElement> jsonElements = JsonSerializer.Deserialize<List<JsonElement>>(responseJson) ?? [];
            foreach (var jsonElement in jsonElements) {
                Dictionary<string, dynamic?>? dic = JsonUtil.ParseJson(jsonElement.ToString());
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
