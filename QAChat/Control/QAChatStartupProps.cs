using System.Collections.ObjectModel;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;

namespace QAChat.Control
{
    public class QAChatStartupProps {
        public QAChatStartupProps(ContentItem clipboardItem) {
            ContentItem = clipboardItem;
        }
        public ContentItem ContentItem { get; set; }

        public Action<ObservableCollection<VectorDBItem>> SelectVectorDBItemAction { get; set; } = (folders) => { };
        
        public Action<ContentItem, bool> SaveCommand { get; set; } = (item, saveChatHistory) => { };

        public Action<List<ChatHistoryItem>> ExportChatCommand { get; set; } = (chatHistory) => { };

    }


}
