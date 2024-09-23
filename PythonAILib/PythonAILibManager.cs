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

        private PythonAILibManager(IPythonAILibConfigParams parmas) {

            ConfigParams = parmas;
            Action<string> infoAction = parmas.GetInfoAction();
            Action<string> warnAction = parmas.GetWarnAction();
            Action<string> errorAction = parmas.GetErrorAction();

            // 言語設定
            PythonAILibStringResources.Lang = parmas.GetLang();
            // Python処理機能の初期化
            PythonExecutor.Init(parmas.GetPythonDllPath(), parmas.GetPathToVirtualEnv());
            // DBControllerの設定
            DataFactory = parmas.GetDataFactory();
            // LogWrapperのログ出力設定
            LogWrapper.SetActions(infoAction, warnAction, errorAction);

        }

        public static void Init(IPythonAILibConfigParams parmas) {

            Instance = new PythonAILibManager(parmas);
        }

        public IDataFactory DataFactory { get; set; }

    }
}
