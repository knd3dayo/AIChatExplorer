using PythonAILib.Model.Content;
using QAChat.Model;
using QAChat.Resource;
using QAChat.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.Item {
    public abstract class ContentItemViewModel(ContentFolderViewModel folderViewModel, ContentItem contentItemBase) : QAChatViewModelBase {
        public ContentItem ContentItem { get; set; } = contentItemBase;

        // Tags
        public HashSet<string> Tags { get => ContentItem.Tags; set { ContentItem.Tags = value; } }


        // 選択中のContentItemBaseを開く
        public abstract void OpenItem();

        // 選択中のContentItemBaseを削除
        public abstract void RemoveItem();
        // OpenContentItemCommand
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            OpenItem();
        });

        // RemoveSelectedItemCommand
        public SimpleDelegateCommand<object> RemoveSelectedItemCommand => new((parameter) => {
            RemoveItem();
        });
    }
}
