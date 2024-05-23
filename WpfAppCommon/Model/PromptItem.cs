
using LiteDB;
using WpfAppCommon;

namespace QAChat.Model {
    public class PromptItem {

        public ObjectId Id { get; set; } = ObjectId.Empty;
        // 名前
        public string Name { get; set; } = "";
        // 説明
        public string Description { get; set; } = "";

        // プロンプト
        public string Prompt { get; set; } = "";

        // PromptItemを取得
        public static PromptItem GetPromptItemById(ObjectId id) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetPromptTemplate(id);

        }
    }
}
