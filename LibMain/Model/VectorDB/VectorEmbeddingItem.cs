using System.Threading.Tasks;
using LibMain.Common;
using LibMain.Model.Chat;
using LibMain.Model.Content;
using LibMain.PythonIF;
using LibMain.PythonIF.Request;
using LibMain.Resources;
using LibMain.Utils.Common;

namespace LibMain.Model.VectorDB {
    public class VectorEmbeddingItem {

        public VectorEmbeddingItem() { }

        public VectorEmbeddingItem(string source_id, string folderId) {
            SourceId = source_id;
            FolderPath = folderId;
        }

        public string? FolderPath { get; set; } = null;

        public string SourceId { get; set; } = "";

        public VectorSourceType SourceType { get; set; } = VectorSourceType.None;

        public string Description { get; set; } = "";
        public string Content { get; set; } = "";

        public string SourcePath { get; set; } = "";

        public Dictionary<string, string> Tags { get; set; } = [];

        public string DocId { get; set; } = string.Empty;

        public double Score { get; set; } = 0.0;

        public List<VectorEmbeddingItem> SubDocs { get; set; } = [];

        public void SetMetadata(string description, string content, VectorSourceType sourceType, string source_path, Dictionary<string, string> tags) {
            Description = description;
            Content = content;
            SourceType = sourceType;
            SourcePath = source_path;
            Tags = tags;
        }

        public async Task SetMetadata(ContentItem item) {
            // タイトルとHeaderTextを追加
            var hederText = await item.GetHeaderTextAsync();
            string description = item.Description + "\n" + hederText;
            // タグを取得
            Dictionary<string, string> tags = item.Tags.ToDictionary(tag => tag, tag => tag);
            if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                SetMetadata(description, item.Content, VectorSourceType.Clipboard, item.SourcePath, tags);
            } else {
                SetMetadata(description, item.Content, VectorSourceType.File, item.SourcePath, tags);
            }
        }
        public static async Task UpdateEmbeddingsAsync(string vectorDBItemName, VectorEmbeddingItem vectorEmbeddingItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatSettings chatSettings = new();
            ChatRequestContext chatRequestContext = new(chatSettings);
            EmbeddingRequest embeddingRequestContext = new(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, vectorEmbeddingItem);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.SavedEmbedding);
            await PythonExecutor.PythonAIFunctions.UpdateEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.SavedEmbedding);
        }

        public static void UpdateEmbeddings(string vectorDBItemName, VectorEmbeddingItem VectorEmbeddingItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

    
            ChatSettings chatSettings = new();
            ChatRequestContext chatRequestContext = new(chatSettings);

            EmbeddingRequest embeddingRequestContext = new(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, VectorEmbeddingItem);

            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.SavedEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.SavedEmbedding);

        }


        public static async Task DeleteEmbeddingsAsync(string vectorDBItemName, VectorEmbeddingItem vectorEmbeddingItem)
        {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatSettings chatSettings = new();
            ChatRequestContext chatRequestContext = new(chatSettings);
            EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, vectorEmbeddingItem);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeletedEmbedding);
            await PythonExecutor.PythonAIFunctions.DeleteEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
        }


        // DeleteEmbeddingsByFolderAsync
        public static async Task DeleteEmbeddingsByFolderAsync(string vectorDBItemName, string folderPath)
        {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatSettings chatSettings = new();
            ChatRequestContext chatRequestContext = new(chatSettings);
            VectorEmbeddingItem vectorEmbeddingItem = new() {
                FolderPath = folderPath,
            };
            EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, vectorEmbeddingItem);
            await PythonExecutor.PythonAIFunctions.DeleteEmbeddingsByFolderAsync(chatRequestContext, embeddingRequestContext);
        }

    }
}
