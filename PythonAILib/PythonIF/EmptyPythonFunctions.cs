using PythonAILib.Model;
using WpfAppCommon.Model;

namespace PythonAILib.PythonIF {
    public class EmptyPythonFunctions : IPythonFunctions {
        private static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;
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


        public ChatResult OpenAIChat(ChatRequest chatController) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public ChatResult LangChainChat(ChatRequest chatController) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public List<VectorSearchResult> VectorSearch(OpenAIProperties props, VectorDBItem vectorDBItem, string content) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }

        public void OpenAIEmbedding(OpenAIProperties props, string text) { 
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

        public void UpdateVectorDBIndex(OpenAIProperties props, IPythonFunctions.GitFileInfo gitFileInfo, VectorDBItem vectorDBItem) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public void UpdateVectorDBIndex(OpenAIProperties props, IPythonFunctions.ContentInfo clipboard, VectorDBItem vectorDBItem) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public string HelloWorld() {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
    }
}
