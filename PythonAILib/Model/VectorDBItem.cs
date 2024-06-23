using System.Text.Json.Serialization;
using PythonAILib.PythonIF;

namespace PythonAILib.Model {
    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public abstract class VectorDBItem {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        // 名前
        [JsonPropertyName("VectorDBName")]
        public string Name { get; set; } = "";
        // 説明
        [JsonPropertyName("VectorDBDescription")]
        public string Description { get; set; } = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";

        [JsonPropertyName("VectorDBURL")]
        public string VectorDBURL { get; set; } = "";

        [JsonPropertyName("IsUseMultiVectorRetriever")]
        public bool IsUseMultiVectorRetriever { get; set; } = false;

        [JsonPropertyName("DocStoreURL")]
        public string DocStoreURL { get; set; } = "";

        [JsonIgnore]
        public VectorDBTypeEnum Type { get; set; } = VectorDBTypeEnum.Faiss;

        // VectorDBTypeString
        [JsonPropertyName("VectorDBTypeString")]
        public string VectorDBTypeString {
            get {
                return Type.ToString();
            }
        }
        // CollectionName
        [JsonPropertyName("CollectionName")]
        public string? CollectionName { get; set; } = null;

        // 有効かどうか
        [JsonIgnore]
        public bool IsEnabled { get; set; } = true;

        // Save
        public abstract void Save();

        // Delete
        public abstract void Delete();
        public abstract void UpdateIndex(IPythonFunctions.ClipboardInfo clipboard);
        public abstract void DeleteIndex(IPythonFunctions.ClipboardInfo clipboard);

    }
}
