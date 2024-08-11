using System.IO;
using PythonAILib.Model;

namespace PythonAILib.PythonIF {
    public class PythonExecutor {
        // String definition instance
        public static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;


        // Template file for custom Python scripts
        public static string TemplateScript { get; } = StringResources.TemplateScript;

        // Python script for OpenAI
        public static string WpfAppCommonOpenAIScript { get; } = StringResources.WpfAppCommonOpenAIScript;
        // Python script for Misc
        public static string WpfAppCommonMiscScript { get; } = StringResources.WpfAppCommonMiscScript;


        public static IPythonAIFunctions PythonAIFunctions { get; set; } = new EmptyPythonAIFunctions();

        public static IPythonMiscFunctions PythonMiscFunctions { get; set; } = new EmptyPythonMiscFunctions();
        // Initialize Python functions
        public static void Init(string pythonPath) {

            PythonAIFunctions = new PythonNetFunctions(pythonPath);
            PythonMiscFunctions = new PythonMiscFunctions(pythonPath);
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
