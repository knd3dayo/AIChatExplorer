using LiteDB.Engine;
using PythonAILib.Model;
using PythonAILib.Model.Abstract;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils;

namespace QAChat {
    public class PythonAILibManager {

        public static PythonAILibManager? Instance { get; private set; }

        public IPythonAILibConfigParams ConfigParams { get; private set; }

        private PythonAILibManager(IPythonAILibConfigParams parameters) {

            ConfigParams = parameters;
            Action<string> infoAction = parameters.GetInfoAction();
            Action<string> warnAction = parameters.GetWarnAction();
            Action<string> errorAction = parameters.GetErrorAction();

            // 言語設定
            PythonAILibStringResources.Lang = parameters.GetLang();
            // Python処理機能の初期化
            PythonExecutor.Init(parameters.GetPythonDllPath(), parameters.GetPathToVirtualEnv());
            // DBControllerの設定
            DataFactory = parameters.GetDataFactory();
            // LogWrapperのログ出力設定
            LogWrapper.SetActions(infoAction, warnAction, errorAction);

        }

        public static void Init(IPythonAILibConfigParams parmas) {

            Instance = new PythonAILibManager(parmas);
        }

        public IDataFactory DataFactory { get; set; }

    }
}
