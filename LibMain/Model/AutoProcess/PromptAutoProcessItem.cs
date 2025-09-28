using LibMain.Model.Chat;
using LibMain.Model.Content;
using LibMain.Model.Prompt;
using LibMain.PythonIF.Request;
using LibMain.PythonIF.Response;
using LibMain.Utils.Python;

namespace LibMain.Model.AutoProcess {
    public class PromptAutoProcessItem : AutoProcessItem {
        public OpenAIExecutionModeEnum Mode { get; set; } = OpenAIExecutionModeEnum.Normal;
        public PromptAutoProcessItem(PromptItem promptItemEntity) : base() {
            PromptItemEntity = promptItemEntity;
        }

        public PromptItem PromptItemEntity { get; set; }


        public override async Task ExecuteAsync(ContentItem applicationItem, ContentFolderWrapper? destinationFolder) {
            ChatRequest chatRequest = new();
            // PromptItemを取得
            var folder = await applicationItem.GetFolderAsync();
            ContentFolderWrapper? clipboardFolder = folder;

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
        }
    }

}
