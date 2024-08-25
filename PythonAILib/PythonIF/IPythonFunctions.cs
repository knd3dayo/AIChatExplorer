using PythonAILib.Model;

namespace PythonAILib.PythonIF {
    public partial interface IPythonAIFunctions {

        public string ExtractText(string path);

        public void OpenAIEmbedding(OpenAIProperties props, string text);

        
        public ChatResult OpenAIChat(ChatRequest chatController);

            
        public ChatResult LangChainChat(ChatRequest chatController);

        
        public List<VectorSearchResult> VectorSearch(OpenAIProperties props, VectorDBItemBase vectorDBItem, VectorSearchRequest request);

        public void UpdateVectorDBIndex(OpenAIProperties props, GitFileInfo gitFileInfo, VectorDBItemBase vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ContentInfo contentInfo, VectorDBItemBase vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ImageInfo imageInfo, VectorDBItemBase vectorDBItem);

        // 引数として渡されたList<List<string>>の文字列をExcelファイルに出力する
        public void ExportToExcel(string filePath, CommonDataTable data);

        // 引数として渡されたExcelファイルを読み込んでList<List<string>>に変換して返す
        public CommonDataTable ImportFromExcel(string filePath);

        //テスト用
        public string HelloWorld();
    }
}
