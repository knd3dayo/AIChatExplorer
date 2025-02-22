using ClipboardApp.Model.Folders.FileSystem;
using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Folders.ShortCut {
    public class ShortCutItem : FileSystemItem {

        // コンストラクタ
        public ShortCutItem(ContentItem item) : base(item) { }

        public ShortCutItem(LiteDB.ObjectId folderObjectId) : base(folderObjectId) { }

        public override ShortCutItem Copy() {
            return new(ContentItemInstance.Copy());
        }
    }
}
