using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using QAChat.ViewModel.Item;
using WpfAppCommon.Utils;

namespace PythonAILibUI.ViewModel.Item {
    public abstract class ContentItemViewModelCommands {

        // フォルダを開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFolderCommand => new((itemViewModel) => {
            OpenFolder(itemViewModel.ContentItem);
        });

        // テキストをファイルとして開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenContentAsFileCommand => new((itemViewModel) => {
            OpenContentAsFile(itemViewModel.ContentItem);
        });

        // ファイルを開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFileCommand => new((itemViewModel) => {
            OpenFile(itemViewModel.ContentItem);
        });


        // ファイルを新規ファイルとして開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFileAsNewFileCommand => new((itemViewModel) => {
            OpenFileAsNewFile(itemViewModel.ContentItem);
        });

        // QAChatButtonCommand
        public SimpleDelegateCommand<ContentItemViewModel> QAChatButtonCommand => new((itemViewModel) => {
            // QAChatControlのDrawerを開く
            OpenOpenAIChatWindowCommand(itemViewModel.ContentItem);
        });
        // アイテム保存
        public SimpleDelegateCommand<ContentItemViewModel> SaveClipboardItemCommand => new((itemViewModel) => {
            itemViewModel.ContentItem.Save();
        });

        // Delete
        public SimpleDelegateCommand<ContentItemViewModel> DeleteItemCommand => new((itemViewModel) => {
            itemViewModel.ContentItem.Delete();
        });


        // OpenContentItemCommand
        public SimpleDelegateCommand<ContentItemViewModel> OpenItemCommand => new((itemViewModel) => {
            OpenItem(itemViewModel.ContentItem);
        });

        // RemoveSelectedItemCommand
        public SimpleDelegateCommand<ContentItemViewModel> RemoveCommand => new((itemViewModel) => {
            RemoveItem(itemViewModel.ContentItem);
        });

        // 選択中のContentItemBaseを開く
        public abstract void OpenItem(ContentItem contentItem);

        // 選択中のContentItemBaseを削除
        public abstract void RemoveItem(ContentItem contentItem);

        public abstract void OpenFolder(ContentItem contentItem);

        // Command to open text content as a file
        public abstract void OpenContentAsFile(ContentItem contentItem);

        public abstract void OpenOpenAIChatWindowCommand(ContentItem? item);

        public abstract void OpenFile(ContentItem contentItem);

        // OpenFileAsNewFile(ContentItem contentItem) 
        public abstract void OpenFileAsNewFile(ContentItem contentItem);

        // async void GenerateTitleCommand(List<ContentItem> contentItem, object afterExecuteAction)
        public abstract void GenerateTitleCommand(List<ContentItem> contentItem, object afterExecuteAction);

        // void ExecutePromptTemplateCommand(List<ContentItem> contentItem, object afterExecuteAction, string promptName)
        public abstract void ExecutePromptTemplateCommand(List<ContentItem> contentItem, object afterExecuteAction, PromptItem promptItem);

        //  void GenerateBackgroundInfoCommand(List<ContentItem> contentItem, object afterExecuteAction)
        public abstract void GenerateBackgroundInfoCommand(List<ContentItem> contentItem, object afterExecuteAction);

        // void GenerateSummaryCommand(List<ContentItem> contentItem, object afterExecuteAction)
        public abstract void GenerateSummaryCommand(List<ContentItem> contentItem, object afterExecuteAction);

        // void GenerateTasksCommand(List<ContentItem> contentItem, object afterExecuteAction)
        public abstract void GenerateTasksCommand(List<ContentItem> contentItem, object afterExecuteAction);

        // void CheckDocumentReliabilityCommand(List<ContentItem> contentItem, object afterExecuteAction)
        public abstract void CheckDocumentReliabilityCommand(List<ContentItem> contentItem, object afterExecuteAction);

        // void GenerateVectorCommand(List<ContentItem> contentItem, object afterExecuteAction)
        public abstract void GenerateVectorCommand(List<ContentItem> contentItem, object afterExecuteAction);

        //  void OpenVectorSearchWindowCommand(ContentFolder folder) 
        public abstract void OpenVectorSearchWindowCommand(ContentFolder folder);

        //  void OpenVectorSearchWindowCommand(ContentItem contentItem)
        public abstract void OpenVectorSearchWindowCommand(ContentItem contentItem);

        // void OpenImageChatWindowCommand(ContentItem item, Action action)
        public abstract void OpenImageChatWindowCommand(ContentItem item, Action action);

        // void OpenRAGManagementWindowCommand()
        public abstract void OpenRAGManagementWindowCommand();

        //  void OpenVectorDBManagementWindowCommand()
        public abstract void OpenVectorDBManagementWindowCommand();

        //  void SettingCommandExecute()
        public abstract void SettingCommandExecute();




    }
}
