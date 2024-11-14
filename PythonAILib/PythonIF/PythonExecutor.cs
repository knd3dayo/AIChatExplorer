using System.IO;
using System.Runtime.CompilerServices;
using Python.Runtime;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;
namespace PythonAILib.PythonIF
{
    public class PythonExecutor {
        // String definition instance
        public static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;


        // Template file for custom Python scripts
        public static string TemplateScript { get; } = StringResources.TemplateScript;

        // Python script for OpenAI
        public static string WpfAppCommonOpenAIScript {
            get {
                string path = Path.Combine(PythonAILibPath, "ai_app_wrapper.py");
                return path;
            }
        }

        // Python script for Misc
        public static string WpfAppCommonMiscScript {
            get {
                string devPath = Path.Combine(PythonAILibPath, "dev");
                string path = Path.Combine(devPath, "misc_app.py");
                return path;
            }
        }
        // HttpsProxy
        public static string HttpsProxy { get; set; } = "";
        // NoProxy
        public static string NoProxy { get; set; } = "";

        public static string? PythonPath { get; set; }

        private static string PathToVirtualEnv { get; set; } = "";

        private static string PythonAILibPath { get; set; } = "python_ai_lib";

        private static IPythonAIFunctions? _pythonAIFunctions;
        public static IPythonAIFunctions PythonAIFunctions {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get {
                if (string.IsNullOrEmpty(PythonPath)) {
                    throw new Exception(StringResources.PythonDLLNotFound);
                }
                if (_pythonAIFunctions == null) {
                    InitPythonNet(PythonPath, PathToVirtualEnv, PythonAILibPath, HttpsProxy, NoProxy);
                    _pythonAIFunctions = new PythonNetFunctions();
                }
                return _pythonAIFunctions;
            }
        }

        private static IPythonMiscFunctions? _pythonMiscFunctions;
        public static IPythonMiscFunctions PythonMiscFunctions {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get {
                if (string.IsNullOrEmpty(PythonPath)) {
                    throw new Exception(StringResources.PythonDLLNotFound);

                }
                if (_pythonMiscFunctions == null) {
                    _pythonMiscFunctions = new PythonMiscFunctions();

                }
                return _pythonMiscFunctions;
            }
        }
        // Initialize Python functions
        public static void Init(string pythonPath, string pathToVirtualEnv, string pythonAILibPathRoot, string httpsProxy, string noProxy) {
            
            HttpsProxy = httpsProxy;
            NoProxy = noProxy;

            PythonPath = pythonPath;
            if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                PathToVirtualEnv = pathToVirtualEnv;
            }
            if (!string.IsNullOrEmpty(pythonAILibPathRoot)) {

                // ★TODO Pythonスクリプトをアプリケーション用ディレクトリにコピーする処理
                // バージョンアップ時には、アプリケーション用ディレクトリにコピーする処理が必要となるが、
                // 未実装のため、一旦コメントアウトしておく
                // PythonAILibPath = Path.Combine(pythonAILibPathRoot, "python_ai_lib");

                PythonAILibPath = "python_ai_lib";

                // Check if the PythonAILibPath exists
                if (!Directory.Exists(PythonAILibPath)) {
                    // ./pythonディレクトリをPythonAILibPathRootへコピーする
                    Tools.CopyDirectory("python_ai_lib", PythonAILibPath, true);
                }
            }
        }


        private static void InitPythonNet(string pythonDLLPath, string pathToVirtualEnv, string pythonAILibPath, string httpsProxy, string noProxy) {
            // Pythonスクリプトを実行するための準備

            // 既に初期化されている場合は初期化しない
            if (PythonEngine.IsInitialized) {
                return;
            }

            // PythonDLLのパスを設定
            Runtime.PythonDLL = pythonDLLPath;

            // Runtime.PythonDLLのファイルが存在するかチェック
            if (!File.Exists(Runtime.PythonDLL)) {
                string message = StringResources.PythonDLLNotFound;
                throw new Exception(message + Runtime.PythonDLL);

            }
            // Venv環境が存在するかチェック
            if (!string.IsNullOrEmpty(pathToVirtualEnv) && !Directory.Exists(pathToVirtualEnv)) {
                string message = StringResources.PythonVenvNotFound;
                throw new Exception(message + pathToVirtualEnv);
            }

            try {
                // venvを使用する場合の設定
                // 公式ドキュメントの設定ではPythonEngine.Initialize()時にクラッシュするため、
                // 以下を参考にして設定を行う
                // https://github.com/pythonnet/pythonnet/issues/1478#issuecomment-897933730

                // PythonEngineにアクセスするためのダミー処理
                string version = PythonEngine.Version;

                if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                    LogWrapper.Info($"Python Version: {version}");
                    // 実行中の Python のユーザー site-packages へのパスを無効にする
                    PythonEngine.SetNoSiteFlag();
                }

                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();


                // sys.prefix、sys.exec_prefixを venvのパスに変更

                using (Py.GIL()) {
                    // fix the prefixes to point to our venv
                    // (This is for Windows, there may be some difference with sys.exec_prefix on other platforms)
                    dynamic sys = Py.Import("sys");
                    dynamic site = Py.Import("site");
                    dynamic os = Py.Import("os");
                    if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                        sys.prefix = pathToVirtualEnv;
                        sys.exec_prefix = pathToVirtualEnv;

                        // This has to be overwritten because site module may already have 
                        // been loaded by the interpreter (but not run yet)
                        site.PREFIXES = new List<PyObject> { sys.prefix, sys.exec_prefix };
                    }
                    // set the path to pythonAILib
                    site.addsitedir(pythonAILibPath);

                    // set the proxy settings
                    if (!string.IsNullOrEmpty(httpsProxy)) {
                        os.environ["HTTPS_PROXY"] = httpsProxy;
                        os.environ["NO_PROXY"] = noProxy;
                    } else {
                        // NO_PROXY="*"
                        os.environ["NO_PROXY"] = "*";
                    }

                    // Run site path modification with tweaked prefixes
                    site.main();
                }

            } catch (TypeInitializationException e) {
                string message = StringResources.PythonInitFailed + e.Message;
                LogWrapper.Error(message);
            }
        }

        // Load Python script
        public static string LoadPythonScript(string scriptName) {
            var file = new FileInfo(scriptName);
            if (!file.Exists) {
                // Load the template file

                file = new FileInfo(TemplateScript);
                if (!file.Exists) {
                    throw new Exception(StringResources.TemplateScriptNotFound);
                }
                return File.ReadAllText(file.FullName);
            }
            // Load the file
            string script = File.ReadAllText(file.FullName);

            return script;
        }
    }

}
