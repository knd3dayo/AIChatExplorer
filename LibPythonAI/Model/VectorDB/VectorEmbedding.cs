using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace LibPythonAI.Model.VectorDB {
    public class VectorEmbedding {

        public static readonly JsonSerializerOptions Options = new() {
            Converters = { new JsonStringEnumConverter() },
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            IncludeFields = true,
            WriteIndented = true
        };

        public VectorEmbedding() { }

        public VectorEmbedding(string source_id, string folderId) {
            SourceId = source_id;
            FolderId = folderId;
        }

        [JsonPropertyName("FolderId")]
        public string? FolderId { get; set; } = null;

        [JsonPropertyName("source_id")]
        public string SourceId { get; set; } = "";

        [JsonPropertyName("source_type")]
        public VectorSourceType SourceType { get; set; } = VectorSourceType.None;

        [JsonPropertyName("description")]

        public string Description { get; set; } = "";
        [JsonPropertyName("content")]
        public string Content { get; set; } = "";

        [JsonPropertyName("source_path")]
        public string SourcePath { get; set; } = "";

        [JsonPropertyName("git_repository_url")]
        public string GitRepositoryUrl { get; set; } = "";

        [JsonPropertyName("git_relative_path")]
        public string GitRelativePath { get; set; } = "";

        [JsonPropertyName("image_url")]
        public string ImageURL { get; set; } = "";

        // doc_id
        [JsonPropertyName("doc_id")]
        public string DocId { get; set; } = string.Empty;

        // Score
        [JsonPropertyName("score")]
        public double Score { get; set; } = 0.0;

        // sub_docs
        [JsonPropertyName("sub_docs")]
        public List<VectorEmbedding> SubDocs { get; set; } = [];


        public void UpdateSourceInfo(string description, string content, VectorSourceType sourceType, string source_path, string git_repository_url, string git_relative_path, string image_url) {
            Description = description;
            Content = content;
            SourceType = sourceType;
            SourcePath = source_path;
            GitRepositoryUrl = git_repository_url;
            GitRelativePath = git_relative_path;
            ImageURL = image_url;

        }
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                ["source_id"] = SourceId,
                ["source_type"] = SourceType.ToString(),
                ["description"] = Description,
                ["content"] = Content,
                ["source_path"] = SourcePath,
                ["git_repository_url"] = GitRepositoryUrl,
                ["git_relative_path"] = GitRelativePath,
                ["image_url"] = ImageURL,
                ["doc_id"] = DocId,
                ["score"] = Score
            };
            if (FolderId != null) {
                dict["FolderId"] = FolderId;
            }
            return dict;
        }

        public string ToJson() {
            return JsonSerializer.Serialize(this, Options);
        }

        public static List<VectorEmbedding> FromJson(string json) {
            JsonSerializerOptions options = Options;
            List<VectorEmbedding>? result = JsonSerializer.Deserialize<List<VectorEmbedding>>(json, options);
            return result ?? [];
        }

        public static VectorEmbedding FromDict(Dictionary<string, object> dict) {
            VectorEmbedding result = new();
            result.SourceId = dict["source_id"].ToString() ?? "";
            result.SourceType = (VectorSourceType)Enum.Parse(typeof(VectorSourceType), dict["source_type"].ToString() ?? "");
            result.Description = dict["description"].ToString() ?? "";
            result.Content = dict["content"].ToString() ?? "";
            result.SourcePath = dict["source_path"].ToString() ?? "";
            result.GitRepositoryUrl = dict["git_repository_url"].ToString() ?? "";
            result.GitRelativePath = dict["git_relative_path"].ToString() ?? "";
            result.ImageURL = dict["image_url"].ToString() ?? "";
            result.DocId = dict["doc_id"].ToString() ?? "";
            result.Score = Convert.ToDouble(dict["score"]);
            return result;
        }
        public static async Task UpdateEmbeddings(string vectorDBItemName, VectorEmbedding vectorEmbedding) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

            // Parallelによる並列処理。4並列
            ChatRequestContext chatRequestContext = new() {
                OpenAIProperties = openAIProperties,
            };

            EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, vectorEmbedding);

            LogWrapper.Info(PythonAILibStringResources.Instance.SavedEmbedding);
            await PythonExecutor.PythonAIFunctions.UpdateEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
            LogWrapper.Info(PythonAILibStringResources.Instance.SavedEmbedding);

        }

        public static async Task DeleteEmbeddings(string vectorDBItemName, VectorEmbedding vectorEmbedding) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                OpenAIProperties = openAIProperties,
            };

            EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, vectorEmbedding);

            LogWrapper.Info(PythonAILibStringResources.Instance.DeletedEmbedding);
            await PythonExecutor.PythonAIFunctions.DeleteEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
            LogWrapper.Info(PythonAILibStringResources.Instance.DeletedEmbedding);
        }

        // DeleteEmbeddingsByFolderAsync
        public static void DeleteEmbeddingsByFolder(string vectorDBItemName, string folderId) {
            Task.Run(() => {
                PythonAILibManager libManager = PythonAILibManager.Instance;
                OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
                ChatRequestContext chatRequestContext = new() {
                    OpenAIProperties = openAIProperties,
                };
                VectorEmbedding vectorEmbedding = new() {
                    FolderId = folderId,
                };
                EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(vectorDBItemName, openAIProperties.OpenAIEmbeddingModel, vectorEmbedding);
                PythonExecutor.PythonAIFunctions.DeleteEmbeddingsByFolderAsync(chatRequestContext, embeddingRequestContext);
            });
        }

    }
}
