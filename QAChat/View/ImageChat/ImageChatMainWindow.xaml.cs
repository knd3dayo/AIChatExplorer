using System.Windows;
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
        public static void OpenMainWindow(ClipboardItem? clipboardItem, bool isStartFromInternalApp, Action afterUpdate) {
            ImageChat.ImageChatMainWindow imageEvidenceCheckerWindow = new();
            ImageChatMainWindowViewModel imageEvidenceCheckerWindowViewModel = (ImageChatMainWindowViewModel)imageEvidenceCheckerWindow.DataContext;
            // Initialize
            imageEvidenceCheckerWindowViewModel.Initialize(clipboardItem, isStartFromInternalApp, afterUpdate);

            imageEvidenceCheckerWindow.Show();
        }
    }
}