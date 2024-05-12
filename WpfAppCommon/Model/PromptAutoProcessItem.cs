using LibGit2Sharp;
using LiteDB;
using QAChat.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public  class PromptAutoProcessItem : SystemAutoProcessItem{
        public PromptItem? PromptItem { get; set; }

        public PromptAutoProcessItem() {
        }
        public PromptAutoProcessItem(PromptItem promptItem){

            PromptItem = promptItem;
            Name = promptItem.Name;
            DisplayName = promptItem.Name;
            Description = promptItem.Description;
            Type = TypeEnum.PromptTemplate;

        }
        public override ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder) {
            
            if (PromptItem == null) {
                return null;
            }
            
            List<ChatItem> chatItems = [];
            ChatResult result = new();
            // ★ OpenAIExecutionModeEnumを選択可能にする
            OpenAIExecutionModeEnum mode = OpenAIExecutionModeEnum.Normal;
            // modeがRAGの場合はLangChainChatを実行
            if (mode == OpenAIExecutionModeEnum.RAG) {
                // LangChainChatを実行
                result = PythonExecutor.PythonFunctions.LangChainChat(clipboardItem.Content, chatItems, VectorDBItem.GetEnabledItems());
            }
            // modeがNormalの場合はOpenAIChatを実行
            else if (mode == OpenAIExecutionModeEnum.Normal) {
                // OpenAIChatを実行
                result = PythonExecutor.PythonFunctions.OpenAIChat(clipboardItem.Content, chatItems);
            } else {
                return clipboardItem;
            }
            // レスポンスをClipboardItemに設定
            clipboardItem.Content = result.Response;
            return clipboardItem;
        }
    }

}
