using ClipboardApp.Model.Folder;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Prompt;

namespace ClipboardApp.Model.AutoProcess
{
    public class PromptAutoProcessItem : SystemAutoProcessItem
    {
        public LiteDB.ObjectId PromptItemId { get; set; } = LiteDB.ObjectId.Empty;
        public OpenAIExecutionModeEnum Mode { get; set; } = OpenAIExecutionModeEnum.Normal;
        public PromptAutoProcessItem()
        {
        }
        public PromptAutoProcessItem(PromptItem promptItem)
        {

            Name = promptItem.Name;
            DisplayName = promptItem.Name;
            Description = promptItem.Description;
            Type = TypeEnum.PromptTemplate;
            PromptItemId = promptItem.Id;

        }
        public override ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder)
        {

            if (PromptItemId == LiteDB.ObjectId.Empty)
            {
                return null;
            }
            ChatRequest chatRequest = new();

            // PromptItemを取得
            PromptItem PromptItem = PromptItem.GetPromptItemById(PromptItemId);
            chatRequest.PromptTemplateText = PromptItem.Prompt;
            chatRequest.ChatMode = Mode;
            ClipboardFolder clipboardFolder = clipboardItem.GetFolder<ClipboardFolder>();

            // ChatRequestContentを作成
            ChatRequestContext chatRequestContent = new() {
                OpenAIProperties = ClipboardAppConfig.Instance.CreateOpenAIProperties(),
                VectorDBItems = [clipboardFolder.GetVectorDBItem()]
            };

            ChatResult? result = chatRequest.ExecuteChat(chatRequestContent, (message) => { });
            if (result == null)
            {
                return clipboardItem;
            }
            // ClipboardItemのContentにレスポンスを設定
            clipboardItem.Content = result.Output;
            // レスポンスをClipboardItemに設定
            clipboardItem.ChatItems = chatRequest.ChatHistory;
            return clipboardItem;
        }
    }

}
