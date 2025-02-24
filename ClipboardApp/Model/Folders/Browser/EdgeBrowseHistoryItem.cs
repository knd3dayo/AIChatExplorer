using PythonAILib.Common;
using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Folders.Browser {
    public class EdgeBrowseHistoryItem : ContentItemWrapper {

        // コンストラクタ
        public EdgeBrowseHistoryItem(ContentItem item) : base(item) { }

        public EdgeBrowseHistoryItem(LiteDB.ObjectId folderObjectId) : base(folderObjectId) { }

        public override EdgeBrowseHistoryItem Copy() {
            return new(ContentItemInstance.Copy());
        }

        public override void Save(bool updateLastModifiedTime = true, bool applyAutoProcess = false) {
            ContentItemInstance.Save(false, applyAutoProcess);
        }
    }
}
