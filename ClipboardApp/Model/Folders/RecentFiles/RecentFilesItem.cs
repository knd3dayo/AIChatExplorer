using PythonAILib.Common;
using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Folders.Browser {
    public class RecentFilesItem : ContentItemWrapper {

        // コンストラクタ
        public RecentFilesItem(ContentItem item) : base(item) { }

        public RecentFilesItem(LiteDB.ObjectId folderObjectId) : base(folderObjectId) { }

        public override RecentFilesItem Copy() {
            return new(ContentItemInstance.Copy());
        }

        public override void Save(bool updateLastModifiedTime = true, bool applyAutoProcess = false) {
            ContentItemInstance.Save(false, applyAutoProcess);
        }
    }
}
