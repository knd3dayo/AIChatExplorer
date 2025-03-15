using ClipboardApp.Model.Folders.FileSystem;
using LibPythonAI.Data;
using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Folders.ShortCut {
    public class ShortCutItem : FileSystemItem {

        // コンストラクタ
        public ShortCutItem(ContentItemEntity item) : base(item) { }

        public ShortCutItem(ContentFolderEntity folder) : base(folder) { }

        public override ShortCutItem Copy() {
            return new(Entity.Copy());
        }
    }
}
