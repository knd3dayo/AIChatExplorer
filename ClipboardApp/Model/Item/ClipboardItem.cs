using PythonAILib.Model.Content;


namespace ClipboardApp.Model.Item {
    public partial class ClipboardItem : ContentItem {
        // コンストラクタ
        public ClipboardItem(LiteDB.ObjectId folderObjectId) {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            CollectionId = folderObjectId;
        }

        public override object Copy() {
            ClipboardItem clipboardItem = new(this.CollectionId);
            CopyTo(clipboardItem);
            return clipboardItem;
        }
    }
}
