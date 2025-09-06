using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Content;
using LibPythonAI.Model.Content;
using LibUIMain.ViewModel.Folder;

namespace AIChatExplorer.ViewModel.Folders.Browser {
    public class EdgeBrowseHistoryItemViewModel : ApplicationItemViewModel {

        // コンストラクタ
        public EdgeBrowseHistoryItemViewModel(ContentFolderViewModel folderViewModel, ContentItem applicationItem) : base(folderViewModel, applicationItem) {

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
            EdgeBrowseHistoryItemMenu applicationItemMenu = new(this);
            ContentItemMenuItems = applicationItemMenu.CreateBasicItemContextMenuItems();
        }


        // Copy
        public override EdgeBrowseHistoryItemViewModel Copy() {
            ContentItem newItem = ContentItem.Copy();
            return new EdgeBrowseHistoryItemViewModel(FolderViewModel, newItem);
        }

    }
}
