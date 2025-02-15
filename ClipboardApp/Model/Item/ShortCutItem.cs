using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Item {
    public class ShortCutItem : FileSystemItem {

        // コンストラクタ
        public ShortCutItem(ContentItem item) : base(item) { }

        public ShortCutItem(LiteDB.ObjectId folderObjectId) : base(folderObjectId) { }

        public override ShortCutItem Copy() {
            return new(ContentItemInstance.Copy());
        }
    }
}
