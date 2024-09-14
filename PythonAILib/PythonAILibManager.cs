using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonAILib.Model;
using PythonAILib.Model.Abstract;
using PythonAILib.PythonIF;
using PythonAILib.Utils;

namespace QAChat {
    public class PythonAILibManager {

        public static PythonAILibManager? Instance { get; private set;}
        private PythonAILibManager(
            string lang, string pythonDllPath, string pythonVenvPath, IDBController dbController, Action<string> infoAction, Action<string> warnAction, Action<string> errorAction) {

            // 言語設定
            PythonAILibStringResources.Lang = lang;
            // Python処理機能の初期化
            PythonExecutor.Init(pythonDllPath, pythonVenvPath);
            // DBControllerの設定
            DBController = dbController;
            // LogWrapperのログ出力設定
            LogWrapper.SetActions(infoAction, warnAction, errorAction);

        }
        public static void Init(string lang, string pythonDllPath, string pythonVenvPath, IDBController dbController, Action<string> infoAction, Action<string> warnAction, Action<string> errorAction) {
            Instance = new PythonAILibManager(lang, pythonDllPath, pythonVenvPath, dbController, infoAction, warnAction, errorAction);
        }

        public IDBController  DBController { get; set; }


    }
}
