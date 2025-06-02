using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.PythonIF;

namespace LibPythonAI.Model.VectorDB {
    public class VectorSearchItem {

        public VectorSearchItem(VectorDBItem vectorDBItem) {
            VectorDBItemName = vectorDBItem.Name;
            Model = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties().OpenAIEmbeddingModel;
            TopK = vectorDBItem.DefaultSearchResultLimit;
            ScoreThreshold = vectorDBItem.DefaultScoreThreshold;
        }


        [JsonPropertyName("name")]
        public string? VectorDBItemName { init; get; } = null;

        [JsonPropertyName("model")]
        public string? Model { get; set; } = null;

        // InputText
        [JsonPropertyName("query")]
        public string? InputText { get; set; } = null;

        //TopK
        public int TopK { get; set; } = 10; // デフォルト値

        // score_threshold
        public float ScoreThreshold { get; set; } = 0.5f;

        // FolderId
        public string? FolderId { get; set; } = null;

        // FolderPath
        public string? FolderPath { get; set; } = null;

        public string DisplayText {
            get {
                VectorDBItem? item = VectorDBItem.GetItemByName(VectorDBItemName);
                if (item == null) {
                    return "";
                }
                if (string.IsNullOrEmpty(item.CollectionName)) {
                    return item.Name;
                }
                if (FolderId == null) {
                    return item.Name;
                }
                ContentFolderWrapper? folder = ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(FolderId);
                if (folder == null) {
                    return item.Name;
                }
                return $"{item.Name}:{folder.ContentFolderPath}";
            }
        }


        public static List<VectorSearchItem> FromListJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Deserialize<List<VectorSearchItem>>(json, options) ?? [];
        }

        // ToListJson
        public static string ToListJson(IEnumerable<VectorSearchItem> items) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(items, options);
        }


        // ベクトル検索を実行する
        public async Task<List<VectorEmbeddingItem>> VectorSearchAsync() {
            // InputTextがnullまたは空文字の場合は空のリストを返す
            if (string.IsNullOrEmpty(InputText)) {
                LogWrapper.Warn("InputText is null or empty.");
                return [];
            }
            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorSearchRequests = [new VectorSearchRequest(this)],
                RAGMode = RAGModeEnum.NormalSearch,
            };

            // ベクトル検索を実行
            List<VectorEmbeddingItem> results = await PythonExecutor.PythonAIFunctions.VectorSearchAsync(chatRequestContext, InputText);
            return results;
        }


    }
}
