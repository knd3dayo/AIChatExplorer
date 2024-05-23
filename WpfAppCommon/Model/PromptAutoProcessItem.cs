using LibGit2Sharp;
using LiteDB;
using QAChat.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

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
            // PromptItemを取得
            PromptItem PromptItem = PromptItem.GetPromptItemById(PromptItemId);

            // promptTextを作成。 PromptItemのPrompt + クリップボードのContent
            string promptText = PromptItem.Prompt + "\n----\n" + clipboardItem.Content;

            List<ChatItem> chatItems = [];
            ChatResult result = new();
            // modeがRAGの場合はLangChainChatを実行
            if (Mode == OpenAIExecutionModeEnum.RAG) {
                Tools.Info("LangChainChatを実行");
                result = PythonExecutor.PythonFunctions.LangChainChat(promptText, chatItems, VectorDBItem.GetEnabledItems());
            }
            // modeがNormalの場合はOpenAIChatを実行
            else if (Mode == OpenAIExecutionModeEnum.Normal) {
                // OpenAIChatを実行
                Tools.Info("OpenAIChatを実行");
                result = PythonExecutor.PythonFunctions.OpenAIChat(promptText, chatItems);
            } else {
                return clipboardItem;
            }
            // リクエストをChatItemsに追加
            clipboardItem.ChatItems.Add(new ChatItem(ChatItem.UserRole, promptText));
            // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
            clipboardItem.ChatItems.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));

            // レスポンスをClipboardItemに設定
            clipboardItem.Content = result.Response;
            return clipboardItem;
        }
    }

}
