using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ClipboardApp.Factory.Default;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using Python.Runtime;

namespace ClipboardApp.PythonIF
{
    public class PythonExecutor {

        public enum PythonExecutionType {
            None = 0,
            PythonNet = 1,
            InternalFlask = 2,
            ExternalFlask = 3
        }
        public static PythonExecutionType PythonExecution {
            get {
                // intからPythonExecutionTypeに変換
                return (PythonExecutionType)Properties.Settings.Default.PythonExecution;
            }
            set {
                Properties.Settings.Default.PythonExecution = (int)value;
                Properties.Settings.Default.Save();
            }
        }

        // カスタムPythonスクリプトの、templateファイル
        public static string TemplateScript = "python/template.py";

        // クリップボードアプリ用のPythonスクリプト
        public static string ClipboardAppUtilsScript = "python/clipboard_app_utils.py";


        public static IPythonFunctions PythonFunctions { get; set; } = new EmptyPythonFunctions();
        public static void Init() {
            // ClipboardAppSettingsのPythonExecutionがPythonNetの場合はInitPythonNetを実行する
            if (PythonExecution == PythonExecutionType.PythonNet) {
                PythonFunctions = new PythonNetFunctions();
            }
        }


        // Pythonファイルを読み込む
        public static string LoadPythonScript(string scriptName) {
            var file = new FileInfo(scriptName);
            if (!file.Exists) {
                // テンプレートファイルを読み込み

                file = new FileInfo(TemplateScript);
                if (!file.Exists) {
                    throw new ClipboardAppException("テンプレートファイルが見つかりません");
                }
                return File.ReadAllText(file.FullName);
            }
            // fileを読み込む
            string script = File.ReadAllText(file.FullName);

            return script;
        }
    }

}
