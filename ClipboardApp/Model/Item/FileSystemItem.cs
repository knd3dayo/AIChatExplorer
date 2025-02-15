using PythonAILib.Common;
using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Item {
    public class FileSystemItem : ContentItemWrapper {

        // コンストラクタ
        public FileSystemItem(ContentItem item) : base(item) { }

        public FileSystemItem(LiteDB.ObjectId folderObjectId) : base(folderObjectId) { }

        public override FileSystemItem Copy() {
            return new(ContentItemInstance.Copy());
        }

        public override void Save(bool updateLastModifiedTime = true, bool applyAutoProcess = false) {
            ContentItemInstance.Save(false, applyAutoProcess);
        }
    }
}
