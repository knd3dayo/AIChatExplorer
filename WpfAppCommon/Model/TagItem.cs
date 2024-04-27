
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;

namespace WpfAppCommon.Model {
    public class TagItem : ObservableObject {
        public ObjectId? Id { get; set; }

        public string Tag { get; set; } = "";

        public bool IsPinned { get; set; } = false;

        public void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteTag(this);
        }
        public void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().InsertTag(this);
        }
    }

}
