using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using LibMain.Common;
using LibMain.Model.Chat;
using LibMain.Model.Content;
using LibMain.PythonIF;
using LibMain.PythonIF.Request;
using LibMain.Utils.Common;

namespace LibMain.Model.VectorDB {
    public class VectorSearchItem {

        public VectorSearchItem(VectorDBItem vectorDBItem) {
            VectorDBItemName = vectorDBItem.Name;
            Model = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties().OpenAIEmbeddingModel;
            TopK = vectorDBItem.DefaultSearchResultLimit;
            ScoreThreshold = vectorDBItem.DefaultScoreThreshold;
            Task.Run(async () => {
                await UpdateDisplayText();
            });
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

        public string DisplayText { get; private set; } = "";

        private async Task UpdateDisplayText() {
            // DisplayTextを更新する
            VectorDBItem? item = VectorDBItem.GetItemByName(VectorDBItemName);
            if (item == null) {
                DisplayText = "";

            } else if (string.IsNullOrEmpty(item.CollectionName)) {
                DisplayText = item.Name;
                return;
            } else if (FolderId == null) {
                DisplayText = item.Name;
                return;
            } else {
                ContentFolderWrapper? folder = await ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(FolderId);
                if (folder == null) {
                    DisplayText = item.Name;
                    return;
                }
                var contentFolderPath = await folder.GetContentFolderPath();
                DisplayText = $"{item.Name}:{contentFolderPath}";
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
            // ChatRequestContextを作成
            ChatSettings chatSettings = new() {
                VectorSearchRequests = [new VectorSearchRequest(this)],
                RAGMode = RAGModeEnum.NormalSearch,
            };
            ChatRequestContext chatRequestContext = new(chatSettings);

            // ベクトル検索を実行
            List<VectorEmbeddingItem> results = await PythonExecutor.PythonAIFunctions.VectorSearchAsync(chatRequestContext, InputText);
            return results;
        }


    }
}
