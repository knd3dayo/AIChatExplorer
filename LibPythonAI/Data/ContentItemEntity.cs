using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using PythonAILib.Utils.Common;

namespace LibPythonAI.Data {
    public class ContentItemEntity {

        private static JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
        // 日時のダミー初期値。2000/1/1 0:0:0
        public static readonly DateTime InitialDateTime = new(2000, 1, 1, 0, 0, 0);


        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? FolderId { get; set; }


        // 生成日時
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        // 更新日時
        public DateTime UpdatedAt { get; set; }
        // ベクトル化日時
        public DateTime VectorizedAt { get; set; } = InitialDateTime;
        // クリップボードの内容
        public string Content { get; set; } = "";
        //説明
        public string Description { get; set; } = "";

        // クリップボードの内容の種類

        public ContentTypes.ContentItemTypes ContentType { get; set; }

        // ChatMessagesJson
        public string ChatMessagesJson { get; set; } = "[]";

        // OpenAIチャットのChatItemコレクション
        [NotMapped]
        public List<ChatMessage> ChatItems {
            get {
                List<ChatMessage>? items = JsonSerializer.Deserialize<List<ChatMessage>>(ChatMessagesJson, jsonSerializerOptions);
                return items ?? [];
            }
            set {
                ChatMessagesJson = JsonSerializer.Serialize(value, jsonSerializerOptions);
            }
        }

        public string PromptChatResultJson { get; set; } = "{}";
        // プロンプトテンプレートに基づくチャットの結果
        [NotMapped]
        public PromptChatResult PromptChatResult {
            get {
                PromptChatResult? result = JsonSerializer.Deserialize<PromptChatResult>(PromptChatResultJson, jsonSerializerOptions);
                return result ?? new();
            }
            set {
                PromptChatResultJson = JsonSerializer.Serialize(value, jsonSerializerOptions);
            }
        }

        //Tags
        public HashSet<TagItemEntity> Tags { get; set; } = [];

        //　貼り付け元のアプリケーション名
        public string SourceApplicationName { get; set; } = "";
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle { get; set; } = "";
        //　貼り付け元のアプリケーションのID
        public int? SourceApplicationID { get; set; }
        //　貼り付け元のアプリケーションのパス
        public string? SourceApplicationPath { get; set; }

        // ピン留め
        public bool IsPinned { get; set; }

        // 文書の信頼度(0-100)
        public int DocumentReliability { get; set; } = 0;
        // 文書の信頼度の判定理由
        public string DocumentReliabilityReason { get; set; } = "";

        // ReferenceVectorDBItemsがフォルダのReferenceVectorDBItemsと同期済みかどうか
        public bool IsReferenceVectorDBItemsSynced { get; set; } = false;

        public string CachedBase64String { get; set; } = "";

        public string ExtendedPropertiesJson { get; set; } = "{}";

        private Dictionary<string, object?>? _extendedProperties;
        // 拡張プロパティ (Dictionary<string, object> は EF でサポートされないため、Json で保存)
        [NotMapped]
        public Dictionary<string, object?> ExtendedProperties {
            get {
                if (_extendedProperties == null) {
                    _extendedProperties = JsonUtil.ParseJson(ExtendedPropertiesJson);
                }
                return _extendedProperties ?? [];
            }
        }

        public void SaveExtendedPropertiesJson() {
            if (_extendedProperties != null) {
                ExtendedPropertiesJson = JsonSerializer.Serialize(ExtendedProperties, jsonSerializerOptions);
            }
        }


        // Copy
        public ContentItemEntity Copy() {
            ContentItemEntity newItem = new() {
                CreatedAt = CreatedAt,
                UpdatedAt = UpdatedAt,
                VectorizedAt = VectorizedAt,
                Content = Content,
                Description = Description,
                ContentType = ContentType,
                ChatItems = ChatItems,
                PromptChatResult = PromptChatResult,
                Tags = Tags,
                SourceApplicationName = SourceApplicationName,
                SourceApplicationTitle = SourceApplicationTitle,
                SourceApplicationID = SourceApplicationID,
                SourceApplicationPath = SourceApplicationPath,
                IsPinned = IsPinned,
                DocumentReliability = DocumentReliability,
                DocumentReliabilityReason = DocumentReliabilityReason,
                IsReferenceVectorDBItemsSynced = IsReferenceVectorDBItemsSynced,
                CachedBase64String = CachedBase64String,
                ExtendedPropertiesJson = ExtendedPropertiesJson
            };
            return newItem;
        }


    }
}
