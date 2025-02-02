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


        // IsSelected
        private bool isSelected = false;
        public bool IsSelected {
            get {
                return isSelected;
            }
            set {
                isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        // IsChecked
        private bool isChecked = false;
        public bool IsChecked {
            get {
                return isChecked;
            }
            set {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

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
