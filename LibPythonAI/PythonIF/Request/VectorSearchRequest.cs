using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace LibPythonAI.PythonIF.Request {
    public class VectorSearchRequest {

        public VectorSearchRequest(string vectorDBName, string inputText, int topK, string? folderId, string contentType) {
            Name = vectorDBName;
            InputText = inputText;
            TopK = topK;
            FolderId = folderId;
            ContentType = contentType;
        }

        public string Name { get; set; } = "";
        // Model
        public string Model { get; set; } = "text-embedding-3-small";

        // TopK
        public int TopK { get; set; } = 5;
        // FolderId
        public string? FolderId { get; set; }
        // ContentType
        public string ContentType { get; set; } = "";


        public string InputText { get; private set; } = "";

        private Dictionary<string, object> GetSearchKwargs() {
            Dictionary<string, object> dict = new() {
                ["k"] = TopK
            };
            // filter 
            Dictionary<string, object> filter = new();
            // folder_idが指定されている場合
            if (string.IsNullOrEmpty(FolderId) == false) {
                filter["folder_id"] = FolderId;
            }
            // content_typeが指定されている場合
            if (string.IsNullOrEmpty(ContentType) == false) {
                filter["content_type"] = ContentType;
            }
            // filterが指定されている場合
            if (filter.Count > 0) {
                dict["filter"] = filter;
            }

            return dict;
        }


        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = [];
            dict["Name"] = Name;
            dict["model"] = Model;
            dict["input_text"] = InputText;
            dict["SearchKwargs"] = GetSearchKwargs();

            return dict;
        }

        public static List<Dictionary<string, object>> ToDictList(List<VectorSearchRequest> requests) {
            List<Dictionary<string, object>> dicts = [];
            foreach (var request in requests) {
                dicts.Add(request.ToDict());
            }
            return dicts;
        }
    }
}
