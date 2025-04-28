using LibPythonAI.Model.Content;
using PythonAILib.Model.Chat;

namespace LibUIPythonAI.ViewModel.Chat {
    public class QAChatStartupProps {
        public QAChatStartupProps(ContentItemWrapper clipboardItem) {
            ContentItem = clipboardItem;
        }
        public ContentItemWrapper ContentItem { get; set; }

        public Action<ContentItemWrapper, bool> CloseCommand { get; set; } = (item, saveChatHistory) => { };

        public Action<List<ChatMessage>> ExportChatCommand { get; set; } = (chatHistory) => { };

    }
}
