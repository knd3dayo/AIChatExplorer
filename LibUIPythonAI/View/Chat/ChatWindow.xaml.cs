using System.Windows;
using LibUIPythonAI.ViewModel.Chat;

namespace LibUIPythonAI.View.Chat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window {
        public ChatWindow() {
            InitializeComponent();
        }

        public static void OpenOpenAIChatWindow(QAChatStartupProps props) {
            LibUIPythonAI.View.Chat.ChatWindow openAIChatWindow = new();
            ChatWindowViewModel mainWindowViewModel = new(props);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}