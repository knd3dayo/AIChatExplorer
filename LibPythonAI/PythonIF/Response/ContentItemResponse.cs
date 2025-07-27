using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF.Request;

namespace LibPythonAI.PythonIF.Response {
    public class ContentItemResponse : ContentItemRequest {
        private ContentItemResponse(ContentItemWrapper contentItem) : base(contentItem) {
            ContentItem = contentItem;
        }

        public static ContentItemResponse FromDict(Dictionary<string, object> dict) {
            if (dict == null || !dict.ContainsKey(ContentItemRequest.ID_KEY)) {
                throw new ArgumentException("Invalid dictionary for ContentItemResponse");
            }
            ContentItemWrapper contentItem = new();
            contentItem.Id = dict.GetValueOrDefault(ContentItemRequest.ID_KEY)?.ToString() ?? "";
            contentItem.FolderId = dict.GetValueOrDefault(ContentItemRequest.FOLDER_ID_KEY)?.ToString() ?? "";
            contentItem.CreatedAt = DateTime.Parse(dict.GetValueOrDefault(ContentItemRequest.CREATED_AT_KEY, "2000-01-01T00:00:00")?.ToString() ?? "2000-01-01T00:00:00");
            contentItem.UpdatedAt = DateTime.Parse(dict.GetValueOrDefault(ContentItemRequest.UPDATED_AT_KEY)?.ToString() ?? "2000-01-01T00:00:00");
            contentItem.VectorizedAt = DateTime.Parse(dict.GetValueOrDefault(ContentItemRequest.VECTORIZED_AT_KEY)?.ToString() ?? "2000-01-01T00:00:00");
            contentItem.Content = dict.GetValueOrDefault(ContentItemRequest.CONTENT_KEY)?.ToString() ?? "";
            contentItem.Description = dict.GetValueOrDefault(ContentItemRequest.DESCRIPTION_KEY)?.ToString() ?? "";
            contentItem.ContentType = Enum.Parse<ContentItemTypes.ContentItemTypeEnum>(dict.GetValueOrDefault(ContentItemRequest.CONTENT_TYPE_KEY)?.ToString() ?? "Text");
            contentItem.ChatMessagesJson = dict.GetValueOrDefault(ContentItemRequest.CHAT_MESSAGES_JSON_KEY)?.ToString() ?? "";
            contentItem.PromptChatResultJson = dict.GetValueOrDefault(ContentItemRequest.PROMPT_CHAT_RESULT_JSON_KEY)?.ToString() ?? "";
            contentItem.IsPinned = bool.TryParse(dict.GetValueOrDefault(ContentItemRequest.IS_PINNED_KEY)?.ToString(), out var isPinned) && isPinned;
            contentItem.CachedBase64String = dict.GetValueOrDefault(ContentItemRequest.CACHED_BASE64_STRING_KEY)?.ToString() ?? "";
            contentItem.ExtendedPropertiesJson = dict.GetValueOrDefault(ContentItemRequest.EXTENDED_PROPERTIES_JSON_KEY)?.ToString() ?? "";

            var tagsValue = dict.GetValueOrDefault(ContentItemRequest.TAG_STRING_KEY);
            if (tagsValue is string tagString) {
                contentItem.Tags = tagString.Split(',', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
            } else if (tagsValue is List<string> tagList) {
                contentItem.Tags = tagList.ToHashSet();
            } else {
                contentItem.Tags = [];
            }

            return new ContentItemResponse(contentItem);
        }
    }
}
