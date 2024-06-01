using System.Windows;

namespace ClipboardApp.View.ClipboardItemFolderView
{
    /// <summary>
    /// FolderEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderEditWindow : Window
    {
        public FolderEditWindow()
        {
            InitializeComponent();
        }
        public static void OpenFolderEditWindow(ClipboardFolderViewModel folderViewModel, FolderEditWindowViewModel.Mode mode, Action afterUpdate) {
            FolderEditWindow folderEditWindow = new();
            FolderEditWindowViewModel folderEditWindowViewModel = (FolderEditWindowViewModel)folderEditWindow.DataContext;
            folderEditWindowViewModel.Initialize(folderViewModel, mode, afterUpdate);
            folderEditWindow.ShowDialog();
        }
    }
}
