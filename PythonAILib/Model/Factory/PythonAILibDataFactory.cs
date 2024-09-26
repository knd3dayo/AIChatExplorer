using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using PythonAILib.Model.Abstract;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resource;
using QAChat;

namespace PythonAILib.Model.Factory {
    public class PythonAILibDataFactory : IDataFactory {

        public const string CHAT_SESSION_COLLECTION_NAME = "chat_session";
        public const string CONTENT_ITEM_COLLECTION_NAME = "clipboard_item";
        public const string CONTENT_ATTACHED_ITEM_COLLECTION_NAME = "clipboard_file";


        public const string PromptTemplateCollectionName = "PromptTemplate";

        // ClipboardRAGSourceItem
        public const string RAGSourceItemCollectionName = "RAGSourceItem";
        // VectorDBItem
        public const string VectorDBItemCollectionName = "VectorDBItem";

        private LiteDatabase? db;

        public LiteDatabase GetDatabase() {
            if (db == null) {
                try {
                    PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);

                    db = new LiteDatabase(libManager.ConfigParams.GetDBPath());

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
                        if (dbItem != null ) {
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

                } catch (Exception e) {
                    throw new Exception("データベースのオープンに失敗しました。" + e.Message);
                }
            }
            return db;
        }



        //-- ContentItemBase

        // ClipboardItemを取得する。
        public ContentItemBase? GetItem(ContentItemBase item) {

            var collection = GetDatabase().GetCollection<ContentItemBase>(CONTENT_ITEM_COLLECTION_NAME);
            var result = collection.FindById(item.Id);
            return result;
        }

        // ClipboardItemをLiteDBに追加または更新する
        public void UpsertItem(ContentItemBase item, bool updateModifiedTime = true) {

            // 更新日時を設定
            if (updateModifiedTime) {
                item.UpdatedAt = DateTime.Now;
            }
            // ファイルがある場合は、追加または更新
            foreach (var file in item.ClipboardItemFiles) {
                UpsertAttachedItem(file);
            }
            var collection = GetDatabase().GetCollection<ContentItemBase>(CONTENT_ITEM_COLLECTION_NAME);
            collection.Upsert(item);
        }


        // アイテムをDBから削除する
        public void DeleteItem(ContentItemBase item) {
            if (item.Id == null) {
                return;
            }
            var collection = GetDatabase().GetCollection<ContentItemBase>(CONTENT_ITEM_COLLECTION_NAME);
            // System.Windows.MessageBox.Show(item.CollectionName);
            collection.Delete(item.Id);
        }

        //-- AttachedItems  
        public void UpsertAttachedItem(ContentAttachedItem item) {

            var collection = GetDatabase().GetCollection<ContentAttachedItem>(CONTENT_ATTACHED_ITEM_COLLECTION_NAME);
            collection.Upsert(item);
        }
        public void DeleteAttachedItem(ContentAttachedItem item) {

            var collection = GetDatabase().GetCollection<ContentAttachedItem>(CONTENT_ATTACHED_ITEM_COLLECTION_NAME);
            collection.Delete(item.Id);
        }
        public ContentAttachedItem? GetAttachedItem(ObjectId id) {
            var collection = GetDatabase().GetCollection<ContentAttachedItem>(CONTENT_ATTACHED_ITEM_COLLECTION_NAME);
            var item = collection.FindById(id);
            return item;
        }


        // Prompt
        // create
        public PromptItem CreatePromptItem() {
            return new PromptItem();
        }

        public void UpsertPromptTemplate(PromptItem item) {
            var db = GetDatabase();

            var col = db.GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.Upsert(item);
        }

        // プロンプトテンプレートを取得する
        public PromptItem GetPromptTemplate(ObjectId objectId) {
            var col = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            return col.FindById(objectId);
        }
        // プロンプトテンプレートを名前で取得する
        public PromptItem? GetPromptTemplateByName(string name) {
            var col = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            return col.FindOne(x => x.Name == name);
        }
        // システム定義のPromptItemを取得する
        public PromptItem? GetSystemPromptTemplateByName(string name) {
            var col = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            var item = col.FindOne(x => x.Name == name);
            if (item != null &&
                (item.PromptTemplateType == PromptItem.PromptTemplateTypeEnum.SystemDefined
                    || item.PromptTemplateType == PromptItem.PromptTemplateTypeEnum.ModifiedSystemDefined)) {
                return item;
            }
            return null;
        }

