using System.Collections.ObjectModel;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;
using LibUIPythonAI.ViewModel.Folder;
using LibPythonAI.Model.Content;

namespace LibUIPythonAI.ViewModel {
    public class QAChatStartupProps {
        public QAChatStartupProps(ContentItemWrapper clipboardItem) {
            ContentItem = clipboardItem;
        }
        public ContentItemWrapper ContentItem { get; set; }

        public Action<ContentItemWrapper, bool> CloseCommand { get; set; } = (item, saveChatHistory) => { };

        public Action<List<ChatMessage>> ExportChatCommand { get; set; } = (chatHistory) => { };

    }
}
