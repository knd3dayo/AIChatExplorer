using System.Collections.ObjectModel;
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

        public static void OpenFolderSelectWindow(ObservableCollection<ContentFolderViewModel> rootFolderViewModels, Action<ContentFolderViewModel, bool> folderSelectedAction) {
            FolderSelectWindow folderSelectWindow = new();
            FolderSelectWindowViewModel folderSelectWindowViewModel = new(rootFolderViewModels, folderSelectedAction);
            folderSelectWindow.DataContext = folderSelectWindowViewModel;
            folderSelectWindow.ShowDialog();
        }
    }
}
