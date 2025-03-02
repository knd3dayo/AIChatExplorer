using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using PythonAILib.Common;

namespace ClipboardApp.Model.Folders.Browser {
    public class EdgeBrowseHistoryItem : ContentItemWrapper {

        // コンストラクタ
        public EdgeBrowseHistoryItem(ContentItemEntity item) : base(item) { }

        public EdgeBrowseHistoryItem(ContentFolderEntity folder) : base(folder) { }

        public override EdgeBrowseHistoryItem Copy() {
            return new(Entity.Copy());
        }

    }
}
