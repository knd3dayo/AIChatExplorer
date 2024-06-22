using QAChat.Model;
using WpfAppCommon.Model;

namespace WpfAppCommon.PythonIF {
    public class EmptyPythonFunctions : IPythonFunctions {
        private static StringResources StringResources { get; } = StringResources.Instance;
        public string ExtractText(string path) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
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


        public ChatResult OpenAIChat(OpenAIProperties props,  ChatController chatController) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public ChatResult LangChainChat(OpenAIProperties openAIProperties , ChatController chatController) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public void OpenAIEmbedding(string text) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        
        public ChatResult OpenAIChatWithVision(OpenAIProperties props, string prompt, IEnumerable<string> imageFileNames) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public string RunScript(string script, string inputJson) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public HashSet<string> ExtractEntity(string SpacyModel, string text) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public void UpdateVectorDBIndex(FileStatus fileStatus, string workingDirPath, string repositoryURL, VectorDBItem vectorDBItem) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public void UpdateVectorDBIndex(IPythonFunctions.VectorDBUpdateMode mode, ClipboardItem item, VectorDBItem vectorDBItem) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public string HelloWorld() {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
    }
}
