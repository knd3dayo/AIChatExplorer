using PythonAILib.Model.Content;

namespace PythonAILib.Model.AutoProcess {
    // 自動処理の引数用のクラス
    public class AutoProcessItemArgs {

        public ContentItem ContentItem { get; set; }
        public ContentFolder? DestinationFolder { get; set; }

        public AutoProcessItemArgs(ContentItem clipboardItem, ContentFolder? destinationFolder) {
            ContentItem = clipboardItem;
            DestinationFolder = destinationFolder;
        }
    }

}
