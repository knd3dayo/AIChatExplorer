using System.Windows.Controls;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.Item;
using QAChat.ViewModel.Content;

namespace ClipboardApp.View.Item {
    /// <summary>
    /// EditItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditItemControl : UserControl {
        public EditItemControl() {
            InitializeComponent();
        }
        public static EditItemControl CreateEditItemControl(ContentFolderViewModel folderViewModel, ContentItemViewModel itemViewModel, Action action) {
            EditItemControl editItemControl = new();
            EditItemWindowViewModel editItemWindowViewModel = new(folderViewModel, itemViewModel, action);
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
