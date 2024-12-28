using PythonAILib.Model.Content;

namespace PythonAILib.Model.AutoProcess {
    // 自動処理の引数用のクラス
    public class AutoProcessItem {

        public ContentItem ContentItem { get; set; }
        public ContentFolder? DestinationFolder { get; set; }

        public AutoProcessItem(ContentItem clipboardItem, ContentFolder? destinationFolder) {
            ContentItem = clipboardItem;
            DestinationFolder = destinationFolder;
        }
    }

}
