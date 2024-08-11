using PythonAILib.Model;
using WpfAppCommon.Model;

namespace PythonAILib.PythonIF {
    public class EmptyPythonMiscFunctions : IPythonMiscFunctions {
        private static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;
        public string GetMaskedString(string spacyModel, string text) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public string GetUnmaskedString(string spacyModel, string maskedText) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public string ExtractTextFromImage(System.Drawing.Image image, string tesseractExePath) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public MaskedData GetMaskedData(string spacyModel, List<string> beforeTextList) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public MaskedData GetUnMaskedData(string spacyModel, List<string> maskedTextList) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public string RunScript(string script, string inputJson) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public HashSet<string> ExtractEntity(string SpacyModel, string text) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

    }
}
