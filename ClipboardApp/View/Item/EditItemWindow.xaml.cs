using System.Windows;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.Content;

namespace ClipboardApp.View.Item
{
    /// <summary>
    /// EditItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditItemWindow : Window {
        public EditItemWindow() {
            InitializeComponent();
        }
        public static void OpenEditItemWindow(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel? itemViewModel, Action action) {
            EditItemWindow editItemWindow = new();
            EditItemWindowViewModel editItemWindowViewModel =new (folderViewModel, itemViewModel, action);
            editItemWindow.DataContext = editItemWindowViewModel;
            editItemWindow.Show();

        }

    }
}
