using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace LibPythonAI.Model.VectorDB {
    public class VectorDBProperty {


        public string?  VectorDBItemName  { init; get; } = null;

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

        public string ToJson() {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(this, options);
        }

        public static string ToListJson(List<VectorDBProperty> items) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(items, options);
        }

        public static VectorDBProperty? FromJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Deserialize<VectorDBProperty>(json, options);
        }

        public static List<VectorDBProperty>? FromListJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Deserialize<List<VectorDBProperty>>(json, options);
        }

        public static VectorDBProperty? FromDict(Dictionary<string, dynamic?> dict) {
            VectorDBProperty item = new() {
                VectorDBItemName = dict["VectorDBItemName"]?.ToString(),
                TopK = Int32.Parse(dict["TopK"]() ?? "5"),
                FolderId = dict["FolderId"](),
                ContentType = dict["ContentType"]() ?? "",
            };
            return item;
        }

        public Dictionary<string, object> ToDict() {
            VectorDBItem? item = VectorDBItem.GetItemByName(VectorDBItemName);
            if (item == null) {
                return new Dictionary<string, object>();
            }
            Dictionary<string, object> dict = item.ToDict();
            var search_kwargs = GetSearchKwargs();
            if (search_kwargs.Count > 0) {
                dict["SearchKwargs"] = search_kwargs;
            }
            return dict;
        }

        // CreateEntriesDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorDBProperty> items) {
            return items.Select(item => item.ToDict()).ToList();
        }


        // ベクトル検索を実行する
        public async Task<List<VectorDBEmbedding>> VectorSearch(string query) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = [this],
                OpenAIProperties = openAIProperties,
            };

            // ベクトル検索を実行
            List<VectorDBEmbedding> results = await PythonExecutor.PythonAIFunctions.VectorSearchAsync(chatRequestContext, query);
            return results;
        }


    }
}