        // 引数として渡されたプロンプトテンプレートを削除する
        public void DeletePromptTemplate(PromptItem item) {

            var col = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.Delete(item.Id);
        }

        // プロンプトテンプレートを全て取得する
        public ICollection<PromptItem> GetAllPromptTemplates() {
            ICollection<PromptItem> collation = [];
            var col = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            foreach (var item in col.FindAll()) {
                collation.Add(item);
            }
            return collation;
        }


        //----  RAGSourceItem
        
        public RAGSourceItem CreateRAGSourceItem() {
            return new RAGSourceItem();
        }
        // update
        public void UpsertRAGSourceItem(RAGSourceItem item) {
            // RAGSourceItemコレクションに、itemを追加または更新
            var collection = GetDatabase().GetCollection<RAGSourceItem>(RAGSourceItemCollectionName);
            collection.Upsert(item);

        }
        // delete
        public void DeleteRAGSourceItem(RAGSourceItem item) {
            // RAGSourceItemコレクションから、itemを削除
            var collection = GetDatabase().GetCollection<RAGSourceItem>(RAGSourceItemCollectionName);
            collection.Delete(item.Id);
        }

        // get
        public IEnumerable<RAGSourceItem> GetRAGSourceItems() {
            // RAGSourceItemコレクションから、すべてのアイテムを取得
            var collection = GetDatabase().GetCollection<RAGSourceItem>(RAGSourceItemCollectionName);
            return collection.FindAll();
        }


        //--- -  VectorDBItem
        // update

        public VectorDBItem GetSystemVectorDBItem() {
            // DBからベクトルDBを取得
            // GetItemsメソッドを呼び出して取得
            IEnumerable<VectorDBItem> items = GetVectorDBItems();
            var item = items.FirstOrDefault(item => item.IsSystem && item.Name == VectorDBItem.SystemCommonVectorDBName);

            if (item == null) {
                PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);

                string docDBPath = libManager.ConfigParams.GetSystemVectorDBPath();
                string vectorDBPath = libManager.ConfigParams.GetSystemDocDBPath();
                item = new VectorDBItem() {
                    Id = LiteDB.ObjectId.Empty,
                    Name = VectorDBItem.SystemCommonVectorDBName,
                    Description = PythonAILibStringResources.Instance.GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions,
                    Type = VectorDBTypeEnum.Chroma,
                    VectorDBURL = vectorDBPath,
                    DocStoreURL = $"sqlite:///{docDBPath}",
                    IsUseMultiVectorRetriever = true,
                    IsEnabled = true,
                    IsSystem = true
                };
                item.Save();
            }
            // IsSystemフラグ導入前のバージョンへの対応
            item.IsSystem = true;
            return item;

        }


        // -- VectorDBItem

        public void UpsertVectorDBItem(VectorDBItem item) {

            // VectorDBItemコレクションに、itemを追加または更新
            var collection = GetDatabase().GetCollection<VectorDBItem>(VectorDBItemCollectionName);
            collection.Upsert(item);

        }
        // delete
        public void DeleteVectorDBItem(VectorDBItem item) {
            // VectorDBItemコレクションから、itemを削除
            var collection = GetDatabase().GetCollection<VectorDBItem>(VectorDBItemCollectionName);
            collection.Delete(item.Id);
        }
        // get
        public IEnumerable<VectorDBItem> GetVectorDBItems() {
            // VectorDBItemコレクションから、すべてのアイテムを取得
            var collection = GetDatabase().GetCollection<VectorDBItem>(VectorDBItemCollectionName);
            return collection.FindAll();
        }

        // -- VectorDBItem
        public VectorDBItem CreateVectorDBItem() {
            return new VectorDBItem();
        }





    }
}
