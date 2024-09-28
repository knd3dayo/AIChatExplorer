using LiteDB;
using PythonAILib.Resource;
using QAChat;

namespace PythonAILib.Model.Tag {
    public class TagItem {
        public ObjectId? Id { get; set; }

        public string Tag { get; set; } = "";

        public bool IsPinned { get; set; } = false;

        public void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.DeleteTag(this);
        }
        public void Save() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.UpsertTag(this);
        }

        public static IEnumerable<TagItem> GetTagList() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            return libManager.DataFactory.GetTagList();
        }
        // タグを検索
        public static IEnumerable<TagItem> FilterTag(string tag, bool exclude) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            return libManager.DataFactory.FilterTag(tag, exclude);
        }

    }

}
