using ClipboardApp.Model.Folders.FileSystem;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;

namespace ClipboardApp.Model.Folders.Outlook {
    public class OutlookItem : ContentItemWrapper {

        // EntryIDの名前
        public const string EntryIDName = "EntryID";

        // コンストラクタ
        public OutlookItem(ContentItemEntity item) : base(item) { }

        public OutlookItem(ContentFolderEntity folder) : base(folder) { }


        public OutlookItem(ContentFolderEntity folder, string entryID) : base(folder) {
            EntryID = entryID;
        }

        public override FileSystemItem Copy() {
            return new(Entity.Copy());
        }

        public string EntryID {
            get {
                if (Entity.ExtendedProperties.TryGetValue(EntryIDName, out var path)) {
                    return (string)path;
                } else {
                    return "";
                }
            }
            set {
                Entity.ExtendedProperties[EntryIDName] = value;
            }
        }

    }
}
