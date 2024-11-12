using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.PythonIF
{
    public partial interface IPythonAIFunctions {

        public string ExtractFileToText(string path);

        public string ExtractBase64ToText(string base64, string extension);

        public void OpenAIEmbedding(OpenAIProperties props, string text);

        public ChatResult OpenAIChat(OpenAIProperties props, ChatRequest chatController);

        public ChatResult LangChainChat(OpenAIProperties props, ChatRequest chatController);

        public List<VectorSearchResult> VectorSearch(OpenAIProperties props, List<VectorDBItem> vectorDBItems, VectorSearchRequest request);

        public void DeleteVectorDBCollection(OpenAIProperties props, VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, GitFileInfo gitFileInfo, VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ContentInfo contentInfo, VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ImageInfo imageInfo, VectorDBItem vectorDBItem);

        // 引数として渡されたList<List<string>>の文字列をExcelファイルに出力する
        public void ExportToExcel(string filePath, CommonDataTable data);

        // 引数として渡されたExcelファイルを読み込んでList<List<string>>に変換して返す
        public CommonDataTable ImportFromExcel(string filePath);

        // GetMimeType
        public string GetMimeType(string filePath);

        //テスト用
        public string HelloWorld();
    }
}
