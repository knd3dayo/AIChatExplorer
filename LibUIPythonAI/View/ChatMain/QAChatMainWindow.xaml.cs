using System.Windows;
using LibUIPythonAI.ViewModel.ChatMain;
using LibUIPythonAI.ViewModel;

namespace LibUIPythonAI.View.ChatMain {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class QAChatMainWindow : Window {
        public QAChatMainWindow() {
            InitializeComponent();
        }

        public static void OpenOpenAIChatWindow(QAChatStartupProps props) {
            LibUIPythonAI.View.ChatMain.QAChatMainWindow openAIChatWindow = new();
            ChatWindowViewModel mainWindowViewModel = new(props);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}