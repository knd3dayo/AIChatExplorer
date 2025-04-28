using System.Windows;
using LibUIPythonAI.ViewModel.Chat;

namespace LibUIAutoGenChat.View.Chat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AutoGenChatWindow : Window {
        public AutoGenChatWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(QAChatStartupProps props) {
            AutoGenChatWindow openAIChatWindow = new();
            ChatWindowViewModel mainWindowViewModel = new(props);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}