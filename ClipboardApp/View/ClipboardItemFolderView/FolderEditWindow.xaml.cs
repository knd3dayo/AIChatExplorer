using System.Windows;
using ClipboardApp.ViewModel;

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
        public static void OpenFolderEditWindow(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow folderEditWindow = new();
            FolderEditWindowViewModel folderEditWindowViewModel = (FolderEditWindowViewModel)folderEditWindow.DataContext;
            folderEditWindowViewModel.Initialize(folderViewModel, afterUpdate);
            folderEditWindow.ShowDialog();
        }
    }
}
