using LibPythonAI.Model.AutoGen;
using LibPythonAI.Model.File;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Search;

namespace LibPythonAI.PythonIF {
    public partial interface IPythonAIFunctions {


        public void UpdateContentFoldersForVectorSearch(List<ContentFolderRequest> folders);

        public void DeleteContentFoldersForVectorSearch(List<ContentFolderRequest> folders);

        public Task<List<PromptItem>> GetPromptItemsAsync();

        public void UpdatePromptItemAsync(PromptItemRequest request);

        public void DeletePromptItemAsync(PromptItemRequest request);

        // SearchRule
        public Task<List<SearchRule>> GetSearchRulesAsync();

        public void UpdateSearchRuleAsync(SearchRuleRequest request);

        public void DeleteSearchRuleAsync(SearchRuleRequest request);

        // AutoProcessItem
        public Task<List<AutoProcessItem>> GetAutoProcessItemsAsync();

        public void UpdateAutoProcessItemAsync(AutoProcessItemRequest request);

        public void DeleteAutoProcessItemAsync(AutoProcessItemRequest request);

        // AutoProcessRule
        public Task<List<AutoProcessRule>> GetAutoProcessRulesAsync();

        public void UpdateAutoProcessRuleAsync(AutoProcessRuleRequest rule);

        public void DeleteAutoProcessRuleAsync(AutoProcessRuleRequest rule);



        public void UpdateTagItemsAsync(List<TagItem> tagItems);

        public void DeleteTagItemsAsync(List<TagItem> tagItems);

        public Task<List<TagItem>> GetTagItemsAsync();

        public Task<string> ExtractFileToTextAsync(string path);

        public Task<string> ExtractBase64ToText(string base64, string extension);

        public Task<ChatResponse> OpenAIChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest);

        public Task<ChatResponse> AutoGenGroupChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest, Action<string> iteration);

        public void CancelAutoGenChat(string sessionToken);

        public Task<List<AutoGenLLMConfig>> GetAutoGenLLMConfigListAsync();

        public Task<AutoGenLLMConfig?> GetAutoGenLLMConfigAsync(string name);

        public void UpdateAutogenLLMConfigAsync(AutoGenLLMConfig config);

        public void DeleteAutogenLLMConfigAsync(AutoGenLLMConfig config);

        // AutoGenTool
        public Task<List<AutoGenTool>> GetAutoGenToolListAsync();

        public Task<AutoGenTool?> GetAutoGenToolAsync(string name);

        public void UpdateAutoGenToolAsync(AutoGenTool config);

        public void DeleteAutoGenToolAsync(AutoGenTool config);

        // AutoGenAgent
        public Task<List<AutoGenAgent>> GetAutoGenAgentListAsync();

        public Task<AutoGenAgent> GetAutoGenAgentAsync(string name);

        public void UpdateAutoGenAgentAsync(AutoGenAgent config);

        public void DeleteAutoGenAgentAsync(AutoGenAgent config);

        // AutoGenGroupChat
        public Task<List<AutoGenGroupChat>> GetAutoGenGroupChatListAsync();

        public Task<AutoGenGroupChat> GetAutoGenGroupChatAsync(string name);

        public void UpdateAutoGenGroupChatAsync(AutoGenGroupChat config);

        public void DeleteAutoGenGroupChatAsync(AutoGenGroupChat config);



        public void UpdateVectorDBItem(VectorDBItem item);

        public void DeleteVectorDBItem(VectorDBItem item);

        public Task<List<VectorDBItem>> GetVectorDBItemsAsync();

        public Task<VectorDBItem?> GetVectorDBItemById(string id);

        public Task<VectorDBItem?> GetVectorDBItemByName(string name);

        public Task<List<VectorEmbeddingItem>> VectorSearchAsync(ChatRequestContext chatRequestContext, string query);

        // delete_embeddings_by_folder
        public void DeleteEmbeddingsByFolderAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest);

        public void DeleteEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest);

        public void UpdateEmbeddingsAsync(ChatRequestContext chatRequestContext, EmbeddingRequest embeddingRequest);

        // 引数として渡されたList<List<string>>の文字列をExcelファイルに出力する
        public void ExportToExcelAsync(string filePath, CommonDataTable data);

        // 引数として渡されたExcelファイルを読み込んでList<List<string>>に変換して返す
        public Task<CommonDataTable> ImportFromExcel(string filePath);

        // GetMimeType
        public Task<string> GetMimeType(string filePath);

        // GetTokenCount
        public Task<long> GetTokenCount(ChatRequestContext chatRequestContext, TokenCountRequest tokenCountRequest);

        // extract_webpage
        public Task<string> ExtractWebPage(string url);

        //テスト用
        public string HelloWorld();

    }
}
