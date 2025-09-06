using LibMain.Data;
using LibMain.PythonIF.Request;

namespace LibMain.PythonIF.Response {
    public class ContentItemResponse : ContentItemRequest {
        private ContentItemResponse(ContentItemEntity entity) : base(entity) {
            Entity = entity;
        }

        public static ContentItemResponse FromDict(Dictionary<string, object> dict) {

            ContentItemEntity entity = ContentItemEntity.FromDict(dict);
            return new ContentItemResponse(entity);
        }
    }
}
