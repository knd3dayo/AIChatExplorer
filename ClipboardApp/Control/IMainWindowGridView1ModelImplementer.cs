using System.Collections.ObjectModel;
using System.Windows;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.ClipboardItemView;
using WpfAppCommon.Utils;

namespace ClipboardApp.Control {
    public interface IMainWindowGridView1ModelImplementer {

        public Visibility PreviewModeVisibility { get; }
        public ClipboardItemViewModel SelectedItem { get; set; }
        public ClipboardFolderViewModel SelectedFolder { get; set; }
        public SimpleDelegateCommand<RoutedEventArgs> ClipboardItemSelectionChangedCommand { get; }
        public SimpleDelegateCommand<object> OpenSelectedItemCommand { get; }
        public SimpleDelegateCommand<object> OpenContentAsFileCommand { get; }
        // MergeItemCommand
        public SimpleDelegateCommand<object> MergeItemCommand { get; }
        // MergeItemWithHeaderCommand
        public SimpleDelegateCommand<object> MergeItemWithHeaderCommand { get; }

        public SimpleDelegateCommand<object> CopyItemCommand { get; }

        public SimpleDelegateCommand<object> DeleteItemCommand { get; }
        // CutItemCommand
        public SimpleDelegateCommand<object> CutItemCommand { get; }
    }
}
