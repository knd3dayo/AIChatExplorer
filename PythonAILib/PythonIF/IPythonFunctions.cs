using PythonAILib.Model;

namespace PythonAILib.PythonIF {
    public partial interface IPythonAIFunctions {

        public string ExtractText(string path);

        public void OpenAIEmbedding(OpenAIProperties props, string text);

        
        public ChatResult OpenAIChat(ChatRequest chatController);


        public ChatResult LangChainChat(ChatRequest chatController);

        
        public List<VectorSearchResult> VectorSearch(OpenAIProperties props, VectorDBItem vectorDBItem, VectorSearchRequest request);

        public void UpdateVectorDBIndex(OpenAIProperties props, GitFileInfo gitFileInfo, VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ContentInfo contentInfo, VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ImageInfo imageInfo, VectorDBItem vectorDBItem);

        //テスト用
        public string HelloWorld();
    }
}
