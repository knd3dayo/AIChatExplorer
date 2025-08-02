using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.File;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;

namespace LibPythonAI.PythonIF {
    public partial interface IPythonAIFunctions {

        // ContentItem
        public Task<ContentItemEntity> GetContentItemByIdAsync(string id);
        public Task<List<ContentItemEntity>> GetContentItemsAsync();
        public Task<List<ContentItemEntity>> GetContentItemsByFolderAsync(string folderId);
        public Task UpdateContentItemAsync(List<ContentItemRequest> requests);
        public Task DeleteContentItemsAsync(List<ContentItemRequest> requests);
        public Task<List<ContentItemEntity>> SearchContentItems(SearchCondition searchCondition);

        public Task<List<ContentFolderEntity>> GetRootContentFoldersAsync();
        public Task<List<ContentFolderEntity>> GetContentFoldersAsync();
        public Task<ContentFolderEntity> GetContentFolderByIdAsync(string id);
        public Task<ContentFolderEntity> GetContentFolderByPathAsync(string name);
        
        public Task<ContentFolderEntity?> GetParentFolderByIdAsync(string id);

        public Task<List<ContentFolderEntity>> GetChildFoldersByIdAsync(string id);


        public Task UpdateContentFoldersAsync(List<ContentFolderRequest> folders);
        public Task DeleteContentFoldersAsync(List<ContentFolderRequest> folders);

        public Task<List<PromptItem>> GetPromptItemsAsync();
        public Task UpdatePromptItemAsync(PromptItemRequest request);
        public Task DeletePromptItemAsync(PromptItemRequest request);
        // SearchRule
        public Task<List<SearchRule>> GetSearchRulesAsync();
        public Task UpdateSearchRuleAsync(SearchRuleRequest request);
        public Task DeleteSearchRuleAsync(SearchRuleRequest request);
        // AutoProcessItem
        public Task<List<AutoProcessItem>> GetAutoProcessItemsAsync();
        public Task UpdateAutoProcessItemAsync(AutoProcessItemRequest request);
        public Task DeleteAutoProcessItemAsync(AutoProcessItemRequest request);
        // AutoProcessRule
        public Task<List<AutoProcessRule>> GetAutoProcessRulesAsync();
        public Task UpdateAutoProcessRuleAsync(AutoProcessRuleRequest rule);
        public Task DeleteAutoProcessRuleAsync(AutoProcessRuleRequest rule);
        public Task UpdateTagItemsAsync(List<TagItem> tagItems);
        public Task DeleteTagItemsAsync(List<TagItem> tagItems);
        public Task<List<TagItem>> GetTagItemsAsync();
        public Task<string> ExtractFileToTextAsync(string path);
        public Task<string> ExtractBase64ToText(string base64, string extension);
        public Task<ChatResponse> OpenAIChatAsync(ChatRequestContext chatRequestContext, ChatRequest chatRequest);

        public Task UpdateVectorDBItemAsync(VectorDBItem item);
        public Task DeleteVectorDBItemAsync(VectorDBItem item);
        public Task<List<VectorDBItem>> GetVectorDBItemsAsync();
        public Task<VectorDBItem?> GetVectorDBItemById(string id);
        public Task<VectorDBItem?> GetVectorDBItemByName(string name);
        public Task<List<VectorEmbeddingItem>> VectorSearchAsync(ChatRequestContext chatRequestContext, string query);
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
        public Task<long> GetTokenCount(ChatRequestContext chatRequestContext, TokenCountRequest tokenCountRequest);
        // extract_webpage
        public Task<string> ExtractWebPage(string url);


    }
}
