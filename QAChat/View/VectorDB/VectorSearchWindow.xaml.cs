using System.Windows;
using PythonAILib.Model.VectorDB;
using QAChat.ViewModel.VectorDBWindow;

namespace QAChat.View.VectorDB {
    /// <summary>
    /// VectorSearchWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class VectorSearchWindow : Window {
        public VectorSearchWindow() {
            InitializeComponent();
        }

        public static void OpenVectorSearchResultWindow(VectorSearchWindowViewModel dataContext) {
            VectorSearchWindow vectorSearchResultWindow = new() {
                DataContext = dataContext
            };
            vectorSearchResultWindow.ShowDialog();
        }
    }
}
