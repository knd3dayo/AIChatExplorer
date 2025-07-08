using AIChatExplorer.Model.Folders.Application;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel.Chat;
using LibUIPythonAI.ViewModel.Folder;


namespace AIChatExplorer.ViewModel.Main {

    public class QAChatStartupProps(ContentItemWrapper applicationItem) : QAChatStartupPropsBase {

        public override ContentItemWrapper GetContentItem() {
            return applicationItem;
        }

        public override FolderViewModelManagerBase GetViewModelManager() {
            // Return the folder view model manager
            return MainWindowViewModel.Instance.RootFolderViewModelContainer;
        }

        public override void SaveCommand(ContentItemWrapper itemWrapper, bool saveChatHistory) {
            // SaveCommand is set in the constructor
            MainWindowViewModel ActiveInstance = MainWindowViewModel.Instance;
            if (!saveChatHistory) {
                return;
            }
            Task.Run(async () => {
                ContentFolderWrapper chatFolder = (ContentFolderWrapper)ActiveInstance.RootFolderViewModelContainer.ChatRootFolderViewModel.Folder;
                await ContentItemCommands.SaveChatHistoryAsync(itemWrapper, chatFolder);

            });
        }
        public override void ExportChatCommand(List<ChatMessage> chatHistory) {
            FolderSelectWindow.OpenFolderSelectWindow(FolderViewModelManagerBase.FolderViewModels, (folder, finished) => {
                if (finished) {
                    ApplicationItem chatHistoryItem = new(folder.Folder.Entity);
                    // タイトルを日付 + 元のタイトルにする
                    chatHistoryItem.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";
                    if (!string.IsNullOrEmpty(applicationItem.Description)) {
                        chatHistoryItem.Description += " " + applicationItem.Description;
                    }
                    // chatHistoryItemの内容をテキスト化
                    string chatHistoryText = "";
                    foreach (var item in chatHistory) {
                        chatHistoryText += $"--- {item.Role} ---\n";
                        chatHistoryText += item.ContentWithSources + "\n\n";
                    }
                    chatHistoryItem.Content = chatHistoryText;
                    chatHistoryItem.Save();
                }
            });
        }

    }
}
