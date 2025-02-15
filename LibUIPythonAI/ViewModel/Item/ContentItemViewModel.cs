using PythonAILib.Model.Content;
using PythonAILibUI.ViewModel.Item;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.ViewModel.Item {
    public abstract class ContentItemViewModel(ContentFolderViewModel folderViewModel, ContentItemWrapper contentItemBase) : ChatViewModelBase {
        public ContentItemWrapper ContentItem { get; set; } = contentItemBase;

        // FolderViewModel
        public ContentFolderViewModel FolderViewModel { get; set; } = folderViewModel;

        public abstract ContentItemViewModelCommands Commands { get; set; }

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

        // DeleteItems
        public static Task DeleteItems(List<ContentItemViewModel> items) {
            return Task.Run(() => {
                var contentItems = items.Select(item => item.ContentItem).ToList();
                ContentItemWrapper.DeleteItems(contentItems);
            });
        }
    }
}
