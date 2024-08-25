using System.Windows;
using QAChat.ViewModel;
using WpfAppCommon.Control.QAChat;

namespace QAChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        public static void OpenOpenAIChatWindow(QAChatStartupProps props) {
            QAChat.MainWindow openAIChatWindow = new();
            MainWindowViewModel mainWindowViewModel = new (props);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}