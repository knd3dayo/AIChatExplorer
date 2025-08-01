using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF.Request;

namespace LibPythonAI.PythonIF.Response {
    public class ContentItemResponse : ContentItemRequest {
        private ContentItemResponse(ContentItemEntity entity) : base(entity) {
            Entity = entity;
        }

        public static ContentItemResponse FromDict(Dictionary<string, object> dict) {
            if (dict == null || !dict.ContainsKey(ContentItemRequest.ID_KEY)) {
                throw new ArgumentException("Invalid dictionary for ContentItemResponse");
            }
            ContentItemEntity entity = new();
            entity.Id = dict.GetValueOrDefault(ContentItemRequest.ID_KEY)?.ToString() ?? "";
            entity.FolderId = dict.GetValueOrDefault(ContentItemRequest.FOLDER_ID_KEY)?.ToString() ?? "";
            entity.CreatedAt = DateTime.Parse(dict.GetValueOrDefault(ContentItemRequest.CREATED_AT_KEY, "2000-01-01T00:00:00")?.ToString() ?? "2000-01-01T00:00:00");
            entity.UpdatedAt = DateTime.Parse(dict.GetValueOrDefault(ContentItemRequest.UPDATED_AT_KEY)?.ToString() ?? "2000-01-01T00:00:00");
            entity.VectorizedAt = DateTime.Parse(dict.GetValueOrDefault(ContentItemRequest.VECTORIZED_AT_KEY)?.ToString() ?? "2000-01-01T00:00:00");
            entity.Content = dict.GetValueOrDefault(ContentItemRequest.CONTENT_KEY)?.ToString() ?? "";
            entity.Description = dict.GetValueOrDefault(ContentItemRequest.DESCRIPTION_KEY)?.ToString() ?? "";
            int contentTypeValue = Int32.Parse(dict.GetValueOrDefault(ContentItemRequest.CONTENT_TYPE_KEY)?.ToString() ?? "0");
            entity.ContentType = (ContentItemTypes.ContentItemTypeEnum)contentTypeValue;

            entity.ChatMessagesJson = dict.GetValueOrDefault(ContentItemRequest.CHAT_MESSAGES_JSON_KEY)?.ToString() ?? "";
            entity.PromptChatResultJson = dict.GetValueOrDefault(ContentItemRequest.PROMPT_CHAT_RESULT_JSON_KEY)?.ToString() ?? "";
            entity.IsPinned = bool.TryParse(dict.GetValueOrDefault(ContentItemRequest.IS_PINNED_KEY)?.ToString(), out var isPinned) && isPinned;
            entity.CachedBase64String = dict.GetValueOrDefault(ContentItemRequest.CACHED_BASE64_STRING_KEY)?.ToString() ?? "";
            entity.ExtendedPropertiesJson = dict.GetValueOrDefault(ContentItemRequest.EXTENDED_PROPERTIES_JSON_KEY)?.ToString() ?? "";

            var tagsValue = dict.GetValueOrDefault(ContentItemRequest.TAG_STRING_KEY);
            if (tagsValue is string tagString) {
                entity.Tags = tagString.Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            } else if (tagsValue is List<string> tagList) {
                entity.Tags = tagList.ToHashSet();
            } else {
                entity.Tags = [];
            }

            return new ContentItemResponse(entity);
        }
    }
}
