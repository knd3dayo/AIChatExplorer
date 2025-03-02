using LibPythonAI.Data;
using PythonAILib.Common;
using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Folders.Browser {
    public class RecentFilesItem : ContentItemWrapper {

        // コンストラクタ
        public RecentFilesItem(ContentItemEntity item) : base(item) { }

        public RecentFilesItem(ContentFolderEntity folder) : base(folder) { }

        public override RecentFilesItem Copy() {
            return new(Entity.Copy());
        }

    }
}
