using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using LibPythonAI.Common;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.PythonIF {
    public class PythonExecutor {
        // String definition instance
        public static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;


        private static IPythonAILibConfigParams? ConfigPrams;

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
        public static void Init(IPythonAILibConfigParams configPrams, Action<Process?> afterStartProcess ) {
            ConfigPrams = configPrams;

            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();

            if (_pythonAIFunctions == null) {
                if (ConfigPrams.IsUseInternalAPI()) {
                    string baseUrl = ConfigPrams.GetAPIServerURL();
                    _pythonAIFunctions = new PythonAPIFunctions(baseUrl);
                    InitInternalAPI(ConfigPrams, afterStartProcess);

                } else if (ConfigPrams.IsUseExternalAPI()) {
                    string baseUrl = ConfigPrams.GetAPIServerURL();
                    _pythonAIFunctions = new PythonAPIFunctions(baseUrl);
                    afterStartProcess(null);

                } else {
                    throw new Exception(StringResources.PythonNotInitialized);
                }
            }
        }

        public enum PythonEnvironmentCheckResult {
            Success,
            PythonNotFound,
            UvNotFound,
            PythonVenvPathNotFound,
            PythonVenvNotFound,
            OpenAIKeyNotSet
        }

        public static PythonEnvironmentCheckResult CheckPythonEnvironment(IPythonAILibConfigParams configPrams) {
            // Check if Python is installed and accessible
            bool checkPython = ProcessUtil.CheckCommand("python", "-V");
            if (!checkPython) {
                return PythonEnvironmentCheckResult.PythonNotFound;
            }
            // Check if uv is installed and accessible
            bool checkUv = ProcessUtil.CheckCommand("uv", "-V");
            if (!checkUv) {
                return PythonEnvironmentCheckResult.UvNotFound;
            }

            // Check if the Python virtual environment exists
            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();
            if (string.IsNullOrEmpty(pathToVirtualEnv)) {
                return PythonEnvironmentCheckResult.PythonVenvPathNotFound;
            }
            // Check if the Python virtual environment is set up correctly
            string pythonVenvPath = Path.Combine(pathToVirtualEnv, "Scripts", "python.exe");

            if (!File.Exists(pythonVenvPath)) {
                return PythonEnvironmentCheckResult.PythonVenvNotFound;
            }
            // Check if OpenAI API key is set
            OpenAIProperties openAIProps = configPrams.GetOpenAIProperties();
            if (string.IsNullOrEmpty(openAIProps.OpenAIKey)) {
                return PythonEnvironmentCheckResult.OpenAIKeyNotSet;
            }

            return PythonEnvironmentCheckResult.Success;
        }

        // Python Venv環境の作成とライブラリのインストールを行うメソッド
        public static void CreatePythonVenvAndInstallLibs(IPythonAILibConfigParams configPrams) {
            // Python仮想環境の作成とライブラリのインストールを行う
            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();
            if (string.IsNullOrEmpty(pathToVirtualEnv)) {
                throw new Exception(StringResources.PythonVenvPathNotFound);
            }
            // Python仮想環境の作成
            // 必要なライブラリのインストール
            ProcessUtil.StartWindowsCommandLine(
                [
                $"cd {configPrams.GetAppDataPath()}",
                $"python -m venv {pathToVirtualEnv}",
                $"curl -L https://github.com/knd3dayo/ai_chat_lib/archive/refs/heads/main.zip -o ai_chat_lib.zip",
                $"call powershell -command \"Expand-Archive  ai_chat_lib.zip\"",
                $"call {pathToVirtualEnv}\\Scripts\\Activate",
                $"pip install ai_chat_lib\\ai_chat_lib-main",
                $"pause"
                ],
                configPrams.GetAppDataPath(),
                waitForExit: true,
                (process) => {
                    if (process.HasExited && process.ExitCode != 0) {
                        throw new Exception(StringResources.PythonLibsInstallationFailed);
                    }
                }
            );
        }


        // InitInternalAPI
        public static void InitInternalAPI(IPythonAILibConfigParams configPrams, Action<Process?> afterStartProcess) {

            LogWrapper.Info("Internal API started");

            // 環境チェック
            var checkResult = CheckPythonEnvironment(configPrams);
            if (checkResult != PythonEnvironmentCheckResult.Success) {
                LogWrapper.Info($"Python environment check failed: {checkResult}");
                return;
            }

            // 自分自身のプロセスIDを取得
            int currentProcessId = Environment.ProcessId;

            // AIアプリケーションサーバーを開始する
            StartAPIServer(configPrams, afterStartProcess);

            // AIアプリケーションプロセスチェッカーを開始する。
            StartProcessChecker(configPrams, (processCheckerProcess) => { });

        }

        private static void StartAPIServer(IPythonAILibConfigParams configPrams, Action<Process?> afterStartProcess) {
            // AIアプリケーションサーバーを開始する

            string pythonAILibPath = configPrams.GetPathToVirtualEnv();
            string serverScriptPath = $"uv --directory {pythonAILibPath} run -m ai_chat_lib.api_modules.ai_app_server";
            // APP_DATAのパスを取得
            string app_data_path = configPrams.GetAppDataPath();
            string serverCmdLine = $"{serverScriptPath} {app_data_path}";
            LogWrapper.Info($"ServerCmdLine:{serverCmdLine}");
            StartPythonConsole(configPrams, serverCmdLine, false, afterStartProcess);
        }

        private static void StartProcessChecker(IPythonAILibConfigParams configPrams, Action<Process?> afterStartProcess) {
            // AIアプリケーションプロセスチェッカーを開始する。

            string pythonAILibPath = configPrams.GetPathToVirtualEnv();
            string url = $"{configPrams.GetAPIServerURL()}/shutdown";
            string processCheckerScriptPath = $"uv --directory {pythonAILibPath} run -m ai_chat_lib.api_modules.ai_app_process_checker";
            int currentProcessId = Environment.ProcessId;
            string processCheckerCmdLine = $"{processCheckerScriptPath} {currentProcessId} {url}";
            LogWrapper.Info($"ProcessCheckerCmdLine:{processCheckerCmdLine}");
            StartPythonConsole(configPrams, processCheckerCmdLine, false, afterStartProcess);
        }

        private static Dictionary<string, string> CreateEnvVarsDict(IPythonAILibConfigParams configPrams) {


            /**
             IPythonAILibConfigParamsから以下の環境変数を設定する。値がnullまたは空文字列ものがある場合は、例外を投げる。
            "OPENAI_API_KEY",
             "AZURE_OPENAI",
             "AZURE_OPENAI_API_VERSION",
             "AZURE_OPENAI_ENDPOINT",
             "OPENAI_BASE_URL"
            **/
            Dictionary<string, string> envVars = [];
            OpenAIProperties openAIProps = configPrams.GetOpenAIProperties();
            if (!string.IsNullOrEmpty(openAIProps.OpenAIKey)) {
                envVars["OPENAI_API_KEY"] = openAIProps.OpenAIKey;
            }

            if (openAIProps.AzureOpenAI) {
                envVars["AZURE_OPENAI"] = "true";
                if (!string.IsNullOrEmpty(openAIProps.AzureOpenAIAPIVersion)) {
                    envVars["AZURE_OPENAI_API_VERSION"] = openAIProps.AzureOpenAIAPIVersion;
                }
                if (!string.IsNullOrEmpty(openAIProps.AzureOpenAIEndpoint)) {
                    envVars["AZURE_OPENAI_ENDPOINT"] = openAIProps.AzureOpenAIEndpoint;
                }
            } else {
                envVars["AZURE_OPENAI"] = "false";
            }
            if (!string.IsNullOrEmpty(openAIProps.OpenAIBaseURL)) {
                envVars["OPENAI_BASE_URL"] = openAIProps.OpenAIBaseURL;
            }
            // OPENAI_COMPLETION_MODEL
            if (!string.IsNullOrEmpty(openAIProps.OpenAICompletionModel)) {
                envVars["OPENAI_COMPLETION_MODEL"] = openAIProps.OpenAICompletionModel;
            }
            // OPENAI_EMBEDDING_MODEL
            if (!string.IsNullOrEmpty(openAIProps.OpenAIEmbeddingModel)) {
                envVars["OPENAI_EMBEDDING_MODEL"] = openAIProps.OpenAIEmbeddingModel;
            }

            // Proxy settings
            string httpsProxy = configPrams.GetHttpsProxy();
            string noProxy = configPrams.GetNoProxy();

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

            // APP_DATA_DIR
            envVars["APP_DATA_DIR"] = configPrams.GetAppDataPath();

            return envVars;

        }

        private static void StartPythonConsole(IPythonAILibConfigParams configPrams, string mainCmdLine, bool showConsole, Action<Process> afterStart) {

            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();
            // Venv環境が存在するかチェック
            if (!string.IsNullOrEmpty(pathToVirtualEnv) && !Directory.Exists(pathToVirtualEnv)) {
                string message = StringResources.PythonVenvPathNotFound;
                LogWrapper.Error($"{message}:{pathToVirtualEnv}");
                return;
            }
            // Environment variables
            Dictionary<string, string> envVars = CreateEnvVarsDict(configPrams);

            // ProcessController 
            List<string> cmdLines = [];

            // Set the code page to UTF-8
            cmdLines.Add("chcp 65001");
            cmdLines.Add(mainCmdLine);

            DataReceivedEventHandler dataReceivedEventHandler = new(DataReceivedAction);

            ProcessUtil.StartWindowsBackgroundCommandLine(cmdLines, envVars, showConsole,
                (process) => {
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
