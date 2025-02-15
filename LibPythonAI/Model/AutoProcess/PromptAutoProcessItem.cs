using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Utils.Python;

namespace PythonAILib.Model.AutoProcess {
    public class PromptAutoProcessItem : SystemAutoProcessItem {
        public LiteDB.ObjectId PromptItemId { get; set; } = LiteDB.ObjectId.Empty;
        public OpenAIExecutionModeEnum Mode { get; set; } = OpenAIExecutionModeEnum.Normal;
        public PromptAutoProcessItem() {
        }
        public PromptAutoProcessItem(PromptItem promptItem) {

            Name = promptItem.Name;
            DisplayName = promptItem.Name;
            Description = promptItem.Description;
            TypeName = TypeEnum.PromptTemplate;
            PromptItemId = promptItem.Id;

        }
        public override void Execute(ContentItemWrapper clipboardItem, ContentFolderWrapper? destinationFolder) {

            if (PromptItemId == LiteDB.ObjectId.Empty) {
                return;
            }
            ChatRequest chatRequest = new();

            // PromptItemを取得
            PromptItem PromptItem = PromptItem.GetPromptItemById(PromptItemId);
            ContentFolderWrapper clipboardFolder = clipboardItem.GetFolder();

            // ChatRequestContentを作成
            ChatRequestContext chatRequestContent = new() {
                OpenAIProperties = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties(),
                VectorDBProperties = [clipboardFolder.GetMainVectorSearchProperty()],
                PromptTemplateText = PromptItem.Prompt,
                ChatMode = Mode
            };

            ChatResult? result = ChatUtil.ExecuteChat(chatRequest, chatRequestContent, (message) => { });
            if (result == null) {
                return;
            }
            // ClipboardItemのContentにレスポンスを設定
            clipboardItem.Content = result.Output;
            // レスポンスをClipboardItemに設定
            clipboardItem.ChatItems = chatRequest.ChatHistory;
        }
    }

}
