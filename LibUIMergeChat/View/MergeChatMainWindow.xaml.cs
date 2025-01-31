using System.Windows;
using MergeChat.ViewModel;
using QAChat.ViewModel;

namespace MergeChat.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MergeChatMainWindow : Window {
        public MergeChatMainWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(QAChatStartupProps props) {
            MergeChat.View.MergeChatMainWindow openAIChatWindow = new();
            MergeChatWindowViewModel mainWindowViewModel = new(props);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}