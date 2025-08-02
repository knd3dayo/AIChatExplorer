using LibPythonAI.Data;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.PythonIF.Response;
using LibPythonAI.Utils.Python;

namespace LibPythonAI.Model.AutoProcess {
    public class PromptAutoProcessItem : AutoProcessItem {
        public OpenAIExecutionModeEnum Mode { get; set; } = OpenAIExecutionModeEnum.Normal;
        public PromptAutoProcessItem(PromptItem promptItemEntity) : base() {
            PromptItemEntity = promptItemEntity;
        }

        public PromptItem PromptItemEntity { get; set; }

        public override void Execute(ContentItemWrapper applicationItem, ContentFolderWrapper? destinationFolder) {

            Task.Run(async () => {

                ChatRequest chatRequest = new();
                // PromptItemを取得
                ContentFolderWrapper? clipboardFolder = applicationItem.Folder;

                // ChatRequestContentを作成
                ChatSettings chatSettings = new() {
                    PromptTemplateText = PromptItemEntity.Prompt,
                };
                if (clipboardFolder != null) {
                    chatSettings.RAGMode = RAGModeEnum.NormalSearch;
                    var item = await clipboardFolder.GetMainVectorSearchItem();
                    chatSettings.VectorSearchRequests = [new VectorSearchRequest(item)];
                }
                ChatRequestContext chatRequestContent = new(chatSettings);

                ChatResponse? result = await ChatUtil.ExecuteChat(Mode, chatRequest, chatRequestContent, (message) => { });
                if (result == null) {
                    return;
                }
                // ApplicationItemのContentにレスポンスを設定
                applicationItem.Content = result.Output;
                // レスポンスをApplicationItemに設定
                applicationItem.ChatItems.Clear();
                applicationItem.ChatItems.AddRange(chatRequest.ChatHistory);
            }).Wait();
        }
    }

}
