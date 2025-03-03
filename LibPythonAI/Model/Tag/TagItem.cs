using LibPythonAI.Data;

namespace LibPythonAI.Model.Tag {
    public class TagItem {

        public TagItemEntity Entity { get; set; }

        public TagItem(TagItemEntity entity) {
            Entity = entity;
        }

        public string Tag {
            get => Entity.Tag;
            set => Entity.Tag = value;
        }

        public bool IsPinned {
            get => Entity.IsPinned;
            set => Entity.IsPinned = value;
        }

        public void Delete() {
            using PythonAILibDBContext db = new();
            var item = db.TagItems.Find(Entity.Id);
            if (item != null) {
                db.Remove(item);
            }
        }
        public void Save() {
            using PythonAILibDBContext db = new();
            var item = db.TagItems.Find(Entity.Id);
            if (item == null) {
                db.TagItems.Add(Entity);
            } else {
                db.TagItems.Entry(item).CurrentValues.SetValues(Entity);
            }
            db.SaveChanges();
        }

        public static IEnumerable<TagItem> GetTagList() {
            using PythonAILibDBContext db = new();
            var tags = db.TagItems;
            foreach (var tag in tags) {
                yield return new TagItem(tag);
            }
        }
        // タグを検索
        public static IEnumerable<TagItem> FilterTag(string tag, bool exclude) {
            using PythonAILibDBContext db = new();
            var items = db.TagItems.Where(x => x.Tag.Contains(tag) == !exclude);
            foreach (var item in items) {
                yield return new TagItem(item);
            }
        }

    }

}
