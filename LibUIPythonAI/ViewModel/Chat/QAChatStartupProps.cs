using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;

namespace LibUIPythonAI.ViewModel.Chat {
    public class QAChatStartupProps {
        public QAChatStartupProps(ContentItemWrapper applicationItem) {
            ContentItem = applicationItem;
        }
        public ContentItemWrapper ContentItem { get; set; }

        public Action<ContentItemWrapper, bool> SaveCommand { get; set; } = (item, saveChatHistory) => { };

        public Action<List<ChatMessage>> ExportChatCommand { get; set; } = (chatHistory) => { };

    }
}
