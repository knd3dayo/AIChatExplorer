using PythonAILib.Model;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.PythonIF
{
    public partial interface IPythonAIFunctions {

        public string ExtractFileToText(string path);

        public string ExtractBase64ToText(string base64);

        public void OpenAIEmbedding(OpenAIProperties props, string text);


        public ChatResult OpenAIChat(OpenAIProperties props, Chat chatController);


        public ChatResult LangChainChat(OpenAIProperties props, Chat chatController);


        public List<VectorSearchResult> VectorSearch(OpenAIProperties props, VectorDBItem vectorDBItem, VectorSearchRequest request);

        public void UpdateVectorDBIndex(OpenAIProperties props, GitFileInfo gitFileInfo, VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ContentInfo contentInfo, VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ImageInfo imageInfo, VectorDBItem vectorDBItem);

        // 引数として渡されたList<List<string>>の文字列をExcelファイルに出力する
        public void ExportToExcel(string filePath, CommonDataTable data);

        // 引数として渡されたExcelファイルを読み込んでList<List<string>>に変換して返す
        public CommonDataTable ImportFromExcel(string filePath);

        //テスト用
        public string HelloWorld();
    }
}
