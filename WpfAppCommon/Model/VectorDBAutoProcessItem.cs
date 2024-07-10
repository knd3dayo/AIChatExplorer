
using PythonAILib.Model;
using PythonAILib.PythonIF;

namespace WpfAppCommon.Model {
    public class VectorDBAutoProcessItem : SystemAutoProcessItem {
        public LiteDB.ObjectId VectorDBItemId { get; set; } = LiteDB.ObjectId.Empty;
        public VectorDBAutoProcessItem() {
        }
        public VectorDBAutoProcessItem(VectorDBItem vectorDBItem) {

            Name = vectorDBItem.Name;
            DisplayName = vectorDBItem.Name;
            Description = vectorDBItem.Description;
            VectorDBItemId = vectorDBItem.Id;

        }
        public override ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder) {

            if (VectorDBItemId == LiteDB.ObjectId.Empty) {
                return null;
            }
            // VectorDBItemを取得
            VectorDBItem? vectorDBItem = ClipboardAppVectorDBItem.GetItemById(VectorDBItemId);
            if (vectorDBItem == null) {
                return clipboardItem;
            }
            // ベクトルDBを更新
            IPythonFunctions.ContentInfo clipboard = new IPythonFunctions.ContentInfo(IPythonFunctions.VectorDBUpdateMode.update, clipboardItem.Id.ToString(), clipboardItem.Content);
            vectorDBItem.UpdateIndex(clipboard);

            return clipboardItem;
        }
    }

}
