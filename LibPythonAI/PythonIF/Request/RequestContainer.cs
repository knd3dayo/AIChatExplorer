using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Model.Chat;

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

        public AutogenRequest? AutogenRequestInstance { get; set; }

        // VectorSearchRequest
        public List<VectorSearchRequest> VectorSearchRequestsInstance { get; set; } = [];

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
                dict["vector_db_props"] = RequestContextInstance.ToDictVectorDBItemsDict();
            }

            if (RequestContextInstance != null) {
                dict["openai_props"] = RequestContextInstance.OpenAIProperties.ToDict();
            }
            if (RequestContextInstance != null) {
                dict["autogen_props"] = RequestContextInstance.AutoGenProperties.ToDict();
            }

            if (RequestContextInstance != null) {
                dict["chat_request_context"] = RequestContextInstance.ToChatRequestContextDict();
            }
            if (ChatRequestInstance != null) {
                dict["chat_request"] = ChatRequestInstance.ToDict();
            }
            if (TokenCountRequestInstance != null) {
                dict["token_count_request"] = TokenCountRequestInstance.ToDict();
            }
            if (AutogenRequestInstance != null) {
                dict["autogen_request"] = AutogenRequestInstance.ToDict();
            }
            if (VectorSearchRequestsInstance != null) {
                dict["vector_search_requests"] = VectorSearchRequest.ToDictList(VectorSearchRequestsInstance);
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

            return dict;
        }
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }

    }
}
