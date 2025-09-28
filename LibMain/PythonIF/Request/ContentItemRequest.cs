using LibMain.Data;
using LibMain.Model.Content;

namespace LibMain.PythonIF.Request {
    public class ContentItemRequest(ContentItemEntity entity) {
        public ContentItemEntity Entity { get; set; } = entity;

        public ContentItem ContentItem {
            get {
                ContentItem item = new() {
                    Entity = Entity
                };
                return item;
            }
        }
        public Dictionary<string, object> ToDict() {
            return Entity.ToDict();
        }

        public static List<Dictionary<string, object>> ToDictList(List<ContentItemRequest> requests) {
            List<Dictionary<string, object>> dictList = [];
            foreach (var request in requests) {
                dictList.Add(request.ToDict());
            }
            return dictList;

        }
    }
}
