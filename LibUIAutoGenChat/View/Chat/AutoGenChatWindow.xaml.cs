using System.Windows;
using LibUIAutoGenChat.ViewModel.Chat;

namespace LibUIAutoGenChat.View.Chat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AutoGenChatWindow : Window {
        public AutoGenChatWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(LibUIPythonAI.ViewModel.Chat.QAChatStartupPropsBase props) {
            AutoGenChatWindow openAIChatWindow = new();
            AutoGenChatWindowViewModel mainWindowViewModel = new(props);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}