using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Item {
    public class OutlookItem : ContentItemWrapper {

        // EntryIDの名前
        public const string EntryIDName = "EntryID";

        // コンストラクタ
        public OutlookItem(ContentItem item) : base(item) { }

        public OutlookItem(LiteDB.ObjectId folderObjectId) : base(folderObjectId) { }


        public OutlookItem(LiteDB.ObjectId collectionId, string entryID) : base(collectionId) {
            EntryID = entryID;
        }

        public override FileSystemItem Copy() {
            return new(ContentItemInstance.Copy());
        }

        public string EntryID {
            get {
                if (ContentItemInstance.ExtendedProperties.TryGetValue(EntryIDName, out var path)) {
                    return (string)path;
                } else {
                    return "";
                }
            }
            set {
                ContentItemInstance.ExtendedProperties[EntryIDName] = value;
            }
        }

    }
}
