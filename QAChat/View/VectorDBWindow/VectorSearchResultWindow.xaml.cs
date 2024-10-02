using System.Windows;
using PythonAILib.Model.VectorDB;
using QAChat.ViewModel.VectorDBWindow;

namespace QAChat.View.VectorDBWindow {
    /// <summary>
    /// VectorSearchResultWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class VectorSearchResultWindow : Window {
        public VectorSearchResultWindow() {
            InitializeComponent();
        }

        public static void OpenVectorSearchResultWindow(List<VectorSearchResult> vectorSearchResults) {
            VectorSearchResultWindow vectorSearchResultWindow = new();
            vectorSearchResultWindow.DataContext = new VectorSearchResultWindowViewModel(vectorSearchResults);
            vectorSearchResultWindow.ShowDialog();
        }
    }
}
