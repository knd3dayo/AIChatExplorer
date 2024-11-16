using System.Windows;
using System.Windows.Controls;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.Content;

namespace ClipboardApp.View.ClipboardItem
{
    /// <summary>
    /// EditItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditItemControl : UserControl {
        public EditItemControl() {
            InitializeComponent();
        }
        public static EditItemControl CreateEditItemControl(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel? itemViewModel, Action action) {
            EditItemControl editItemControl = new();
            EditItemWindowViewModel editItemWindowViewModel = new(folderViewModel, itemViewModel, action) ;
            editItemControl.DataContext = editItemWindowViewModel;
            return editItemControl;
        }

        public void SetCloseUserControl(Action closeUserControl) {
            if (DataContext is EditItemWindowViewModel) {
                ((EditItemWindowViewModel)DataContext).CloseUserControl = closeUserControl;
            }
        }

    }
}
