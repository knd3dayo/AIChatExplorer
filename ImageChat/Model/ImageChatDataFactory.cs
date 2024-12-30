using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Content;

namespace ImageChat.Model
{
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

    }
}
