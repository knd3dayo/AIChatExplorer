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
using ImageChat.ViewModel;
using WpfAppCommon.Model;

namespace ImageChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }
        public static void OpenMainWindow(ClipboardItem? clipboardItem, bool isStartFromInternalApp, Action afterUpdate) {
            ImageChat.MainWindow imageEvidenceCheckerWindow = new();
            MainWindowViewModel imageEvidenceCheckerWindowViewModel = (MainWindowViewModel)imageEvidenceCheckerWindow.DataContext;
            // Initialize
            imageEvidenceCheckerWindowViewModel.Initialize(clipboardItem, isStartFromInternalApp, afterUpdate);

            imageEvidenceCheckerWindow.Show();
        }
    }
}