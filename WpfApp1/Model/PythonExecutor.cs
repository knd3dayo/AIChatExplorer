using System.Collections.ObjectModel;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using Python.Runtime;
using System.ComponentModel;
using WpfApp1.Utils;
using WpfApp1.View.OpenAIView;

namespace WpfApp1.Model
{
    public class PythonExecutor
    {
        // Pythonが有効かどうか
        public static bool IsPythonEnabled = false;

        // カスタムPythonスクリプトの、templateファイル
        public static string TemplateScript = "python/template.py";

        // クリップボードアプリ用のPythonスクリプト
        public static string ClipboardAppUtilsScript = "python/clipboard_app_utils.py";

        // FlaskのPythonスクリプト
        public static string FlaskScript = "python/flask_app.py";

        public static void Init()
        {
            // Pythonスクリプトを実行するための準備
            // PythonDLLのパスを設定
            Runtime.PythonDLL = Properties.Settings.Default.PYTHON_DLL_PATH;

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
                IsPythonEnabled = true;

            }
            catch (TypeInitializationException e)
            {
                string message = "Pythonの初期化に失敗しました。" + e.Message;
                message += "\n" + "PythonDLLのパスを確認してください。";
                Tools.ShowMessage(message);
            }
            // PropertyでUSE_PYTHON_FLASK=Trueとなっている場合はFlaskを起動
            if (Properties.Settings.Default.USE_PYTHON_FLASK)
            {
                _tokenSource = new CancellationTokenSource();
                RunFlask(_tokenSource.Token);
            }

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
        public static void FlaskThread()
        {
            // FlaskのPythonスクリプトを読み込み別スレッドで実行する
            // スレッドを起動した後、python flask_app.pyを実行する
            // OSはWindowsを想定しているため、python3ではなくpythonを実行する
            // python flask_app.pyを実行すると、Flaskアプリが起動する
            // Flaskアプリはエフェメラルポートで起動するため、ポート番号は固定されていない
            // ポート番号はFlaskアプリが起動した後に取得する

            // テンポラリディレクトリを取得
            string tempDir = Path.GetTempPath();
            // 標準出力、標準エラー保存用のテンポラリファイルを作成
            string tempOutputFilePath = Path.GetTempFileName();

            // FlaskのPythonスクリプトを読み込む
            string script = LoadPythonScript(FlaskScript);
            // Flaskアプリのポート番号を取得
            BindFreePort();
            // FlaskPortを取得できなかった場合はエラーを表示
            if (FlaskPort == -1)
            {
                Tools.ShowMessage("Flaskアプリのポート番号を取得できませんでした");
                return;
            }
            // script内の{{OPENAI_API_KEY}}をProperties.Settings.Default.OPENAI_API_KEYに置換する
            script = script.Replace("{{OPENAI_API_KEY}}", Properties.Settings.Default.OPENAI_API_KEY);
            // script内の{{AZURE_OPENAI}}をProperties.Settings.Default.AZURE_OPENAIに置換する
            script = script.Replace("{{AZURE_OPENAI}}", Properties.Settings.Default.AZURE_OPENAI.ToString());
            // script内の{{CHAT_MODEL_NAME}}をProperties.Settings.Default.CHAT_MODEL_NAMEに置換する
            script = script.Replace("{{CHAT_MODEL_NAME}}", Properties.Settings.Default.CHAT_MODEL_NAME);
            // script内の{{AZURE_OPENAI_ENDPOINT}}をProperties.Settings.Default.AZURE_OPENAI_ENDPOINTに置換する
            if (string.IsNullOrEmpty(Properties.Settings.Default.AZURE_OPENAI_ENDPOINT))
            {
                script = script.Replace("{{AZURE_OPENAI_ENDPOINT}}", "None");
            }
            else
            {
                script = script.Replace("{{AZURE_OPENAI_ENDPOINT}}", Properties.Settings.Default.AZURE_OPENAI_ENDPOINT);
            }


            // scriptの中のポート番号をFlaskPortに置換する
            script = script.Replace("{{PORT}}", FlaskPort.ToString());

            // テンポラリファイルにPythonスクリプトを書き込む
            string tempFilePath = Path.Combine(tempDir, "flask_app.py");
            File.WriteAllText(tempFilePath, script);

            // Pythonスクリプトを実行する

            System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
            psi.FileName = "python";
            psi.Arguments = tempFilePath;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.CreateNoWindow = true;
            System.Diagnostics.Process? p = System.Diagnostics.Process.Start(psi);
            if (p == null)
            {
                Tools.ShowMessage("Flaskアプリの起動に失敗しました");
                return;
            }
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            string error = p.StandardError.ReadToEnd();
            // tempOutputFilePathに標準出力、標準エラーを書き込む
            File.WriteAllText(tempOutputFilePath, output + error);

        }
        public static void RunFlask(CancellationToken token)
        {
            BackgroundWorker wc = new BackgroundWorker();
            wc.WorkerSupportsCancellation = true;
            wc.DoWork += (sender, e) =>
            {
                FlaskThread();
            };
            token.Register(() =>
            {
                wc.CancelAsync();
            });
            wc.RunWorkerAsync();

        }

