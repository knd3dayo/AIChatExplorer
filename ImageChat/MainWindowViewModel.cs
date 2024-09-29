using ClipboardApp.Model;
using QAChat;
using QAChat.ViewModel.ImageChat;

namespace ImageChat {
    public class MainWindowViewModel : ImageChatViewModelBase {


        public MainWindowViewModel() {
            Init();
            ImageChatMainWindowViewModel = new ImageChatMainWindowViewModel(new PythonAILib.Model.Content.ContentItem(), () => { });
        }

        public void Init() {

            ImageChatPythonAILibConfigParams configParams = new();
            PythonAILibManager.Init(configParams);

        }

        public ImageChatMainWindowViewModel ImageChatMainWindowViewModel { get; set; }

    }
}
