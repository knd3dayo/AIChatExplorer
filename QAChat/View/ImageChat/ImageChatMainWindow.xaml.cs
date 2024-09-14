using System.Windows;
using PythonAILib.Model;
using QAChat.ViewModel.ImageChat;
using WpfAppCommon.Model;

namespace QAChat.View.ImageChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ImageChatMainWindow : Window {
        public ImageChatMainWindow() {
            InitializeComponent();
        }
        public static void OpenMainWindow(ContentItemBase clipboardItem, Action afterUpdate) {
            ImageChat.ImageChatMainWindow imageEvidenceCheckerWindow = new();
            imageEvidenceCheckerWindow.DataContext = new ImageChatMainWindowViewModel(clipboardItem, afterUpdate);
            imageEvidenceCheckerWindow.Show();
        }
    }
}