        // テンプレートファイルを読み込む
        public static string LoadPythonScript(string scriptName)
        {
            var info = Application.GetResourceStream(new Uri(scriptName, UriKind.Relative));
            string script = "";
            if (info == null)
            {
                throw new ThisApplicationException("Pythonスクリプトファイルが見つかりません:" + scriptName);
            }
            using (var sr = new StreamReader(info.Stream))
            {
                script = sr.ReadToEnd();
            }
            return script;
        }
        public static string ExtractText(string path)
        {
            string script = LoadPythonScript(ClipboardAppUtilsScript);
            // Pythonスクリプトを実行する
            using (Py.GIL())
            {
                try
                {
                    PyModule ps = Py.CreateScope();
                    ps.Exec(script);

                    // Pythonスクリプトの関数を呼び出す
                    dynamic extract_text = ps.Get("extract_text");
                    // extract_textが呼び出せない場合は例外をスロー
                    if (extract_text == null)
                    {
                        throw new ThisApplicationException("Pythonスクリプトファイルにextract_text関数が見つかりません");
                    }
                    // extract_text関数を呼び出す
                    string result = extract_text(path);
                    return result;
                }
                catch (PythonException e)
                {
                    string pythonErrorMessage = e.Message;
                    string message = "Pythonスクリプトの実行中にエラーが発生しました\n";
                    if (pythonErrorMessage.Contains("No module named"))
                    {
                        message += "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。\n";
                    }
                    message += string.Format("メッセージ:\n{0}\nスタックトレース:\n{1}", e.Message, e.StackTrace);

                    throw new ThisApplicationException(message);

                }

            }
        }
        // データをマスキングする
        public static string MaskData(string text)
        {
            // PropertiesからSPACY_MODEL_NAMEを取得
            string SPACY_MODEL_NAME = Properties.Settings.Default.SPACY_MODEL_NAME;
            // SPACY_MODEL_NAMEが空の場合は例外をスロー
            if (string.IsNullOrEmpty(SPACY_MODEL_NAME))
            {
                throw new ThisApplicationException("Spacyモデル名が設定されていません。設定画面からSPACY_MODEL_NAMEを設定してください");
            }
            // PYthonスクリプトを読み込む
            string script = LoadPythonScript(ClipboardAppUtilsScript);

            // Pythonスクリプトを実行する
            using (Py.GIL())
            {
                try
                {
                    PyModule ps = Py.CreateScope();
                    ps.Exec(script);

                    // Pythonスクリプトの関数を呼び出す
                    dynamic mask_data = ps.Get("mask_data");
                    // mask_dataが呼び出せない場合は例外をスロー
                    if (mask_data == null)
                    {
                        throw new ThisApplicationException("Pythonスクリプトファイルにmask_data関数が見つかりません");
                    }
                    // mask_data関数を呼び出す
                    string result = mask_data(SPACY_MODEL_NAME, text);
                    return result;
                }
                catch (PythonException e)
                {
                    string pythonErrorMessage = e.Message;
                    string message = "Pythonスクリプトの実行中にエラーが発生しました\n";
                    if (pythonErrorMessage.Contains("No module named"))
                    {
                        message += "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。\n";
                    }
                    message += string.Format("メッセージ:\n{0}\nスタックトレース:\n{1}", e.Message, e.StackTrace);

                    throw new ThisApplicationException(message);

                }

            }
        }
        public static string OpenAIChat(List<JSONChatItem> jSONChatItems)
        {
            // Pythonスクリプトを実行する
            using (Py.GIL())
            {
                try
                {
                    PyModule ps = Py.CreateScope();
                    string script = LoadPythonScript(ClipboardAppUtilsScript);
                    ps.Exec(script);
                    // Pythonスクリプトの関数を呼び出す
                    dynamic openai_chat = ps.Get("openai_chat");
                    // openai_chatが呼び出せない場合は例外をスロー
                    if (openai_chat == null)
                    {
                        throw new ThisApplicationException("Pythonスクリプトファイルにopenai_chat関数が見つかりません");
                    }
                    // openai_chat関数を呼び出す
                    // 引数として
                    // - ChatItemsをJSON文字列に変換したもの
                    // - json_mode
                    // - azure_openai
                    // - openai_api_key
                    // - chat_model_name
                    // - azure_openai_endpoint
                    // を渡す
                    // ChatItemsをJSON文字列に変換
                    string json_string = ToJson(jSONChatItems);
                    bool json_mode = false;
                    bool azure_openai = Properties.Settings.Default.AZURE_OPENAI;
                    string openai_api_key = Properties.Settings.Default.OPENAI_API_KEY;
                    string chat_model_name = Properties.Settings.Default.CHAT_MODEL_NAME;
                    string azure_openai_endpoint = Properties.Settings.Default.AZURE_OPENAI_ENDPOINT;


                    string result = openai_chat(json_string, json_mode, azure_openai, openai_api_key, chat_model_name, azure_openai_endpoint);
                    // System.Windows.MessageBox.Show(result);


                    return result;
                }
                catch (PythonException e)
                {
                    string pythonErrorMessage = e.Message;
                    string message = "Pythonスクリプトの実行中にエラーが発生しました\n";
                    if (pythonErrorMessage.Contains("No module named"))
                    {
                        message += "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。\n";
                    }
                    message += string.Format("メッセージ:\n{0}\nスタックトレース:\n{1}", e.Message, e.StackTrace);

                    throw new ThisApplicationException(message);

                }

            }

        }

