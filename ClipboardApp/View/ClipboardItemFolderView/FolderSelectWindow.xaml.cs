using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.ClipboardItemFolderView
{
    /// <summary>
    /// FolderSelectWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderSelectWindow : Window
    {
        public FolderSelectWindow()
        {
            InitializeComponent();
        }

        public static void OpenFolderSelectWindow(ClipboardFolderViewModel rootFolderViewModel, Action<ClipboardFolderViewModel> folderSelectedAction) {
            FolderSelectWindow folderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel folderSelectWindowViewModel = new FolderSelectWindowViewModel();
            folderSelectWindowViewModel.Initialize(rootFolderViewModel, folderSelectedAction);
            folderSelectWindow.DataContext = folderSelectWindowViewModel;
            folderSelectWindow.ShowDialog();
        }
    }
}
