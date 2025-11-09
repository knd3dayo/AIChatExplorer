using LibMain.Common;
using LibMain.Model.Chat;
using LibMain.Model.VectorDB;
using LibMain.Resources;
using LibMain.Utils.Common;

namespace LibMain.PythonIF.Request {
    public class EmbeddingRequest {

        public const string VECTOR_DB_NAME_KEY = "vector_db_name";
        public const string SOURCE_ID_KEY = "source_id";
        public const string CONTENT_KEY = "content";

        // metadata keys
        public const string METADATA_KEY = "metadata";
        public const string FOLDER_PATH_KEY = "folder_path";
        public const string SOURCE_TYPE_KEY = "source_type";
        public const string DESCRIPTION_KEY = "description";
        public const string SOURCE_PATH_KEY = "source_path";
        public const string IMAGE_URL_KEY = "image_url";
        public const string TAGS_KEY = "tags";


        public EmbeddingRequest(VectorEmbeddingItem embedding) {
            Embedding = embedding;
        }

        public VectorEmbeddingItem Embedding { get; set; }

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict[VECTOR_DB_NAME_KEY] = Embedding.VectorDBName;
            // source_id
            dict[SOURCE_ID_KEY] = Embedding.SourceId;
            dict[CONTENT_KEY] = Embedding.Content;

            // metadata
            Dictionary<string, object> medatada = [];

            medatada[SOURCE_PATH_KEY] = Embedding.SourcePath;
            medatada[SOURCE_TYPE_KEY] = Embedding.SourceType.ToString();
            medatada[DESCRIPTION_KEY] = Embedding.Description;
            medatada[FOLDER_PATH_KEY] = Embedding.FolderPath ?? "";
            medatada[IMAGE_URL_KEY] = Embedding.ImageUrl;

            // tags
            if (Embedding.Tags != null && Embedding.Tags.Count > 0) {
                medatada[TAGS_KEY] = Embedding.Tags;
            }
            dict[METADATA_KEY] = medatada;

            return dict;
        }


        public static async Task UpdateEmbeddingsAsync(string vectorDBItemName, VectorEmbeddingItem vectorEmbeddingItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            ChatSettings chatSettings = new();
            ChatRequestContext chatRequestContext = new(chatSettings);
            EmbeddingRequest embeddingRequestContext = new( vectorEmbeddingItem);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.SavedEmbedding);
            await PythonExecutor.PythonAIFunctions.UpdateEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.SavedEmbedding);
        }


        public static async Task DeleteEmbeddingsAsync(VectorEmbeddingItem vectorEmbeddingItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatSettings chatSettings = new();
            ChatRequestContext chatRequestContext = new(chatSettings);
            EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(vectorEmbeddingItem);
            LogWrapper.Info(PythonAILibStringResourcesJa.Instance.DeletedEmbedding);
            await PythonExecutor.PythonAIFunctions.DeleteEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
        }


        // DeleteEmbeddingsByFolderAsync
        public static async Task DeleteEmbeddingsByFolderAsync(VectorEmbeddingItem vectorEmbeddingItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            ChatSettings chatSettings = new();
            ChatRequestContext chatRequestContext = new(chatSettings);
            EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(vectorEmbeddingItem);
            await PythonExecutor.PythonAIFunctions.DeleteEmbeddingsByFolderAsync(chatRequestContext, embeddingRequestContext);
        }

    }
}
