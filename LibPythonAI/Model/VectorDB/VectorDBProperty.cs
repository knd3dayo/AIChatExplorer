using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace LibPythonAI.Model.VectorDB {
    public class VectorDBProperty {

        public VectorDBPropertyEntity Entity { get; set; }
        public VectorDBProperty(VectorDBPropertyEntity entity) {
            Entity = entity;
        }

        public VectorDBItem? VectorDBItem  {
            get {
                VectorDBItem? item = VectorDBItem.GetItemById(Entity.VectorDBItemId);
                return item;
            }
        }
        public string Id { get => Entity.Id; }

        //TopK
        public int TopK {
            get => Entity.TopK;
            set => Entity.TopK = value;
        }

        // FolderId
        public ContentFolderWrapper? Folder {
            get {
                ContentFolderWrapper? folder = ContentFolderWrapper.GetFolderById(Entity.FolderId);
                return folder;
            }
        }
        // ContentType
        public string ContentType {
            get => Entity.ContentType;
            set => Entity.ContentType = value;
        }

        // VectorDBEntries
        public List<VectorMetadata> VectorMetadataList { get; set; } = new();

        // SearchKWArgs
        private Dictionary<string, object> GetSearchKWArgs() {
            Dictionary<string, object> dict = new() {
                ["k"] = TopK
            };
            // filter 
            Dictionary<string, object> filter = new();
            // folder_idが指定されている場合
            if (Folder != null) {
                filter["folder_id"] = Folder.Id.ToString();
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
            return VectorDBItem;
        }

        public string DisplayText {
            get {
                VectorDBItem? item = GetVectorDBItem();
                if (item == null) {
                    return "";
                }
                if (string.IsNullOrEmpty(item.CollectionName)) {
                    return item.Name;
                }
                if (Folder == null) {
                    return item.Name;
                }

                return $"{item.Name}:{Folder.ContentFolderPath}";
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
            Dictionary<string, object> dict = VectorDBItem?.ToDict() ?? [];
            // FolderId
            if (Folder != null) {
                dict["FolderId"] = Folder.Id.ToString();
            }
            var search_kwargs = GetSearchKWArgs();
            if (search_kwargs.Count > 0) {
                dict["SearchKWArgs"] = search_kwargs;
            }
            dict["Description"] = Folder?.Description ?? "";
            // vector_db_entriesを追加
            dict["VectorMetadataList"] = VectorMetadataList;

            return dict;
        }


        // CreateEntriesDictList
        public static List<Dictionary<string, object>> ToDictList(IEnumerable<VectorDBProperty> items) {
            return items.Select(item => item.ToDict()).ToList();
        }

        public static void UpdateEmbeddings(List<VectorDBProperty> items) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = items,
                OpenAIProperties = openAIProperties
            };
            LogWrapper.Info(PythonAILibStringResources.Instance.SaveEmbedding);
            PythonExecutor.PythonAIFunctions.UpdateEmbeddings(chatRequestContext);
            LogWrapper.Info(PythonAILibStringResources.Instance.SavedEmbedding);
        }

        public static void DeleteEmbeddings(List<VectorDBProperty> items) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBProperties = items,
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
        public  void DeleteVectorDBCollection() {
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


        // Equals
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            VectorDBProperty other = (VectorDBProperty)obj;
            return VectorDBItem == other.VectorDBItem && Folder == other.Folder;
        }
        public override int GetHashCode() {
            if (VectorDBItem == null || Folder == null) {
                return 0;
            }

            return VectorDBItem.GetHashCode() ^ Folder.GetHashCode();
        }
    }
}
