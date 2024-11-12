using System.Diagnostics;
using System.IO;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;
using PythonAILib.Utils.Python;
using WpfAppCommon.Utils;

namespace QAChat.Utils
{
    public class AutoGenProcessController {

        public static Process? AutoGenStudioProcess { get; set; }

        public static Process? AutoGenGroupChatTest1Process { get; set; }
        public static void StartAutoGenStudio(string? venvPath = "") {
            if (AutoGenStudioProcess != null) {
                return;
            }
            // Start AutoGenStudio
            List<string> cmdLines = [];
            // venvが有効な場合は、activate.batを実行
            if (!string.IsNullOrEmpty(venvPath)) {
                string venvActivateScript = Path.Combine(venvPath, "Scripts", "activate");
                cmdLines.Add($"call {venvActivateScript}");
            }
            // autogenstudioを起動するコマンド
            cmdLines.Add("autogenstudio ui --port 8081");
            AutoGenStudioProcess = ProcessUtil.StartWindowsBackgroundCommandLine(cmdLines, "", (process) => { }, (content) => { });

            // 5秒後にブラウザを起動
            Task.Run(() => {
                Thread.Sleep(5000);
                ProcessUtil.StartProcess("http://localhost:8081", "", (process) => { }, (content) => { });
            });

        }

        // Stop AutoGenStudio
        public static void StopAutoGenStudio() {
            if (AutoGenStudioProcess == null) {
                return;
            }
            ProcessUtil.StopProcess(AutoGenStudioProcess);
            AutoGenStudioProcess = null;
        }

        // StartAutoGenGroupChatTest1
        public static void StartAutoGenGroupChatTest1(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems, ChatRequest chatRequest, Action<string> afterProcessEnd) {
            // 結果を出力するテンポラリファイル
            string tempFile = Path.GetTempFileName();

            // パラメーターファイルを作成
            string parametersJson = DebugUtil.CreateParameterJson(openAIProperties, vectorDBItems, chatRequest);
            File.WriteAllText(DebugUtil.DebugRequestParametersFile, parametersJson);

            // AutoGenGroupChatTest1を起動するコマンド
            List<string> cmdLines = DebugUtil.CreateAutoGenGroupChatTest1CommandLine(DebugUtil.DebugRequestParametersFile, tempFile);

            AutoGenGroupChatTest1Process = ProcessUtil.StartWindowsCommandLine(cmdLines, "", (process) => { }, (content) => {
                // テンポラリファイルから文字列を取得
                string result = File.ReadAllText(tempFile);
                afterProcessEnd(result);
                // テンポラリファイルがあれば削除
                if (File.Exists(tempFile)) {
                    File.Delete(tempFile);
                }
                if (File.Exists(DebugUtil.DebugRequestParametersFile)) {
                    File.Delete(DebugUtil.DebugRequestParametersFile);
                }

                AutoGenStudioProcess = null;
            });

        }
    }
}
