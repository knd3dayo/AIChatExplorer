using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Settings;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.Content;
using PythonAILibUI.ViewModel.Item;
using ClipboardApp.Model.Item;
using ClipboardApp.ViewModel.Main;
using ClipboardApp.ViewModel.Content;

namespace ClipboardApp.ViewModel.Folders.Browser {
    public class EdgeBrowseHistoryItemViewModel : ClipboardItemViewModel {

        // コンストラクタ
        public EdgeBrowseHistoryItemViewModel(ContentFolderViewModel folderViewModel, ContentItemWrapper clipboardItem) : base(folderViewModel, clipboardItem) {
            if (folderViewModel.Commands == null) {
                throw new Exception("folderViewModel.Commands is null");
            }
            ContentItem = clipboardItem;
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
                EdgeBrowseHistoryItemMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.ContentItemMenuItems;
            }
        }

        // Copy
        public override EdgeBrowseHistoryItemViewModel Copy() {
            ContentItemWrapper newItem = ContentItem.Copy();
            return new EdgeBrowseHistoryItemViewModel(FolderViewModel, newItem);
        }

    }
}
