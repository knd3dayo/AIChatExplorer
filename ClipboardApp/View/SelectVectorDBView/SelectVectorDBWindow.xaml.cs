using System.Windows;
using ClipboardApp.ViewModel;
using PythonAILib.Model.Abstract;

namespace ClipboardApp.View.SelectVectorDBView {
    /// <summary>
    /// SelectVectorDBWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectVectorDBWindow : Window {
        public SelectVectorDBWindow() {
            InitializeComponent();
        }
        public static void OpenSelectVectorDBWindow(ClipboardFolderViewModel folderViewModel ,Action<List<VectorDBItemBase>> action) {
            SelectVectorDBWindow window = new();
            SelectVectorDBItemWindowViewModel viewModel = new(folderViewModel, action);
            window.DataContext = viewModel;
            window.ShowDialog();
        }
    }

}
