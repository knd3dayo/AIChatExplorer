using LiteDB;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.Script;
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


        private void Upgrade(ILiteDatabase db) {

            #region バージョンアップ後の暫定処理
            // WpfAppCommon.Model.ClipboardItemをClipboardApp.Model.ClipboardItemに変更
            /*** クラスの場所変更時の暫定的な処理
            var collection = db.GetCollection(CONTENT_ITEM_COLLECTION_NAME);
            foreach (var item in collection.FindAll()) {
                string typeString = item["_type"];
                if (typeString == "WpfAppCommon.Model.ClipboardApp.ClipboardItem, WpfAppCommon") {
                    item["_type"] = "ClipboardApp.Model.ClipboardItem, ClipboardApp";
                    collection.Update(item);
                }
                var fileItems = item["ClipboardItemFiles"];
                if (fileItems != null && fileItems.AsArray != null) {
                    foreach (var fileItem in fileItems.AsArray) {
                        string? fileTypeString = fileItem["_type"];
                        if (fileTypeString == null) {
                            continue;
                        }
                        if (fileTypeString.Contains("WpfAppCommon.Model.ClipboardApp.ClipboardItemFile, WpfAppCommon")) {
                            string newTypeString = fileTypeString.Replace("WpfAppCommon.Model.ClipboardApp.ClipboardItemFile, WpfAppCommon", "ClipboardApp.Model.ClipboardItemFile, ClipboardApp");
                            fileItem["_type"] = newTypeString;
                            collection.Update(item);
                        }
                    }
                }

            }
            // ClipboardItemFile
            var fileCollection = db.GetCollection(CONTENT_ATTACHED_ITEM_COLLECTION_NAME);
            foreach (var item in fileCollection.FindAll()) {
                string typeString = item["_type"];
                if (typeString == "WpfAppCommon.Model.ClipboardApp.ClipboardItemFile, WpfAppCommon") {
                    item["_type"] = "ClipboardApp.Model.ClipboardItemFile, ClipboardApp";
                    fileCollection.Update(item);
                }
            }
            ***/
            /***
            var collection = db.GetCollection(VectorDBItemCollectionName);
            foreach (var item in collection.FindAll()) {
                string typeString = item["_type"];
                if (typeString == "WpfAppCommon.Model.ClipboardAppVectorDBItem, WpfAppCommon") {
                    item["_type"] = "PythonAILib.Model.VectorDB.VectorDBItem, PythonAILib";
                    collection.Update(item);
                }
            }
            collection = db.GetCollection(RAGSourceItemCollectionName);
            foreach (var item in collection.FindAll()) {
                var dbItem = item["VectorDBItem"];
                if (dbItem != null) {
                    string? fileTypeString = dbItem["_type"];
                    if (fileTypeString == null) {
                        continue;
                    }
                    if (fileTypeString.Contains("WpfAppCommon.Model.ClipboardAppVectorDBItem, WpfAppCommon")) {
                        string newTypeString = fileTypeString.Replace("WpfAppCommon.Model.ClipboardAppVectorDBItem, WpfAppCommon", "PythonAILib.Model.VectorDB.VectorDBItem, PythonAILib");
                        dbItem["_type"] = newTypeString;
                        collection.Update(item);
                    }
                }

            }
            // PromptItemのChatTypeをRAGからOpenAIRAGに変更
            var collection = db.GetCollection(PromptTemplateCollectionName);
            foreach (var item in collection.FindAll()) {
                string chatTypeString = item["ChatType"];
                if (chatTypeString == "RAG") {
                    item["ChatType"] = "OpenAIRAG";
                    collection.Update(item);
                }
            }
            // PromptItemのPromptResultTypeをTitleTextContentからTextContentに変更
            collection = db.GetCollection(PromptTemplateCollectionName);
            foreach (var item in collection.FindAll()) {
                string promptResultTypeString = item["PromptResultType"];
                if (promptResultTypeString == "TitleTextContent") {
                    item["PromptResultType"] = "TextContent";
                    collection.Update(item);
                }
            }
            // PromptItemのPromptResultTypeをListからListContentに変更
            collection = db.GetCollection(PromptTemplateCollectionName);
            foreach (var item in collection.FindAll()) {
                string promptResultTypeString = item["PromptResultType"];
                if (promptResultTypeString == "List") {
                    item["PromptResultType"] = "ListContent";
                    collection.Update(item);
                }
            }
            // PromptItemのPromptResultTypeをTextからTextContentに変更
            collection = db.GetCollection(PromptTemplateCollectionName);
            foreach (var item in collection.FindAll()) {
                string promptResultTypeString = item["PromptResultType"];
                if (promptResultTypeString == "Text") {
                    item["PromptResultType"] = "TextContent";
                    collection.Update(item);
                }
            }

            // ClipboardItemにTaskItemがある場合は削除
            collection = db.GetCollection(CONTENT_ITEM_COLLECTION_NAME);
            foreach (var item in collection.FindAll()) {
                    // taskItemを削除
                    item.Remove("Tasks");
                    collection.Update(item);
                foreach (var col in item) {
                    if (col.Value.ToString().Contains("TaskItem")) {
                        item.Remove(col.Key);
                        collection.Update(item);
                    }
                }
            }
            // ClipboardItemにSummaryがある場合は削除
            collection = db.GetCollection(CONTENT_ITEM_COLLECTION_NAME);
            foreach (var item in collection.FindAll()) {
                // taskItemを削除
                item.Remove("Summary");
                collection.Update(item);
            }
            // ClipboardItemにBackgroundInfoがある場合は削除
            collection = db.GetCollection(CONTENT_ITEM_COLLECTION_NAME);
            foreach (var item in collection.FindAll()) {
                // taskItemを削除
                item.Remove("BackgroundInfo");
                collection.Update(item);
            }
            // PromptItemのPromptResultTypeをComplexContentからTableContentに変更
            var collection = db.GetCollection(PromptTemplateCollectionName);
            collection = db.GetCollection(PromptTemplateCollectionName);
            foreach (var item in collection.FindAll()) {
                string promptResultTypeString = item["PromptResultType"];
                if (promptResultTypeString == "ComplexContent") {
                    item["PromptResultType"] = "TableContent";
                    collection.Update(item);
                }
            }
        ***/
            var collection = db.GetCollection(CONTENT_FOLDERS_COLLECTION_NAME);
            foreach (var row in collection.FindAll()) {
                var items = row["ReferenceVectorDBItems"];
                if (items != null && items.AsArray != null) {
                    foreach (var item in items.AsArray) {
                        string? target = item["_type"];
                        if (target == null) {
                            continue;
                        }
                        if (target.Contains("ClipboardApp.Model.ClipboardAppVectorDBItem, ClipboardApp")) {
                            string newTypeString = target.Replace("ClipboardApp.Model.ClipboardAppVectorDBItem, ClipboardApp", "PythonAILib.Model.VectorDB.VectorDBItem, PythonAILib");
                            item["_type"] = newTypeString;
                            collection.Update(row);
                        }
                    }
                }
            }

            var itemsCollection = db.GetCollection(CONTENT_ITEM_COLLECTION_NAME);
            foreach (var row in itemsCollection.FindAll()) {
                var items = row["ReferenceVectorDBItems"];
                if (items != null && items.AsArray != null) {
                    foreach (var item in items.AsArray) {
                        string? target = item["_type"];
                        if (target == null) {
                            continue;
                        }
                        if (target.Contains("ClipboardApp.Model.ClipboardAppVectorDBItem, ClipboardApp")) {
                            string newTypeString = target.Replace("ClipboardApp.Model.ClipboardAppVectorDBItem, ClipboardApp", "PythonAILib.Model.VectorDB.VectorDBItem, PythonAILib");
                            item["_type"] = newTypeString;
                            itemsCollection.Update(row);
                        }
                    }
                }
            }

            #endregion
        }


    }
}
