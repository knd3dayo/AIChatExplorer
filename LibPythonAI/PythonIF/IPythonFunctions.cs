using LibPythonAI.Model.Content;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;

namespace PythonAILib.PythonIF {
    public partial interface IPythonAIFunctions {


        public Task<List<ContentFolderWrapper>> GetRootContentFolders(List<string> folderTypeStrings);


        public void UpdateTagItems(List<TagItem> tagItems);

        public void DeleteTagItems(List<TagItem> tagItems);

        public List<TagItem> GetTagItems();

        public string ExtractFileToText(string path);

        public string ExtractBase64ToText(string base64, string extension);

        public Task<ChatResult> OpenAIChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest);

        public Task<ChatResult> AutoGenGroupChat(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration);

        public void CancelAutoGenChat(string sessionToken);

        public void UpdateVectorDBItem(VectorDBItem item);

        public void DeleteVectorDBItem(VectorDBItem item);

        public Task<List<VectorDBItem>> GetVectorDBItemsAsync();

        public VectorDBItem? GetVectorDBItemById(string id);

        public VectorDBItem? GetVectorDBItemByName(string name);
        
        public List<VectorDBEmbedding> VectorSearch(ChatRequestContext chatRequestContext, string query);

        // delete_embeddings_by_folder
        public void DeleteEmbeddingsByFolder(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest);

        public void DeleteEmbeddings(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest);

        public void UpdateEmbeddings(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest);

        // 引数として渡されたList<List<string>>の文字列をExcelファイルに出力する
        public void ExportToExcel(string filePath, CommonDataTable data);

        // 引数として渡されたExcelファイルを読み込んでList<List<string>>に変換して返す
        public CommonDataTable ImportFromExcel(string filePath);

        // GetMimeType
        public string GetMimeType(string filePath);

        // GetTokenCount
        public long GetTokenCount(ChatRequestContext chatRequestContext, string inputText);

        // extract_webpage
        public string ExtractWebPage(string url);

        //テスト用
        public string HelloWorld();

    }
}
