using System.Windows;
using LibUIPythonAI.ViewModel.RAG;
using LibUIPythonAI.ViewModel.RAG;

namespace LibUIPythonAI.View.RAG {
    /// <summary>
    /// EditItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditRAGSourceWindow : Window {
        public EditRAGSourceWindow() {
            InitializeComponent();
        }

        public static void OpenEditRAGSourceWindow(RAGSourceItemViewModel ragSourceItemViewModel, Action<RAGSourceItemViewModel> callback) {
            EditRAGSourceWindow editRAGSourceWindow = new();
            EditRAGSourceWindowViewModel editRAGSourceWindowViewModel = new (ragSourceItemViewModel, callback);
            editRAGSourceWindow.DataContext = editRAGSourceWindowViewModel;
            editRAGSourceWindow.ShowDialog();
        }
    }
}
