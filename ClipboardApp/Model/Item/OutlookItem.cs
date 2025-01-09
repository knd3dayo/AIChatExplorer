using PythonAILib.Model.Content;

namespace ClipboardApp.Model.Item {
    public class OutlookItem : ContentItem {
        public string EntryID { get; set; } = "";

        public OutlookItem() : base() { }

        public OutlookItem(LiteDB.ObjectId collectionId, string entryID) : base(collectionId) {
            EntryID = entryID;
        }
    }
}
