using System.Windows;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Folders.Clipboard;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Main {
    public interface IMainPanelImplementer {

        public ClipboardItemViewModel? SelectedItem { get; set; }
        public ClipboardFolderViewModel? SelectedFolder { get; set; }
        // FolderViewModels
        public ClipboardAppRootFolderViewModelContainer RootFolderViewModelContainer { get; set; }
        // Item
        public SimpleDelegateCommand<RoutedEventArgs> ClipboardItemSelectionChangedCommand { get; }
        public SimpleDelegateCommand<object> OpenSelectedItemCommand { get; }
        public SimpleDelegateCommand<object> OpenContentAsFileCommand { get; }

        public SimpleDelegateCommand<object> CopyItemCommand { get; }
        public SimpleDelegateCommand<object> DeleteItemCommand { get; }
        // CutItemCommand
        public SimpleDelegateCommand<object> CutItemCommand { get; }

        // PasteCommand
        public SimpleDelegateCommand<object> PasteCommand { get; }
        // Folder
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand { get; }
    }
}
