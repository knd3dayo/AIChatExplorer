using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfAppCommon.Model;
using static QAChat.MainWindowViewModel;

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