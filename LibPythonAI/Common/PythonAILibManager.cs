using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace PythonAILib.Common {
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

        /// <summary>
        /// PythonAILibManagerの初期化
        /// IPythonAILibConfigPramsを実装したクラスを引数に渡す
        /// </summary>
        /// <param name="parmas"></param>
        public static void Init(IPythonAILibConfigParams parmas) {

            Instance = new PythonAILibManager(parmas);

        }


        public IPythonAILibConfigParams ConfigParams { get; private set; }

        private PythonAILibManager(IPythonAILibConfigParams parameters) {

            ConfigParams = parameters;

            // 言語設定
            PythonAILibStringResources.Lang = parameters.GetLang();
            // Python処理機能の初期化
            PythonExecutor.Init(parameters, afterStartProcess: (process) => {
                // プロセス開始後の処理
                Task.Run(async () => {

                    // DBの初期化
                    PythonAILibDBContext.Init();
                    // PromptItemの初期化
                    await PromptItem.LoadItemsAsync();
                    await VectorDBItem.LoadItemsAsync();

                   
                });
            });

            // LogWrapperの初期化
            string logDirPath = Path.Combine(parameters.GetAppDataPath(), "log");
            LogWrapper.SetLogFolder(logDirPath);

            // LogWrapperのログ出力設定
            LogWrapper.SetActions(parameters.GetLogWrapperAction());



        }

    }
}
