using System.Windows;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.Folder;
using QAChat.ViewModel.Folder;

namespace ClipboardApp.View.Folder {
    /// <summary>
    /// FolderEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderEditWindow : Window {
        public FolderEditWindow() {
            InitializeComponent();
        }
        public static void OpenFolderEditWindow(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow folderEditWindow = new() {
                DataContext = new FolderEditWindowViewModel(folderViewModel, afterUpdate)
            };
            folderEditWindow.ShowDialog();
        }
    }
}
