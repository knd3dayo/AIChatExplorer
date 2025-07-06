using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.PythonIF.Response {
    public class ChatResponse : PythonScriptResult {

        public List<VectorEmbeddingItem> SourceDocuments { get; set; } = [];
        public long TotalTokens { get; set; } = 0;

        public ChatResponse() { }

        public override void LoadFromJson(string json) {
            base.LoadFromJson(json);

            // total_tokensを取得
            if (Parameters.TryGetValue("total_tokens", out dynamic? totalTokensValue)) {
                if (totalTokensValue is decimal totalTokens) {
                    TotalTokens = decimal.ToInt64(totalTokens);
                }
            }
            // Errorがある場合はLogWrapper.Errorを呼び出す
            if (Parameters.TryGetValue("error", out dynamic? errorValue)) {
                LogWrapper.Error(errorValue?.ToString());
            }

            // resultStringからDictionaryに変換する。
            Dictionary<string, object>? resultDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json, JsonUtil.JsonSerializerOptions);
            if (resultDict == null) {
                throw new Exception(PythonAILibStringResourcesJa.Instance.OpenAIResponseEmpty);
            }
            // documentsがある場合は取得
            if (resultDict.ContainsKey("documents")) {
                JsonElement? documentsObject = (JsonElement)resultDict["documents"];
                // List<VectorSearchResult>に変換
                SourceDocuments = EmbeddingResponse.FromJson(documentsObject.ToString() ?? "[]").Select(x => x.CreateVectorEmbeddingItem()).ToList();
            }
        }
    }

}
