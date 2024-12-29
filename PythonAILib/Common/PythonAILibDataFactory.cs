using LiteDB;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.Script;
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

        // AutoGenTools
        public const string AutoGenToolCollectionName = "AutoGenTool";

        // AutoGenAgents
        public const string AutoGenAgentCollectionName = "AutoGenAgent";

        // AutoGenGroupChat
        public const string AutoGenGroupChatCollectionName = "AutoGenGroupChat";

        // AutoGenNormalChatCollectionName
        public const string AutoGenNormalChatCollectionName = "AutoGenNormalChat";
        // AutoGenNestedChatCollectionName
        public const string AutoGenNestedChatCollectionName = "AutoGenNestedChat";

        private LiteDatabase? db;

        public LiteDatabase GetDatabase() {
            if (db == null) {
                try {
                    PythonAILibManager libManager = PythonAILibManager.Instance;

                    db = new LiteDatabase(libManager.ConfigParams.GetDBPath());

                    // バージョンアップ
                    Upgrade(db);

                } catch (Exception e) {
                    throw new Exception("データベースのオープンに失敗しました。" + e.Message);
                }
            }
            return db;
        }

        //-- ContentItem
        public ILiteCollection<T> GetItemCollection<T>() where T : ContentItem {
            var collection = GetDatabase().GetCollection<T>(CONTENT_ITEM_COLLECTION_NAME);
            return collection;
        }


        //-- ContentFolder
        public ILiteCollection<T> GetFolderCollection<T>() where T : ContentFolder {
            var collection = GetDatabase().GetCollection<T>(CONTENT_FOLDERS_COLLECTION_NAME);
            return collection;
        }
        // --
        public ILiteCollection<T> GetShortCutFolderCollection<T>() where T : ContentFolder {
            var collection = GetDatabase().GetCollection<T>(SHORTCUT_FOLDERS_COLLECTION_NAME);
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

        // --- ScriptItem関連 ----------------------------------------------
        public ILiteCollection<T> GetScriptCollection<T>() where T : ScriptItem {
            var collection = GetDatabase().GetCollection<T>(SCRIPT_COLLECTION_NAME);
            return collection;
        }

        //--- Statistics
        public ILiteCollection<T> GetStatisticsCollection<T>() where T : MainStatistics {
            var collection = GetDatabase().GetCollection<T>(StatisticsCollectionName);
            return collection;
        }
        // --- AutoGenTools
        public ILiteCollection<T> GetAutoGenToolCollection<T>() where T : AutoGenTool {
            var collection = GetDatabase().GetCollection<T>(AutoGenToolCollectionName);
            return collection;
        }
        // --- AutoGenAgents
        public ILiteCollection<T> GetAutoGenAgentCollection<T>() where T : AutoGenAgent {
            var collection = GetDatabase().GetCollection<T>(AutoGenAgentCollectionName);
            return collection;
        }
        // --- AutoGentGroupChat
        public ILiteCollection<T> GetAutoGenGroupChatCollection<T>() where T : AutoGenGroupChat {
            var collection = GetDatabase().GetCollection<T>(AutoGenGroupChatCollectionName);
            return collection;
        }

        // --- AutoGenNormalChat
        public ILiteCollection<T> GetAutoGenNormalChatCollection<T>() where T : AutoGenNormalChat {
            var collection = GetDatabase().GetCollection<T>(AutoGenNormalChatCollectionName);
            return collection;
        }

        // --- AutoGenNestedChat
        public ILiteCollection<T> GetAutoGenNestedChatCollection<T>() where T : AutoGenNestedChat {
            var collection = GetDatabase().GetCollection<T>(AutoGenNestedChatCollectionName);
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
            var collection = db.GetCollection(CONTENT_FOLDERS_COLLECTION_NAME);
            foreach (var item in collection.FindAll()) {
                string typeString = item["_type"];
                if (typeString == "ClipboardApp.Model.Search.SearchFolder, ClipboardApp") {
                    item["_type"] = "ClipboardApp.Model.Folder.SearchFolder, ClipboardApp";
                    collection.Update(item);
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

                #endregion
            }
        }


    }
}
