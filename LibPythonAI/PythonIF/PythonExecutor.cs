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
        public static void Init(IPythonAILibConfigParams configPrams, Action<Process?> afterStartProcess ) {
            ConfigPrams = configPrams;

            string pathToVirtualEnv = configPrams.GetPathToVirtualEnv();

            if (_pythonAIFunctions == null) {
                if (ConfigPrams.UseInternalAPI()) {
                    string baseUrl = ConfigPrams.GetAPIServerURL();
                    _pythonAIFunctions = new PythonAPIFunctions(baseUrl);
                    InitInternalAPI(ConfigPrams, afterStartProcess);

                } else if (ConfigPrams.UseExternalAPI()) {
                    string baseUrl = ConfigPrams.GetAPIServerURL();
                    _pythonAIFunctions = new PythonAPIFunctions(baseUrl);
                    afterStartProcess(null);

                } else {
                    throw new Exception(StringResources.PythonNotInitialized);
                }
            }
        }

        // InitInternalAPI
        public static void InitInternalAPI(IPythonAILibConfigParams configPrams, Action<Process?> afterStartProcess) {

            LogWrapper.Info("Internal API started");

            // 自分自身のプロセスIDを取得
            int currentProcessId = Environment.ProcessId;

            // AIアプリケーションサーバーを開始する
            string serverScriptPath = "-m ai_chat_lib.api_modules.ai_app_server";
            // APP_DATAのパスを取得
            string app_data_path = configPrams.GetAppDataPath();
            string serverCmdLine = $"{serverScriptPath} {app_data_path}";
            LogWrapper.Info($"ServerCmdLine:{serverCmdLine}");

            StartPythonConsole(configPrams, serverCmdLine, false, afterStartProcess);

            // AIアプリケーションプロセスチェッカーを開始する。
            string url = $"{configPrams.GetAPIServerURL()}/shutdown";

            string processCheckerScriptPath = "-m ai_chat_lib.api_modules.ai_app_process_checker";
            string processCheckerCmdLine = $"{processCheckerScriptPath} {currentProcessId} {url}";
            LogWrapper.Info($"ProcessCheckerCmdLine:{processCheckerCmdLine}");

            StartPythonConsole(configPrams, processCheckerCmdLine, false, (Process) => { });

        }

        private static Dictionary<string, string> SetOpenAIPropsEnvVars(IPythonAILibConfigParams configPrams) {
            /**
             IPythonAILibConfigParamsから以下の環境変数を設定する。値がnullまたは空文字列ものがある場合は、例外を投げる。
            "OPENAI_API_KEY",
             "AZURE_OPENAI",
             "AZURE_OPENAI_API_VERSION",
             "AZURE_OPENAI_ENDPOINT",
             "OPENAI_BASE_URL"
            **/
            Dictionary<string, string> envVars = new();
            OpenAIProperties openAIProps = configPrams.GetOpenAIProperties();
            if (!string.IsNullOrEmpty(openAIProps.OpenAIKey)) {
                envVars["OPENAI_API_KEY"] = openAIProps.OpenAIKey;
            } else {
                throw new ArgumentException($"{StringResources.PropertyNotSet} {nameof(openAIProps.OpenAIKey)}");
            }

            if (openAIProps.AzureOpenAI) {
                envVars["AZURE_OPENAI"] = "true";
                if (!string.IsNullOrEmpty(openAIProps.AzureOpenAIAPIVersion)) {
                    envVars["AZURE_OPENAI_API_VERSION"] = openAIProps.AzureOpenAIAPIVersion;
                } else {
                    throw new ArgumentException($"{StringResources.PropertyNotSet} {nameof(openAIProps.AzureOpenAIAPIVersion)}");
                }
                if (!string.IsNullOrEmpty(openAIProps.AzureOpenAIEndpoint)) {
                    envVars["AZURE_OPENAI_ENDPOINT"] = openAIProps.AzureOpenAIEndpoint;
                } else {
                    throw new ArgumentException($"{StringResources.PropertyNotSet} {nameof(openAIProps.AzureOpenAIEndpoint)}");
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
            } else {
                throw new ArgumentException($"{StringResources.PropertyNotSet} {nameof(openAIProps.OpenAICompletionModel)}");
            }
            // OPENAI_EMBEDDING_MODEL
            if (!string.IsNullOrEmpty(openAIProps.OpenAIEmbeddingModel)) {
                envVars["OPENAI_EMBEDDING_MODEL"] = openAIProps.OpenAIEmbeddingModel;
            } else {
                throw new ArgumentException($"{StringResources.PropertyNotSet} {nameof(openAIProps.OpenAIEmbeddingModel)}");
            }

            return envVars;

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
            Dictionary<string, string> envVars = SetOpenAIPropsEnvVars(configPrams);

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
