using System.Windows;
using QAChat.ViewModel.AutoGen;

namespace QAChat.View.AutoGen {
    /// <summary>
    /// ListAutoGenItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListAutoGenItemWindow : Window {
        public ListAutoGenItemWindow() {
            InitializeComponent();
        }

        public static void OpenListAutoGenItemWindow() {
            ListAutoGenItemWindow listAutoGenItemWindow = new();
            ListAutoGenItemWindowViewModel listAutoGenItemWindowViewModel = new();
            listAutoGenItemWindow.DataContext = listAutoGenItemWindowViewModel;
            listAutoGenItemWindow.Show();
        }
    }
}
