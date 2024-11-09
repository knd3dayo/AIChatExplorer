using System.Diagnostics;
using System.IO;
using PythonAILib.Model;
using PythonAILib.Model.VectorDB;
using WpfAppCommon.Utils;

namespace QAChat.Utils {
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
            if (! string.IsNullOrEmpty(venvPath)) {
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
        public static void StartAutoGenGroupChatTest1(OpenAIProperties openAIProperties, List<VectorDBItem> vectorDBItems ,string message, Action<string> afterProcessEnd, string? venvPath= "") {
            // Start AutoGenStudio
            List<string> cmdLines = [];
            // 文字コードをUTF-8に設定
            cmdLines.Add("chcp 65001");
            // venvが有効な場合は、activate.batを実行
            if (!string.IsNullOrEmpty(venvPath)) {
                string venvActivateScript = Path.Combine(venvPath, "Scripts", "activate");
                cmdLines.Add($"call {venvActivateScript}");
            }
            // python_ai_libフォルダに移動
            cmdLines.Add("cd python_ai_lib");

            // 結果を出力するテンポラリファイル
            string tempFile = Path.GetTempFileName();
            // openAIPropertiesをJSONファイルに保存
            string openAIPropertiesJsonFile = Path.GetTempFileName();
            File.WriteAllText(openAIPropertiesJsonFile, openAIProperties.ToJson());
            // vectorDBItemsをJSONファイルに保存
            string vectorDBItemsJsonFile = Path.GetTempFileName();
            File.WriteAllText(vectorDBItemsJsonFile, VectorDBItem.ToJson(vectorDBItems));

            // python ai_app_autogen_group_chat_test.py -m <質問内容> -o <出力ファイル名> -p OpenAIPropertiesのJSONファイル -v ベクトルDB設定のJSONファイル 
            cmdLines.Add($"python ai_app_autogen_group_chat_test.py -m {message} -o {tempFile} -p {openAIPropertiesJsonFile} -v {vectorDBItemsJsonFile}");
            cmdLines.Add("pause");
            AutoGenGroupChatTest1Process = ProcessUtil.StartWindowsCommandLine(cmdLines, "", (process) => { }, (content) => {
                // テンポラリファイルから文字列を取得
                string result = File.ReadAllText(tempFile);
                afterProcessEnd(result);
                // テンポラリファイルがあれば削除
                if (File.Exists(tempFile)) {
                    File.Delete(tempFile);
                }
                if (File.Exists(openAIPropertiesJsonFile)) {
                    File.Delete(openAIPropertiesJsonFile);
                }
                if (File.Exists(vectorDBItemsJsonFile)) {
                    File.Delete(vectorDBItemsJsonFile);
                }

                AutoGenStudioProcess = null;
            });

        }
    }
}
