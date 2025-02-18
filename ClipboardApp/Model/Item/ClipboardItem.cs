using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Item {
    public partial class ClipboardItem : ContentItemWrapper {
        // コンストラクタ
        public ClipboardItem(ContentItem item) : base(item) {
            SourcePath = item.ObjectPath;
        }

        public ClipboardItem(LiteDB.ObjectId folderObjectId) : base(folderObjectId) {
            SourcePath = base.ObjectPath;
        }

        public override ClipboardItem Copy() {
            return new(ContentItemInstance.Copy());
        }
    }
}
