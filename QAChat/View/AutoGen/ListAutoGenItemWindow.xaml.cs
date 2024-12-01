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

        public static void OpenListAutoGenItemWindow(bool selectGroupChatMode = false) {
            ListAutoGenItemWindow listAutoGenItemWindow = new();
            ListAutoGenItemWindowViewModel listAutoGenItemWindowViewModel = new(selectGroupChatMode);
            listAutoGenItemWindow.DataContext = listAutoGenItemWindowViewModel;
            listAutoGenItemWindow.Show();
        }
    }
}
