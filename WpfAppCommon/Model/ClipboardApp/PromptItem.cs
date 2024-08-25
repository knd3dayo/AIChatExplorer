using LiteDB;
using WpfAppCommon.Model.QAChat;

namespace WpfAppCommon.Model.ClipboardApp {
    public class PromptItem : PromptItemBase {

        public ObjectId Id { get; set; } = ObjectId.Empty;
        // 名前

        // PromptItemを取得
        public static PromptItem GetPromptItemById(ObjectId id) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetPromptTemplate(id);

        }

        // Save
        public override void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertPromptTemplate(this);
        }

    }
}
