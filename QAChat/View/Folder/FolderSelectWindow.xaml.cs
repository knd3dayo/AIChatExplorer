using System.Windows;
using QAChat.ViewModel.Folder;

namespace QAChat.View.Folder {
    /// <summary>
    /// FolderSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderSelectWindow : Window {
        public FolderSelectWindow() {
            InitializeComponent();
        }

        public static void OpenFolderSelectWindow(ContentFolderViewModel rootFolderViewModel, Action<ContentFolderViewModel> folderSelectedAction) {
            FolderSelectWindow folderSelectWindow = new();
            FolderSelectWindowViewModel folderSelectWindowViewModel = new(rootFolderViewModel, folderSelectedAction);
            folderSelectWindow.DataContext = folderSelectWindowViewModel;
            folderSelectWindow.ShowDialog();
        }
    }
}
