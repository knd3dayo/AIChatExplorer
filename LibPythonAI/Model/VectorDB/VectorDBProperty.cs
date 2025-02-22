using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace PythonAILib.Model.VectorDB {
    public class VectorDBProperty {

        public VectorDBProperty() { }

        public VectorDBProperty(VectorDBItem vectorDBItem, LiteDB.ObjectId? folderId ) {
            VectorDBItemId = vectorDBItem.Id;
            FolderId = folderId ?? LiteDB.ObjectId.Empty;
            TopK = vectorDBItem.DefaultSearchResultLimit;
        }

        public LiteDB.ObjectId VectorDBItemId { get; set; } = LiteDB.ObjectId.NewObjectId();

        //TopK
        public int TopK { get; set; }

        // FolderId
        public LiteDB.ObjectId FolderId { get; private set; } = LiteDB.ObjectId.Empty;

        // ContentType
        public string ContentType { get; set; } = string.Empty;

        // VectorDBEntries
        public List<VectorMetadata> VectorMetadataList { get; set; } = [];

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

                return $"{item.Name}:{folder.ContentFolderPath}";
            }
        }

        // Description
        public string GetDescription() {

            VectorDBItem? item = GetVectorDBItem();
            if (item == null) {
                return "";
            }

            string description = PythonExecutor.PythonAIFunctions.GetVectorDBDescription(item.CatalogDBURL, item.VectorDBURL, item.CollectionName, FolderId.ToString());
            if (string.IsNullOrEmpty(description)) {
                return item.Description;
            }
            return description;
        }

        // UpdateVectorDBDescription
        public void UpdateCatalogDescription(string description) {
            VectorDBItem? item = GetVectorDBItem();
            if (item == null) {
                return;
            }
            PythonExecutor.PythonAIFunctions.UpdateVectorDBDescription(item.CatalogDBURL, item.VectorDBURL, item.CollectionName, FolderId.ToString(), description);
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
            // vector_db_entriesを追加
            dict["vector_db_metadata_list"] = VectorMetadataList;

            return dict;
        }


        // CreateEntriesDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorDBProperty> items) {
            return items.Select(item => item.ToDict()).ToList();
        }
        public void UpdateEmbeddings() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = [this],
                OpenAIProperties = openAIProperties
            };
            LogWrapper.Info(PythonAILibStringResources.Instance.SaveEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateEmbeddings(chatRequestContext);
            LogWrapper.Info(PythonAILibStringResources.Instance.SavedEmbedding);
        }

        public void DeleteEmbeddings() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = [this],
                OpenAIProperties = openAIProperties
            };
            LogWrapper.Info(PythonAILibStringResources.Instance.DeleteEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateEmbeddings(chatRequestContext);
            LogWrapper.Info(PythonAILibStringResources.Instance.DeletedEmbedding);
        }

        // ベクトル検索を実行する
        public List<VectorMetadata> VectorSearch(string query) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = [this],
                OpenAIProperties = openAIProperties
            };

            // ベクトル検索を実行
            List<VectorMetadata> results = PythonExecutor.PythonAIFunctions.VectorSearch(chatRequestContext, query);
            return results;
        }

        // フォルダに設定されたVectorDBのコレクションを削除
        public void DeleteVectorDBCollection() {
            Task.Run(() => {

                PythonAILibManager libManager = PythonAILibManager.Instance;
                OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

                ChatRequestContext chatRequestContext = new() {
                    OpenAIProperties = openAIProperties,
                    VectorDBProperties = [this]
                };
                PythonExecutor.PythonAIFunctions.DeleteVectorDBCollection(chatRequestContext);

            });
        }
        // フォルダに設定されたVectorDBのコレクションをアップデート
        public void UpdateVectorDBCollection(string description) {
            Task.Run(() => {
                this.UpdateCatalogDescription(description);
            });
        }

        // フォルダに設定されたVectorDBのインデックスを更新
        public void RefreshVectorDBCollection(string description, Action afterRefresh) {
            // ベクトルを全削除
            DeleteVectorDBCollection();
            // コレクション/カタログの更新
            UpdateVectorDBCollection(description);

            afterRefresh();
        }




        // Equals
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            VectorDBProperty other = (VectorDBProperty)obj;
            return VectorDBItemId == other.VectorDBItemId && FolderId == other.FolderId;
        }
        public override int GetHashCode() {
            return VectorDBItemId.GetHashCode() ^ FolderId.GetHashCode();
        }
    }
}
