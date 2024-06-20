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

namespace ImageChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        public static void OpenMainWindow(bool isStartFromInternalApp) {
            ImageChat.MainWindow imageEvidenceCheckerWindow = new();
            ImageChat.MainWindowViewModel imageEvidenceCheckerWindowViewModel = (ImageChat.MainWindowViewModel)imageEvidenceCheckerWindow.DataContext;
            // 内部プロジェクトからの起動をFalse
            imageEvidenceCheckerWindowViewModel.IsStartFromInternalApp = isStartFromInternalApp;
            imageEvidenceCheckerWindow.Show();
        }
    }
}