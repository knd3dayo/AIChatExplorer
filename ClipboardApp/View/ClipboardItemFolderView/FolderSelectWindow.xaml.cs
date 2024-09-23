using System.Windows;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.ClipboardItemFolderView {
    /// <summary>
    /// FolderSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderSelectWindow : Window {
        public FolderSelectWindow() {
            InitializeComponent();
        }

        public static void OpenFolderSelectWindow(ClipboardFolderViewModel rootFolderViewModel, Action<ClipboardFolderViewModel> folderSelectedAction) {
            FolderSelectWindow folderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel folderSelectWindowViewModel = new(rootFolderViewModel, folderSelectedAction);
            folderSelectWindow.DataContext = folderSelectWindowViewModel;
            folderSelectWindow.ShowDialog();
        }
    }
}
