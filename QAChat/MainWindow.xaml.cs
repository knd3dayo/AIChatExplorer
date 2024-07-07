using System.Windows;
using WpfAppCommon.Control.QAChat;

namespace QAChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        public static void OpenOpenAIChatWindow(QAChatStartupProps props) {
            QAChat.MainWindow openAIChatWindow = new();
            QAChat.MainWindowViewModel mainWindowViewModel = (QAChat.MainWindowViewModel)openAIChatWindow.DataContext;
            mainWindowViewModel.Initialize(props);

            openAIChatWindow.Show();
        }
    }
}