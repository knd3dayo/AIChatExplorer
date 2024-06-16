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
        public ChatResult LangChainChat(Dictionary<string, string> props, IEnumerable<VectorDBItem> vectorDBItems, string request_json) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public ChatResult LangChainChat(IEnumerable<VectorDBItem> vectorDBItems, string prompt, IEnumerable<ChatItem> chatHistory) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public ChatResult LangChainChat(Dictionary<string, string> props, IEnumerable<VectorDBItem> vectorDBItems, string prompt, IEnumerable<ChatItem> chatHistory) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public void OpenAIEmbedding(string text) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public ChatResult OpenAIChatWithVision(string prompt, IEnumerable<string> imageFileNames) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public ChatResult OpenAIChatWithVision(string prompt, IEnumerable<string> imageFileNames, Dictionary<string, string> props) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public string RunScript(string script, string inputJson) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public HashSet<string> ExtractEntity(string SpacyModel, string text) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public ChatResult OpenAIChat(Dictionary<string, string> props, string requestJson) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory, Dictionary<string, string> props) {
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
