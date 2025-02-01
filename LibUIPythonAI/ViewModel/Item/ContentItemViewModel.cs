using PythonAILib.Model.Content;
using PythonAILibUI.ViewModel.Item;
using QAChat.Model;
using QAChat.Resource;
using QAChat.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.Item {
    public abstract class ContentItemViewModel(ContentFolderViewModel folderViewModel, ContentItem contentItemBase) : QAChatViewModelBase {
        public ContentItem ContentItem { get; set; } = contentItemBase;

        // FolderViewModel
        public ContentFolderViewModel FolderViewModel { get; set; } = folderViewModel;

        // Tags
        public HashSet<string> Tags { get => ContentItem.Tags; set { ContentItem.Tags = value; } }

        // SelectedTabIndex
        private int selectedTabIndex = 0;
        public int SelectedTabIndex {
            get {
                return selectedTabIndex;
            }
            set {
                selectedTabIndex = value;
                // LastSelectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }
    }
}
