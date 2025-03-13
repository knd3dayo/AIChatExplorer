using System.Windows;
using LibUIPythonAI.ViewModel;
using LibUIPythonAI.ViewModel.Chat;

namespace LibUIPythonAI.View.Chat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class QAChatWindow : Window {
        public QAChatWindow() {
            InitializeComponent();
        }

        public static void OpenOpenAIChatWindow(QAChatStartupProps props) {
            LibUIPythonAI.View.Chat.QAChatWindow openAIChatWindow = new();
            ChatWindowViewModel mainWindowViewModel = new(props);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}