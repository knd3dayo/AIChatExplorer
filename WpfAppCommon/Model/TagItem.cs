
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
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertTag(this);
        }

        public static IEnumerable<TagItem> GetTagList() {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetTagList();
        }
        // タグを検索
        public static IEnumerable<TagItem> FilterTag(string tag, bool exclude) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().FilterTag(tag, exclude);
        }

    }

}
