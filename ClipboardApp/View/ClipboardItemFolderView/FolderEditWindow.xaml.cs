using System.Windows;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.ClipboardItemFolderView {
    /// <summary>
    /// FolderEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderEditWindow : Window {
        public FolderEditWindow() {
            InitializeComponent();
        }
        public static void OpenFolderEditWindow(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow folderEditWindow = new() {
                DataContext = new FolderEditWindowViewModel(folderViewModel, afterUpdate)
            };
            folderEditWindow.ShowDialog();
        }
    }
}
