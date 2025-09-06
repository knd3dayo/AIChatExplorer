using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using LibMain.Model.Chat;
using LibMain.Model.Content;
using LibMain.Model.Prompt;
using LibMain.Utils.Common;

namespace LibMain.Data {
    public class ContentItemEntity {


        // id
        public const string ID_KEY = "id";
        // folder_id
        public const string FOLDER_ID_KEY = "folder_id";
        // created_at
        public const string CREATED_AT_KEY = "created_at";
        // updated_at
        public const string UPDATED_AT_KEY = "updated_at";
        // vectorized_at
        public const string VECTORIZED_AT_KEY = "vectorized_at";
        // content
        public const string CONTENT_KEY = "content";
        // description
        public const string DESCRIPTION_KEY = "description";
        // content_type
        public const string CONTENT_TYPE_KEY = "content_type";
        // chat_messages_json
        public const string CHAT_MESSAGES_JSON_KEY = "chat_messages_json";
        // prompt_chat_result_json
        public const string PROMPT_CHAT_RESULT_JSON_KEY = "prompt_chat_result_json";
        // tag_string
        public const string TAG_STRING_KEY = "tag_string";
        // is_pinned
        public const string IS_PINNED_KEY = "is_pinned";
        // cached_base64_string
        public const string CACHED_BASE64_STRING_KEY = "cached_base64_string";
        // extended_properties_json
        public const string EXTENDED_PROPERTIES_JSON_KEY = "extended_properties_json";

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

        public ContentItemTypes.ContentItemTypeEnum ContentType { get; set; }

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

        // ピン留め
        public bool IsPinned { get; set; }

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
                ExtendedPropertiesJson = JsonSerializer.Serialize(_extendedProperties, JsonUtil.JsonSerializerOptions);
            }
        }
        public void SavePromptChatResultJson() {
            if (_promptChatResult != null) {
                PromptChatResultJson = _promptChatResult.ToJson();
            }
        }

        public void SaveChatMessagesJson() {
            if (_chatItems != null) {
                ChatMessagesJson = JsonSerializer.Serialize(_chatItems, JsonUtil.JsonSerializerOptions);
            }
        }

        public void SaveTagString() {
            if (_tagIds != null) {
                TagString = string.Join(",", _tagIds);
            }
        }


        // SaveObjectToJson
        public void SaveObjectToJson() {
            this.SaveExtendedPropertiesJson();
            this.SavePromptChatResultJson();
            this.SaveChatMessagesJson();
            this.SaveTagString();
        }
        // Copy
        public ContentItemEntity Copy() {
            SaveObjectToJson();
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
                IsPinned = IsPinned,
                CachedBase64String = CachedBase64String,
                ExtendedPropertiesJson = ExtendedPropertiesJson
            };
            return newItem;
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

        public Dictionary<string, object> ToDict() {
            SaveObjectToJson();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict[ID_KEY] = Id;
            dict[FOLDER_ID_KEY] = FolderId ?? "";
            dict[CREATED_AT_KEY] = CreatedAt.ToString("o");
            dict[UPDATED_AT_KEY] = UpdatedAt.ToString("o");
            dict[VECTORIZED_AT_KEY] = VectorizedAt.ToString("o");
            dict[CONTENT_KEY] = Content;
            dict[DESCRIPTION_KEY] = Description;
            dict[CONTENT_TYPE_KEY] = (int)ContentType;
            dict[CHAT_MESSAGES_JSON_KEY] = ChatMessagesJson;
            dict[PROMPT_CHAT_RESULT_JSON_KEY] = PromptChatResultJson;
            dict[TAG_STRING_KEY] = string.Join(",", Tags);
            dict[IS_PINNED_KEY] = IsPinned;
            dict[CACHED_BASE64_STRING_KEY] = CachedBase64String;
            dict[EXTENDED_PROPERTIES_JSON_KEY] = ExtendedPropertiesJson;
            return dict;
        }
        public static ContentItemEntity FromDict(Dictionary<string, object> dict) {
            if (dict == null || !dict.ContainsKey(ID_KEY)) {
                throw new ArgumentException("Invalid dictionary for ContentItemResponse");
            }
            ContentItemEntity entity = new();
            entity.Id = dict.GetValueOrDefault(ID_KEY)?.ToString() ?? "";
            entity.FolderId = dict.GetValueOrDefault(FOLDER_ID_KEY)?.ToString() ?? "";
            entity.CreatedAt = DateTime.Parse(dict.GetValueOrDefault(CREATED_AT_KEY, "2000-01-01T00:00:00")?.ToString() ?? "2000-01-01T00:00:00");
            entity.UpdatedAt = DateTime.Parse(dict.GetValueOrDefault(UPDATED_AT_KEY)?.ToString() ?? "2000-01-01T00:00:00");
            entity.VectorizedAt = DateTime.Parse(dict.GetValueOrDefault(VECTORIZED_AT_KEY)?.ToString() ?? "2000-01-01T00:00:00");
            entity.Content = dict.GetValueOrDefault(CONTENT_KEY)?.ToString() ?? "";
            entity.Description = dict.GetValueOrDefault(DESCRIPTION_KEY)?.ToString() ?? "";
            int contentTypeValue = Int32.Parse(dict.GetValueOrDefault(CONTENT_TYPE_KEY)?.ToString() ?? "0");
            entity.ContentType = (ContentItemTypes.ContentItemTypeEnum)contentTypeValue;

            entity.ChatMessagesJson = dict.GetValueOrDefault(CHAT_MESSAGES_JSON_KEY)?.ToString() ?? "";
            entity.PromptChatResultJson = dict.GetValueOrDefault(PROMPT_CHAT_RESULT_JSON_KEY)?.ToString() ?? "";
            entity.IsPinned = dict.GetValueOrDefault(IS_PINNED_KEY)?.ToString() == "1";
            entity.CachedBase64String = dict.GetValueOrDefault(CACHED_BASE64_STRING_KEY)?.ToString() ?? "";
            entity.ExtendedPropertiesJson = dict.GetValueOrDefault(EXTENDED_PROPERTIES_JSON_KEY)?.ToString() ?? "";

            var tagsValue = dict.GetValueOrDefault(TAG_STRING_KEY);
            if (tagsValue is string tagString) {
                entity.Tags = tagString.Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            } else if (tagsValue is List<string> tagList) {
                entity.Tags = tagList.ToHashSet();
            } else {
                entity.Tags = [];
            }

            return entity;
        }

    }
}
