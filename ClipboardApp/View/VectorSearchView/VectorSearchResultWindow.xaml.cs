using System.Windows;
using ClipboardApp.ViewModel;
using PythonAILib.Model.VectorDB;

namespace ClipboardApp.View.VectorSearchView
{
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
