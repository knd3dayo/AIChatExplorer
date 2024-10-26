using System.Windows;
using ClipboardApp.ViewModel;
using ClipboardApp.ViewModel.VectorDB;
using PythonAILib.Model.VectorDB;

namespace ClipboardApp.View.VectorDBView
{
    /// <summary>
    /// SelectVectorDBWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectVectorDBWindow : Window {
        public SelectVectorDBWindow() {
            InitializeComponent();
        }
        public static void OpenSelectVectorDBWindow(ClipboardFolderViewModel folderViewModel, bool closeAfterSelect, Action<List<VectorDBItem>> action) {
            SelectVectorDBWindow window = new();
            SelectVectorDBItemWindowViewModel viewModel = new(folderViewModel, closeAfterSelect, action);
            window.DataContext = viewModel;
            window.ShowDialog();
        }
    }

}
