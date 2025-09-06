using LibMain.Data;
using LibMain.Model.Content;

namespace LibMain.PythonIF.Request {
    public class ContentFolderRequest(ContentFolderEntity entity) {
        public ContentFolderEntity Entity { get; set; } = entity;

        // ToDict
        public Dictionary<string, object> ToDict() {
            return Entity.ToDict();
        }

        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<ContentFolderRequest> requests) {
            List<Dictionary<string, object>> dictList = new();
            foreach (var request in requests) {
                dictList.Add(request.ToDict());
            }
            return dictList;
        }
    }
}
