using System.Collections.ObjectModel;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
using QAChat.ViewModel.Folder;

namespace QAChat.ViewModel {
    public class QAChatStartupProps {
        public QAChatStartupProps(ContentItem clipboardItem) {
            ContentItem = clipboardItem;
        }
        public ContentItem ContentItem { get; set; }

        public Action<ObservableCollection<VectorDBItem>> SelectVectorDBItemAction { get; set; } = (folders) => { };

        public Action<ObservableCollection<VectorDBItem>> EditVectorDBItemAction { get; set; } = (folders) => { };

        public Action<ContentItem, bool> SaveCommand { get; set; } = (item, saveChatHistory) => { };

        public Action<List<ChatMessage>> ExportChatCommand { get; set; } = (chatHistory) => { };

    }
}