        public static ClipboardItem? RunScript(ScriptItem scriptItem, ClipboardItem clipboardItem)
        {
            if (scriptItem == null)
            {
                throw new ThisApplicationException("スクリプトが指定されていません");
            }
            if (clipboardItem == null)
            {
                throw new ThisApplicationException("クリップボードアイテムが指定されていません");
            }
            if (string.IsNullOrEmpty(scriptItem.Content))
            {
                throw new ThisApplicationException("スクリプトが空です");
            }
            if (string.IsNullOrEmpty(clipboardItem.Content))
            {
                throw new ThisApplicationException("クリップボードアイテムの内容が空です");
            }
            // Pythonスクリプトを実行する
            using (Py.GIL())
            {
                try
                {
                    PyModule ps = Py.CreateScope();
                    ps.Exec(scriptItem.Content);

                    // Pythonスクリプトの関数を呼び出す
                    dynamic func_name = ps.Get("execute");
                    // extract_textが呼び出せない場合は例外をスロー
                    if (func_name == null)
                    {
                        throw new ThisApplicationException("Pythonスクリプトファイルに、execute関数が見つかりません");
                    }
                    // ClipboardItemをJSON文字列に変換
                    string json_string = ToJson(clipboardItem);
                    // extract_text関数を呼び出す
                    string result = func_name(json_string);
                    ClipboardItem? resultItem = FromJson(result);
                    if (resultItem == null)
                    {
                        Tools.ShowMessage("Pythonの処理結果のJSON文字列をClipboardItemに変換できませんでした");
                        return null;
                    }
                    return resultItem;

                }
                catch (PythonException e)
                {
                    string pythonErrorMessage = e.Message;
                    string message = "Pythonスクリプトの実行中にエラーが発生しました\n";
                    if (pythonErrorMessage.Contains("No module named"))
                    {
                        message += "Pythonのモジュールが見つかりません。pip install <モジュール名>>でモジュールをインストールしてください。\n";
                    }
                    message += string.Format("メッセージ:\n{0}\nスタックトレース:\n{1}", e.Message, e.StackTrace);

                    throw new ThisApplicationException(message);

                }
                catch (Exception e)
                {
                    string message = "Pythonスクリプトの実行中にエラーが発生しました\n";
                    message += string.Format("メッセージ:\n{0}\nスタックトレース:\n{1}", e.Message, e.StackTrace);
                    throw new ThisApplicationException(message);
                }

            }

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

        // ClipboardItemをJSON文字列に変換する
        public static string ToJson(ClipboardItem item)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return JsonSerializer.Serialize(item, options);
        }
        // JSON文字列をClipboardItemに変換する
        public static ClipboardItem? FromJson(string json)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            ClipboardItem? item = JsonSerializer.Deserialize<ClipboardItem>(json, options);
            if (item == null)
            {
                Tools.ShowMessage("Pythonの処理結果のJSON文字列をClipboardItemに変換できませんでした");
                return null;
            }

            return item;

        }

        // ChatItemsをJSON文字列に変換する
        public static string ToJson(List<JSONChatItem> items)
        {
            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return JsonSerializer.Serialize(items, options);
        }
    }
}
