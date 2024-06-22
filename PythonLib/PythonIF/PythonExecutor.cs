using System.IO;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace WpfAppCommon.PythonIF {
    public class PythonExecutor {
        // String definition instance
        public static StringResources StringResources { get; } = StringResources.Instance;

        public enum PythonExecutionType {
            None = 0,
            PythonNet = 1,
            InternalFlask = 2,
            ExternalFlask = 3
        }
        public static PythonExecutionType PythonExecution {
            get {
                // Convert from int to PythonExecutionType
                return (PythonExecutionType)ClipboardAppConfig.PythonExecute;
            }

        }

        // Template file for custom Python scripts
        public static string TemplateScript { get; } = StringResources.TemplateScript;

        // Python script for clipboard app
        public static string WpfAppCommonUtilsScript { get; } = StringResources.WpfAppCommonUtilsScript;

        public static IPythonFunctions PythonFunctions { get; set; } = new EmptyPythonFunctions();
        public static void Init(string pythonPath) {


            // If PythonExecution in WpfAppCommonSettings is PythonNet, execute InitPythonNet
            if (PythonExecution == PythonExecutionType.PythonNet) {
                PythonFunctions = new PythonNetFunctions(pythonPath);
            }
        }

        // Load Python script
        public static string LoadPythonScript(string scriptName) {
            var file = new FileInfo(scriptName);
            if (!file.Exists) {
                // Load the template file

                file = new FileInfo(TemplateScript);
                if (!file.Exists) {
                    throw new ThisApplicationException(StringResources.TemplateScriptNotFound);
                }
                return File.ReadAllText(file.FullName);
            }
            // Load the file
            string script = File.ReadAllText(file.FullName);

            return script;
        }
    }

}
