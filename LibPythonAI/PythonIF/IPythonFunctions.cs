using LibPythonAI.Model.AutoGen;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.File;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;

namespace PythonAILib.PythonIF {
    public partial interface IPythonAIFunctions {


        public Task<List<ContentFolderWrapper>> GetRootContentFolders(List<string> folderTypeStrings);


        public Task UpdateTagItemsAsync(List<TagItem> tagItems);

        public Task DeleteTagItemsAsync(List<TagItem> tagItems);

        public Task<List<TagItem>> GetTagItemsAsync();

        public Task<string> ExtractFileToTextAsync(string path);

        public Task<string> ExtractBase64ToText(string base64, string extension);

        public Task<ChatResult> OpenAIChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest);

        public Task<ChatResult> AutoGenGroupChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration);

        public void CancelAutoGenChat(string sessionToken);

        public Task<List<AutoGenLLMConfig>> GetAutoGenLLMConfigListAsync();

        public Task<AutoGenLLMConfig?> GetAutoGenLLMConfigAsync(string name);

        public Task UpdateAutogenLLMConfigAsync(AutoGenLLMConfig config);

        public Task DeleteAutogenLLMConfigAsync(AutoGenLLMConfig config);

        // AutoGenTool
        public Task<List<AutoGenTool>> GetAutoGenToolListAsync();

        public Task<AutoGenTool?> GetAutoGenToolAsync(string name);

        public Task UpdateAutoGenToolAsync(AutoGenTool config);

        public Task DeleteAutoGenToolAsync(AutoGenTool config);

        // AutoGenAgent
        public Task<List<AutoGenAgent>> GetAutoGenAgentListAsync();

        public Task<AutoGenAgent> GetAutoGenAgentAsync(string name);

        public Task UpdateAutoGenAgentAsync(AutoGenAgent config);

        public Task DeleteAutoGenAgentAsync(AutoGenAgent config);

        // AutoGenGroupChat
        public Task<List<AutoGenGroupChat>> GetAutoGenGroupChatListAsync();

        public Task<AutoGenGroupChat> GetAutoGenGroupChatAsync(string name);

        public Task UpdateAutoGenGroupChatAsync(AutoGenGroupChat config);

        public Task DeleteAutoGenGroupChatAsync(AutoGenGroupChat config);



        public Task UpdateVectorDBItem(VectorDBItem item);

        public Task DeleteVectorDBItem(VectorDBItem item);

        public Task<List<VectorDBItem>> GetVectorDBItemsAsync();

        public VectorDBItem? GetVectorDBItemById(string id);

        public VectorDBItem? GetVectorDBItemByName(string name);

        public Task<List<VectorEmbedding>> VectorSearchAsync(ChatRequestContext chatRequestContext, string query);

        // delete_embeddings_by_folder
        public Task DeleteEmbeddingsByFolderAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest);

        public Task DeleteEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest);

        public Task UpdateEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest);

        // 引数として渡されたList<List<string>>の文字列をExcelファイルに出力する
        public Task ExportToExcelAsync(string filePath, CommonDataTable data);

        // 引数として渡されたExcelファイルを読み込んでList<List<string>>に変換して返す
        public Task<CommonDataTable> ImportFromExcel(string filePath);

        // GetMimeType
        public Task<string> GetMimeType(string filePath);

        // GetTokenCount
        public Task<long> GetTokenCount(ChatRequestContext chatRequestContext, string inputText);

        // extract_webpage
        public Task<string> ExtractWebPage(string url);

        //テスト用
        public string HelloWorld();

    }
}
