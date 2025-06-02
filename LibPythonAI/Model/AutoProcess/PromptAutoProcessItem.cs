using LibPythonAI.Data;
using LibPythonAI.Model.AutoProcess;
using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
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

        public override async Task Execute(ContentItemWrapper applicationItem, ContentFolderWrapper? destinationFolder) {


            ChatRequest chatRequest = new();

            // PromptItemを取得
            ContentFolderWrapper? clipboardFolder = applicationItem.GetFolder();

            // ChatRequestContentを作成
            ChatRequestContext chatRequestContent = new() {
                PromptTemplateText = PromptItemEntity.Prompt,
            };
            if (clipboardFolder != null) {
                chatRequestContent.RAGMode = RAGModeEnum.NormalSearch;
                chatRequestContent.VectorSearchRequests = [ new VectorSearchRequest(clipboardFolder.GetMainVectorSearchItem())];
            }

            ChatResponse? result = await ChatUtil.ExecuteChat(Mode, chatRequest, chatRequestContent , (message) => { });
            if (result == null) {
                return;
            }
            // ApplicationItemのContentにレスポンスを設定
            applicationItem.Content = result.Output;
            // レスポンスをApplicationItemに設定
            applicationItem.ChatItems.Clear();
            applicationItem.ChatItems.AddRange(chatRequest.ChatHistory);
        }
    }

}
