using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace PythonAILib.Model.VectorDB {
    public class VectorSearchProperty {

        public VectorSearchProperty() { }

        public VectorSearchProperty(VectorDBItem vectorDBItem) {
            VectorDBItem = vectorDBItem;
        }

        public VectorDBItem VectorDBItem { get; set; }

        //TopK
        public int TopK { get; set; } = 10;

        // FolderId
        public string FolderId { get; set; } = string.Empty;

        // ContentType
        public string ContentType { get; set; } = string.Empty;
        // SearchKWArgs
        public Dictionary<string, object> SearchKWArgs {
            get {
                Dictionary<string, object> dict = new();
                dict["top_k"] = TopK;
                // filter 
                Dictionary<string, object> filter = new();
                // folder_idが指定されている場合
                if (FolderId != string.Empty) {
                    filter["folder_id"] = FolderId;
                }
                // content_typeが指定されている場合
                if (ContentType != string.Empty) {
                    filter["content_type"] = ContentType;
                }
                // filterが指定されている場合
                if (filter.Count > 0) {
                    dict["filter"] = filter;
                }

                return dict;
            }
        }

        public string ToJson() {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(this, options);
        }

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = VectorDBItem.ToDict();
            dict["search_kwargs"] = SearchKWArgs;
            return dict;

        }

        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorSearchProperty> items) {
            return items.Select(item => item.ToDict()).ToList();
        }

    }
}
