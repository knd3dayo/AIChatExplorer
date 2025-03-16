using LibPythonAI.Data;
using LibPythonAI.Model.Content;

namespace AIChatExplorer.Model.Item {
    public partial class ClipboardItem : ContentItemWrapper {
        // コンストラクタ
        public ClipboardItem(ContentItemEntity item) : base(item) { }

        public ClipboardItem(ContentFolderEntity folder) : base(folder) { }

        public override ClipboardItem Copy() {
            return new(Entity.Copy());
        }
    }
}
