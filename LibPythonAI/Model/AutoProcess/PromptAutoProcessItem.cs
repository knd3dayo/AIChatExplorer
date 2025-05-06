using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Python;
using PythonAILib.Common;
using PythonAILib.Model.Chat;

namespace PythonAILib.Model.AutoProcess {
    public class PromptAutoProcessItem : AutoProcessItem {
        public OpenAIExecutionModeEnum Mode { get; set; } = OpenAIExecutionModeEnum.Normal;
        public PromptAutoProcessItem(AutoProcessItemEntity autoProcessItemEntity, PromptItemEntity promptItemEntity) : base(autoProcessItemEntity) {
            PromptItemEntity = promptItemEntity;
        }

        public PromptItemEntity PromptItemEntity { get; set; }

        public override void Execute(ContentItemWrapper clipboardItem, ContentFolderWrapper? destinationFolder) {


            ChatRequest chatRequest = new();

            // PromptItemを取得
            ContentFolderWrapper? clipboardFolder = clipboardItem.GetFolder();

            // ChatRequestContentを作成
            ChatRequestContext chatRequestContent = new() {
                OpenAIProperties = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties(),
                PromptTemplateText = PromptItemEntity.Prompt,
                ChatMode = Mode,
            };
            if (clipboardFolder != null) {
                chatRequestContent.UseVectorDB = true;
                chatRequestContent.VectorDBProperties = [clipboardFolder.GetMainVectorSearchProperty()];
            }

            ChatResult? result = ChatUtil.ExecuteChat(chatRequest, chatRequestContent , (message) => { });
            if (result == null) {
                return;
            }
            // ClipboardItemのContentにレスポンスを設定
            clipboardItem.Content = result.Output;
            // レスポンスをClipboardItemに設定
            clipboardItem.ChatItems.Clear();
            clipboardItem.ChatItems.AddRange(chatRequest.ChatHistory);
        }
    }

}
