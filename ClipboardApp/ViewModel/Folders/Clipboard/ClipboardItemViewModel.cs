using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using PythonAILib.Model.Content;
using PythonAILibUI.ViewModel.Item;

namespace ClipboardApp.ViewModel.Content {
    public partial class ClipboardItemViewModel : ContentItemViewModel {

        // コンストラクタ
        public ClipboardItemViewModel(ContentFolderViewModel folderViewModel, ContentItemWrapper clipboardItem) : base(folderViewModel, clipboardItem) {
            ContentItem = clipboardItem;
            FolderViewModel = folderViewModel;
            Content = ContentItem.Content;
            Description = ContentItem.Description;
            Tags = ContentItem.Tags;
            SourceApplicationTitleText = ContentItem.SourceApplicationTitle;
            Commands = new ClipboardItemViewModelCommands();
            OnPropertyChanged(nameof(Content));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(Tags));
            OnPropertyChanged(nameof(SourceApplicationTitleText));
            OnPropertyChanged(nameof(FileTabVisibility));

        }

        // Commands
        public override ContentItemViewModelCommands Commands { get; set; }


        // Context Menu

        public ObservableCollection<MenuItem> ContentItemMenuItems {
            get {
                ClipboardItemMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.ContentItemMenuItems;
            }
        }

        // Copy
        public ClipboardItemViewModel Copy() {
            ContentItemWrapper newItem = ContentItem.Copy();
            return new ClipboardItemViewModel(FolderViewModel, newItem);
        }



    }
}
