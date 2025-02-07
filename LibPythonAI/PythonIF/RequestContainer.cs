using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Model.Chat;

namespace LibPythonAI.PythonIF {
    public class RequestContainer {

        static readonly JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };

        public ChatRequestContext? RequestContextInstance { get; set; }
        public ChatRequest? ChatRequestInstance { get; set; }

        public TokenCountRequest? TokenCountRequestInstance { get; set; }

        public AutogenRequest? AutogenRequestInstance { get; set; }

        // QueryRequest
        public QueryRequest? QueryRequestInstance { get; set; }

        // CatalogRequest
        public CatalogRequest? CatalogRequestInstance { get; set; }

        // ExcelRequest
        public ExcelRequest? ExcelRequestInstance { get; set; }

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            if (RequestContextInstance != null) {
                dict["context"] = RequestContextInstance.ToDict();
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
            if (QueryRequestInstance != null) {
                dict["query_request"] = QueryRequestInstance.ToDict();
            }
            if (CatalogRequestInstance != null) {
                dict["catalog_request"] = CatalogRequestInstance.ToDict();
            }
            if (ExcelRequestInstance != null) {
                dict["excel_request"] = ExcelRequestInstance.ToDict();
            }

            return dict;
        }
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }

    }
}
