using System.Diagnostics;
using System.IO;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;
using PythonAILib.Utils.Python;
using WpfAppCommon.Utils;

namespace LibUIPythonAI.Utils {
    public class AutoGenProcessController {

        public static Process? AutoGenGroupChatTest1Process { get; set; }


        // StartAutoGenGroupChatTest1
        public static void StartAutoGenGroupChatTest1(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> afterProcessEnd) {
            // 結果を出力するテンポラリファイル
            string tempFile = Path.GetTempFileName();

            string parametersJson = DebugUtil.CreateParameterJson(chatRequestContext, chatRequest);
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

                AutoGenGroupChatTest1Process = null;
            });

        }
    }
}
