using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using QAChat.Model;

namespace ClipboardApp.PythonIF {
    public class FlaskPythonFunctions : IPythonFunctions {
        // FlaskのPythonスクリプト
        public static string FlaskScript = "python/flask_app.py";

        public static string GetApiUrl(string path) {
            if (FlaskPort == -1) {
                Tools.Error("Flaskアプリのポート番号を取得できませんでした");
                return "";
            }
            return "http://localhost:" + FlaskPort + path;
        }
        public static int FlaskPort { get; private set; } = -1;
        private static void BindFreePort() {
            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, 0));
            var endPoint = (IPEndPoint?)socket.LocalEndPoint;
            FlaskPort = endPoint?.Port ?? -1;
        }
        private static CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public static void Dispose() {
            if (_tokenSource != null) {
                _tokenSource.Cancel();
            }
        }
        public static Process? CreateProcess() {
            // FlaskのPythonスクリプトを読み込み別スレッドで実行する
            // スレッドを起動した後、python flask_app.pyを実行する
            // OSはWindowsを想定しているため、python3ではなくpythonを実行する
            // python flask_app.pyを実行すると、Flaskアプリが起動する
            // Flaskアプリはエフェメラルポートで起動するため、ポート番号は固定されていない
            // ポート番号はFlaskアプリが起動した後に取得する

            // Flaskアプリのポート番号を取得
            BindFreePort();
            // FlaskPortを取得できなかった場合はエラーを表示
            if (FlaskPort == -1) {
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
            psi.EnvironmentVariables["OPENAI_EMBEDDING_BASE_URL"] = Properties.Settings.Default.OpenAIEmbeddingBaseURL;
            psi.EnvironmentVariables["OPENAI_COMPLETION_BASE_URL"] = Properties.Settings.Default.OpenAICompletionBaseURL;
            psi.EnvironmentVariables["SPACY_MODEL_NAME"] = Properties.Settings.Default.SpacyModel;
            psi.EnvironmentVariables["PORT"] = FlaskPort.ToString();

            Process? p = new Process();
            p.StartInfo = psi;
            p.EnableRaisingEvents = true;

            p.ErrorDataReceived += (sender, e) => {
                if (e.Data != null) {
                    Tools.Info(e.Data);
                }
            };
            p.OutputDataReceived += (sender, e) => {
                if (e.Data != null) {
                    Tools.Info(e.Data);
                }
            };
            return p;

        }
        public static void FlaskThread(Process process) {
            // Pythonスクリプトを実行する
            process.Start();
            if (process == null) {
                Tools.Error("Flaskアプリの起動に失敗しました");
                return;
            }
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

        }
        public static void RunFlask() {
            // FlaskのPythonスクリプトを読み込み別スレッドで実行する
            Process? process = CreateProcess();
            if (process == null) {
                Tools.Error("Flaskアプリの起動に失敗しました");
                return;
            }

            _tokenSource = new CancellationTokenSource();

            BackgroundWorker wc = new BackgroundWorker();
            wc.WorkerSupportsCancellation = true;
            wc.DoWork += (sender, e) => {
                FlaskThread(process);
            };
            _tokenSource.Token.Register(() => {
                wc.CancelAsync();
            });
            wc.RunWorkerAsync();

        }

        public FlaskPythonFunctions() {
            RunFlask();
        }
        public string ExtractText(string path) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string GetMaskedString(string text) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string GetUnmaskedString(string maskedText) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public string ExtractTextFromImage(System.Drawing.Image image) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public MaskedData GetMaskedData(List<string> beforeTextList) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public MaskedData GetUnMaskedData(List<string> maskedTextList) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }

        public ChatResult LangChainChat(string prompt, IEnumerable<ChatItem> chatHistory) { 
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void OpenAIEmbedding(string text) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void SaveFaissIndex() {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public void LoadFaissIndex() {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }

        public void RunScript(ScriptItem scriptItem, ClipboardItem clipboardItem) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
        public HashSet<string> ExtractEntity(string text) {
            throw new ClipboardAppException("Pythonが有効になっていません。設定画面でPythonExecuteを設定してください。");
        }
    }
}
