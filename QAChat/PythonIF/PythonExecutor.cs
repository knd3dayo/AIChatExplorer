using System.Collections.ObjectModel;
using System.IO;
using Python.Runtime;
using QAChat.Utils;

namespace QAChat.PythonIF
{
    public class PythonExecutor {


        // Pythonスクリプト
        public static string RetrievalScript = "python/retrieval_qa_util.py";

        public static PythonNetFunctions? PythonNetFunctions { get; private set; }

        public static void Init() {
            // Pythonスクリプトを実行するための準備
            // PythonDLLのパスを設定
            Runtime.PythonDLL = Properties.Settings.Default.PythonDllPath;

            // Runtime.PythonDLLのファイルが存在するかチェック
            if (!File.Exists(Runtime.PythonDLL)) {
                string message = "PythonDLLが見つかりません。";
                message += "\n" + "PythonDLLのパスを確認してください:";
                Tools.ShowMessage(message + Runtime.PythonDLL);
                return;
            }

            try {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
                PythonNetFunctions = new PythonNetFunctions();

            } catch (TypeInitializationException e) {
                string message = "Pythonの初期化に失敗しました。" + e.Message;
                message += "\n" + "PythonDLLのパスを確認してください。";
                Tools.ShowMessage(message);
            }
        }
        public static string LoadPythonScript(string scriptName) {
            var file = new FileInfo(scriptName);
            if (!file.Exists) {
                throw new ThisApplicationException("テンプレートファイルが見つかりません");
            }
            // fileを読み込む
            string script = File.ReadAllText(file.FullName);

            return script;
        }

    }

}
