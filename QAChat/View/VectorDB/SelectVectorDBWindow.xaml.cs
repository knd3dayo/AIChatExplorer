using System.Windows;
using PythonAILib.Model.VectorDB;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.VectorDB;

namespace QAChat.View.VectorDB
{
    /// <summary>
    /// SelectVectorDBWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectVectorDBWindow : Window {
        public SelectVectorDBWindow() {
            InitializeComponent();
        }
        public static void OpenSelectVectorDBWindow(ContentFolderViewModel folderViewModel, bool closeAfterSelect, Action<List<VectorDBItem>> action) {
            SelectVectorDBWindow window = new();
            SelectVectorDBItemWindowViewModel viewModel = new(folderViewModel, closeAfterSelect, action);
            window.DataContext = viewModel;
            window.ShowDialog();
        }
    }

}
