using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Item {
    public partial class ClipboardItem : ContentItemWrapper {
        // コンストラクタ
        public ClipboardItem(ContentItem item) : base(item) { }

        public ClipboardItem(LiteDB.ObjectId folderObjectId) : base(folderObjectId) { }

        public override ClipboardItem Copy() {
            return new(ContentItemInstance.Copy());
        }
    }
}
