using System.IO;
using LiteDB;
using PythonAILib.Model;
using PythonAILib.Model.Abstract;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.Tag;
using PythonAILib.Model.VectorDB;

namespace ImageChat.Model {
    internal class ImageChatDataFactory : IDataFactory {

        public const string IMAGECHAT_ITEM_COLLECTION_NAME = "imagechat_item";
        public const string IMAGECHAT_FILE_COLLECTION_NAME = "imagechat_file";

        public const string PromptTemplateCollectionName = "PromptTemplate";
        public const string RAGSourceItemCollectionName = "RAGSourceItem";
        public const string VectorDBItemCollectionName = "VectorDBItem";


        private static ImageChatDataFactory? _instance;
        public static ImageChatDataFactory Instance {
            get {
                if (_instance == null) {
                    _instance = new ImageChatDataFactory();
                }
                return _instance;
            }
        }

        private LiteDatabase? db;


        public  LiteDatabase GetDatabase() {
            if (db == null) {
                try {
                    /// AppDataフォルダーパスを取得
                    string appDataPath = ImageChatConfig.Instance.AppDataFolder;
                    // データベースファイルのパスを作成
                    string dbPath = Path.Combine(appDataPath, "imagechat.db");
                    db = new LiteDatabase(dbPath);


                } catch (Exception e) {
                    throw new Exception("データベースのオープンに失敗しました。" + e.Message);
                }
            }
            return db;
        }




        //-- ContentItemBase
        public ContentItemBase? GetItem(ContentItemBase item) {
            if (item is not ImageChatContentItem) {
                // Incorrect argument type error
                throw new Exception("Incorrect argument type.");
            }
            ImageChatContentItem imageChatContentItem = (ImageChatContentItem)item;
            var collection = GetDatabase().GetCollection<ImageChatContentItem>(IMAGECHAT_ITEM_COLLECTION_NAME);
            var result = collection.FindById(imageChatContentItem.Id);
            return result;
        }

        public void UpsertItem(ContentItemBase item, bool updateModifiedTime = true) {
            if (item is not ImageChatContentItem) {
                // Incorrect argument type error
                throw new Exception("Incorrect argument type.");
            }
            ImageChatContentItem imageChatContentItem = (ImageChatContentItem)item;

            // 更新日時を設定
            if (updateModifiedTime) {
                item.UpdatedAt = DateTime.Now;
            }

            var collection = GetDatabase().GetCollection<ImageChatContentItem>(IMAGECHAT_ITEM_COLLECTION_NAME);
            collection.Upsert(imageChatContentItem);
        }


        public void DeleteItem(ContentItemBase item) {
            if (item is not ImageChatContentItem) {
                // Incorrect argument type error
                throw new Exception("Incorrect argument type.");
            }
            ImageChatContentItem imageChatContentItem = (ImageChatContentItem)item;
            if (imageChatContentItem.Id == null) {
                return;
            }
            var collection = GetDatabase().GetCollection<ImageChatContentItem>(IMAGECHAT_ITEM_COLLECTION_NAME);
            // System.Windows.MessageBox.Show(item.CollectionName);
            collection.Delete(imageChatContentItem.Id);
        }

        // Prompt
        // create
        public PromptItem CreatePromptItem() {
            return new PromptItem();
        }

        public ICollection<PromptItem> GetAllPromptTemplates() {
            ICollection<PromptItem> collation = [];
            var col = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            foreach (var item in col.FindAll()) {
                collation.Add(item);
            }
            return collation;
        }

        public void UpsertPromptTemplate(PromptItem promptItem) {
            var collection = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            collection.Upsert(promptItem);
        }

        public PromptItem GetPromptTemplate(ObjectId id) {
            var collection = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            return collection.FindById(id);
        }

        public PromptItem? GetPromptTemplateByName(string name) {
            var collection = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            return collection.FindOne(x => x.Name == name);
        }

        public PromptItem? GetSystemPromptTemplateByName(string name) {
            var collection = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            return collection.FindOne(x => x.Name == name &&
                (x.PromptTemplateType == PromptItem.PromptTemplateTypeEnum.SystemDefined
                    || x.PromptTemplateType == PromptItem.PromptTemplateTypeEnum.ModifiedSystemDefined)
                    );
        }

        public void DeletePromptTemplate(PromptItem promptItem) {
            var collection = GetDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            collection.Delete(promptItem.Id);
        }

        //----  RAGSourceItem
        public RAGSourceItem CreateRAGSourceItem() {
            return new RAGSourceItem();
        }

        public IEnumerable<RAGSourceItem> GetRAGSourceItems() {
            // RAGSourceItemコレクションから、すべてのアイテムを取得
            var collection = GetDatabase().GetCollection<RAGSourceItem>(RAGSourceItemCollectionName);
            return collection.FindAll();

        }
        //----  RAGSourceItem
        // update
        public void UpsertRAGSourceItem(RAGSourceItem item) {
            var collection = GetDatabase().GetCollection<RAGSourceItem>(RAGSourceItemCollectionName);
            collection.Upsert(item);
        }
        // delete
        public void DeleteRAGSourceItem(RAGSourceItem item) {
            var collection = GetDatabase().GetCollection<RAGSourceItem>(RAGSourceItemCollectionName);
            collection.Delete(item.Id);
        }


        //-- AttachedItems
        public void UpsertAttachedItem(ContentAttachedItem item) {
            var collection = GetDatabase().GetCollection<ContentAttachedItem>(IMAGECHAT_FILE_COLLECTION_NAME);
            collection.Upsert(item);
        }
        public void DeleteAttachedItem(ContentAttachedItem item) {
            var collection = GetDatabase().GetCollection<ContentAttachedItem>(IMAGECHAT_FILE_COLLECTION_NAME);
            collection.Delete(item.Id);

        }
        public ContentAttachedItem? GetAttachedItem(ObjectId id) {
            var collection = GetDatabase().GetCollection<ContentAttachedItem>(IMAGECHAT_FILE_COLLECTION_NAME);
            return collection.FindById(id);
        }




        // -- VectorDBItem
        public VectorDBItem CreateVectorDBItem() {
            return new VectorDBItem();
        }

        public VectorDBItem GetSystemVectorDBItem() {
            throw new NotImplementedException();
        }

        // update
        public void UpsertVectorDBItem(VectorDBItem item) {
            var collection = GetDatabase().GetCollection<VectorDBItem>(VectorDBItemCollectionName);
            collection.Upsert(item);
        }
        // delete
        public void DeleteVectorDBItem(VectorDBItem item) {
            var collection = GetDatabase().GetCollection<VectorDBItem>(VectorDBItemCollectionName);
            collection.Delete(item.Id);
        }
        // get
        public IEnumerable<VectorDBItem> GetVectorDBItems() {
            // VectorDBItemコレクションから、すべてのアイテムを取得
            var collection = GetDatabase().GetCollection<VectorDBItem>(VectorDBItemCollectionName);
            return collection.FindAll();
        }

        public IEnumerable<TagItem> GetTagList() {
            throw new NotImplementedException();
        }

        public void DeleteTag(TagItem tag) {
            throw new NotImplementedException();
        }

        public void UpsertTag(TagItem tag) {
            throw new NotImplementedException();
        }

        public IEnumerable<TagItem> FilterTag(string tag, bool exclude) {
            throw new NotImplementedException();
        }


    }
}
