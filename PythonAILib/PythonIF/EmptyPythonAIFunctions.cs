using PythonAILib.Model;
using WpfAppCommon.Model;

namespace PythonAILib.PythonIF {
    public class EmptyPythonAIFunctions : IPythonAIFunctions {
        private static PythonAILibStringResources StringResources { get; } = PythonAILibStringResources.Instance;
        public string ExtractText(string path) {
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

        public void UpdateVectorDBIndex(OpenAIProperties props, GitFileInfo gitFileInfo, VectorDBItem vectorDBItem) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public void UpdateVectorDBIndex(OpenAIProperties props, ContentInfo clipboard, VectorDBItem vectorDBItem) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public void UpdateVectorDBIndex(OpenAIProperties props, ImageInfo imageInfo, VectorDBItem vectorDBItem) {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
        public string HelloWorld() {
            throw new NotImplementedException(StringResources.PythonNotEnabledMessage);
        }
    }
}
