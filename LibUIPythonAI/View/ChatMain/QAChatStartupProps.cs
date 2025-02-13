using System.Collections.ObjectModel;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.ViewModel {
    public class QAChatStartupProps {
        public QAChatStartupProps(ContentItem clipboardItem) {
            ContentItem = clipboardItem;
        }
        public ContentItem ContentItem { get; set; }

        public Action<ContentItem, bool> CloseCommand { get; set; } = (item, saveChatHistory) => { };

        public Action<List<ChatMessage>> ExportChatCommand { get; set; } = (chatHistory) => { };

    }
}
