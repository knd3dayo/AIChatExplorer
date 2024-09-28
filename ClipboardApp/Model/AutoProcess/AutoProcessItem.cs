using ClipboardApp.Model.Folder;

namespace ClipboardApp.Model.AutoProcess
{
    // 自動処理の引数用のクラス
    public class AutoProcessItemArgs
    {

        public ClipboardItem ClipboardItem { get; set; }
        public ClipboardFolder? DestinationFolder { get; set; }

        public AutoProcessItemArgs(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder)
        {
            ClipboardItem = clipboardItem;
            DestinationFolder = destinationFolder;
        }
    }

}
