namespace PythonAILib.Utils.Common {
    public class LogWrapper {

        private static Action<string> DebugAction = (message) => { };
        private static Action<string> InfoAction = (message) => { };
        private static Action<string> WarnAction = (message) => { };
        private static Action<string> ErrorAction = (message) => { };

        public static void SetActions(Action<string> debugAction, Action<string> infoAction, Action<string> warnAction, Action<string> errorAction) {
            DebugAction = debugAction;
            InfoAction = infoAction;
            WarnAction = warnAction;
            ErrorAction = errorAction;
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
 


    }
}
