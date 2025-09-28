using System.IO;
using NLog;
using NLog.Targets;

namespace LibMain.Utils.Common {
    public class LogWrapper {

        private static Action<string> DebugAction = (message) => { };
        private static Action<string> InfoAction = (message) => { };
        private static Action<string> WarnAction = (message) => { };
        private static Action<string> ErrorAction = (message) => { };
        private static Action<bool, string> UpdateInProgressAction = (value, message) => { };

        public static void SetLogFolder(string logDirPath) {
            var target = (FileTarget)LogManager.Configuration.FindTargetByName("logFile");
            if (!Directory.Exists(logDirPath)) {
                Directory.CreateDirectory(logDirPath);
            }
            target.FileName = Path.Combine(logDirPath, "log.txt");
            LogManager.ReconfigExistingLoggers();
        }
        public static void SetActions(ILogWrapperAction logWrapperAction) {
            DebugAction = logWrapperAction.Debug;
            InfoAction = logWrapperAction.Info;
            WarnAction = logWrapperAction.Warn;
            ErrorAction = logWrapperAction.Error;
        }

        public static void Debug(string message) {
            DebugAction(message);
        }

        public static void Info(string message) {
            InfoAction(message);
        }

        public static void Warn(string message) {
            WarnAction(message);
        }

        public static void Error(string message) {
            ErrorAction(message);
        }
        public static void UpdateInProgress(bool value, string message = "") {
            UpdateInProgressAction(true, message);
        }



    }
}
