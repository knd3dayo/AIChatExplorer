using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.PythonIF;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibPythonAI.Model.Search;

namespace LibPythonAI.Common {
    public class PythonAILibManager {

        private static PythonAILibManager? _instance;
        public static PythonAILibManager Instance {
            get {
                return _instance ?? throw new Exception(PythonAILibStringResourcesJa.Instance.PythonAILibManagerIsNotInitialized);
            }
            private set {
                _instance = value;
            }
        }

        /// <summary>
        /// PythonAILibManagerの初期化
        /// IPythonAILibConfigPramsを実装したクラスを引数に渡す
        /// </summary>
        /// <param name="parmas"></param>
        public static void Init(IPythonAILibConfigParams parmas) {
            //　環境変数HTTP_PROXY,HTTPS_PROXYの設定
            string proxyUrl = parmas.GetHttpsProxy();
            if (!string.IsNullOrEmpty(proxyUrl)) {
                Environment.SetEnvironmentVariable("HTTP_PROXY", proxyUrl);
                Environment.SetEnvironmentVariable("HTTPS_PROXY", proxyUrl);
            }
            // 環境変数NO_PROXYの設定
            string noProxy = parmas.GetNoProxy();
            if (!string.IsNullOrEmpty(noProxy)) {
                Environment.SetEnvironmentVariable("NO_PROXY", noProxy);
            }
            try {

                Instance = new PythonAILibManager(parmas);
            } catch (Exception ex) {

                LogWrapper.Error($"{PythonAILibStringResourcesJa.Instance.PythonAILibManagerInitializationFailed}  {ex}");

            }
        }


        public IPythonAILibConfigParams ConfigParams { get; private set; }

        private PythonAILibManager(IPythonAILibConfigParams parameters) {

            ConfigParams = parameters;

            // 言語設定
            PythonAILibStringResourcesJa.Lang = parameters.GetLang();
            // Python処理機能の初期化
            PythonExecutor.Init(parameters, afterStartProcess: (process) => {
                // プロセス開始後の処理
                Task.Run(async () => {

                    // PromptItemの初期化
                    await PromptItem.LoadItemsAsync();

                    // VectorDBItemの初期化
                    await VectorDBItem.LoadItemsAsync();

                    // AutoProcessItemの初期化
                    await AutoProcessItem.LoadItemsAsync();

                    // AutoProcessRuleの初期化
                    await AutoProcessRule.LoadItemsAsync();

                    // SearchRuleの初期化
                    await SearchRule.LoadItemsAsync();

                });
            });


        }

    }
}
