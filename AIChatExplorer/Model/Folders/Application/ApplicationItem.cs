using LibPythonAI.Data;
using LibPythonAI.Model.Content;

namespace AIChatExplorer.Model.Item {
    public partial class ApplicationItem : ContentItemWrapper {
        // コンストラクタ
        public ApplicationItem() : base() {
        }

        public ApplicationItem(ContentFolderEntity folder) : base(folder) { }

        public override ApplicationItem Copy() {
            return new() {
                Entity = Entity.Copy(),
                ChatSettings = ChatSettings
            };
        }
    }
}
