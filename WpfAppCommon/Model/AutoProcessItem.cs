using System.Collections.ObjectModel;

namespace WpfAppCommon.Model {
    // 自動処理の引数用のクラス
    public class AutoProcessItemArgs {

        public ClipboardItem ClipboardItem { get; set; }
        public ClipboardFolder? DestinationFolder { get; set; }

        public AutoProcessItemArgs(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder) {
            ClipboardItem = clipboardItem;
            DestinationFolder = destinationFolder;
        }
    }

}
