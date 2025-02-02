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
using LibUIPythonAI.ViewModel.RAG;

namespace LibUIPythonAI.View.RAG
{
    /// <summary>
    /// UpdateRAGIndexWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UpdateRAGIndexWindow : Window {
        public UpdateRAGIndexWindow() {
            InitializeComponent();
        }

        public static void OpenUpdateRAGIndexWindow(RAGSourceItemViewModel ItemViewModel, Action<RAGSourceItemViewModel> callback) {
            UpdateRAGIndexWindow updateRAGIndexWindow = new();
            UpdateRAGIndexWindowViewModel updateRAGIndexWindowViewModel = new (ItemViewModel, callback);
            updateRAGIndexWindow.DataContext = updateRAGIndexWindowViewModel;
            updateRAGIndexWindow.ShowDialog();
        }
    }
}
