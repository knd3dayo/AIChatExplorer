using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;

namespace PythonAILib.Model.VectorDB {
    public class VectorSearchProperty {

        public VectorSearchProperty() { }

        public VectorSearchProperty(VectorDBItem vectorDBItem) {
            VectorDBItemId = vectorDBItem.Id;
            TopK = vectorDBItem.DefaultSearchResultLimit;
        }

        public LiteDB.ObjectId VectorDBItemId { get; set; } = LiteDB.ObjectId.NewObjectId();

        //TopK
        public int TopK { get; set; }

        // FolderId
        public LiteDB.ObjectId FolderId { get; set; } = LiteDB.ObjectId.Empty;

        // ContentType
        public string ContentType { get; set; } = string.Empty;

        // SearchKWArgs
        private Dictionary<string, object> GetSearchKWArgs() {
            Dictionary<string, object> dict = new() {
                ["k"] = TopK
            };
            // filter 
            Dictionary<string, object> filter = new();
            // folder_idが指定されている場合
            if (FolderId != LiteDB.ObjectId.Empty) {
                filter["folder_id"] = FolderId.ToString();
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
        // VectorDBItem
        public VectorDBItem? GetVectorDBItem() {
            return VectorDBItem.GetItemById(VectorDBItemId);
        }

        [LiteDB.BsonIgnore]
        public string DisplayText {
            get {
                VectorDBItem? item = GetVectorDBItem();
                if (item == null) {
                    return "";
                }
                if (string.IsNullOrEmpty(item.CollectionName)) {
                    return item.Name;
                }
                if (FolderId == LiteDB.ObjectId.Empty) {
                    return item.Name;
                }
                // ContentFolderを取得
                var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
                ContentFolder? folder = collection.FindById(FolderId);
                if (folder == null) {
                    return item.Name;
                }

                return $"{item.Name}:{folder.FolderPath}";
            }
        }

        // Description
        public string GetDescription (){

            VectorDBItem? item = GetVectorDBItem();
            if (item == null) {
                return "";
            }

            string description = PythonExecutor.PythonAIFunctions.GetCatalogDescription(item.CatalogDBURL, item.VectorDBURL, item.CollectionName, FolderId.ToString());
            if (string.IsNullOrEmpty(description)) {
                return item.Description;
            }
            return description;
        }

        // UpdateCatalogDescription
        public void UpdateCatalogDescription(string description) {
            VectorDBItem? item = GetVectorDBItem();
            if (item == null) {
                return;
            }
            PythonExecutor.PythonAIFunctions.UpdateCatalogDescription(item.CatalogDBURL, item.VectorDBURL, item.CollectionName, FolderId.ToString(), description);
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
            Dictionary<string, object> dict = VectorDBItem.GetItemById(VectorDBItemId)?.ToDict() ?? [];
            // FolderId
            if (FolderId != LiteDB.ObjectId.Empty) {
                dict["folder_id"] = FolderId.ToString();
            }
            var search_kwargs = GetSearchKWArgs();
            if (search_kwargs.Count > 0) {
                dict["search_kwargs"] = search_kwargs;
            }
            dict["vector_db_description"] = GetDescription();

            return dict;
        }

        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorSearchProperty> items) {
            return items.Select(item => item.ToDict()).ToList();
        }
        public void UpdateIndex(VectorDBEntry contentInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorSearchProperties = [this],
                OpenAIProperties = openAIProperties
            };
            LogWrapper.Info(PythonAILibStringResources.Instance.SaveEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(chatRequestContext, contentInfo);
            LogWrapper.Info(PythonAILibStringResources.Instance.SavedEmbedding);
        }

        public void DeleteIndex(VectorDBEntry contentInfo) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorSearchProperties = [this],
                OpenAIProperties = openAIProperties
            };
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(chatRequestContext, contentInfo);
            LogWrapper.Info(PythonAILibStringResources.Instance.DeletedEmbedding);
        }

    }
}
