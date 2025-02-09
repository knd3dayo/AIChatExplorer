using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using LibUIPythonAI.ViewModel.Item;
using WpfAppCommon.Utils;
using LibUIPythonAI.Utils;

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


        // アイテム保存
        public SimpleDelegateCommand<ContentItemViewModel> SaveClipboardItemCommand => new((itemViewModel) => {
            itemViewModel.ContentItem.Save();
        });

        // Delete
        public SimpleDelegateCommand<ContentItemViewModel> DeleteItemCommand => new((itemViewModel) => {
            itemViewModel.ContentItem.Delete();
        });


        // OpenContentItemCommand
        public abstract SimpleDelegateCommand<ContentItemViewModel> OpenItemCommand { get; }


        public abstract void OpenFolder(ContentItem contentItem);

        // Command to open text content as a file
        public abstract void OpenContentAsFile(ContentItem contentItem);

        public abstract void OpenOpenAIChatWindowCommand(ContentItem? item);

        public abstract void OpenFile(ContentItem contentItem);

        // OpenFileAsNewFile(ContentItem contentItem) 
        public abstract void OpenFileAsNewFile(ContentItem contentItem);


    }
}
