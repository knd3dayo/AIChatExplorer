using LibPythonAI.Data;
using LibPythonAI.Model.Content;

namespace AIChatExplorer.Model.Folders.Application {
    public partial class ApplicationItem : ContentItem {
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
