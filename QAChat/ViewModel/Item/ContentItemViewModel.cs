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

    }
}
