using System.IO;
using LiteDB;
using PythonAILib.Model.Abstract;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.Tag;
using PythonAILib.Model.VectorDB;

namespace ImageChat.Model {
    internal class ImageChatDataFactory : PythonAILibDataFactory {

        public const string IMAGECHAT_ITEM_COLLECTION_NAME = "imagechat_item";
        public const string IMAGECHAT_FILE_COLLECTION_NAME = "imagechat_file";

        private static ImageChatDataFactory? _instance;
        public static ImageChatDataFactory Instance {
            get {
                if (_instance == null) {
                    _instance = new ImageChatDataFactory();
                }
                return _instance;
            }
        }


        //-- ContentItem
        public override ContentItem? GetItem(ContentItem item) {
            if (item is not ImageChatContentItem) {
                // Incorrect argument type error
                throw new Exception("Incorrect argument type.");
            }
            ImageChatContentItem imageChatContentItem = (ImageChatContentItem)item;
            var collection = GetDatabase().GetCollection<ImageChatContentItem>(IMAGECHAT_ITEM_COLLECTION_NAME);
            var result = collection.FindById(imageChatContentItem.Id);
            return result;
        }

        public override void UpsertItem(ContentItem item, bool updateModifiedTime = true) {
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


        public override void DeleteItem(ContentItem item) {
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

        public override void UpsertAttachedItem(ContentAttachedItem item) {

            var collection = GetDatabase().GetCollection<ContentAttachedItem>(CONTENT_ATTACHED_ITEM_COLLECTION_NAME);
            collection.Upsert(item);
        }
        public override void DeleteAttachedItem(ContentAttachedItem item) {
            if (item.Id == null) {
                return;
            }
            var collection = GetDatabase().GetCollection<ContentAttachedItem>(CONTENT_ATTACHED_ITEM_COLLECTION_NAME);
            collection.Delete(item.Id);
        }
        public override ContentAttachedItem? GetAttachedItem(ObjectId id) {
            var collection = GetDatabase().GetCollection<ContentAttachedItem>(CONTENT_ATTACHED_ITEM_COLLECTION_NAME);
            var item = collection.FindById(id);
            return item;
        }




    }
}
