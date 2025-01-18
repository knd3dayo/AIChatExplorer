using System.Windows;
using PythonAILibUI.ViewModel.QAChatMain;
using QAChat.ViewModel;

namespace QAChat.View.QAChatMain
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class QAChatMainWindow : Window {
        public QAChatMainWindow() {
            InitializeComponent();
        }

        public static void OpenOpenAIChatWindow(QAChatStartupProps props) {
            QAChat.View.QAChatMain.QAChatMainWindow openAIChatWindow = new();
            QAChatWindowViewModel mainWindowViewModel = new(props);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}