using LibMain.Model.Chat;
using LibMain.Model.Content;
using LibUIMain.ViewModel.Folder;

namespace LibUIMain.ViewModel.Chat {
    public abstract class QAChatStartupPropsBase {
        
        public abstract ContentItem GetContentItem();
        
        public abstract FolderViewModelManagerBase GetViewModelManager();

        public abstract void SaveCommand(ContentItem itemWrapper, bool flag);

        public abstract void ExportChatCommand(List<ChatMessage> messages);

    }
}
