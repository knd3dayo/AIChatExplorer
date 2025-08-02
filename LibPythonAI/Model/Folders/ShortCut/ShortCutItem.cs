using AIChatExplorer.Model.Folders.FileSystem;
using LibPythonAI.Data;

namespace AIChatExplorer.Model.Folders.ShortCut {
    public class ShortCutItem : FileSystemItem {

        
        public override ShortCutItem Copy() {
            return new() {
                Entity = Entity.Copy(),
                ChatSettings = ChatSettings
            };
        }
    }
}
