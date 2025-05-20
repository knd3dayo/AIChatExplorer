using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Resources;

namespace PythonAILib.PythonIF {
    public class PythonExecutor {
        // String definition instance
        public static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;


        private static IPythonAILibConfigParams? ConfigPrams;

        public static string? PythonPath { get; set; }

        private static string PythonAILibPath { get; set; } = DefaultPythonAILibDir;

        private const string DefaultPythonAILibDir = "python_lib";

        private static IPythonAIFunctions? _pythonAIFunctions;
        public static IPythonAIFunctions PythonAIFunctions {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get {
                if (_pythonAIFunctions == null) {
                    throw new Exception(StringResources.PythonNotInitialized);
                }
                return _pythonAIFunctions;
            }
        }

        // Initialize Python functions
        public static void Init(IPythonAILibConfigParams configPrams) {
            ConfigPrams = configPrams;

            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();
            string appDataDir = configPrams.GetAppDataPath();


            if (!string.IsNullOrEmpty(appDataDir)) {

                PythonAILibPath = Path.Combine(appDataDir, DefaultPythonAILibDir);
                // Check if the PythonAILibPath exists
                // ./pythonディレクトリをPythonAILibPathRootへコピーする
                Tools.CopyDirectory(DefaultPythonAILibDir, PythonAILibPath, true, true);
            }
            if (_pythonAIFunctions == null) {
                if (ConfigPrams.UseInternalAPI()) {
                    InitInternalAPI(ConfigPrams);
                    string baseUrl = ConfigPrams.GetAPIServerURL();
                    _pythonAIFunctions = new PythonAPIFunctions(baseUrl);

                } else if (ConfigPrams.UseExternalAPI()) {
                    string baseUrl = ConfigPrams.GetAPIServerURL();
                    _pythonAIFunctions = new PythonAPIFunctions(baseUrl);
                } else {
                    throw new Exception(StringResources.PythonNotInitialized);
                }
            }
        }

        // InitInternalAPI
        public static void InitInternalAPI(IPythonAILibConfigParams configPrams) {

            LogWrapper.Info("Internal API started");

            // 自分自身のプロセスIDを取得
            int currentProcessId = Environment.ProcessId;

            // AIアプリケーションサーバーを開始する
            string serverScriptPath = "-m ai_chat_lib.api_modules.ai_app_server";
            // APP_DATAのパスを取得
            string app_data_path = configPrams.GetAppDataPath();
            string serverCmdLine = $"{serverScriptPath} {app_data_path}";
            LogWrapper.Info($"ServerCmdLine:{serverCmdLine}");

            StartPythonConsole(configPrams, serverCmdLine, false, (process) => { });

            // AIアプリケーションプロセスチェッカーを開始する。
            string url = $"{configPrams.GetAPIServerURL()}/shutdown";

            string processCheckerScriptPath = "-m ai_chat_lib.api_modules.ai_app_process_checker";
            string processCheckerCmdLine = $"{processCheckerScriptPath} {currentProcessId} {url}";
            LogWrapper.Info($"ProcessCheckerCmdLine:{processCheckerCmdLine}");

            StartPythonConsole(configPrams, processCheckerCmdLine, false, (Process) => { });

        }


        private static void StartPythonConsole(IPythonAILibConfigParams configPrams, string cmdLine, bool showConsole, Action<Process> afterStart) {
            // Pythonスクリプトを実行するための準備
            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();
            string appDataDir = configPrams.GetAppDataPath();
            string pythonAILibPath = PythonAILibPath;
            string httpsProxy = configPrams.GetHttpsProxy();
            string noProxy = configPrams.GetNoProxy();

            // Venv環境が存在するかチェック
            if (!string.IsNullOrEmpty(pathToVirtualEnv) && !Directory.Exists(pathToVirtualEnv)) {
                string message = StringResources.PythonVenvNotFound;
                LogWrapper.Error($"{message}:{pathToVirtualEnv}");
                return;
            }
            // Environment variables
            Dictionary<string, string> envVars = new();

            // PYTHONPATH
            envVars["PYTHONPATH"] = pythonAILibPath;
            // Set the proxy settings
            if (!string.IsNullOrEmpty(httpsProxy)) {
                envVars["HTTPS_PROXY"] = httpsProxy;
                envVars["NO_PROXY"] = noProxy;
            } else {
                // NO_PROXY="*"
                envVars["NO_PROXY"] = "*";
            }
            // LOGLEVEL
            envVars["LOGLEVEL"] = "DEBUG";

            // ProcessController 
            List<string> cmdLines = [];
            // cd to the appDataDir
            cmdLines.Add($"cd {appDataDir}");

            // Set the code page to UTF-8
            cmdLines.Add("chcp 65001");
            // Activate the venv if it is valid
            if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                envVars["VIRTUAL_ENV"] = pathToVirtualEnv;
                string venvActivateScript = Path.Combine(pathToVirtualEnv, "Scripts", "activate");
                cmdLines.Add($"call {venvActivateScript}");
            }

            cmdLines.Add($"python {cmdLine}");

            DataReceivedEventHandler dataReceivedEventHandler = new(DataReceivedAction);

            ProcessUtil.StartWindowsBackgroundCommandLine(cmdLines, envVars, showConsole,
                (Process process) => {
                    // 5秒待機した後、processが終了したかどうかを確認する
                    Task.Run(() => {
                        // このスレッドを5秒間待機
                        Task.Delay(5000).Wait();

                        if (process.HasExited) {
                            LogWrapper.Error($"Process failed: {process.Id}");
                        } else {
                            LogWrapper.Info($"Process started: {process.Id}");
                            afterStart(process);
                        }

                    });
                }, OutputDataReceived: dataReceivedEventHandler, ErrorDataReceived: dataReceivedEventHandler
                );

        }

        private static readonly Action<object, DataReceivedEventArgs> DataReceivedAction = (sender, e) => {
            Task.Run(() => {
                string? message = e.Data;
                if (message != null) {
                    LogWrapper.Debug("api server output:" + message);
                }
            });
        };


        // Load Python script
        public static string LoadPythonScript(string scriptName) {
            var file = new FileInfo(scriptName);
            // Load the file
            string script = File.ReadAllText(file.FullName);

            return script;
        }
    }

}
