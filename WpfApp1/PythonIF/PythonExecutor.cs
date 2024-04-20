using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Net;
using Python.Runtime;
using System.ComponentModel;
using ClipboardApp.Utils;
using System.Diagnostics;
using ClipboardApp.Model;

namespace ClipboardApp.PythonIF
{
    public class PythonExecutor
    {

        public enum PythonExecutionType
        {
            None = 0,
            PythonNet = 1,
            InternalFlask = 2,
            ExternalFlask = 3
        }
        public static PythonExecutionType PythonExecution
        {
            get
            {
                // intからPythonExecutionTypeに変換
                return (PythonExecutionType)Properties.Settings.Default.PythonExecution;
            }
            set
            {
                Properties.Settings.Default.PythonExecution = (int)value;
                Properties.Settings.Default.Save();
            }
        }

        // カスタムPythonスクリプトの、templateファイル
        public static string TemplateScript = "python/template.py";

        // クリップボードアプリ用のPythonスクリプト
        public static string ClipboardAppUtilsScript = "python/clipboard_app_utils.py";

        // FlaskのPythonスクリプト
        public static string FlaskScript = "python/flask_app.py";

        public static IPythonFunctions PythonFunctions { get; set; } = new EmptyPythonFunctions();
        public static void Init()
        {
            // ClipboardAppSettingsのPythonExecutionがPythonNetの場合はInitPythonNetを実行する
            if (PythonExecution == PythonExecutionType.PythonNet)
            {
                InitPythonNet();
                PythonFunctions = new PythonNetFunctions();
            }

            // ClipboardAppSettingsのPythonExecutionがInternalFlaskの場合はFlaskアプリを起動する
            if (PythonExecution == PythonExecutionType.InternalFlask)
            {
                RunFlask();
            }
        }
        private static void InitPythonNet()
        {
            // Pythonスクリプトを実行するための準備
            // PythonDLLのパスを設定
            Runtime.PythonDLL = Properties.Settings.Default.PythonDllPath;

            // Runtime.PythonDLLのファイルが存在するかチェック
            if (!File.Exists(Runtime.PythonDLL))
            {
                string message = "PythonDLLが見つかりません。";
                message += "\n" + "PythonDLLのパスを確認してください:";
                Tools.ShowMessage(message + Runtime.PythonDLL);
                return;
            }

            try
            {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();

            }
            catch (TypeInitializationException e)
            {
                string message = "Pythonの初期化に失敗しました。" + e.Message;
                message += "\n" + "PythonDLLのパスを確認してください。";
                Tools.ShowMessage(message);
            }
        }

        public static string GetApiUrl(string path)
        {
            if (FlaskPort == -1)
            {
                Tools.Error("Flaskアプリのポート番号を取得できませんでした");
                return "";
            }
            return "http://localhost:" + FlaskPort + path;
        }
        public static int FlaskPort { get; private set; } = -1;
        private static void BindFreePort()
        {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            var endPoint = (IPEndPoint?)socket.LocalEndPoint;
            FlaskPort = endPoint?.Port ?? -1;
        }
        private static CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public static void Dispose()
        {
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
            }
        }
        public static Process? CreateProcess()
        {
            // FlaskのPythonスクリプトを読み込み別スレッドで実行する
            // スレッドを起動した後、python flask_app.pyを実行する
            // OSはWindowsを想定しているため、python3ではなくpythonを実行する
            // python flask_app.pyを実行すると、Flaskアプリが起動する
            // Flaskアプリはエフェメラルポートで起動するため、ポート番号は固定されていない
            // ポート番号はFlaskアプリが起動した後に取得する

            // Flaskアプリのポート番号を取得
            BindFreePort();
            // FlaskPortを取得できなかった場合はエラーを表示
            if (FlaskPort == -1)
            {
                Tools.Error("Flaskアプリのポート番号を取得できませんでした");
                return null;
            }
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "python";
            psi.Arguments = FlaskScript;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            // psiに環境変数を設定
            psi.EnvironmentVariables["OPENAI_API_KEY"] = Properties.Settings.Default.OpenAIKey;
            psi.EnvironmentVariables["AZURE_OPENAI"] = Properties.Settings.Default.AzureOpenAI.ToString();
            psi.EnvironmentVariables["CHAT_MODEL_NAME"] = Properties.Settings.Default.OpenAICompletionModel;
            psi.EnvironmentVariables["AZURE_OPENAI_ENDPOINT"] = Properties.Settings.Default.AzureOpenAIEndpoint;
            psi.EnvironmentVariables["SPACY_MODEL_NAME"] = Properties.Settings.Default.SpacyModel;
            psi.EnvironmentVariables["PORT"] = FlaskPort.ToString();

            Process? p = new Process();
            p.StartInfo = psi;
            p.EnableRaisingEvents = true;

            p.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Tools.Info(e.Data);
                }
            };
            p.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                {
                    Tools.Info(e.Data);
                }
            };
            return p;

        }
        public static void FlaskThread(Process process)
        {
            // Pythonスクリプトを実行する
            process.Start();
            if (process == null)
            {
                Tools.Error("Flaskアプリの起動に失敗しました");
                return;
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

        }
        public static void RunFlask()
        {
            // FlaskのPythonスクリプトを読み込み別スレッドで実行する
            Process? process = CreateProcess();
            if (process == null)
            {
                Tools.Error("Flaskアプリの起動に失敗しました");
                return;
            }

            _tokenSource = new CancellationTokenSource();

            BackgroundWorker wc = new BackgroundWorker();
            wc.WorkerSupportsCancellation = true;
            wc.DoWork += (sender, e) =>
            {
                FlaskThread(process);
            };
            _tokenSource.Token.Register(() =>
            {
                wc.CancelAsync();
            });
            wc.RunWorkerAsync();

        }

        // Pythonファイルを読み込む
        public static string LoadPythonScript(string scriptName)
        {
            var file = new FileInfo(scriptName);
            if (!file.Exists)
            {
                // テンプレートファイルを読み込み

                file = new FileInfo(TemplateScript);
                if (!file.Exists)
                {
                    throw new ThisApplicationException("テンプレートファイルが見つかりません");
                }
                return File.ReadAllText(file.FullName);
            }
            // fileを読み込む
            string script = File.ReadAllText(file.FullName);

            return script;
        }
        public static ObservableCollection<ScriptItem> ScriptItems
        {
            get
            {
                var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ScriptItem>(ClipboardDatabaseController.SCRIPT_COLLECTION_NAME);
                return new ObservableCollection<ScriptItem>(collection.FindAll());
            }
        }
        public static void SaveScriptItem(ScriptItem scriptItem)
        {
            var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ScriptItem>(ClipboardDatabaseController.SCRIPT_COLLECTION_NAME);
            collection.Upsert(scriptItem);
        }
        public static void DeleteScriptItem(ScriptItem scriptItem)
        {
            var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ScriptItem>(ClipboardDatabaseController.SCRIPT_COLLECTION_NAME);
            collection.Delete(scriptItem.Id);
        }
    }

}
