using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.ViewModel.Chat {
    public abstract class QAChatStartupPropsBase {
        
        public abstract ContentItemWrapper GetContentItem();
        
        public abstract FolderViewModelManagerBase GetViewModelManager();

        public abstract void SaveCommand(ContentItemWrapper itemWrapper, bool flag);

        public abstract void ExportChatCommand(List<ChatMessage> messages);

    }
}
