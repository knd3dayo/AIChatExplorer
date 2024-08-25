using PythonAILib.Model;

namespace WpfAppCommon.Model {
    public  class PromptAutoProcessItem : SystemAutoProcessItem{
        public LiteDB.ObjectId PromptItemId { get; set; } = LiteDB.ObjectId.Empty;
        public OpenAIExecutionModeEnum Mode { get; set; } = OpenAIExecutionModeEnum.Normal;
        public PromptAutoProcessItem() {
        }
        public PromptAutoProcessItem(PromptItem promptItem){

            Name = promptItem.Name;
            DisplayName = promptItem.Name;
            Description = promptItem.Description;
            Type = TypeEnum.PromptTemplate;
            PromptItemId = promptItem.Id;

        }
        public override ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder) {
            
            if (PromptItemId == LiteDB.ObjectId.Empty) {
                return null;
            }
            ChatRequest chatController = new(ClipboardAppConfig.CreateOpenAIProperties());

            // PromptItemを取得
            PromptItem PromptItem = PromptItem.GetPromptItemById(PromptItemId);
            chatController.PromptTemplateText = PromptItem.Prompt;
            chatController.ChatMode = Mode;
            ClipboardFolder? clipboardFolder = clipboardItem.GetFolder();

            chatController.VectorDBItems = [];
            // フォルダのVectorDBItemを追加
            chatController.VectorDBItems.Add(clipboardFolder.GetVectorDBItem());

            ChatResult? result = chatController.ExecuteChat();
            if (result == null) {
                return clipboardItem;
            }
            // ClipboardItemのContentにレスポンスを設定
            clipboardItem.Content = result.Response;
            // レスポンスをClipboardItemに設定
            clipboardItem.ChatItems = chatController.ChatHistory;
            return clipboardItem;
        }
    }

}
