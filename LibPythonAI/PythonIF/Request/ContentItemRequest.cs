using LibPythonAI.Data;
using LibPythonAI.Model.Content;

namespace LibPythonAI.PythonIF.Request {
    public class ContentItemRequest(ContentItemEntity entity) {
        public ContentItemEntity Entity { get; set; } = entity;

        public ContentItemWrapper ContentItem {
            get {
                ContentItemWrapper item = new() {
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
