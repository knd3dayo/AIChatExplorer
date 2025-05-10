using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Tag;
using LibPythonAI.Model.VectorDB;

namespace LibPythonAI.PythonIF.Request {
    public class RequestContainer {

        static readonly JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };

        public string SessionToken { get; set; } = "";

        public ChatRequestContext? RequestContextInstance { get; set; }
        public ChatRequest? ChatRequestInstance { get; set; }

        public TokenCountRequest? TokenCountRequestInstance { get; set; }

        
        // ContentFolderRequests
        public List<ContentFolderRequest> ContentFolderRequestsInstance { get; set; } = [];


        // VectorDBItemInstance
        public VectorDBItem? VectorDBItemInstance { get; set; } = null;

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

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];

            if (RequestContextInstance != null) {
                dict["openai_props"] = RequestContextInstance.OpenAIProperties.ToDict();
                dict["autogen_props"] = RequestContextInstance.AutoGenProperties.ToDict();
                dict["chat_request_context"] = RequestContextInstance.ToChatRequestContextDict();
                dict["vector_search_requests"] = RequestContextInstance.ToDictVectorDBItemsDict();
            }


            if (ChatRequestInstance != null) {
                dict["chat_request"] = ChatRequestInstance.ToDict();
            }
            if (TokenCountRequestInstance != null) {
                dict["token_count_request"] = TokenCountRequestInstance.ToDict();
            }
            if (VectorDBItemInstance != null) {
                dict["vector_db_item_request"] = VectorDBItemInstance.ToDict();
            }

            if (EmbeddingRequestInstance != null) {
                dict["embedding_request"] = EmbeddingRequestInstance.ToDict();
            }
            if (ExcelRequestInstance != null) {
                dict["excel_request"] = ExcelRequestInstance.ToDict();
            }
            if (FileRequestInstance != null) {
                dict["file_request"] = FileRequestInstance.ToDict();
            }
            if (WebRequestInstance != null) {
                dict["web_request"] = WebRequestInstance.ToDict();
            }
            if (SessionToken != "") {
                dict["session_token"] = SessionToken;
            }
            if (TagItemsInstance.Count > 0) {
                dict["tag_item_requests"] = TagItem.ToDictList(TagItemsInstance);
            }
            if (ContentFolderRequestsInstance.Count > 0) {
                dict["content_folder_requests"] = ContentFolderRequest.ToDictList(ContentFolderRequestsInstance);
            }

            return dict;
        }
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }

    }
}
