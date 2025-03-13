using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Resources;
using PythonAILib.Utils.Common;

namespace PythonAILib.PythonIF {
    public class PythonExecutor {
        // String definition instance
        public static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;

        // Python script for OpenAI
        public static string OpenAIScript {
            get {
                string path = Path.Combine(PythonAILibPath, "ai_app_wrapper.py");
                return path;
            }
        }

        // Python script for Misc
        public static string MiscScript {
            get {
                string devPath = Path.Combine(PythonAILibPath, "dev");
                string path = Path.Combine(devPath, "misc_app.py");
                return path;
            }
        }

        private static IPythonAILibConfigParams? ConfigPrams;

        public static string? PythonPath { get; set; }

        private static string PathToVirtualEnv { get; set; } = "";

        private static string PythonAILibPath { get; set; } = DefaultPythonAILibDir;

        private const string DefaultPythonAILibDir = "python_ai_lib";

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

            if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                PathToVirtualEnv = pathToVirtualEnv;
            }
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

            // AIアプリケーションプロセスチェッカーを開始する。
            string url = $"{configPrams.GetAPIServerURL()}/shutdown";

            // プロセスを終了する
            StartPython(configPrams, $"ai_app_server_shutdown.py {url}", false, (Process) => { });


            LogWrapper.Info("Internal API started");

            // 自分自身のプロセスIDを取得
            int currentProcessId = Process.GetCurrentProcess().Id;

            // StartPython(configPrams, $"ai_app_process_checker.py {currentProcessId} {url}", true, (Process) => { });
            StartPythonConsole(configPrams, $"ai_app_process_checker.py {currentProcessId} {url}", false, (Process) => { });

            // AIアプリケーションサーバーを開始する。
            // StartPython(configPrams, "ai_app_server.py", true, (process) => { });
            StartPythonConsole(configPrams, "ai_app_server.py", false, (process) => { });
        }


        private static void StartPythonConsole(IPythonAILibConfigParams configPrams, string scriptPath, bool showConsole, Action<Process> afterStart) {
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
            // PF_TRACE
            envVars["PF_TRACE"] = "true";
            // PF_TRACE_PATH
            envVars["PF_TRACE_PATH"] = Path.Combine(appDataDir, "pf_trace");
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

            // ProcessController 
            List<string> cmdLines = [];
            // cd to the appDataDir
            cmdLines.Add($"cd {appDataDir}");

            // Set the code page to UTF-8
            cmdLines.Add("chcp 65001");
            // Activate the venv if it is valid
            if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                string venvActivateScript = Path.Combine(pathToVirtualEnv, "Scripts", "activate");
                cmdLines.Add($"call {venvActivateScript}");
            }
            string serverScriptPath = Path.Combine(pythonAILibPath, scriptPath);
            cmdLines.Add($"python {serverScriptPath}");

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

        private static void StartPython(IPythonAILibConfigParams configPrams, string scriptPath, bool background, Action<Process> afterStart) {
            // Pythonスクリプトを実行するための準備
            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();
            string appDataDir = configPrams.GetAppDataPath();
            string pythonAILibPath = PythonAILibPath;
            string httpsProxy = configPrams.GetHttpsProxy();
            string noProxy = configPrams.GetNoProxy();

            // Venv環境が存在するかチェック
            if (!string.IsNullOrEmpty(pathToVirtualEnv) && !Directory.Exists(pathToVirtualEnv)) {
                string message = StringResources.PythonVenvNotFound;
                LogWrapper.Error($"{message}:{pathToVirtualEnv}" );
                return;
            }

            Dictionary<string, string> envVars = new() {
                // Set the code page to UTF-8
                ["PYTHONUTF8"] = "1"
            };
            // PF_TRACE
            envVars["PF_TRACE"] = "true";

            // VIRTUAL_ENV
            if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                envVars["VIRTUAL_ENV"] = pathToVirtualEnv;
            }
            // PYTHONPATH
            envVars["PYTHONPATH"] = pythonAILibPath;
            // HTTPS_PROXY
            if (!string.IsNullOrEmpty(httpsProxy)) {
                envVars["HTTPS_PROXY"] = httpsProxy;
                envVars["NO_PROXY"] = noProxy;
            } else {
                // NO_PROXY="*"
                envVars["NO_PROXY"] = "*";
            }
            // PATH
            string pythonExe = "python";
            if (!string.IsNullOrEmpty(pathToVirtualEnv)) {
                string path = Environment.GetEnvironmentVariable("PATH") ?? "";
                path = $"{Path.Combine(pathToVirtualEnv, "Scripts")}{Path.PathSeparator}{path}";
                envVars["PATH"] = path;
                pythonExe = Path.Combine(pathToVirtualEnv, "Scripts", "python.exe");
            }

            string serverScriptPath = Path.Combine(pythonAILibPath, scriptPath);

            // PF_TRACE
            envVars["PF_TRACE"] = "true";

            DataReceivedEventHandler dataReceivedEventHandler = new(DataReceivedAction);

            bool showConsole = false;

            if (background) {
                ProcessUtil.StartBackgroundProcess(pythonExe, serverScriptPath, envVars, showConsole, (Process process) => {
                    // 5秒待機した後、processが終了したかどうかを確認する
                    Task.Run(() => {
                        // このスレッドを5秒間待機
                        Task.Delay(5000).Wait();

                        if (process.HasExited) {
                            LogWrapper.Error($"Process failed: {process.Id}: {pythonExe} {serverScriptPath} ");
                        } else {
                            LogWrapper.Info($"Process started: {process.Id}:  {pythonExe} {serverScriptPath} ");
                            afterStart(process);
                        }

                    });
                }, ErrorDataReceived: dataReceivedEventHandler);
            } else {
                ProcessUtil.StartForegroundProcess(pythonExe, serverScriptPath, envVars, showConsole, (Process process) => { },
                    OutputDataReceived: dataReceivedEventHandler, ErrorDataReceived: dataReceivedEventHandler);

            }
        }

        private static readonly Action<object, DataReceivedEventArgs> DataReceivedAction = (sender, e) => {
            Task.Run(() => {
                string? message = e.Data;
                if (message != null) {
                    LogWrapper.Debug("flask output:" + message);
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
