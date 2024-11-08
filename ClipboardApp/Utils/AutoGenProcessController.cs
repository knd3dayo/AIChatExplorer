using System.Diagnostics;
using WpfAppCommon.Utils;

namespace ClipboardApp.Utils {
    public class AutoGenProcessController {

        public static Process? AutoGenStudioProcess { get; set; }

        public static void StartAutoGenStudio() {
            if (AutoGenStudioProcess != null) {
                return;
            }
            // Start AutoGenStudio
            AutoGenStudioProcess = ProcessUtil.StartProcess("autogenstudio", " ui --port 8081", (process) => { }, (content) => {});
        }

        // Stop AutoGenStudio
        public static void StopAutoGenStudio() {
            if (AutoGenStudioProcess == null) {
                return;
            }
            ProcessUtil.StopProcess(AutoGenStudioProcess);
            AutoGenStudioProcess = null;
        }

    }
}
