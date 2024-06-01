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
using ClipboardApp.View.ClipboardItemFolderView;

namespace ClipboardApp.View.ClipboardItemView
{
    /// <summary>
    /// EditItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditItemWindow : Window
    {
        public EditItemWindow()
        {
            InitializeComponent();
        }
        public static void OpenEditItemWindow(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel? itemViewModel, Action action) {
            EditItemWindow editItemWindow = new();
            EditItemWindowViewModel editItemWindowViewModel = (EditItemWindowViewModel)editItemWindow.DataContext;
            editItemWindowViewModel.Initialize(folderViewModel, itemViewModel, action);
            editItemWindow.ShowDialog();
        }
    }
}
