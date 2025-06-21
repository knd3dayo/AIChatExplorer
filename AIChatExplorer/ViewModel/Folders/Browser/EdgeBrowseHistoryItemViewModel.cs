using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Content;
using LibPythonAI.Model.Content;
using LibUIPythonAI.ViewModel.Folder;

namespace AIChatExplorer.ViewModel.Folders.Browser {
    public class EdgeBrowseHistoryItemViewModel : ApplicationItemViewModel {

        // コンストラクタ
        public EdgeBrowseHistoryItemViewModel(ContentFolderViewModel folderViewModel, ContentItemWrapper applicationItem) : base(folderViewModel, applicationItem) {

            ContentItem = applicationItem;
            FolderViewModel = folderViewModel;
            Content = ContentItem.Content;
            Description = ContentItem.Description;
            Tags = ContentItem.Tags;
            SourceApplicationTitleText = ContentItem.SourceApplicationTitle;
            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Tags));
            OnPropertyChanged(nameof(SourceApplicationTitleText));
            OnPropertyChanged(nameof(FileTabVisibility));

        }

        // Context Menu

        public override ObservableCollection<MenuItem> ContentItemMenuItems {
            get {
                EdgeBrowseHistoryItemMenu applicationItemMenu = new(this);
                return applicationItemMenu.ContentItemMenuItems;
            }
        }

        // Copy
        public override EdgeBrowseHistoryItemViewModel Copy() {
            ContentItemWrapper newItem = ContentItem.Copy();
            return new EdgeBrowseHistoryItemViewModel(FolderViewModel, newItem);
        }

    }
}
