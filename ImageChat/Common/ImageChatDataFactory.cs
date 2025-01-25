using PythonAILib.Common;

namespace ImageChat.Common {
    internal class ImageChatDataFactory : PythonAILibDataFactory {


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
