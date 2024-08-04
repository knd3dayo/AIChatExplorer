using System.Windows;
using PythonAILib.Model;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.VectorSearchView {
    /// <summary>
    /// VectorSearchResultWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class VectorSearchResultWindow : Window {
        public VectorSearchResultWindow() {
            InitializeComponent();
        }

        public static void OpenVectorSearchResultWindow(List<VectorSearchResult> vectorSearchResults) {
            VectorSearchResultWindow vectorSearchResultWindow = new ();
            VectorSearchResultWindowViewModel vectorSearchResultWindowViewModel = (VectorSearchResultWindowViewModel) vectorSearchResultWindow.DataContext;
            vectorSearchResultWindowViewModel.Initialize(vectorSearchResults);
            vectorSearchResultWindow.DataContext = vectorSearchResultWindowViewModel;
            vectorSearchResultWindow.ShowDialog();
        }
    }
}
