using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF.Request;
using PythonAILib.Common;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace LibPythonAI.Model.VectorDB {
    public class VectorSearchProperty {


        public string? VectorDBItemName { init; get; } = null;

        public string? Model { get; set; } = null;

        // InputText
        public string? InputText { get; set; } = null;

        //TopK
        public int TopK { get; set; } = 5; // デフォルト値

        // FolderId
        public string? FolderId { get; set; } = null;

        public string ContentType { init; get; } = string.Empty;

        // SearchKWargs
        private Dictionary<string, object> GetSearchKwargs() {
            Dictionary<string, object> dict = new() {
                ["k"] = TopK
            };
            // filter 
            Dictionary<string, object> filter = new();
            // folder_idが指定されている場合
            if (FolderId != null) {
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


        public string DisplayText {
            get {
                VectorDBItem? item = VectorDBItem.GetItemByName(VectorDBItemName);
                if (item == null) {
                    return "";
                }
                if (string.IsNullOrEmpty(item.CollectionName)) {
                    return item.Name;
                }
                if (FolderId == null) {
                    return item.Name;
                }
                ContentFolderWrapper? folder = ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(FolderId);
                if (folder == null) {
                    return item.Name;
                }
                return $"{item.Name}:{folder.ContentFolderPath}";
            }
        }


        public static List<VectorSearchProperty>? FromListJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Deserialize<List<VectorSearchProperty>>(json, options);
        }


        public Dictionary<string, object> ToDict() {
            if (string.IsNullOrEmpty(VectorDBItemName)) {
                throw new Exception(PythonAILibStringResources.Instance.PropertyNotSet("VectorDBItemName"));
            }
            if (string.IsNullOrEmpty(Model)) {
                throw new Exception(PythonAILibStringResources.Instance.PropertyNotSet("Model"));
            }

            Dictionary<string, object> dict = [];
            dict["Name"] = VectorDBItemName;
            dict["model"] = Model;
            var search_kwargs = GetSearchKwargs();
            if (search_kwargs.Count > 0) {
                dict["SearchKwargs"] = search_kwargs;
            }
            if (!string.IsNullOrEmpty(InputText)) {
                dict["input_text"] = InputText;
            }
            return dict;
        }

        // CreateEntriesDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorSearchProperty> items) {
            return items.Select(item => item.ToDict()).ToList();
        }

        // ToListJson
        public static string ToListJson(IEnumerable<VectorSearchProperty> items) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(items, options);
        }


        // ベクトル検索を実行する
        public async Task<List<VectorEmbedding>> VectorSearch(string query) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            // ChatRequestContextを作成
            this.InputText = query;
            ChatRequestContext chatRequestContext = new() {
                VectorSearchRequests = [ this],
                OpenAIProperties = openAIProperties,
                UseVectorDB = true,
            };

            // ベクトル検索を実行
            List<VectorEmbedding> results = await PythonExecutor.PythonAIFunctions.VectorSearchAsync(chatRequestContext, query);
            return results;
        }


    }
}
