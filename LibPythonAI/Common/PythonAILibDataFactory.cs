using LiteDB;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.Search;
using PythonAILib.Model.Statistics;
using PythonAILib.Model.Tag;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Common {
    public class PythonAILibDataFactory : IDataFactory {

        public const string CHAT_SESSION_COLLECTION_NAME = "chat_session";
        public const string CONTENT_ITEM_COLLECTION_NAME = "clipboard_item";
        public const string CONTENT_ATTACHED_ITEM_COLLECTION_NAME = "clipboard_file";

        public const string CONTENT_FOLDERS_COLLECTION_NAME = "folders";
        public const string SHORTCUT_FOLDERS_COLLECTION_NAME = "shortcut_folders";
        public const string ROOT_FOLDERS_COLLECTION_NAME = "root_folders";

        public const string TAG_COLLECTION_NAME = "tags";
        public const string SCRIPT_COLLECTION_NAME = "scripts";
        public const string SEARCH_CONDITION_RULES_COLLECTION_NAME = "search_condition_rules";

        public const string AUTO_PROCESS_RULES_COLLECTION_NAME = "auto_process_rules";
        public const string SEARCH_CONDITION_APPLIED_CONDITION_NAME = "applied_globally";

        public const string PromptTemplateCollectionName = "PromptTemplate";

        // ClipboardRAGSourceItem
        public const string RAGSourceItemCollectionName = "RAGSourceItem";
        // VectorDBItem
        public const string VectorDBItemCollectionName = "VectorDBItem";

        // Statistics
        public const string StatisticsCollectionName = "Statistics";

        private LiteDatabase? db;

        public LiteDatabase GetDatabase() {
            if (db == null) {
                try {
                    PythonAILibManager libManager = PythonAILibManager.Instance;

                    db = new LiteDatabase(libManager.ConfigParams.GetDBPath());

                    // バージョンアップ
                    Upgrade(db);

                } catch (Exception e) {
                    throw new Exception("Failed to open the database." + e.Message);
                }
            }
            return db;
        }

        //-- Entity
        public ILiteCollection<T> GetItemCollection<T>() where T : ContentItem {
            var collection = GetDatabase().GetCollection<T>(CONTENT_ITEM_COLLECTION_NAME);
            return collection;
        }


        //-- ContentFolder
        public ILiteCollection<T> GetFolderCollection<T>() where T : ContentFolder {
            var collection = GetDatabase().GetCollection<T>(CONTENT_FOLDERS_COLLECTION_NAME);
            return collection;
        }

        // -- PromptItem
        public ILiteCollection<T> GetPromptCollection<T>() where T : PromptItem {
            var collection = GetDatabase().GetCollection<T>(PromptTemplateCollectionName);
            return collection;
        }

        //----  RAGSourceItem
        public ILiteCollection<T> GetRAGSourceCollection<T>() where T : RAGSourceItem {
            var collection = GetDatabase().GetCollection<T>(RAGSourceItemCollectionName);
            return collection;
        }
        //--- -  VectorDBItem
        public ILiteCollection<T> GetVectorDBCollection<T>() where T : VectorDBItem {
            var collection = GetDatabase().GetCollection<T>(VectorDBItemCollectionName);
            return collection;
        }


        // --- TagItem関連 ----------------------------------------------
        public ILiteCollection<T> GetTagCollection<T>() where T : TagItem {
            var collection = GetDatabase().GetCollection<T>(TAG_COLLECTION_NAME);
            return collection;
        }

        //--- Statistics
        public ILiteCollection<T> GetStatisticsCollection<T>() where T : MainStatistics {
            var collection = GetDatabase().GetCollection<T>(StatisticsCollectionName);
            return collection;
        }
        public ILiteCollection<SearchRule> GetSearchRuleCollection() {
            return GetDatabase().GetCollection<SearchRule>(SEARCH_CONDITION_RULES_COLLECTION_NAME);
        }


        //---- 自動処理関連 ----------------------------------------------
        public ILiteCollection<AutoProcessRule> GetAutoProcessRuleCollection() {
            return GetDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
        }

        private void Upgrade(ILiteDatabase db) {

            #region バージョンアップ後の暫定処理
            var items_collection = db.GetCollection(CONTENT_ITEM_COLLECTION_NAME);
            foreach (var item in items_collection.FindAll()) {
                // ClipboardApp.item.ClipboardItem, ClipboardApp -> ClipboardApp.Model.Item.ClipboardItem, ClipboardApp
                string typeString = item["_type"];
                if (typeString == "ClipboardApp.Model.Item.ClipboardItem, ClipboardApp") {
                    item["_type"] = null;
                    items_collection.Update(item);
                }
            }

            /**

            var collection = db.GetCollection(CONTENT_FOLDERS_COLLECTION_NAME);
            foreach (var item in collection.FindAll()) {
                string typeString = item["_type"];
                if (typeString == "ClipboardApp.Model.Search.SearchFolder, ClipboardApp") {
                    item["_type"] = "ClipboardApp.Model.Folder.SearchFolder, ClipboardApp";
                    collection.Update(item);
                }
                var folderTypeString = item["FolderType"];
                if (folderTypeString.GetType() == typeof(string)) {
                    // Normal
                    if (folderTypeString == "Normal") {
                        item["FolderType"] = new BsonDocument { { "Name", "Normal" } };
                        collection.Update(item);
                    }
                    if (folderTypeString == "Search") {
                        item["FolderType"] = new BsonDocument { { "Name", "Search" } };
                        collection.Update(item);
                    }
                    // Chat
                    if (folderTypeString == "Chat") {
                        item["FolderType"] = new BsonDocument { { "Name", "Chat" } };
                        collection.Update(item);
                    }
                    // ImageCheck
                    if (folderTypeString == "ImageCheck") {
                        item["FolderType"] = new BsonDocument { { "Name", "ImageCheck" } };
                        collection.Update(item);
                    }
                    // FileSystem
                    if (folderTypeString == "FileSystem") {
                        item["FolderType"] = new BsonDocument { { "Name", "FileSystem" } };
                        collection.Update(item);
                    }
                    // ShortCut
                    if (folderTypeString == "ShortCut") {
                        item["FolderType"] = new BsonDocument { { "Name", "ShortCut" } };
                        collection.Update(item);
                    }
                    // Outlook
                    if (folderTypeString == "Outlook") {
                        item["FolderType"] = new BsonDocument { { "Name", "Outlook" } };
                        collection.Update(item);
                    }
                }
            }

            // SearchRule
            var searchRuleCollection = db.GetCollection(SEARCH_CONDITION_RULES_COLLECTION_NAME);
            foreach (var item in searchRuleCollection.FindAll()) {
                string typeString = item["_type"];
                if (typeString == "ClipboardApp.Model.Search.SearchFolder, ClipboardApp") {
                    item["_type"] = "ClipboardApp.Model.Folder.SearchFolder, ClipboardApp";
                    collection.Update(item);
                }
                var searchFolderString = item["SearchFolder"];
                if (searchFolderString != null) {
                    var arrayVal = searchFolderString.AsDocument;
                    if (arrayVal != null) {
                        var searchFolderTypeString = arrayVal["_type"];
                        if (searchFolderTypeString == "ClipboardApp.Model.Search.SearchFolder, ClipboardApp") {
                            arrayVal["_type"] = "ClipboardApp.Model.Folder.SearchFolder, ClipboardApp";
                            item["SearchFolder"] = searchFolderString;
                            searchRuleCollection.Update(item);
                        }
                    }
                }
            **/

            #endregion
        }
    }

}
