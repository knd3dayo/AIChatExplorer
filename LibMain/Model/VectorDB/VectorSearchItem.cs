using System.Text.Json;
using System.Text.Json.Serialization;
using LibMain.PythonIF;
using LibMain.PythonIF.Request;
using LibMain.Utils.Common;

namespace LibMain.Model.VectorDB {
    public class VectorSearchItem {

        public VectorSearchItem(VectorDBItem vectorDBItem) {
            VectorDBItemName = vectorDBItem.Name;
            TopK = vectorDBItem.DefaultSearchResultLimit;
            ScoreThreshold = vectorDBItem.DefaultScoreThreshold;
            UpdateDisplayText();
        }


        [JsonPropertyName("vector_db_name")]
        public string? VectorDBItemName { init; get; } = null;

        // InputText
        [JsonPropertyName("query")]
        public string? InputText { get; set; } = null;

        //TopK
        public int TopK { get; set; } = 10; // デフォルト値

        // score_threshold
        public float ScoreThreshold { get; set; } = 0.5f;

        // FolderPath
        public string? FolderPath { get; set; } = null;

        public string DisplayText { get; private set; } = "";

        private void UpdateDisplayText() {
            // DisplayTextを更新する
            VectorDBItem? item = VectorDBItem.GetItemByName(VectorDBItemName);
            if (item == null) {
                DisplayText = "";

            } else if (string.IsNullOrEmpty(item.CollectionName)) {
                DisplayText = item.Name;
                return;
            } else if (FolderPath == null) {
                DisplayText = item.Name;
                return;
            } else {
                DisplayText = $"{item.Name}:{FolderPath}";
            }
        }



        public static List<VectorSearchItem> FromListJson(string json) {

            return JsonSerializer.Deserialize<List<VectorSearchItem>>(json, JsonUtil.JsonSerializerOptions) ?? [];
        }

        // ToListJson
        public static string ToListJson(IEnumerable<VectorSearchItem> items) {

            return JsonSerializer.Serialize(items, JsonUtil.JsonSerializerOptions);
        }


        // ベクトル検索を実行する
        public async Task<List<VectorEmbeddingItem>> VectorSearchAsync() {
            // InputTextがnullまたは空文字の場合は空のリストを返す
            if (string.IsNullOrEmpty(InputText)) {
                LogWrapper.Warn("InputText is null or empty.");
                return [];
            }

            VectorSearchRequest vectorSearchRequest = new(this);

            // ベクトル検索を実行
            List<VectorEmbeddingItem> results = await PythonExecutor.PythonAIFunctions.VectorSearchAsync(vectorSearchRequest);
            return results;
        }


    }
}
