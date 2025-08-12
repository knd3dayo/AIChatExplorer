using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.PythonIF;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.Folders;

namespace LibPythonAI.Common {
    public class PythonAILibManager {

        private static PythonAILibManager? _instance;
        public static PythonAILibManager Instance {
            get {
                return _instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            }
            private set {
                _instance = value;
            }
        }

        // 初期化タスク
        private Task? _initTask;

        /// <summary>
        /// PythonAILibManagerの初期化
        /// IPythonAILibConfigPramsを実装したクラスを引数に渡す
        /// </summary>
        /// <param name="parmas"></param>
        public static void Init(IPythonAILibConfigParams parmas) {
            // Set environment variables HTTP_PROXY, HTTPS_PROXY
            string proxyUrl = parmas.GetHttpsProxy();
            if (!string.IsNullOrEmpty(proxyUrl)) {
                Environment.SetEnvironmentVariable("HTTP_PROXY", proxyUrl);
                Environment.SetEnvironmentVariable("HTTPS_PROXY", proxyUrl);
            }
            // Set environment variable NO_PROXY
            string noProxy = parmas.GetNoProxy();
            if (!string.IsNullOrEmpty(noProxy)) {
                Environment.SetEnvironmentVariable("NO_PROXY", noProxy);
            }
            try {
                Instance = new PythonAILibManager(parmas);
                // PythonExecutor initialization 
                Instance._initTask = Task.Run(() => Instance.InitPythonExecutor(parmas));

                // Wait for initialization to complete. If not initialized in 3 minutes, throw exception
                if (!Instance.WaitForInitialization(180000)) {
                    string timeoutMsg = "PythonAILibManager initialization timed out.";
                    LogWrapper.Error(timeoutMsg);
                    throw new TimeoutException(timeoutMsg);
                }
            } catch (Exception ex) {
                string errorMsg = $"PythonAILibManager initialization failed: {ex.Message}";
                LogWrapper.Error(errorMsg);
                throw new Exception(errorMsg, ex);
            }
        }

        private bool WaitForInitialization(int timeout = 60000) {
            int elapsed = 0;
            while (!IsInitialized && elapsed < timeout) {
                Thread.Sleep(100);
                elapsed += 100;
            }
            return IsInitialized;
        }

        public IPythonAILibConfigParams ConfigParams { get; private set; }

        public bool IsInitialized { get; private set; } = false;

        private PythonAILibManager(IPythonAILibConfigParams parameters) {

            ConfigParams = parameters;
            // 言語設定
            PythonAILibStringResourcesJa.Lang = parameters.GetLang();
        }

        private void InitPythonExecutor(IPythonAILibConfigParams parameters) {
            // PythonExecutorの初期化
            // Python処理機能の初期化
            PythonExecutor.Init(parameters, afterStartProcess: async (process) => {
                var folderTask = FolderManager.InitAsync();
                var promptTask = PromptItem.LoadItemsAsync();
                var vectorTask = VectorDBItem.LoadItemsAsync();
                var autoProcessItemTask = AutoProcessItem.LoadItemsAsync();
                var autoProcessRuleTask = AutoProcessRule.LoadItemsAsync();
                var searchRuleTask = SearchRule.LoadItemsAsync();

                // 並列実行
                await Task.WhenAll(folderTask, promptTask, vectorTask, autoProcessItemTask, autoProcessRuleTask, searchRuleTask);

                // PythonAILibManagerの初期化完了
                IsInitialized = true;
            });
        }

    }
}
