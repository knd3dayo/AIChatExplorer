using System.IO;
using PythonAILib.Model;
using WpfAppCommon.PythonIF;

namespace PythonAILib.PythonIF {
    public class PythonExecutor {
        // String definition instance
        public static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;


        // Template file for custom Python scripts
        public static string TemplateScript { get; } = StringResources.TemplateScript;

        // Python script for clipboard app
        public static string WpfAppCommonUtilsScript { get; } = StringResources.WpfAppCommonUtilsScript;

        public static IPythonFunctions PythonFunctions { get; set; } = new EmptyPythonFunctions();
        public static void Init(string pythonPath) {

            PythonFunctions = new PythonNetFunctions(pythonPath);
        }

        // Load Python script
        public static string LoadPythonScript(string scriptName) {
            var file = new FileInfo(scriptName);
            if (!file.Exists) {
                // Load the template file

                file = new FileInfo(TemplateScript);
                if (!file.Exists) {
                    throw new Exception(StringResources.TemplateScriptNotFound);
                }
                return File.ReadAllText(file.FullName);
            }
            // Load the file
            string script = File.ReadAllText(file.FullName);

            return script;
        }
    }

}
