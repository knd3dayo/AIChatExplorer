using System.Text.Json;
using LibMain.Model.Search;
using LibMain.Model.Tag;
using LibMain.Utils.Common;

namespace LibMain.PythonIF.Request {
    public class RequestContainer {


        public const string CONTENT_ITEM_REQUESTS_KEY = "content_item_requests";
        public const string CONTENT_FOLDER_REQUESTS_KEY = "content_folder_requests";
        public const string AUTO_PROCESS_ITEMS_KEY = "auto_process_item_requests";
        public const string AUTO_PROCESS_RULES_KEY = "auto_process_rule_requests";
        public const string SEARCH_RULE_REQUESTS_KEY = "search_rule_requests";
        public const string PROMPT_ITEM_REQUESTS_KEY = "prompt_item_requests";
        public const string TAG_ITEM_REQUESTS_KEY = "tag_item_requests";
        public const string VECTOR_DB_ITEM_REQUEST_KEY = "vector_db_item_request";
        public const string EMBEDDING_REQUEST_KEY = "embedding_request";
        public const string VECTOR_SEARCH_REQUESTS_KEY = "vector_search_requests";
        public const string CHAT_REQUEST_KEY = "chat_request";
        public const string CHAT_REQUEST_CONTEXT_KEY = "chat_request_context";
        public const string TOKEN_COUNT_REQUEST_KEY = "token_count_request";
        public const string AUTOGEN_PROPS_KEY = "autogen_props";
        public const string AUTOGEN_TOOL_REQUEST_KEY = "autogen_tool_request";
        public const string AUTOGEN_AGENT_REQUEST_KEY = "autogen_agent_request";
        public const string AUTOGEN_GROUP_CHAT_REQUEST_KEY = "autogen_group_chat_request";
        public const string AUTOGEN_LLM_CONFIG_REQUEST_KEY = "autogen_llm_config_request";
        public const string SESSION_TOKEN_KEY = "session_token";
        public const string EXCEL_REQUEST_KEY = "excel_request";
        public const string FILE_REQUEST_KEY = "file_request";
        public const string WEB_REQUEST_KEY = "web_request";
        public const string SEARCH_REQUEST_KEY = "search_request";
        public string SessionToken { get; set; } = "";

        public ChatRequestContext? RequestContextInstance { get; set; }
        public ChatRequest? ChatRequestInstance { get; set; }

        public TokenCountRequest? TokenCountRequestInstance { get; set; }

        // ContentItemRequests
        public List<ContentItemRequest> ContentItemRequestsInstance { get; set; } = [];
        // ContentFolderRequests
        public List<ContentFolderRequest> ContentFolderRequestsInstance { get; set; } = [];

        // PromptItemsInstance
        public List<PromptItemRequest> PromptItemsInstance { get; set; } = [];

        // VectorDBItemRequestInstance
        public VectorDBItemRequest? VectorDBItemRequestInstance { get; set; } = null;

        // AutoProcessItemsInstance
        public List<AutoProcessItemRequest> AutoProcessItemsInstance { get; set; } = [];

        // AutoProcessRulesInstance
        public List<AutoProcessRuleRequest> AutoProcessRulesInstance { get; set; } = [];

        // SearchRuleRequestsInstance
        public List<SearchRuleRequest> SearchRuleRequestsInstance { get; set; } = [];

        // TagItemInstance
        public List<TagItem> TagItemsInstance { get; set; } = [];

        // ExcelRequest
        public ExcelRequest? ExcelRequestInstance { get; set; }

        // FileRequest
        public FileRequest? FileRequestInstance { get; set; }

        // WebRequest
        public WebRequest? WebRequestInstance { get; set; }

        // EmbeddingRequest
        public EmbeddingRequest? EmbeddingRequestInstance { get; set; }

        // SearchRequest
        public SearchCondition? SearchRequestInstance { get; set; }

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];


            if (ContentItemRequestsInstance.Count > 0) {
                dict[CONTENT_ITEM_REQUESTS_KEY] = ContentItemRequest.ToDictList(ContentItemRequestsInstance);
            }
            if (RequestContextInstance != null) {
                dict[CHAT_REQUEST_CONTEXT_KEY] = RequestContextInstance.ToChatRequestContextDict();

                if (RequestContextInstance.VectorSearchRequests.Count > 0) {
                    dict[VECTOR_SEARCH_REQUESTS_KEY] = RequestContextInstance.ToDictVectorDBRequestDict();
                }
            }

            if (EmbeddingRequestInstance != null) {
                dict[EMBEDDING_REQUEST_KEY] = EmbeddingRequestInstance.ToDict();
            }


            if (ChatRequestInstance != null) {
                dict[CHAT_REQUEST_KEY] = ChatRequestInstance.ToDict();
            }
            if (TokenCountRequestInstance != null) {
                dict[TOKEN_COUNT_REQUEST_KEY] = TokenCountRequestInstance.ToDict();
            }
            if (VectorDBItemRequestInstance != null) {
                dict[VECTOR_DB_ITEM_REQUEST_KEY] = VectorDBItemRequestInstance.ToDict();
            }

            if (ExcelRequestInstance != null) {
                dict[EXCEL_REQUEST_KEY] = ExcelRequestInstance.ToDict();
            }
            if (FileRequestInstance != null) {
                dict[FILE_REQUEST_KEY] = FileRequestInstance.ToDict();
            }
            if (WebRequestInstance != null) {
                dict[WEB_REQUEST_KEY] = WebRequestInstance.ToDict();
            }
            if (SessionToken != "") {
                dict[SESSION_TOKEN_KEY] = SessionToken;
            }
            if (AutoProcessItemsInstance.Count > 0) {
                dict[AUTO_PROCESS_ITEMS_KEY] = AutoProcessItemRequest.ToDictList(AutoProcessItemsInstance);
            }
            if (AutoProcessRulesInstance.Count > 0) {
                dict[AUTO_PROCESS_RULES_KEY] = AutoProcessRuleRequest.ToDictList(AutoProcessRulesInstance);
            }
            if (PromptItemsInstance.Count > 0) {
                dict[PROMPT_ITEM_REQUESTS_KEY] = PromptItemRequest.ToDictList(PromptItemsInstance);
            }
            if (SearchRuleRequestsInstance.Count > 0) {
                dict[SEARCH_RULE_REQUESTS_KEY] = SearchRuleRequest.ToDictList(SearchRuleRequestsInstance);
            }
            if (TagItemsInstance.Count > 0) {
                dict[TAG_ITEM_REQUESTS_KEY] = TagItem.ToDictList(TagItemsInstance);
            }
            if (ContentFolderRequestsInstance.Count > 0) {
                dict[CONTENT_FOLDER_REQUESTS_KEY] = ContentFolderRequest.ToDictList(ContentFolderRequestsInstance);
            }
            if (SearchRequestInstance != null) {
                dict[SEARCH_REQUEST_KEY] = SearchRequestInstance.ToDict();
            }
            return dict;
        }
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), JsonUtil.JsonSerializerOptions);
        }

    }
}
