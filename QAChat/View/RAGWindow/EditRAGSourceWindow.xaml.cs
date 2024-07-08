using System.Windows;
using QAChat.ViewModel;

namespace QAChat.View.RAGWindow
{
    /// <summary>
    /// EditItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditRAGSourceWindow : Window {
        public EditRAGSourceWindow() {
            InitializeComponent();
        }

        public static void OpenEditRAGSourceWindow(RAGSourceItemViewModel ragSourceItemViewModel, Action<RAGSourceItemViewModel> callback) {
            EditRAGSourceWindow editRAGSourceWindow = new();
            EditRAGSourceWindowViewModel editRAGSourceWindowViewModel = (EditRAGSourceWindowViewModel)editRAGSourceWindow.DataContext;
            editRAGSourceWindowViewModel.Initialize(ragSourceItemViewModel, callback);
            editRAGSourceWindow.ShowDialog();
        }
    }
}
