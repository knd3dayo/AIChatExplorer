using LiteDB;
using PythonAILib.Common;

namespace PythonAILib.Model.Tag {
    public class TagItem {
        public ObjectId? Id { get; set; }

        public string Tag { get; set; } = "";

        public bool IsPinned { get; set; } = false;

        public void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetTagCollection<TagItem>();
            collection.Delete(Id);
        }
        public void Save() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetTagCollection<TagItem>();
            var tag = collection.FindOne(x => x.Tag == Tag);
            if (tag != null) {
                tag.IsPinned = IsPinned;
                collection.Update(tag);
            } else {
                collection.Insert(this);
            }
        }

        public static IEnumerable<TagItem> GetTagList() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            return libManager.DataFactory.GetTagCollection<TagItem>().FindAll();
        }
        // タグを検索
        public static IEnumerable<TagItem> FilterTag(string tag, bool exclude) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetTagCollection<TagItem>();
            if (exclude) {
                return collection.FindAll().Where(x => x.Tag.Contains(tag) == false);
            } else {
                return collection.FindAll().Where(x => x.Tag.Contains(tag));
            }
        }

    }

}
