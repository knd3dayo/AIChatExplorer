using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibPythonAI.Model.Content;

namespace LibPythonAI.PythonIF.Request {
    public  class ContentItemRequest {

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

        public ContentItemRequest(ContentItemWrapper contentItem) {
            ContentItem = contentItem;
        }
        public ContentItemWrapper ContentItem { get; set; }

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            dict[ID_KEY] = ContentItem.Id;
            dict[FOLDER_ID_KEY] = ContentItem.FolderId ?? "";
            dict[CREATED_AT_KEY] = ContentItem.CreatedAt.ToString("o");
            dict[UPDATED_AT_KEY] = ContentItem.UpdatedAt.ToString("o");
            dict[VECTORIZED_AT_KEY] = ContentItem.VectorizedAt.ToString("o");
            dict[CONTENT_KEY] = ContentItem.Content;
            dict[DESCRIPTION_KEY] = ContentItem.Description;
            dict[CONTENT_TYPE_KEY] = ContentItem.ContentType.ToString();
            dict[CHAT_MESSAGES_JSON_KEY] = ContentItem.ChatMessagesJson;
            dict[PROMPT_CHAT_RESULT_JSON_KEY] = ContentItem.PromptChatResultJson;
            dict[TAG_STRING_KEY] = string.Join(",", ContentItem.Tags);
            dict[IS_PINNED_KEY] = ContentItem.IsPinned;
            dict[CACHED_BASE64_STRING_KEY] = ContentItem.CachedBase64String;
            dict[EXTENDED_PROPERTIES_JSON_KEY] = ContentItem.ExtendedPropertiesJson;
            return dict;
        }

        public static List<Dictionary<string, object>> ToDictList(List<ContentItemRequest> requests) {
            List<Dictionary<string, object>> dictList = [];
            foreach (var request in requests) {
                dictList.Add(request.ToDict());
            }
            return dictList;

        }
    }
}
