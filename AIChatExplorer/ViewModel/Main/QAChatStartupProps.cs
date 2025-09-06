using AIChatExplorer.Model.Folders.Application;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Content;
using LibUIMain.View.Folder;
using LibUIMain.ViewModel.Chat;
using LibUIMain.ViewModel.Folder;


namespace AIChatExplorer.ViewModel.Main {

    public class QAChatStartupProps(ContentItem applicationItem) : QAChatStartupPropsBase {

        public override ContentItem GetContentItem() {
            return applicationItem;
        }

        public override FolderViewModelManagerBase GetViewModelManager() {
            // Return the folder view model manager
            return MainWindowViewModel.Instance.RootFolderViewModelContainer;
        }

        public override void SaveCommand(ContentItem itemWrapper, bool saveChatHistory) {
            // SaveCommand is set in the constructor
            MainWindowViewModel ActiveInstance = MainWindowViewModel.Instance;
            if (!saveChatHistory) {
                return;
            }
            _ = SaveChatHistoryAsync(itemWrapper, ActiveInstance);
        }

        private async Task SaveChatHistoryAsync(ContentItem itemWrapper, MainWindowViewModel ActiveInstance)
        {
            try
            {
                ContentFolderWrapper? chatFolder = (ContentFolderWrapper?)ActiveInstance.RootFolderViewModelContainer.ChatRootFolderViewModel?.Folder;
                if (chatFolder != null)
                {
                    await ContentItemCommands.SaveChatHistoryAsync(itemWrapper, chatFolder);
                }
            }
            catch (Exception ex)
            {
                // TODO: ログ出力やユーザー通知
            }
        }
        public override void ExportChatCommand(List<ChatMessage> chatHistory) {
            FolderSelectWindow.OpenFolderSelectWindow(FolderViewModelManagerBase.FolderViewModels, async (folder, finished) => {
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
                    await chatHistoryItem.SaveAsync();
                }
            });
        }

    }
}
