using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibGit2Sharp;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using PythonAILib.Utils.Common;

namespace LibPythonAI.Data {
    public class ContentItemEntity {

        private static readonly JsonSerializerOptions jsonSerializerOptions = new() {
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
        private List<ChatMessage>? _chatItems;
        [NotMapped]
        public List<ChatMessage> ChatItems {
            get {
                if (_chatItems == null) {
                    _chatItems = ChatMessage.FromListJson(ChatMessagesJson);
                }
                return _chatItems ?? new();
            }
        }

        public string PromptChatResultJson { get; set; } = "{}";

        // プロンプトテンプレートに基づくチャットの結果
        private PromptChatResult? _promptChatResult;
        [NotMapped]
        public PromptChatResult PromptChatResult {
            get {
                if (_promptChatResult == null) {
                    _promptChatResult = PromptChatResult.FromJson(PromptChatResultJson);
                }
                return _promptChatResult ?? new();
            }
        }

        public string TagString { get; set; } = "";
        //Tags
        private HashSet<string>? _tagIds;
        [NotMapped]
        public HashSet<string> Tags {
            get {
                if (_tagIds == null) {
                    _tagIds = TagString.Split(',').ToHashSet();
                }
                return _tagIds;
            }
            set {
                _tagIds = value;
            }
        }

        //　貼り付け元のアプリケーション名
        public string SourceApplicationName { get; set; } = "";
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle { get; set; } = "";
        //　貼り付け元のアプリケーションのID
        public int SourceApplicationID { get; set; }
        //　貼り付け元のアプリケーションのパス
        public string SourceApplicationPath { get; set; } = "";

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
                ExtendedPropertiesJson = JsonSerializer.Serialize(_extendedProperties, jsonSerializerOptions);
            }
        }
        public void SavePromptChatResultJson() {
            if (_promptChatResult != null) {
                PromptChatResultJson = _promptChatResult.ToJson();
            }
        }

        public void SaveChatMessagesJson() {
            if (_chatItems != null) {
                ChatMessagesJson = JsonSerializer.Serialize(_chatItems, jsonSerializerOptions);
            }
        }

        public void SaveTagString() {
            if (_tagIds != null) {
                TagString = string.Join(",", _tagIds);
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
                ChatMessagesJson = ChatMessagesJson,
                PromptChatResultJson = PromptChatResultJson,
                TagString = TagString,
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

        // 複数アイテムのSave
        public static void SaveItems(List<ContentItemEntity> items) {
            using PythonAILibDBContext context = new();
            foreach (var item in items) {
                item.SaveExtendedPropertiesJson();
                item.SavePromptChatResultJson();
                item.SaveChatMessagesJson();
                item.SaveTagString();
                var ExistingItem = context.ContentItems.FirstOrDefault(x => x.Id == item.Id);
                if (ExistingItem == null) {
                    context.ContentItems.Add(item);
                } else {
                    context.Entry(ExistingItem).CurrentValues.SetValues(item);
                }
            }
            context.SaveChanges();
        }

        // 複数アイテムのDelete
        public static void DeleteItems(List<ContentItemEntity> items) {
            using PythonAILibDBContext context = new();
            foreach (var item in items) {
                var ExistingItem = context.ContentItems.FirstOrDefault(x => x.Id == item.Id);
                if (ExistingItem != null) {
                    context.ContentItems.Remove(ExistingItem);
                }
            }
            context.SaveChanges();
        }
        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            ContentItemEntity item = (ContentItemEntity)obj;
            return Id == item.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

    }
}
