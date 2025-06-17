using LibPythonAI.Common;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.VectorDB {
    public class VectorEmbeddingItem {


        public VectorEmbeddingItem() { }

        public VectorEmbeddingItem(string source_id, string folderId) {
            SourceId = source_id;
            FolderId = folderId;
        }

        public string? FolderId { get; set; } = null;

        public string? FolderPath { get; set; } = null;

        public string SourceId { get; set; } = "";

        public VectorSourceType SourceType { get; set; } = VectorSourceType.None;

        public string Description { get; set; } = "";
        public string Content { get; set; } = "";

        public string SourcePath { get; set; } = "";

        public string DocId { get; set; } = string.Empty;

        public double Score { get; set; } = 0.0;

        public List<VectorEmbeddingItem> SubDocs { get; set; } = [];

        public void SetMetadata(string description, string content, VectorSourceType sourceType, string source_path) {
            Description = description;
            Content = content;
            SourceType = sourceType;
            SourcePath = source_path;

        }

        public void SetMetadata(ContentItemWrapper item) {
            // タイトルとHeaderTextを追加
            string description = item.Description + "\n" + item.HeaderText;
            if (item.ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                SetMetadata(description, item.Content, VectorSourceType.Clipboard, item.SourcePath);
            } else {
                SetMetadata(description, item.Content, VectorSourceType.File, item.SourcePath);
            }
        }

        public static void UpdateEmbeddings(string vectorDBItemName, VectorEmbeddingItem VectorEmbeddingItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

            // Parallelによる並列処理。4並列
            ChatRequestContext chatRequestContext = new() {
            };

            EmbeddingRequest embeddingRequestContext = new(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, VectorEmbeddingItem);

            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.SavedEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.SavedEmbedding);

        }

        public static void DeleteEmbeddings(string vectorDBItemName, VectorEmbeddingItem VectorEmbeddingItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new();

            EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, VectorEmbeddingItem);

            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeletedEmbedding);
            PythonExecutor.PythonAIFunctions.DeleteEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeletedEmbedding);
        }

        // DeleteEmbeddingsByFolderAsync
        public static void DeleteEmbeddingsByFolder(string vectorDBItemName, string folderId) {
            Task.Run(() => {
                PythonAILibManager libManager = PythonAILibManager.Instance;
                OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
                ChatRequestContext chatRequestContext = new() {};
                VectorEmbeddingItem VectorEmbeddingItem = new() {
                    FolderId = folderId,
                };
                EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, VectorEmbeddingItem);
                PythonExecutor.PythonAIFunctions.DeleteEmbeddingsByFolderAsync(chatRequestContext, embeddingRequestContext);
            });
        }

    }
}
